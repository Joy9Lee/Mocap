using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Drawing;
using dataProcess;
using collector;
using System.Runtime.InteropServices;

namespace NetWork
{
    public delegate void EchoHandler(Node node);    //收到子节点应答，更新子节点信息
    public delegate void EmgDataHandler(byte ID, byte channl, double x1, double y1, double x2, double y2);//处理显示sEMG信息
    public delegate void MotionDataHandler(byte ID, float[] acc, float[] meg, float[] gyo);//处理显示运动信息
    public enum Code    //传输指令代码
    {
        //FROM PC
        BROADCAST   = (byte)0x20,
        START_RAW	= (byte)0x21,
        STOP_RAW	= (byte)0x22,
        START_IMU	= (byte)0x23,
        STOP_IMU	= (byte)0x24,
        START_EMG	= (byte)0x25,
        STOP_EMG	= (byte)0x26,
        RESET	    = (byte)0x27,
        HIBERNATE   = (byte)0x28,

        //TO PC
        ACK = (byte)0x33,
        MOTION = (byte)0x34,
        EMG = (byte)0x35,
        MESSAGE = (byte)0x36
    }

    //Udp网络类，用于广播及控制
    public class UdpNetWork
    {
        public List<Node> nodes = new List<Node> { };
        public Dictionary<int, int> chlDic = new Dictionary<int, int> { }; //用于绑定(ID chl)与显示通道
        private Byte[] sendBytes = new Byte[4];
        private UdpClient udpClient;
        private Thread Udplistener;
        private IPEndPoint boardcast;
        public event EchoHandler EchoArrived;

        public void UdpInIt()
        {
            udpClient = new UdpClient(5000);
            IPAddress ip = IPAddress.Parse("192.168.1.255"); 
            //IPAddress ip = IPAddress.Broadcast;//双网卡可能会出现bug,不能确定使用哪个网卡。
            boardcast = new IPEndPoint(ip, 5002);
            //udpClient.Connect(boardcastAdd, 5001);
            Udplistener = new Thread(new ThreadStart(Listener));
            Udplistener.Start();
        }
        /*
        /// <summary>
        /// 广播获取子节点信息
        /// </summary>
        public void Boardcast(byte[] code)
        {
            switch (code[0])
            {
                case (byte)Code.BROADCAST:
                    udpClient.Send(code, code.Length, boardcast);
                    return;
                case (byte)Code.CONTROL:
                    sendBytes =code;
                    udpClient.Send(code, code.Length, boardcast);
                    return;
            }
        }
        */
        /// <summary>
        /// 广播获取子节点信息
        /// </summary>
        public void Boardcast(byte code)
        {
            byte[] package = new byte[4];    //创建发送数据包数组
            //填充包头
            package[0] = 0xA0; //协议版本
            package[1] = code;
            package[2] = 0;  //填充数据长度
            package[3] = 0;
            udpClient.Send(package, 4, boardcast);
        }
        /*
        public void Boardcast(byte code, byte[] content)
        {
            byte[] package = new byte[code + content.Length];    //创建发送数据包数组
            //填充包头
            package[0] = 0xA0; //协议版本
            package[1] = code;
            package[2] = (byte)((content.Length & 0x0000ff00) >> 8);  //填充数据长度
            package[3] = (byte)(content.Length & 0x000000ff);
        }
        */
        //监听子节点应答，用于第一次建立连接
        public void Listener()
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Node nodeTemp;
            Byte[] receiveBytes;
            while (true)
            {
                bool existing = false;
                receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                Trace.WriteLine("GetUdpPackage");
                if (receiveBytes[0] == 0xA0 && receiveBytes[1] == 0x20 && receiveBytes[2] == 80)
                {

                    foreach (Node node in nodes)
                    {
                        //The udp Port is 5002, TCP is 5001, Address type can't used to judge whether is equal.
                        if (node.ipEndPoint.Address.ToString() == RemoteIpEndPoint.Address.ToString())
                        {
                            existing = true;
                        }

                    }
                    if (existing == false)
                    {
                        //提取结构体buffer
                        byte[] temp = new byte[receiveBytes.Length - 4];
                        Array.Copy(receiveBytes, 4, temp, 0, temp.Length);

                        RemoteIpEndPoint.Port = 5001;    //TCP Port is 5001
                        //nodeTemp = new Node(RemoteIpEndPoint, receiveBytes[1]);
                        nodeTemp = new Node(RemoteIpEndPoint, temp);
                        nodes.Add(nodeTemp);
                        //Trigger a even
                        EchoArrived(nodeTemp);
                        for (byte i = 0; i < 4; i++)
                        {
                            if (Convert.ToBoolean((nodeTemp.profiles.EMG_CHL >> (7 - i)) & 0x01))
                            {
                                chlDic.Add(nodeTemp.profiles.ID * 4 + i, chlDic.Count); //添加肌电通道,字典索引
                            }
                        }
                    }

                }
                else
                {
                    Trace.WriteLine("UDP Header error!");
                }
            }
        }
    }
    //节点类，用于连接后的数据传输
    public class Node
    {

        /*******************************************
        /-----------------Variables----------------/
        *******************************************/

        public IPEndPoint ipEndPoint;           //ip,port信息
        public Profiles profiles;
        private Socket clientSocket;            //子结点通信socket
        private double timesTamp = 0.001;       //时间标记


        
        //滤波器系数
        private double[] coefA_HighPass = new double[] { 1, -2.994167310674, 2.988351619079, -0.9941842836731 };
        private double[] coefB_HighPass = new double[] { 0.9970879016782, -2.991263705035, 2.991263705035, -0.9970879016782 };
        private double[] coefA_Notch50 = new double[] { 1, -1.8902954887884542, 0.98757429910907057 };
        private double[] coefB_Notch50 = new double[] { 0.99378714955453529, -1.8902954887884542, 0.99378714955453529 };
        
        //滤波器对象
        private Filter[] highPass = new Filter[4];
        private Filter[] notch50 = new Filter[4];
        
        //sEMG信号
        public List<double[]> sEmgData = new List<double[]> { };
        
        //九轴信号
        public List<float[]> acc = new List<float[]> { };
        public List<float[]> meg = new List<float[]> { };
        public List<float[]> gyo = new List<float[]> { };

        public List<Quat> LqGS = new List<Quat> { };
        public List<Quat> LqGB = new List<Quat> { };

        //校准工具类 姿态估计类
        private Calibration cali = new Calibration();       // 初始数据校准类
        public MahonyAHRS fusionAHRS;                               // 姿态估计类 (for > 200) 

        //子结点初始姿态 驱动模型四元数
        public Quat qGS0;               // 各结点初始姿态
        public Quat qGB;                // 驱动unity模型的四元数

        /*******************************************
        /-------------------Events-----------------/
        *******************************************/

        // 事件
        public event EmgDataHandler EmgDataArrived;
        public event MotionDataHandler MotionDataArrived;

        /*******************************************
        /-----------------Functions----------------/
        *******************************************/

        /// <summary>
        /// 各结点构造函数 结点初始化操作
        /// </summary>
        /// <param name="myIpEnd">结点IP</param>
        /// <param name="myNodeID">结点ID</param>
        public Node(IPEndPoint myIpEnd, Byte myNodeID)
        {
            ipEndPoint = myIpEnd;
            profiles.ID = myNodeID;
            sEmgData.Add(new double[4]);

            CollectorUtil.setQGB0ForNode(profiles.ID);         // 为各结点设置初始qGB0 [全局坐标系 -> 各关节坐标系]
        }

        public Node(IPEndPoint myIpEnd, Byte[] msg)
        {
            ipEndPoint = myIpEnd;
            profiles = (Profiles)Community.BytesToStruct(msg, profiles.GetType());

            for (int i = 0; i < 4; i++)
            {
                highPass[i] = new Filter(coefA_HighPass, coefB_HighPass ,4);
                notch50[i] = new Filter(coefA_Notch50, coefB_Notch50, 3);
            }
                sEmgData.Add(new double[4]);
            CollectorUtil.setQGB0ForNode(profiles.ID);          // 为各结点设置初始qGB0
        }

        public void Connect()
        {
            //根据通道数初始化滤波器
            for (int i = 0; i < 4; i++)
            {
                if (Convert.ToBoolean((profiles.EMG_CHL >> i) & 0x01))
                {
                    highPass[i] = new Filter(coefA_HighPass, coefB_HighPass, 4);
                    notch50[i] = new Filter(coefA_Notch50, coefB_Notch50, 3);
                }
            }
                //创建Socket对象TCP
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //这是一个异步的建立连接，当连接建立成功时调用connectCallback方法
                IAsyncResult result = clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallback), clientSocket);
                //这里做一个超时的监测，当连接超过5秒还没成功表示超时
                bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                if (!success)
                {
                    //超时
                    Closed();
                    Trace.WriteLine("connect Time Out");
                }
                else
                {
                    //与socket建立连接成功，开启线程接受服务端数据。
                    Thread thread = new Thread(new ThreadStart(ReceiveSorket));
                    thread.IsBackground = true;
                    thread.Start();
                }
            
        }

        private void ConnectCallback(IAsyncResult asyncConnect)
        {
            Trace.WriteLine("Device No." + profiles.ID + " has Connected Succeed");
            //Refrash ListView
        }

        //关闭Socket
        public void Closed()
        {

            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            clientSocket = null;
        }

        private void ReceiveSorket()
        {
            int mDataLen = Marshal.SizeOf(typeof(MotionData));
            int eDataLen = Marshal.SizeOf(typeof(EmgData));
            int mPackLen = mDataLen * 10 + 4;
            int timeTag = 0;
            //在这个线程中接收节点采集的数据
            while (true)
            {
                if (!clientSocket.Connected)
                {
                    //与服务器断开连接跳出循环
                    Trace.WriteLine("Failed to clientSocket server.");
                    clientSocket.Close();
                    break;
                }
                try
                {
                    //接受数据保存至bytes当中
                    byte[] bytes = new byte[512];
                    //Receive方法中会一直等待服务端回发消息
                    //如果没有回发会一直在这里等着。
                    int len = clientSocket.Receive(bytes, 4, 0);
                    if (len < 0)
                    {
                        Trace.WriteLine("Can't get header");
                        clientSocket.Close();
                        break;
                    }

                    //监测包头长度，
                    if (len != 4 && bytes[0] != 0xA0)
                    {
                        Trace.WriteLine("length is not equal 4, or Version is wrong!");
                        break;
                    }

                    switch (bytes[1])
                    {
                            //运动数据包
                        case 0x30:                      //motion package
                            if (BitConverter.ToInt16(bytes, 2) != mPackLen + 4) //4 is head packet
                            {
                                Trace.WriteLine("Motion package header error");
                            }
                            clientSocket.Receive(bytes, 2, 0);
                            timeTag = BitConverter.ToInt16(bytes, 0);
                            Trace.WriteLine("the" + timeTag + " packet is received!\r\n");
                            
                            //解析数据段
                            for (int i = 0; i < 10; i++)         
                            {
                                len = clientSocket.Receive(bytes, mDataLen, 0);
                                if (len != mDataLen)
                                {
                                    Trace.WriteLine("Motion package length error!");
                                    break;
                                }
                                mPackagePrase(bytes, i);
                            }
                            //校验和监测
                            clientSocket.Receive(bytes, 2, 0);
                            if(BitConverter.ToInt16(bytes, 0) != 0)
                            {
                                Trace.WriteLine("checksum error");
                            }
                            break;
                            //肌电数据包
                        case 0x35:
                            byte timesTag = bytes[1];
                            for (int i = 0; i < 20; i++)         
                            {
                                len = clientSocket.Receive(bytes, eDataLen, 0);
                                if (len != eDataLen)
                                {
                                    Trace.WriteLine("Motion package length error!");
                                    break;
                                }
                                ePackagePrase(bytes, i, timesTag);
                            }
                            //校验和
                            clientSocket.Receive(bytes, 2, 0);
                            if(BitConverter.ToInt16(bytes, 0) != 0)
                            {
                                Trace.WriteLine("checksum error");
                            }
                            break;

                        default:
                            Trace.WriteLine("Package phrase error, bad package header!");
                            break;

                    }
                
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Failed to clientSocket error." + e);
                    clientSocket.Close();
                    break;
                }
            }
        }

        private void ePackagePrase(byte[] bytes, int count,byte timesTag)
        {
            double[] EmgData = new double[4];
            int index = 0;
            for (byte j = 0; j < 4; j++)
            {
                if (Convert.ToBoolean((profiles.EMG_CHL >> (7 - j)) & 0x01))
                {
                    EmgData[j] = utility.Merge(bytes[index], bytes[index + 1], bytes[index + 2]);       //3byte合并
                    EmgData[j] = utility.Convert(EmgData[j]);                                           //补码转源码
                    EmgData[j] = highPass[j].Filtering(EmgData[j]);                                     //去基线
                    EmgData[j] = notch50[j].Filtering(EmgData[j]);                                      //去工频
                    EmgDataArrived(profiles.ID, j, timesTamp - 0.001, sEmgData.Last()[j], timesTamp, EmgData[j]);//肌电接受事件，用于委托显示波形
                    //数据存储
                }
                index = index + 3;
            }
            timesTamp += 0.001;
            sEmgData.Add(EmgData); 
        }
        private void mPackagePrase(byte[] bytes, int count)
        {
            float[] fAccData = new float[3];
            float[] fMegData = new float[3];
            float[] fGyoData = new float[3];
            MotionData mData = new MotionData();
            mData = (MotionData)Community.BytesToStruct(bytes, mData.GetType());
            for (int i = 0; i < 3; i++)
            {
                fAccData[i] = (float)(mData.Acc[i] * 9.8 / 2048.0);
                fMegData[i] = (float)(mData.Meg[i] * 0.3);
                fGyoData[i] = (float)(mData.Gyo[i] / 16.4);
            }

            for (int i = 0; i < 3; i++)
            {
                fAccData[i] = profiles.ACC_FACTOR[i] * fAccData[i] - profiles.ACC_BIAS[i];
                fMegData[i] = profiles.MEG_FACTOR[i] * fMegData[i] - profiles.MEG_BIAS[i];
                fGyoData[i] = profiles.GYO_FACTOR[i] * fGyoData[i] - profiles.GYO_BIAS[i];
            }

            // 前 INITCOUNT 采样点 [采样计数++ 传感器数据累加]
            if (cali.sampleCount < Calibration.INITCOUNT)
            {
                cali.InitLow200(profiles.ID, fAccData, fGyoData, fMegData);
            }
            // 第 INITCOUNT 采样点 [采样计数++ 传感器累加 求均值 归一化向量]
            else if (cali.sampleCount == Calibration.INITCOUNT)
            {
                // 非根结点 需要等待根结点初始化完成
                if (profiles.ID != CollectorUtil.ROOTID)
                {
                    while (!Calibration.RootInit) ;
                }
                qGS0 = cali.InitEqual200(profiles.ID, fAccData, fGyoData, fMegData);
                Trace.WriteLine("qGS0 = " + qGS0.w + " " + qGS0.x + " " + qGS0.y + " " + qGS0.z);
                fusionAHRS = new MahonyAHRS(qGS0, 1f / CollectorUtil.SAMPLERATE, CollectorUtil.KP, 0f);
            }
            // INITCOUNT 采样点以后 进行姿态估计
            else
            {
                // 去除陀螺仪常值漂移
                cali.RemoveGyrBias(fGyoData);

                // 梯度下降估计姿态
                fusionAHRS.Update(fusionAHRS.deg2rad(fGyoData[0]), fusionAHRS.deg2rad(fGyoData[1]), fusionAHRS.deg2rad(fGyoData[2]), fAccData[0], fAccData[1], fAccData[2], fMegData[0], fMegData[1], fMegData[2]);

                Quat qGS = new Quat(fusionAHRS.Quaternion[0], fusionAHRS.Quaternion[1], fusionAHRS.Quaternion[2], fusionAHRS.Quaternion[3]);
                LqGS.Add(qGS);

                Quat qSB = CollectorUtil.qSBDict[profiles.ID];

                this.qGB = Calibration.QuatMultiply(qGS, qSB);

                //委托显示事件,10次更新一次
                if (count == 0)
                {
                    MotionDataArrived(profiles.ID, fAccData, fMegData, fGyoData);
                }
            }

            //数据存储
            acc.Add(fAccData);
            meg.Add(fMegData);
            gyo.Add(fGyoData);
            LqGB.Add(qGB);


            
        }

        public void SendPacket(byte[] payload, byte key)
        {
            PacketHeader header;
            header.version = 0xA0;
            header.key = key;
            header.length = (short)payload.Length;
            byte[] pHeader = Community.StructToBytes(header);
            byte[] packet = new byte[pHeader.Length + payload.Length];
            pHeader.CopyTo(packet, 0);
            payload.CopyTo(packet, pHeader.Length);

            clientSocket.Send(packet, packet.Length, SocketFlags.None);
        

        }

        public void SendPacket(byte key)
        {
            PacketHeader header;
            header.version = 0xA0;
            header.key = key;
            header.length = 0;
            byte[] pHeader = Community.StructToBytes(header);

            clientSocket.Send(pHeader, pHeader.Length, SocketFlags.None);
        }

        public override string ToString()   //For comboBox
        {
            return "No." + profiles.ID.ToString();
        }

        public void SetProfiles()
        {
            byte[] sendBuffer = Community.StructToBytes(profiles);
            //clientSocket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
            this.SendPacket(sendBuffer, 0x10);

        }
    }

}
