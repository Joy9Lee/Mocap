using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace collector
{
    public static class CollectorUtil
    {
        /*------------------Const Variable------------------*/
        public static byte ROOTID = 0;          // 根结点ID
        public static float SAMPLERATE = 100.0f;
        public static float KP = 5.0f;

        // model -> 结点 body
        public static Dictionary<byte, Quat> qGB0Dict = new Dictionary<byte, Quat>();

        // 结点 sensor坐标系 -> body坐标系
        public static Dictionary<byte, Quat> qSBDict = new Dictionary<byte, Quat>();

        /*------------------Util Functions------------------*/
        /// <summary>
        /// 为指定ID的结点设置qGB0 [这步操作在初始化结点时完成]
        /// </summary>
        /// <param name="ID">结点ID</param>
        public static void setQGB0ForNode(byte ID)
        {
            switch(ID)
            {
                // 根结点
                case 0:
                    qGB0Dict.Add(ID, new Quat(1, 0, 0, 0));
                    break;
                
                // 子结点
                case 1:
                    qGB0Dict.Add(ID, new Quat(0.707f, 0.707f, 0, 0));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 将传感器数据写入指定路径下的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="Acc"></param>
        /// <param name="Gyr"></param>
        /// <param name="Mag"></param>
        /// <param name="IsFileExists">本地文件是否存在</param>
        public static void WriteFile(string path, string fileName, float[] Acc, float[] Gyr, float[] Mag, bool IsFileExists)
        {
            string txtfile = path + fileName;

            if (IsFileExists)
            {
                File.WriteAllText(txtfile, string.Empty);   // 文件已存在 那就先清空内容
                IsFileExists = false;
            }

            string accStr = string.Empty;
            foreach (float f in Acc)
            {
                accStr += f.ToString();
                accStr += "\t";
            }

            string gyrStr = string.Empty;
            foreach (float f in Gyr)
            {
                gyrStr += f.ToString();
                gyrStr += "\t";
            }

            string magStr = string.Empty;
            foreach (float f in Mag)
            {
                magStr += f.ToString();
                magStr += "\t";
            }

            File.AppendAllLines(txtfile, new string[] { accStr + "\t" + gyrStr + "\t" + magStr}, Encoding.Default);
        }

        public static void WriteFile(string path, string fileName, Quat q, bool IsFileExists)
        {
            string txtfile = path + fileName ;

            if (IsFileExists)
            {
                File.WriteAllText(txtfile, string.Empty);   // 文件已存在 那就先清空内容
                IsFileExists = false;
            }

            File.AppendAllLines(txtfile, new string[] { q.w + "\t" + q.x + "\t" + q.y + "\t" + q.z }, Encoding.Default);
        }

        public static void WriteFile(string path, string fileName, float[] vec, bool IsFileExists)
        {
            string txtfile = path +  fileName;

            if (IsFileExists)
            {
                File.WriteAllText(txtfile, string.Empty);   // 文件已存在 那就先清空内容
                IsFileExists = false;
            }

            File.AppendAllLines(txtfile, new string[] { vec[0] + "\t" + vec[1] + "\t" + vec[2]}, Encoding.Default);
        }
    }
}