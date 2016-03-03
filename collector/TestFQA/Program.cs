using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using collector;

namespace TestFQA
{
    class Program
    {
        static void Main(string[] args)
        {
            //float[] aMea = { 0, 1, 0};
            //float[] mMea = { 1, 0, 0 };
            //float[] refMag = { 0, 1, 0 };
            ////Quat q1 = //-0.6485133 -0.6621662 -0.375455  Y 90
            //Quat q1 = new Quat(0, 0.4373179f, -0.6482004f, 0.623369F);
            //Quat q2 = new Quat(0.707f, 0, 0.707f, 0);
            
            //Quat q = CalibrationUtil.QuatMultiply(q1, q2.invQuat());
            //q = CalibrationUtil.QuatMultiply(q2, q);
            ////Quat q = CalibrationUtil.FQA(refMag, aMea, mMea);

            //Quat q1 = //-0.3681358f, -0.3918864f, 0.8431496f
            //Quat q1 = new Quat(0, 0.4197861f, - 0.6648057f, - 0.6179102f);
            //Quat q2 = new Quat(0.707f, 0, 0, -0.707f);
            
            //Quat q = CalibrationUtil.QuatMultiply(q1, q2.invQuat());
            //q = CalibrationUtil.QuatMultiply(q2, q);


            //float[][] aMea = new float[][] { new float[] { 0, 1, 0 }, new float[] { 0, 0, -1 }, new float[] { 0, 1, 0 }, new float[] { 1, 0, 0 }, new float[] { 0.866f, 0.5f, 0 } };
            //float[][] mMea = new float[][] { new float[] { 1, 0, 0 }, new float[] { 1, 0, 0 }, new float[] { 1, 0, 0 }, new float[] { 0, -1, 0 }, new float[] { 0.5f, -0.866f, 0 } };
            //float[] mRef = new float[] { 1, 0, 0 };

            //Quat q;
            //for (int i = 0; i < 5; i++)
            //{
            //    q = Calibration.FQAUnity(mRef, aMea[i], mMea[i]);
            //    Trace.WriteLine(q.w + " " + q.x + " " + q.y + " " + q.z);
            //    Trace.WriteLine("--------------------------------------");
            //}


            Quat q;
            float[] mRef = new float[] { -0.1855289f, -0.8060142f, 0.5620677f };
            float[] aMea = new float[] { -0.1139f, -0.0382f, -0.9928f };
            float[] mMea = new float[] {  0.3931f,    0.4425f,    0.8060f};

            q = Calibration.FQAUnity(mRef, aMea, mMea);
            Trace.WriteLine(q.w + " " + q.x + " " + q.y + " " + q.z);
            Trace.WriteLine("--------------------------------------");
        }
    }
}
