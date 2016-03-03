using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetWork;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Threading;
using waveformClib;

namespace collector
{
    delegate void SafeRefreshNodesList(Node node);
    delegate void SafeMotionTexRefresh(byte ID, float[] acc, float[] meg, float[] gyo);

    public partial class Main_Form : Form

    {

        private NetWork.UdpNetWork netWork;
        private myState state = myState.stop;
        private Profiles_Form fProfiles;
        //存储波形颜色
        private Color[] waveColor = new Color[] { Color.Orange, Color.Blue, Color.Red, Color.Purple, Color.Yellow, Color.Pink, Color.Silver, Color.Green };
        enum myState
        {
            start,
            stop
        }
        public Main_Form()
        {
            InitializeComponent();

            //Control.CheckForIllegalCrossThreadCalls = false;
            xAxisText.Text = waveform1.VisualGetX().ToString(); //获取绘图控件坐标范围
            yAxisText.Text = waveform1.VisualGetY().ToString();
            Start.Enabled = false;
            Profiles.Enabled = false;
            netWork = new UdpNetWork(); 
            netWork.UdpInIt();
            netWork.EchoArrived += new EchoHandler(RefreshNodesList);//绑定udp广播应答委托，用于刷新节点信息。

        }

        //更改绘图坐标范围
        private void AxisSet_Click(object sender, EventArgs e)
        {
            int x, y;
            if (int.TryParse(xAxisText.Text.ToString(), out x) && int.TryParse(yAxisText.Text.ToString(), out y))
            {
                waveform1.VisualSet(x, y);
            }
        }
        //广播获取子节点信息
        private void Boardcast_Click(object sender, EventArgs e)
        {
            netWork.Boardcast((byte)Code.BROADCAST);
        }

        //刷新子节点列表
        private void RefreshNodesList(Node node)
        {
            if (NodeList.InvokeRequired)
            {
                SafeRefreshNodesList objSet = new SafeRefreshNodesList(RefreshNodesList);
                NodeList.Invoke(objSet, new object[] {  node });
            }
            else
            {
                System.Windows.Forms.ListViewItem listViewItem = new System.Windows.Forms.ListViewItem(new string[] {
                //node.ID.ToString(),
                node.profiles.ID.ToString(),
                node.ipEndPoint.Address.ToString(),
                node.profiles.EMG_RATE.ToString()}, -1);

                NodeList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
                listViewItem});

                node.EmgDataArrived += new EmgDataHandler(DrawWave);
                node.MotionDataArrived += new MotionDataHandler(MotionTexRefresh);
            }
        }

        //绘制波形
        public void DrawWave(byte ID, byte channl, double x1, double y1, double x2, double y2)
        {
            //已知bug偏置为负，为什么上移？
            int index = netWork.chlDic[ID * 4 + channl];
            double offset = - 0.007 + 0.002 * index;
            waveform1.drawpoint(x1, y1 + offset, x2, y2 + offset, waveColor[index]);            
        }

        /// <summary>
        /// 控件刷新运动数据
        /// </summary>
        /// <param name="ID">node ID</param>
        /// <param name="acc">对应ID结点的acc数据</param>
        /// <param name="mag">对应ID结点的gyr数据</param>
        /// <param name="gyo">对应ID结点的mag数据</param>
        public void MotionTexRefresh(byte ID, float[] acc, float[] mag, float[] gyo)
        {
            if (this.InvokeRequired)
            {
                SafeMotionTexRefresh objSet = new SafeMotionTexRefresh(MotionTexRefresh);
                this.Invoke(objSet, new object[] { ID, acc, mag, gyo });
            }
            else
            {
                x_acc_text.Text = acc[0].ToString();
                y_acc_text.Text = acc[1].ToString();
                z_acc_text.Text = acc[2].ToString();
                x_meg_text.Text = mag[0].ToString();
                y_meg_text.Text = mag[1].ToString();
                z_meg_text.Text = mag[2].ToString();
                x_gyo_text.Text = gyo[0].ToString();
                y_gyo_text.Text = gyo[1].ToString();
                z_gyo_text.Text = gyo[2].ToString();

                // 根据ID找相应结点
                Node node = netWork.nodes.Find(
                                delegate(Node nd)
                                {
                                    return nd.profiles.ID == ID;
                                }
                    );

                // 向本地写入运动数据 & 姿态
                if (ID != CollectorUtil.ROOTID)
                {
                    //BUG “System.IO.IOException”类型的第一次机会异常在 mscorlib.dll 中发生
                    //CollectorUtil.WriteFile(@"C:\Users\Desktop\", "nodeQuaternion.txt", node.qGB, false);
                }

                // 驱动unity模型
                axUnityWebPlayer1.SendMessage("Male", "ReceiveMsg", ID + " " + node.qGB.x + " " + node.qGB.y + " " + node.qGB.z + " " + node.qGB.w);

            }
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            foreach(Node node in netWork.nodes)
            {
                node.Connect();
            }
            Start.Enabled = true;
            Profiles.Enabled = true;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            byte[] code;
            DirectoryInfo di;

            //
            if(state == myState.stop)
            {
                //code = new byte[] { (byte)Code.CONTROL, 0x00 };
                ((Button)sender).Text = "Stop";
                state = myState.start;
                netWork.Boardcast((byte)Code.START_RAW);
            }
            else
            {
                ((Button)sender).Text = "Start";
                state = myState.stop;
                netWork.Boardcast((byte)Code.STOP_RAW);
                Trace.WriteLine("Disconnect");
                
                di = System.IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd HH：m：ss"));

                //等待缓存处理完毕
                Thread.Sleep(1000);

                //写文件操作，存储数据
                foreach(Node node in netWork.nodes)
                {
                    DirectoryInfo diTemp = System.IO.Directory.CreateDirectory(di.FullName + "\\Motion" + node.profiles.ID.ToString());// netWork.nodes.IndexOf(node).ToString() node.ID.ToString()
                    //accerData
                    FileStream accFs = new FileStream(diTemp.FullName + "\\" + "acc.txt", FileMode.Create);
                    StreamWriter accSw = new StreamWriter(accFs, Encoding.Default);
                    foreach (float[] acc in node.acc)
                    {
                        accSw.WriteLine(acc[0] + "  " + acc[1] + "   " + acc[2]);
                    }
                    //accSw.WriteLine("END");
                    accSw.Close();
                    accFs.Close();

                    FileStream megFs = new FileStream(diTemp.FullName + "\\" + "meg.txt", FileMode.Create);
                    StreamWriter megSw = new StreamWriter(megFs, Encoding.Default);
                    foreach (float[] meg in node.meg)
                    {
                        megSw.WriteLine(meg[0] + "  " + meg[1] + "   " + meg[2]);
                    }
                    //megSw.WriteLine("END");
                    megSw.Close();
                    megFs.Close();

                    FileStream gyoFs = new FileStream(diTemp.FullName + "\\" + "gyo.txt", FileMode.Create);
                    StreamWriter gyoSw = new StreamWriter(gyoFs, Encoding.Default);
                    foreach (float[] gyo in node.gyo)
                    {
                        gyoSw.WriteLine(gyo[0] + "  " + gyo[1] + "   " + gyo[2]);
                    }
                    //gyoSw.WriteLine("END");
                    gyoSw.Close();
                    gyoFs.Close();

                    FileStream qGSFs = new FileStream(diTemp.FullName + "\\" + "qGS.txt", FileMode.Create);
                    StreamWriter qGSSw = new StreamWriter(qGSFs, Encoding.Default);
                    foreach (Quat qGS in node.LqGS)
                    {
                        qGSSw.WriteLine(qGS.w + "  " + qGS.x + "   " + qGS.y + " " + qGS.z);
                    }
                    //gyoSw.WriteLine("END");
                    qGSSw.Close();
                    qGSFs.Close();

                    FileStream qGBFs = new FileStream(diTemp.FullName + "\\" + "qGB.txt", FileMode.Create);
                    StreamWriter qGBSw = new StreamWriter(qGBFs, Encoding.Default);
                    foreach (Quat qGB in node.LqGB)
                    {
                        qGBSw.WriteLine(qGB.w + "  " + qGB.x + "   " + qGB.y + " " + qGB.z);
                    }
                    //gyoSw.WriteLine("END");
                    qGBSw.Close();
                    qGBFs.Close();

                    DirectoryInfo diTemp2 = System.IO.Directory.CreateDirectory(di.FullName + "\\EMG" + node.profiles.ID.ToString());// netWork.nodes.IndexOf(node).ToString() node.ID.ToString()
                    FileStream emgFs = new FileStream(diTemp2.FullName + "\\" + "emg.txt", FileMode.Create);
                    StreamWriter emgSw = new StreamWriter(emgFs, Encoding.Default);
                    foreach (double[] emg in node.sEmgData)
                    {
                        emgSw.WriteLine(emg[0] + "  " + emg[1] + "   " + emg[2] + "   " + emg[3]);
                    }
                    emgSw.WriteLine("END");
                    emgSw.Close();
                    emgFs.Close();
                }
               
            }
            
        }

        private void Profiles_Click(object sender, EventArgs e)
        {
            if (netWork.nodes.Count == 0)
            {
                //MessageBox msgBox = new MessageBox("No node!");
                MessageBox.Show("No node!");
            }
            else
            {
                if (fProfiles == null || fProfiles.IsDisposed)
                {
                    fProfiles = new Profiles_Form(netWork);
                    fProfiles.Show();
                }
                else
                {
                    fProfiles.Activate();
                }
            }
        }

    }
}
