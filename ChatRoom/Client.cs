using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Collections;

namespace CsSocket_Multi2_C
{
    enum ClientStatus
    {
        FailedToConnect,
        Connected,
        Received,
        Sent,
        Disconnected
    }
    class Client
    {
        private static Socket _clientSocket;
        private static UdpClient uc, us;
        private static IPEndPoint ipep_b, ipep_c;
        private static ArrayList _master = new ArrayList(), _read = new ArrayList();
        private static Thread ReceiveThread;
        private static byte[] _buffer = new byte[8192];
        public static string LocalIP = "";
        private static List<string> _ChatContent;
        private static bool ready,alreadyrecving;
        private static string name;

        private static byte[] currentPicture = null;
        public static string pictureExtension = null;

        private static byte[] currentFile = null;
        public static string fileExtension = null;
        public static List<string> ChatContent
        {
            get
            {
                return _ChatContent;
            }

            set
            {
                _ChatContent = value;
            }
        }
        public static string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public static bool Ready
        {
            get
            {
                return ready;
            }

            set
            {
                ready = value;
            }
        }

        public static IPEndPoint Ipep_c
        {
            get
            {
                return ipep_c;
            }

            set
            {
                ipep_c = value;
            }
        }

        public static byte[] CurrentPicture
        {
            get
            {
                return currentPicture;
            }

            set
            {
                currentPicture = value;
            }
        }

        public static byte[] CurrentFile
        {
            get
            {
                return currentFile;
            }

            set
            {
                currentFile = value;
            }
        }

        public static Socket ClientSocket
        {
            get
            {
                return _clientSocket;
            }

            set
            {
                _clientSocket = value;
            }
        }

        public static void Clientstartup(string ip)
        {
            _ChatContent = new List<string>();
            ipep_c = new IPEndPoint(IPAddress.Any, 2000);
            ipep_b = new IPEndPoint(IPAddress.Broadcast, 2000);

            //TCP
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            ReceiveThread = new Thread(ReceiveLoop);

            //UDP

            uc = new UdpClient();//Receive
            uc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            uc.Client.Bind(Ipep_c);

            us = new UdpClient();//Broadcast
            us.Client.EnableBroadcast = true;
            //us.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            StartReceiveThread();
        }

        //************************************************
        //TCP
        public static ClientStatus LoopConnect(string ip)
        {
            while (!ClientSocket.Connected)
            {
                try
                {
                    ClientSocket.Connect(ip, 3000);
                }
                catch (SocketException)
                {
                    return ClientStatus.FailedToConnect;
                }
            }
            ipep_b = new IPEndPoint(IPAddress.Parse(ip), 2000);
            return ClientStatus.Connected;
        }

        public static int Connect(byte[] sendname, string ip)//TCP
        {
            try
            {
                if (!ready)
                {
                    if (LoopConnect(ip) == ClientStatus.Connected)
                    {
                        ready = true;
                    }
                    else
                    {
                        return -1;
                    }
                }
                ClientSocket.Send(sendname);
                ChatContent.Add("[" + name + "] 已與伺服器連線\r\n");
                return 1;
            }
            catch
            {
                return -1;
            }

        }

        public static void StartReceiveThread()//TCP
        {
            ReceiveThread.Start();
        }

        private static void ReceiveLoop()
        {
            _master.Clear();
            _read.Clear();
            _master.Add(ClientSocket);
            _master.Add(uc.Client);
            while (true)
            {
                _read = new ArrayList(_master);
                try
                {
                    Socket.Select(_read, null, null, 1000);
                    for (int i = 0; i < _read.Count; i++)
                    {
                        if (_read[i] == uc.Client)
                        {
                            byte[] recbyte = uc.Receive(ref ipep_c); 
                            string textrec = Encoding.Default.GetString(recbyte);
                            ChatContent.Add(textrec);
                        }
                        else
                        {
                            if (ready)
                            {
                                try
                                {
                                    NetworkStream ns = new NetworkStream(ClientSocket);
                                    byte[] extbuf = new byte[3];
                                    byte[] sizebuf = new byte[4];
                                    
                                    if (!alreadyrecving)
                                    {
                                        ReadAllData(ns,extbuf);
                                        string extension = Encoding.Default.GetString(extbuf);
                                        ReadAllData(ns, sizebuf);//1st get size

                                        int count = BitConverter.ToInt32(sizebuf, 0);
                                        byte[] getf = new byte[count];
                                        ReadAllData(ns, getf);
                                        switch (extension)
                                        {
                                            case "jpg":
                                                pictureExtension = extension;
                                                CurrentPicture = getf;
                                                ChatContent.Add("[" + Name + "] 收到" + extension + "圖片, Size = " + getf.Length + "\n");
                                                break;
                                            case "png":
                                                pictureExtension = extension;
                                                CurrentPicture = getf;
                                                ChatContent.Add("[" + Name + "] 收到" + extension + "圖片, Size = " + getf.Length + "\n");
                                                break;
                                            case "bmp":
                                                pictureExtension = extension;
                                                CurrentPicture = getf;
                                                ChatContent.Add("[" + Name + "] 收到" + extension + "圖片, Size = " + getf.Length + "\n");
                                                break;
                                            case "gif":
                                                pictureExtension = extension;
                                                CurrentPicture = getf;
                                                ChatContent.Add("[" + Name + "] 收到" + extension + "圖片, Size = " + getf.Length + "\n");
                                                break;
                                            default:
                                                fileExtension = extension;
                                                CurrentFile = getf;
                                                ChatContent.Add("[" + Name + "] 收到" + extension + "檔案, Size = " + getf.Length + "\n");
                                                break;
                                        }
                                        

                                        
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                }
                    
            }//while
        }
        private static void ReadAllData(NetworkStream n, byte[] toread)
        {
            alreadyrecving = true;
            int sizeleft = toread.Length, offset = 0;
            while (sizeleft > 0)
            {
                int get = n.Read(toread, offset, sizeleft);
                if (get <= 0) throw new EndOfStreamException("Error");
                sizeleft -= get;
                offset += get;
            }
            alreadyrecving = false;
            n.Flush();
        }


        public static void SendFile(byte[] ext,byte[] sndfile)//TCP , SendPhoto
        {
            if (ready)
            {
                NetworkStream ns = new NetworkStream(ClientSocket);
                byte[] extsend = ext;
                ns.Write(ext, 0, ext.Length);
                ChatContent.Add("[" + name + "] 已發送副檔名.\n");

                byte[] buffer = sndfile;
                ns.Write(BitConverter.GetBytes(sndfile.Length), 0, 4);
                ns.Write(buffer, 0, buffer.Length);
                ChatContent.Add("[" + name + "] 已發送檔案.\n");
            }
            else
            {
                ChatContent.Add("[Error**]未連線\r\n");
            }
        }



        //************************************************
        //UDP
        

        public static void SendMsg(string sendmsg)
        {
            byte[] sendbuf = new byte[1024];
            sendbuf = Encoding.Default.GetBytes(sendmsg);
            try
            {
                us.Send(sendbuf, sendbuf.Length, ipep_b);
            }
            catch
            {
                MessageBox.Show("ServerAddress Error!", "Error");
            }
        }



        public static void Stop()
        {
            ClientSocket.Close();
            ReceiveThread.Abort();
            uc.Close();
            us.Close();
        }

    }
}
