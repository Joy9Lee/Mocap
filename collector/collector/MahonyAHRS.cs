using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace collector
{
    public class MahonyAHRS
    {
        //************************************//

        // 采样周期
        public float SamplePeriod { get; set; }

        // 比例因子
        public float Kp { get; set; }

        // 积分增益
        public float Ki { get; set; }

        // 输出四元数
        public float[] Quaternion;

        // 积分误差
        private float[] eInt { get; set; }



        //************************************//
        // 构造函数
        public MahonyAHRS(Quat qGS, float samplePeriod)
            : this(qGS, samplePeriod, 1f, 0f)
        {

        }

        public MahonyAHRS(Quat qGS, float samplePeriod, float kp)
            : this(qGS, samplePeriod, kp, 0f)
        {

        }

        public MahonyAHRS(Quat qGS, float samplePeriod, float kp, float ki)
        {
            SamplePeriod = samplePeriod;
            Kp = kp;
            Ki = ki;
            
            Quaternion = new float[] {qGS.w, qGS.x, qGS.y, qGS.z};
            
            eInt = new float[] { 0f, 0f, 0f };
        }

        //************************************//

        /// <summary>
        /// 梯度下降法 — 融合加速度计 & 陀螺仪 & 磁力计
        /// </summary>
        /// <param name="gx"></param>
        /// <param name="gy"></param>
        /// <param name="gz"></param>
        /// <param name="ax"></param>
        /// <param name="ay"></param>
        /// <param name="az"></param>
        /// <param name="mx"></param>
        /// <param name="my"></param>
        /// <param name="mz"></param>
        public void Update(float gx, float gy, float gz, float ax, float ay, float az, float mx, float my, float mz)
        {
            float q1 = Quaternion[0], q2 = Quaternion[1], q3 = Quaternion[2], q4 = Quaternion[3];   // short name local variable for readability
            float norm;
            float hx, hy, bx, bz;
            float vx, vy, vz, wx, wy, wz;
            float ex, ey, ez;
            float pa, pb, pc;

            float hz, by;

            // Auxiliary variables to avoid repeated arithmetic
            float q1q1 = q1 * q1;
            float q1q2 = q1 * q2;
            float q1q3 = q1 * q3;
            float q1q4 = q1 * q4;
            float q2q2 = q2 * q2;
            float q2q3 = q2 * q3;
            float q2q4 = q2 * q4;
            float q3q3 = q3 * q3;
            float q3q4 = q3 * q4;
            float q4q4 = q4 * q4;

            // Normalize accelerometer measurement
            norm = (float)Math.Sqrt(ax * ax + ay * ay + az * az);
            if (norm == 0f)
                return; // handle NaN
            norm = 1 / norm;        // use reciprocal for division
            ax *= norm;
            ay *= norm;
            az *= norm;

            // Normalize magnetometer measurement
            norm = (float)Math.Sqrt(mx * mx + my * my + mz * mz);
            if (norm == 0f)
                return; // handle NaN
            norm = 1 / norm;        // use reciprocal for division
            mx *= norm;
            my *= norm;
            mz *= norm;

            // Reference direction of Earth's magnetic field
            hx = 2f * mx * (0.5f - q3q3 - q4q4) + 2f * my * (q2q3 - q1q4) + 2f * mz * (q2q4 + q1q3);
            hy = 2f * mx * (q2q3 + q1q4) + 2f * my * (0.5f - q2q2 - q4q4) + 2f * mz * (q3q4 - q1q2);

            hz = 2f * mx * (q2q4 - q1q3) + 2f * my * (q3q4 + q1q2) + 2f * mz * (0.5f - q2q2 - q3q3);

            //DirectoryInfo di;
            //di = System.IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\MagRefVec");


            //CollectorUtil.WriteFile(di.FullName, "\\MagRefVec.txt", new float[3]{hx, hy, hz}, false);

            // 地磁水平方向和垂直方向分量
            //bx = (float)Math.Sqrt((hx * hx) + (hy * hy));
            //bz = 2f * mx * (q2q4 - q1q3) + 2f * my * (q3q4 + q1q2) + 2f * mz * (0.5f - q2q2 - q3q3);

            bx = hx;
            by = hy;
            bz = hz;

            //// Estimated direction of gravity and magnetic field
            //vx = 2f * (q2q4 - q1q3);
            //vy = 2f * (q1q2 + q3q4);
            //vz = q1q1 - q2q2 - q3q3 + q4q4;

            // Estimated direction of gravity Eg = [0 0 1 0] Y轴
            vx = 2.0f * (q1 * q4 + q2 * q3);
            vy = q1 * q1 - q2 * q2 - q4 * q4 + q3 * q3;
            vz = 2.0f * (q3 * q4 - q1 * q2);

            //wx = 2f * bx * (0.5f - q3q3 - q4q4) + 2f * bz * (q2q4 - q1q3);
            //wy = 2f * bx * (q2q3 - q1q4) + 2f * bz * (q1q2 + q3q4);
            //wz = 2f * bx * (q1q3 + q2q4) + 2f * bz * (0.5f - q2q2 - q3q3);

            wx = 2f * bx * (0.5f - q3q3 - q4q4) + 2f * by * (q1q4 + q2q3) + 2f * bz * (q2q4 - q1q3);
            wy = 2f * bx * (q2q3 - q1q4) + 2f * by * (0.5f - q2q2 - q4q4) + 2f * bz * (q1q2 + q3q4);
            wz = 2f * bx * (q1q3 + q2q4) + 2f * by * (q3q4 - q1q2) + 2f * bz * (0.5f - q2q2 - q3q3);

            // Error is cross product between estimated direction and measured direction of gravity
            ex = (ay * vz - az * vy) + (my * wz - mz * wy);
            ey = (az * vx - ax * vz) + (mz * wx - mx * wz);
            ez = (ax * vy - ay * vx) + (mx * wy - my * wx);

            if (Ki > 0f)
            {
                eInt[0] += ex;      // accumulate integral error
                eInt[1] += ey;
                eInt[2] += ez;
            }
            else
            {
                eInt[0] = 0.0f;     // prevent integral wind up
                eInt[1] = 0.0f;
                eInt[2] = 0.0f;
            }

            // Apply feedback terms
            gx = gx + Kp * ex + Ki * eInt[0];
            gy = gy + Kp * ey + Ki * eInt[1];
            gz = gz + Kp * ez + Ki * eInt[2];

            // Integrate rate of change of quaternion
            pa = q2;
            pb = q3;
            pc = q4;
            q1 = q1 + (-q2 * gx - q3 * gy - q4 * gz) * (0.5f * SamplePeriod);
            q2 = pa + (q1 * gx + pb * gz - pc * gy) * (0.5f * SamplePeriod);
            q3 = pb + (q1 * gy - pa * gz + pc * gx) * (0.5f * SamplePeriod);
            q4 = pc + (q1 * gz + pa * gy - pb * gx) * (0.5f * SamplePeriod);

            // Normalise quaternion
            norm = (float)Math.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4);
            norm = 1.0f / norm;
            Quaternion[0] = q1 * norm;
            Quaternion[1] = q2 * norm;
            Quaternion[2] = q3 * norm;
            Quaternion[3] = q4 * norm;
        }

        /// <summary>
        /// 梯度下降法 — 融合加速度计 & 陀螺仪
        /// </summary>
        /// <param name="gx"></param>
        /// <param name="gy"></param>
        /// <param name="gz"></param>
        /// <param name="ax"></param>
        /// <param name="ay"></param>
        /// <param name="az"></param>
        public void Update(float gx, float gy, float gz, float ax, float ay, float az)
        {
            float q1 = Quaternion[0], q2 = Quaternion[1], q3 = Quaternion[2], q4 = Quaternion[3];   // short name local variable for readability
            float norm;
            float vx, vy, vz;
            float ex, ey, ez;
            float pa, pb, pc;

            // Normalise accelerometer measurement
            norm = (float)Math.Sqrt(ax * ax + ay * ay + az * az);
            if (norm == 0f)
                return; // handle NaN
            norm = 1 / norm;        // use reciprocal for division
            ax *= norm;
            ay *= norm;
            az *= norm;

            //// Estimated direction of gravity Eg = [0 0 0 1] Z轴
            //vx = 2.0f * (q2 * q4 - q1 * q3);
            //vy = 2.0f * (q1 * q2 + q3 * q4);
            //vz = q1 * q1 - q2 * q2 - q3 * q3 + q4 * q4;

            // Estimated direction of gravity Eg = [0 0 1 0] Y轴
            vx = 2.0f * (q1 * q4 + q2 * q3);
            vy = q1 * q1 - q2 * q2 - q4 * q4 + q3 * q3;
            vz = 2.0f * (q3 * q4 - q1 * q2);

            // Error is cross product between estimated direction and measured direction of gravity
            ex = (ay * vz - az * vy);
            ey = (az * vx - ax * vz);
            ez = (ax * vy - ay * vx);
            if (Ki > 0f)
            {
                eInt[0] += ex;      // accumulate integral error
                eInt[1] += ey;
                eInt[2] += ez;
            }
            else
            {
                eInt[0] = 0.0f;     // prevent integral wind up
                eInt[1] = 0.0f;
                eInt[2] = 0.0f;
            }

            // Apply feedback terms
            gx = gx + Kp * ex + Ki * eInt[0];
            gy = gy + Kp * ey + Ki * eInt[1];
            gz = gz + Kp * ez + Ki * eInt[2];

            // Integrate rate of change of quaternion
            pa = q2;
            pb = q3;
            pc = q4;
            q1 = q1 + (-q2 * gx - q3 * gy - q4 * gz) * (0.5f * SamplePeriod);
            q2 = pa + (q1 * gx + pb * gz - pc * gy) * (0.5f * SamplePeriod);
            q3 = pb + (q1 * gy - pa * gz + pc * gx) * (0.5f * SamplePeriod);
            q4 = pc + (q1 * gz + pa * gy - pb * gx) * (0.5f * SamplePeriod);

            // Normalise quaternion
            norm = (float)Math.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4);
            norm = 1.0f / norm;
            Quaternion[0] = q1 * norm;
            Quaternion[1] = q2 * norm;
            Quaternion[2] = q3 * norm;
            Quaternion[3] = q4 * norm;
        }

        /// <summary>
        /// 角度转换成弧度
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public float deg2rad(float degrees)
        {
            return (float)(Math.PI / 180) * degrees;
        }
    }
}
