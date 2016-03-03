/*
 ******************CalibrationUtil 工具类 ******************
 * 功能说明：用于各结点的初始化校准 
 *  1.对于根结点
 *      给定的初始姿态 & 根节点的传感器观测数据
 *      计算出地磁参考向量(重力参考向量目前仍采用(0, 0, g))
 *      refVec = q * Sm * q^(-1)
 *  2.对于其他结点 
 *      根据根结点计算的参考向量 计算出相应结点的初始姿态qGS(FQA)
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace collector
{
    // 四元数定义
    public struct Quat
    {
        public float w;
        public float x;
        public float y;
        public float z;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="w"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Quat(float w, float x, float y, float z)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// 四元数的逆
        /// </summary>
        /// <returns></returns>
        public Quat invQuat()
        {
            return new Quat(this.w, -this.x, -this.y, -this.z);
        }

        /// <summary>
        /// 归一化四元数
        /// </summary>
        /// <returns></returns>
        public Quat normalizeQuat()
        {
            float norm = this.w * this.w + this.x * this.x + this.y * this.y + this.z * this.z;

            return new Quat(this.w / norm, this.x / norm, this.y / norm, this.z / norm);
        }
    }

    public class Calibration
    {
        public int sampleCount = 0;                          // 采样点计数
        public float[] initAcc = { 0.0f, 0.0f, 0.0f };       // 初始化采样阶段的加速度向量(最终输出为均值归一化的向量)
        public float[] initGyr = { 0.0f, 0.0f, 0.0f };
        public float[] initMag = { 0.0f, 0.0f, 0.0f };

        public float[] accMeanVec = { 0.0f, 0.0f, 0.0f };   // 加速度均值向量
        public float[] GyrMeanVec = { 0.0f, 0.0f, 0.0f };   // 陀螺仪均值向量 
        public float[] MagMeanVec = { 0.0f, 0.0f, 0.0f };   // 磁均值向量

        public static int INITCOUNT = 200;                  // 初始化的采样点数
        public static int BUFFERSize = 10;                  // 数据窗口大小

        public static bool IsDataFileExists = true;         // 数据文件在本地磁盘是否存在标志位

        public static float[] MagRefVec = { 0.0f, 0.0f, 0.0f }; // 磁参考向量

        public static bool RootInit = false;                // 根结点初始化标志位

        /// <summary>
        /// 前INITCOUNT 采样点 初始化操作
        /// </summary>
        /// <param name="ID">node ID</param>
        /// <param name="Acc">node measured Acc</param>
        /// <param name="Gyr">node measured Gyr</param>
        /// <param name="Mag">node measured Mag</param>
        public void InitLow200(byte ID, float[] Acc, float[] Gyr, float[] Mag)
        {
            // 采样点累加
            sampleCount++;

            SumUpVec(Acc, initAcc);
            SumUpVec(Gyr, initGyr);
            SumUpVec(Mag, initMag);
        }

        /// <summary>
        /// 传入结点ID 和 对应的传感器观测原始数据 计算结点初始姿态
        /// </summary>
        /// <param name="ID">node ID</param>
        /// <param name="Acc">node measured Acc</param>
        /// <param name="Gyr">node measured Gyr</param>
        /// <param name="Mag">node measured Mag</param>
        /// <returns></returns>
        public Quat InitEqual200(byte ID, float[] Acc, float[] Gyr, float[] Mag)
        {
            sampleCount++;

            // 累加
            SumUpVec(Acc, initAcc);
            SumUpVec(Gyr, initGyr);
            SumUpVec(Mag, initMag);

            // 求(加速度 陀螺仪 磁力计)均值向量
            MeanVec(initAcc, INITCOUNT, accMeanVec);
            MeanVec(initGyr, INITCOUNT, GyrMeanVec);
            MeanVec(initMag, INITCOUNT, MagMeanVec);

            // 归一化(加速度向量 磁向量)
            NormalizeVec(initAcc);
            NormalizeVec(initMag);

            // 获取磁参考向量 & 结点初始姿态
            return GetRefOrQuatoffset(ID, Acc, Mag);
        }

        /// <summary>
        /// 获取结点初始姿态
        /// 根结点计算磁参考向量(初始姿态给定) 子结点计算初始姿态
        /// </summary>
        /// <param name="ID">node ID</param>
        /// <param name="Acc">node measured Acc</param>
        /// <param name="Mag">node measured Mag</param>
        /// <returns>计算的初始姿态</returns>
        public Quat GetRefOrQuatoffset(byte ID, float[] Acc, float[] Mag)
        {
            Quat qGS;
            // 根结点 计算磁参考向量
            if (ID == CollectorUtil.ROOTID)
            {
                // 获取磁参考向量
                MagRefVec = GetRef(initMag, CollectorUtil.qGB0Dict[ID]);
                CollectorUtil.qSBDict.Add(ID, new Quat(1, 0, 0, 0));

                RootInit = true;                // 根结点初始化标志位置位
                qGS = new Quat(1, 0 , 0, 0);    // 根结点初始化姿态

                ////////////////Debug////////////////
                Trace.WriteLine("ID = 0 meanMag" + initMag[0] + " " + initMag[1] + " " + initMag[2]);
                Trace.WriteLine("ID = 0 meanAcc" + initAcc[0] + " " + initAcc[1] + " " + initAcc[2]);
            }
            // 其他子结点 计算qSB(先计算初始姿态 接着计算qSB)
            else
            {

                ////////////////Debug////////////////
                Trace.WriteLine("ID = 1 meanMag" + initMag[0] + " " + initMag[1] + " " + initMag[2]);
                Trace.WriteLine("ID = 1 meanAcc" + initAcc[0] + " " + initAcc[1] + " " + initAcc[2]);
                //float[] aMea = { Acc[2], Acc[0], Acc[1] };
                //float[] mMea = { Mag[2], Mag[0], -Mag[1] };
                //float[] refMag = { MagRefVec[2], MagRefVec[0], -MagRefVec[1] };
                
                //qGS = FQANED(refMag, aMea, mMea);
                qGS = FQAUnity(MagRefVec, Acc, Mag);
                //qGS = new Quat(0.866f, 0, 0.5f, 0);
                ////////////////Debug////////////////
                //Trace.WriteLine("qGS0 = " + qGS.w + " " + qGS.z + " " + qGS.y + " " + qGS.z);

                // qSB = qGS^(-1) * qGB0
                Quat qSB = QuatMultiply(qGS.invQuat(), CollectorUtil.qGB0Dict[ID]);
                
                CollectorUtil.qSBDict.Add(ID, qSB);                                 // 将对应ID结点的qSB保存到字典
            }

            return qGS;
            
        }

        /// <summary>
        /// 根据根结点的初始姿态 计算磁参考向量
        /// </summary>
        /// <param name="MeaVec">磁观测向量</param>
        /// <param name="qGB">根结点初始姿态</param>
        /// <returns></returns>
        public static float[] GetRef(float[] MeaVec, Quat qGB)
        {
            float[] refVec = { 0.0f, 0.0f, 0.0f };

            Quat qMea = new Quat(0, MeaVec[0], MeaVec[1], MeaVec[2]);
            Quat qGBinv = new Quat(qGB.w, -qGB.x, -qGB.y, -qGB.z);
            Quat q_temp = QuatMultiply(qMea, qGBinv);
            Quat qRefVec = QuatMultiply(qGB, q_temp);   // q * m * q^(-1)

            refVec[0] = qRefVec.x;
            refVec[1] = qRefVec.y;
            refVec[2] = qRefVec.z;

            return refVec;
        }

        /// <summary>
        /// 四元数乘法
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns>q1 * q2</returns>
        public static Quat QuatMultiply(Quat q1, Quat q2)
        {
            Quat q = new Quat(0, 0, 0, 0);

            q.w = q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z;
            q.x = q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y;
            q.y = q1.w * q2.y - q1.x * q2.z + q1.y * q2.w + q1.z * q2.x;
            q.z = q1.w * q2.z + q1.x * q2.y - q1.y * q2.x + q1.z * q2.w;

            return q;
        }

        /// <summary>
        /// 去除陀螺仪的常值漂移
        /// </summary>
        /// <param name="GyrMeaVec">观测陀螺仪数据</param>
        public void RemoveGyrBias(float[] GyrMeaVec)
        {
            for (int i = 0; i < GyrMeaVec.Length; i++)
            {
                GyrMeaVec[i] -= GyrMeanVec[i];
            }
        }

        /// <summary>
        /// 向量v绕四元数q旋转
        /// </summary>
        /// <param name="v"></param>
        /// <param name="q"></param>
        public static void RotateVec(float[] v, Quat q)
        {
            Quat q11 = QuatMultiply(new Quat(0, v[0], v[1], v[2]), q.invQuat());
            Quat q12 = QuatMultiply(q, q11);

            v[0] = q12.x;
            v[1] = q12.y;
            v[2] = q12.z;
        }

        /// <summary>
        /// 归一化传感器观测向量
        /// </summary>
        /// <param name="mea"></param>
        /// <returns>normalized measurement</returns>
        public static float[] normalizeMeasure(float[] mea)
        {
            float[] temp = new float[mea.Length];
            float norm = 0.0f;
            
            for(int i = 0; i < mea.Length; i++)
            {
                temp[i] = mea[i];
                norm += (temp[i] * temp[i]);
            }

            norm = (float)Math.Sqrt(norm);
            for (int i = 0; i < mea.Length; i++)
            {
                temp[i] /= norm;
            }

            return temp;
        }

        /// <summary>
        /// FQA算法估计初始姿态 坐标系 : (X 前 Y 右 Z 下)
        /// </summary>
        /// <param name="mag">参考磁向量</param>
        /// <param name="accMea">观测重力向量</param>
        /// <param name="magMea">观测磁向量</param>
        /// <returns>初始姿态</returns>
        public static Quat FQANED(float[] mag, float[] accMea, float[] magMea)
        {
            float[] normlizedAccMea = normalizeMeasure(accMea);
            float[] normlizedMagMea = normalizeMeasure(magMea);

            //Trace.WriteLine(normlizedAccMea[0] + " " + normlizedAccMea[1] + " " + normlizedAccMea[2]);
            //Trace.WriteLine(normlizedMagMea[0] + " " + normlizedMagMea[1] + " " + normlizedMagMea[2]);
            //Trace.WriteLine(mag[0] + " " + mag[1] + " " + mag[2]);

	        float th_0 = 0.2f;				// A threshold to determine whether cos(Pitch) is near zero.
	        float th_zero = 0.002f;
	        float alpha = 0.2f * 3.1416f;		// The offset to rotate Acc and Mag when cos(Pitch) is near ZERO【Y轴旋转角=90°】
	        Quat q_alpha = new Quat(0, (float)Math.Cos(0.5*alpha), 0, (float)Math.Sin(0.5*alpha));		// offset quaternion q_alpha
	        Quat q_x = new Quat(1, 0, 1, 0);
	        Quat q_y = new Quat(1, 1, 0, 0);
	        Quat q_z = new Quat(1, 0, 0, 1);
	        int flag = 0;
	        float[] Mag_xy = new float[3];
            float[] Mag_e = new float[3];
	        float temp;
	        float sin_x, sin_y, sin_z, cos_x, cos_y, cos_z;			    // sin () cos()
	        float sin_xh, sin_yh, sin_zh, cos_xh, cos_yh, cos_zh;		// sin (/2) cos(/2)

            float[] Acc = new float[3];
            float[] Mag = new float[3];

	        for (int i = 0; i < mag.Length; i++)
	        {
                Acc[i] = normlizedAccMea[i];
                Mag[i] = normlizedMagMea[i];
		        Mag_xy[i] = Mag[i];
	        }

	        sin_x = -Acc[0];					                // sin(pitch)
	        cos_x = (float)Math.Sqrt(1 - sin_x * sin_x);	// cos(pitch)

	        float cosxAbs = Math.Abs(cos_x);

	        if (cosxAbs < th_0)					// to determine whether cos(Pitch) is near zero 0.2f
	        {
                RotateVec(Acc, q_alpha);
                RotateVec(Mag, q_alpha);

		        //Quaternion_VecRot(q_alpha, Acc);	// Acc = q_alpha*Acc*q_alpha^(-1)
		        //Quaternion_VecRot(q_alpha, Mag);	// Mag = q_alpha*Mag*q_alpha^(-1)
		        
                flag = 1;						// singularity occurs
		
		        for (int i = 0; i < mag.Length; i++)			
		        {
			        Mag_xy[i] = Mag[i];			// Mag_xy重新赋值(Mag has been rotated)
		        }
		        sin_x = -Acc[0];				// Acc has been rotated 重新计算sin(pitch)
		        cos_x = (float)Math.Sqrt(1 - sin_x * sin_x);// 重新计算sin(pitch)
	        }
	
	        //--- Pitch/elevation quaternion calculation
	        if (sin_x == 0)
	        {
		        if (cos_x > 0)
		        {
			        sin_xh = 0;
			        cos_x = 1;
		        }
		        else if (cos_x < 0)
		        {
			        sin_xh = 1;
			        cos_x = -1;
		        }
                else
                {
                    sin_xh = 0;
                }
	        }
	        else
	        {
                sin_xh = (sin_x / Math.Abs(sin_x)) * ((float)Math.Sqrt(0.5 * (1 - cos_x)));
	        }
            cos_xh = (float)Math.Sqrt((0.5 * (1 + cos_x)));
            q_x.y = sin_xh;
            q_x.w = cos_xh;		// pitch quaternion
            //q_x.y = -sin_xh;
            //q_x.w = -cos_xh;		// pitch quaternion

            //--- Roll/bank quaternion calculation
	        sin_y = Acc[1] / cos_x;		// sin(Roll)
	        cos_y = Acc[2] / cos_x;		// cos(Roll)

	        if (sin_y == 0)
	        {
		        if (cos_y > 0)
		        {
			        sin_yh = 0;
			        cos_y = 1;
		        }
		        else if (cos_y < 0)
		        {
			        sin_yh = 1;
			        cos_y = -1;
		        }
                else
                {
                    sin_yh = 0;
                }
	        }// END if (sin_x == 0)
            else if (Math.Abs(sin_y) > th_zero)
	        {
                sin_yh = (sin_y / Math.Abs(sin_y)) * ((float)Math.Sqrt(0.5 * (1 - cos_y)));
	        }
            else
	        {
                sin_yh = (float)Math.Sqrt((0.5 * (1 - cos_y)));
	        }
            cos_yh = (float)Math.Sqrt(0.5 * (1 + cos_y));
            q_y.x = sin_yh;
            q_y.w = cos_yh;		// roll quaternion
            //q_y.x = -sin_yh;
            //q_y.w = -cos_yh;		// roll quaternion


	        //--- yaw/bank quaternion calculation
	        q_x = QuatMultiply(q_x, q_y);			// q_x = q_x*q_y
	        RotateVec(Mag_xy, q_x);			        // Mag_xy = q_x*q_y*Mag_xy*q_y^(-1)*q_x^(-1) rotate measured magnetic vector to Global Coordinate System

            temp = (float)Math.Sqrt(mag[1] * mag[1] + mag[0] * mag[0]);	// mx^2+my^2
	
	        for (int i = 0; i < 2; i++)
	        {
	           Mag_e[i] = mag[i] / temp;	// normalize ---> Mx  My
	        }

            temp = (float)Math.Sqrt(Mag_xy[1] * Mag_xy[1] + Mag_xy[0] * Mag_xy[0]);		// nx^2+ny^2
	        for (int i = 0; i < 2; i++)
	        {
	           Mag_xy[i] = Mag_xy[i] / temp;		// normalize ---> Nx Ny
	        }

	        cos_z = Mag_xy[0] * Mag_e[0] + Mag_xy[1] * Mag_e[1];		// cos(Yaw)
	        sin_z = -Mag_xy[1] * Mag_e[0] + Mag_xy[0] * Mag_e[1];		// sin(Yaw)

	        if (sin_z == 0)
	        {
		        if (cos_z > 0)
		        {
			        sin_zh = 0;
			        cos_z = 1;
		        }
		        else if (cos_z < 0)
		        {
			        sin_zh = 1;
			        cos_z = -1;
		        }
                else
                {
                    sin_zh = 0;
                }
	        }// END if (sin_z == 0)
            else if (Math.Abs(sin_z) > th_zero)
	        {
                sin_zh = (sin_z / Math.Abs(sin_z)) * ((float)Math.Sqrt(0.5 * (1 - cos_z)));
	        }
            else
	        {
                sin_zh = (float)Math.Sqrt((0.5 * (1 - cos_z)));
	        }

            cos_zh = (float)Math.Sqrt((0.5 * (1 + cos_z)));
            q_z.z = sin_zh;
            q_z.w = cos_zh;		// yaw quaternion
            //q_z.z = -sin_zh;
            //q_z.w = -cos_zh;		// yaw quaternion

           //--- whole quaternion calculation
	        q_z = QuatMultiply(q_z, q_x);	// q_z = q_z*q_x*q_y

            if (flag == 1)
	        {
		        q_z = QuatMultiply(q_z, q_alpha);
	        }

	        q_z.normalizeQuat();		// normalize computed q_z

            return q_z;
        }

        /// <summary>
        /// FQA算法估计初始姿态 坐标系 : (X 右 Y 上 Z 前)
        /// </summary>
        /// <param name="refMag">根结点计算的磁参考向量</param>
        /// <param name="accMea">加速度观测向量</param>
        /// <param name="magMea">磁观测向量</param>
        /// <returns>初始姿态</returns>
        /// 
        /// ---------------变量说明----------------///
        /// q_X q_Y q_Z 表示绕对应轴旋转的四元数
        /// sin_X sin_Y sin_Z 表示绕对应轴旋转角的正弦值
        /// sin_Xh sin_Yh sin_Zh 表示绕对应轴旋转的半角正弦值
        /// ---------------变量说明----------------///
        public static Quat FQAUnity(float[] refMag, float[] accMea, float[] magMea)
        {
            // 归一化观测数据(加速度观测 磁观测)
            float[] normlizedAccMea = normalizeMeasure(accMea);
            float[] normlizedMagMea = normalizeMeasure(magMea);

            ////////////Debug////////////
            //Trace.WriteLine(normlizedAccMea[0] + " " + normlizedAccMea[1] + " " + normlizedAccMea[2]);
            //Trace.WriteLine(normlizedMagMea[0] + " " + normlizedMagMea[1] + " " + normlizedMagMea[2]);

            float th_singularity = 0.2f;				// A threshold to determine whether cos(Pitch) is near zero.
            float th_zero = 0.002f;
            float alpha = 0.2f * 3.1416f;		// The offset to rotate Acc and Mag when cos(Pitch) is near ZERO【Y轴旋转角=90°】
            
            // Singularity compensation [rotate alpha]
            Quat q_alpha = new Quat((float)Math.Cos(0.5 * alpha), 0, 0, (float)Math.Sin(0.5 * alpha));		// offset quaternion q_alpha
            
            Quat q_X = new Quat(1, 1, 0, 0);
            Quat q_Y = new Quat(1, 0, 1, 0);
            Quat q_Z = new Quat(1, 0, 0, 1);

            // 奇异性标志位
            bool singularity_flag = false;

            float[] Mag_xy = new float[3];
            float[] Mag_e = new float[3];
            
            // sin () cos() 正余弦
            float sin_X, cos_X;         // Roll  φ
            float sin_Y, cos_Y;         // Yaw   ψ
            float sin_Z, cos_Z;         // Pitch θ

            // sin (/2) cos(/2) 半角正余弦
            float sin_Xh, cos_Xh;       // φ/2
            float sin_Yh, cos_Yh;       // ψ/2       
            float sin_Zh, cos_Zh;       // θ/2

            float[] Acc = new float[3];
            float[] Mag = new float[3];
            float temp_mag_norm;            // temp variable calculate mag vector norm

            for (int i = 0; i < refMag.Length; i++)
            {
                // 归一化重力观测
                Acc[i] = normlizedAccMea[i];
                // 归一化磁观测
                Mag[i] = normlizedMagMea[i];
                Mag_xy[i] = Mag[i];
            }

            // unity坐标系下 [0 g 0] 绕Z旋转 ax = g*sin(θ) ay = g*cos(θ)
            sin_Z = Acc[0];					                // sin(θ)
            cos_Z = (float)Math.Sqrt(1 - sin_Z * sin_Z);	// cos(θ)

            ////////////Debug////////////
            //Trace.WriteLine("sin(Z) = " + sin_Z);
            //Trace.WriteLine("cos(Z) = " + cos_Z);
            
            // singularity occurs
            if (Math.Abs(cos_Z) < th_singularity)					// to determine whether cos(Pitch) is near zero 0.2f
            {
                //Acc = q_alpha * Acc * q_alpha^(-1)
                RotateVec(Acc, q_alpha);
                //Mag = q_alpha * Mag * q_alpha^(-1)
                RotateVec(Mag, q_alpha);

                singularity_flag = true;						    // singularity occurs , flag set to true

                for (int i = 0; i < refMag.Length; i++)
                {
                    Mag_xy[i] = Mag[i];			        // Mag_xy重新赋值(Mag has been rotated)
                }
                sin_Z = Acc[0];				            // Acc has been rotated 重新计算sin(pitch)
                cos_Z = (float)Math.Sqrt(1 - sin_Z * sin_Z);// 重新计算sin(pitch)
            }

            //--- Pitch/elevation quaternion calculation
            if (sin_Z == 0)                             
            {
                if (cos_Z > 0)                      // θ = 0°
                {
                    sin_Zh = 0;
                    cos_Z = 1;
                }
                else if (cos_Z < 0)                 // θ = 180°
                {
                    sin_Zh = 1;
                    cos_Z = -1;
                }
                else                                
                {
                    sin_Zh = 0;
                }
            }
            else
            {
                sin_Zh = (sin_Z / Math.Abs(sin_Z)) * ((float)Math.Sqrt(0.5 * (1 - cos_Z)));
            }
            cos_Zh = (float)Math.Sqrt((0.5 * (1 + cos_Z)));
            q_Z.z = sin_Zh;
            q_Z.w = cos_Zh;		// pitch quaternion

            ////////////Debug////////////
            Trace.WriteLine("q_Z = " + q_Z.w + " " + q_Z.x + " " + q_Z.y + " " + q_Z.z);

            // 接着绕X旋转 ax = g*sin(θ) ay = g*cos(θ)*cos(φ) az = -g*cos(θ)*sin(φ)
            //--- Roll/bank quaternion calculation
            sin_X = -Acc[2] / cos_Z;		    // sin(φ)
            cos_X = Acc[1] / cos_Z;		        // cos(φ)

            ////////////Debug////////////
            //Trace.WriteLine("X = " + sin_X);
            //Trace.WriteLine("X = " + cos_X);

            if (sin_X == 0)
            {
                if (cos_X > 0)              // φ = 0°
                {
                    sin_Xh = 0;
                    cos_X = 1;
                }
                else if (cos_X < 0)         // φ = 180°
                {
                    sin_Xh = 1;
                    cos_X = -1;
                }
                else
                {
                    sin_Xh = 0;
                }
            }
            else if (Math.Abs(sin_X) > th_zero)     // φ > 0°
            {
                sin_Xh = (sin_X / Math.Abs(sin_X)) * ((float)Math.Sqrt(0.5 * (1 - cos_X)));
            }
            else                                    // φ ≈ 0°
            {
                sin_Xh = (float)Math.Sqrt((0.5 * (1 - cos_X)));
            }
            cos_Xh = (float)Math.Sqrt(0.5 * (1 + cos_X));
            q_X.x = sin_Xh;
            q_X.w = cos_Xh;

            ////////////Debug////////////
            Trace.WriteLine("q_X = " + q_X.w + " " + q_X.x + " " + q_X.y + " " + q_X.z);

            //--- yaw/bank quaternion calculation
            Quat qt = QuatMultiply(q_Z, q_X);			// qt = q_Z*q_X

            //rotate measured magnetic vector to Global Coordinate System
            RotateVec(Mag_xy, qt);                      // Mag_xy = q_Z*q_X*Mag_xy*q_X^(-1)*q_Z^(-1)

            ////////////Debug////////////
            //Trace.WriteLine("rotated mag " + Mag_xy[0] + " " + Mag_xy[1] + " " + Mag_xy[2]);


            // 这里要注意 水平面分量现在是 : X & Z
            temp_mag_norm = (float)Math.Sqrt(refMag[2] * refMag[2] + refMag[0] * refMag[0]);	// mx^2+my^2

            // 参考磁向量 水平面分量
            Mag_e[0] = refMag[0] / temp_mag_norm;	    // normalize ---> Mx  My
            Mag_e[2] = refMag[2] / temp_mag_norm;	    // normalize ---> Mx  My

            temp_mag_norm = (float)Math.Sqrt(Mag_xy[2] * Mag_xy[2] + Mag_xy[0] * Mag_xy[0]);		// nx^2+ny^2

            // 磁观测向量旋转到model坐标系下 水平面分量
            Mag_xy[0] = Mag_xy[0] / temp_mag_norm;		// normalize ---> Nx Ny
            Mag_xy[2] = Mag_xy[2] / temp_mag_norm;		// normalize ---> Nx Ny

            cos_Y = Mag_xy[0] * Mag_e[0] + Mag_xy[2] * Mag_e[2];		// cos(Yaw)
            sin_Y = -Mag_xy[2] * Mag_e[0] + Mag_xy[0] * Mag_e[2];		// sin(Yaw)

            ////////////Debug////////////
            //Trace.WriteLine("Y = " + sin_Y);
            //Trace.WriteLine("Y = " + cos_Y);

            if (sin_Y == 0)
            {
                if (cos_Y > 0)                  // ψ = 0
                {
                    sin_Yh = 0;
                    cos_Y = 1;
                }
                else if (cos_Y < 0)             // ψ = 180°
                {
                    sin_Yh = 1;
                    cos_Y = -1;
                }
                else                            
                {
                    sin_Yh = 0;
                }
            }
            else if (Math.Abs(sin_Y) > th_zero) // ψ > 0°
            {
                sin_Yh = (sin_Y / Math.Abs(sin_Y)) * ((float)Math.Sqrt(0.5 * (1 - cos_Y)));
            }
            else                                // ψ ≈ 0°
            {
                sin_Yh = (float)Math.Sqrt((0.5 * (1 - cos_Y)));
            }

            cos_Yh = (float)Math.Sqrt((0.5 * (1 + cos_Y)));
            q_Y.y = sin_Yh;
            q_Y.w = cos_Yh;		            // yaw quaternion
            q_Y = q_Y.invQuat();
            ////////////Debug////////////
            Trace.WriteLine("q_Y = " + q_Y.w + " " + q_Y.x + " " + q_Y.y + " " + q_Y.z);

            //--- whole quaternion calculation
            //q_Y = QuatMultiply(q_X, QuatMultiply(q_Z, q_Y));	// q = q_X*q_Z*q_Y [ or (q_φ * q_θ * q_ψ) ]
            q_Y = QuatMultiply(q_Y, QuatMultiply(q_Z, q_X));
            // 检查是否产生了奇异性
            if (singularity_flag)
            {
                // rotate back
                q_Y = QuatMultiply(q_Y, q_alpha);

                ////////////Debug////////////
                //Trace.WriteLine("Singularity occurs");
            }

            // ensure the output is normalized
            q_Y.normalizeQuat();

            return q_Y;                             // 输出估计的初始姿态
        }


        /// <summary>
        /// 两个向量累加 结果输出到目标向量obj
        /// </summary>
        /// <param name="v">源向量</param>
        /// <param name="obj">目标向量</param>
        public static void SumUpVec(float[] v, float[] obj)
        {
            for (int i = 0; i < v.Length; i++)
            {
                obj[i] += v[i];
            }
        }

        /// <summary>
        /// 求向量均值
        /// </summary>
        /// <param name="v"></param>
        /// <param name="total"></param>
        /// <param name="meanVec">均值化后的向量</param>
        public static void MeanVec(float[] v, int total, float[] meanVec)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] /= (float)total;
                meanVec[i] = v[i];
            }
        }

        /// <summary>
        /// 归一化向量
        /// </summary>
        /// <param name="v">源向量</param>
        public static void NormalizeVec(float[] v)
        {
            float norm = 0.0f;
            for (int i = 0; i < v.Length; i++)
            {
                norm += v[i] * v[i];
            }
            norm = (float)Math.Sqrt(norm);  // 模值

            for (int i = 0; i < v.Length; i++)
            {
                v[i] /= norm;
            }
        }
    }
}