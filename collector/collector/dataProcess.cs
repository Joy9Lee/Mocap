using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace dataProcess
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PacketHeader
    {
        //未解决c struct与c# struct内存对其问题，需要将变量按字长排序
        public byte version;
        public byte key;
        public short length;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct Profiles
    {
        //未解决c struct与c# struct内存对其问题，需要将变量按字长排序
        public byte     ID;
        public byte     BAT;
        public byte     MOTION_EN;
        public byte     EMG_CHL;
        public short    MOTION_RATE;
        public short    EMG_RATE;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public float[]		ACC_FACTOR;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public float[]		ACC_BIAS;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public float[]		MEG_FACTOR;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public float[]		MEG_BIAS;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public float[]		GYO_FACTOR;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public float[]		GYO_BIAS;

    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MotionData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Int16[] Acc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Int16[] Meg;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Int16[] Gyo;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct EmgData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ch1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ch2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ch3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ch4;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MotionPackage
    {
        public byte Type;
        public byte TimeStape;
        public Int16 Len;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public MotionData[] Motiondata;
        public Int16 CkSum;
        
    }

    public static class Community
    {
        public static byte[] StructToBytes(object obj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(obj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(obj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }

        public static object BytesToStruct(byte[] bytes, Type type)
        {
            //得到结构的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构
            return obj;
        }

        public static object Motion_BytesToStruct(byte[] bytes, Type type, int i)
        {
            //得到结构的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 3 + 36 * i, structPtr, size);
            //将内存空间转换为目标结构
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构
            return obj;
        }
    }

    public class utility
    {


        /// <summary>
        /// 补码转源码
        /// </summary>
        public static double Convert(double x)
        {
            double y = 0;
            if (x >= 8388608)
                y = 2.4 * (x - 16777215) / (8388607);
            //dataOut = 2.4*((x-16777215)>>23);
            else if (x < 8388608)
                y = 2.4 * x / (8388607);
            //dataOut= 2.4*(x>>23);
            return y;
        }
       //3byte拼接
        public static int Merge(int x, int y, int z)
        {
            int data;
            data = x * 65536 + y * 256 + z;
            return data;
        }
    }
    //数字滤波器
    public class Filter
    {

        private double[] CoefA;
        private double[] CoefB;
        private double[] Input_temp;
        private double[] Output_temp;
        private int Order;

        public Filter(double[] myCoefA, double[] myCoefB, int myOrder)
        {
            if (myCoefA.Length != myOrder || myCoefB.Length != myOrder)
            {
                throw new ApplicationException("Order isn't matched");
            }
            CoefA = myCoefA;
            CoefB = myCoefB;
            Order = myOrder;
            Input_temp = new double[Order - 1];
            Output_temp = new double[Order - 1];

        }

        public double Filtering(double x)
        {
            int i = 0;
            double y = 0;
            // IIR/FIR Filter
            y = CoefB[0] * x;
            for (i = 1; i < Order; i++)
            {
                y += CoefB[i] * Input_temp[i - 1] - CoefA[i] * Output_temp[i - 1];
            }
            for (i = Order - 2; i > 0; i--)
            {
                Input_temp[i] = Input_temp[i - 1];
                Output_temp[i] = Output_temp[i - 1];
            }
            Input_temp[0] = x;
            Output_temp[0] = y;
            return y;
        }
    }


}
