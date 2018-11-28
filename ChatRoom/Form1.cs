using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using CsSocket_Multi2_C;
using System.IO;
using System.Drawing.Imaging;
using System.Media;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ChatRoom
{
    public partial class Form1 : Form
    {
        TextBox TB_SendMsg;
        TextBox TB_SendFile;
        Button   BT_Connect,BT_BrowseFile,BT_SendFile;
        TextBox TB_SendIP,TB_Name;
        Hashtable emotions;
        SoundPlayer sp = new SoundPlayer();
        Random rm = new Random();
        int currentchatsize;
        bool connectfail = false;
        string lastmsg = "";
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            KeyPreview = true;
            currentchatsize = 0;
            CreateEmotions();
            SetIpAddress();
            EnterChatMode();
        }
        //取得並設定自己的ip位置
        private void SetIpAddress()
        {
            var ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(IPAddress ipaddress in ips)
            {
                if(ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    Client.LocalIP = ipaddress.ToString();
                }
            }

        }
        
        //以HashTable初始化本機內建表情符號
        private void CreateEmotions()
        {
            emotions = new Hashtable(4);
            emotions.Add(":)", Properties.Resources.smile);
            emotions.Add(":D", Properties.Resources.surprised);
            emotions.Add(":))", Properties.Resources.Tongue_Out_Emoji);
            emotions.Add(":(", Properties.Resources.verySad);
            emotions.Add("<3", Properties.Resources.Kissing_face);
        }
        //由timer確認TextBox內容是否增加
        private void Parse(object o, EventArgs e)
        {
            foreach (string emostr in emotions.Keys)
            {
                while (RTB_ChatContent.Text.Contains(emostr))
                {
                    int index = RTB_ChatContent.Text.IndexOf(emostr);
                    RTB_ChatContent.Select(index, emostr.Length);
                    Clipboard.SetImage((Image)emotions[emostr]);
                    RTB_ChatContent.Paste();
                }
            }
        }//Parsing smileys
        private void Parse2(object o, EventArgs e)
        {
            if (Client.ChatContent.Count > 0)
            {
                if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("圖片, Size = "))
                {
                    sp.Stream = Properties.Resources.Clown_Horn;
                    sp.Play();
                }
                else if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("/marmot"))
                {
                    sp.Stream = Properties.Resources.Marmot_Scream;
                    sp.Play();
                    int index = RTB_ChatContent.Text.IndexOf("/marmot");
                    RTB_ChatContent.Select(index, "/marmot".Length);
                    Bitmap bmp = new Bitmap(Properties.Resources.Scream_Niko, new Size(170, 150));
                    Clipboard.SetImage(bmp);
                    RTB_ChatContent.Paste();
                }
                else if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("/horn"))
                {
                    sp.Stream = Properties.Resources.Air_Horn_Sound_Effect;
                    int index = RTB_ChatContent.Text.IndexOf("/horn");
                    RTB_ChatContent.Select(index, "/horn".Length);
                    Bitmap bmp = new Bitmap(Properties.Resources.A1_AirHorn, new Size(170, 50));
                    Clipboard.SetImage(bmp);
                    RTB_ChatContent.Paste();
                    sp.Play();
                }
                else if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("/ah"))
                {
                    sp.Stream = Properties.Resources.Howie_Scream;
                    int index = RTB_ChatContent.Text.IndexOf("/ah");
                    RTB_ChatContent.Select(index, "/ah".Length);
                    Bitmap bmp = new Bitmap(Properties.Resources.A1_Ah, new Size(170, 150));
                    Clipboard.SetImage(bmp);
                    RTB_ChatContent.Paste();
                    sp.Play();
                }
                else if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("/suka"))
                {
                    sp.Stream = Properties.Resources.SUKA;
                    int index = RTB_ChatContent.Text.IndexOf("/suka");
                    RTB_ChatContent.Select(index, "/suka".Length);
                    Bitmap bmp = new Bitmap(Properties.Resources.A1_Suka, new Size(170, 150));
                    Clipboard.SetImage(bmp);
                    RTB_ChatContent.Paste();
                    sp.Play();
                }
                else if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("/..."))
                {
                    sp.Stream = Properties.Resources.Crow;
                    int index = RTB_ChatContent.Text.IndexOf("/...");
                    RTB_ChatContent.Select(index, "/...".Length);
                    Bitmap bmp = new Bitmap(Properties.Resources.A1_DotDotDot, new Size(200, 150));
                    Clipboard.SetImage(bmp);
                    RTB_ChatContent.Paste();
                    sp.Play();
                }
                else if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("/happy"))
                {
                    sp.Stream = Properties.Resources.Clown_Horn;
                    int index = RTB_ChatContent.Text.IndexOf("/happy");
                    RTB_ChatContent.Select(index, "/happy".Length);
                    Bitmap bmp = new Bitmap(Properties.Resources.A1_Clown_Horn, new Size(170, 150));
                    Clipboard.SetImage(bmp);
                    RTB_ChatContent.Paste();
                    sp.Play();
                }
                else if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("/fart"))
                {
                    int a = rm.Next(1, 5);
                    switch (a)
                    {
                        case 1:
                            sp.Stream = Properties.Resources.f1;
                            break;
                        case 2:
                            sp.Stream = Properties.Resources.f2;
                            break;
                        case 3:
                            sp.Stream = Properties.Resources.f3;
                            break;
                        case 4:
                            sp.Stream = Properties.Resources.f4;
                            break;
                    }
                    int index = RTB_ChatContent.Text.IndexOf("/fart");
                    RTB_ChatContent.Select(index, "/fart".Length);
                    Bitmap bmp = new Bitmap(Properties.Resources.image_Fart, new Size(170, 150));
                    Clipboard.SetImage(bmp);
                    RTB_ChatContent.Paste();
                    sp.Play();
                }
                else if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("/shit"))
                {
                    int a = rm.Next(1, 4);
                    switch (a)
                    {
                        case 1:
                            sp.Stream = Properties.Resources.SH;
                            break;
                        case 2:
                            sp.Stream = Properties.Resources.SH2;
                            break;
                        case 3:
                            sp.Stream = Properties.Resources.SH3;
                            break;
                    }
                    int index = RTB_ChatContent.Text.IndexOf("/shit");
                    RTB_ChatContent.Select(index, "/shit".Length);
                    Bitmap bmp = new Bitmap(Properties.Resources.A1_Shit, new Size(170, 150));
                    Clipboard.SetImage(bmp);
                    RTB_ChatContent.Paste();
                    sp.Play();
                }
                else if (Client.ChatContent[Client.ChatContent.Count - 1].Contains("/help"))
                {
                    RTB_ChatContent.AppendText(
                        "/+marmot - 土撥鼠尖叫，有圖\n"
                        + "/+horn - 噓聲喇叭\n"
                        + "/suka - 俄文髒話\n"
                        + "/shit - 隨機的Oh shit!!\n"
                        + "/+fart - 隨機放屁聲\n"
                        + "/+ah - 慘叫\n"
                        +"/+happy - 小丑喇叭\n"
                        +"/+... - 烏鴉\n");
                }
                else if(Client.ChatContent[Client.ChatContent.Count - 1].Contains("/real"))
                {
                    int a = rm.Next(1, 4);
                    switch (a)
                    {
                        case 1:
                            sp.Stream = Properties.Resources.real1;
                            break;
                        case 2:
                            sp.Stream = Properties.Resources.real2;
                            break;
                        case 3:
                            sp.Stream = Properties.Resources.real3;
                            break;
                    }
                    int index = RTB_ChatContent.Text.IndexOf("/real");
                    RTB_ChatContent.Select(index, "/real".Length);
                    Bitmap bmp = new Bitmap(Properties.Resources.A2_real, new Size(170, 150));
                    Clipboard.SetImage(bmp);
                    RTB_ChatContent.Paste();
                    sp.Play();
                }
                else
                {
                    sp.Stream = Properties.Resources.FB_Message;
                    sp.Play();
                }
            }
            else if (connectfail)
            {
                sp.Stream = Properties.Resources.IP_empty;
                sp.Play();
                connectfail = false;
            }
        }//Parsing sounds 
        
        //完成初始化，以及進入聊天室介面
        protected void EnterChatMode()
        {
            TB_SendIP = new TextBox();
            TB_SendIP.Location = new Point(560, 90);
            TB_SendIP.Font = new Font("華康棒棒體W5", 15);
            TB_SendIP.Multiline = true;
            TB_SendIP.Size = new Size(200, 40);
            TB_SendIP.Text = "";
            TB_SendIP.Visible = true;
            Controls.Add(TB_SendIP);
            while (TB_SendIP.Text == "")
            {
                string serverip = Microsoft.VisualBasic.Interaction.InputBox("Serverip = ");
                TB_SendIP.Text = serverip;
            }
            Client.Clientstartup(TB_SendIP.Text);

            TB_Name = new TextBox();
            TB_Name.Location = new Point(580 ,280);
            TB_Name.Font = new Font("華康棒棒體W5", 25);
            TB_Name.Multiline = true;
            TB_Name.Size = new Size(120, 50);
            TB_Name.Text = "";
            TB_Name.Visible = true;
            TB_Name.ReadOnly = true;
            Controls.Add(TB_Name);
            while (TB_Name.Text == "")
            {
                string name = Microsoft.VisualBasic.Interaction.InputBox("Name = ");
                TB_Name.Text = name;
                Client.Name = name;
            }
            
            RTB_ChatContent.Location = new Point(32, 57);
            RTB_ChatContent.Size = new Size(459, 400);//459, 562
            RTB_ChatContent.Visible = true;
            RTB_ChatContent.Multiline = true;
            RTB_ChatContent.ScrollBars = RichTextBoxScrollBars.Vertical;
            RTB_ChatContent.Font = new Font("華康棒棒體W5", 16 ,FontStyle.Bold);
            RTB_ChatContent.ForeColor = Color.DarkBlue;
            RTB_ChatContent.TextChanged += new EventHandler(Parse);
            RTB_ChatContent.TextChanged += new EventHandler(Parse2);
            Controls.Add(RTB_ChatContent);

            TB_SendMsg = new TextBox();
            TB_SendMsg.Location = new Point(32,540);//32,675
            TB_SendMsg.Font = new Font("華康棒棒體W5",20);
            TB_SendMsg.Size = new Size(459,60);
            TB_SendMsg.Visible = true;
            Controls.Add(TB_SendMsg);

            TB_SendMsg.Focus();
            //*******************************************************
            TB_SendFile = new TextBox();
            TB_SendFile.Location = new Point(32, 600);//32,780
            TB_SendFile.Multiline = true;
            TB_SendFile.Size = new Size(459, 20);
            TB_SendFile.Visible = true;
            TB_SendFile.ReadOnly = true; //0604
            Controls.Add(TB_SendFile);
            
            BT_BrowseFile = new Button();
            BT_BrowseFile.Text = "瀏覽檔案";
            BT_BrowseFile.Location = new Point(700, 600);//700,760
            BT_BrowseFile.Size = new Size(100, 50);
            BT_BrowseFile.Visible = true;
            Controls.Add(BT_BrowseFile);
            BT_BrowseFile.Click += new EventHandler(BrowseFile);

            BT_SendFile = new Button();
            BT_SendFile.Text = "發送檔案";
            BT_SendFile.Location = new Point(600, 600);//600,760
            BT_SendFile.Size = new Size(100, 50);
            BT_SendFile.Visible = true;
            Controls.Add(BT_SendFile);
            BT_SendFile.Click += new EventHandler(SendFile);

            BT_Connect = new Button();
            BT_Connect.Text = "連線至Server";
            BT_Connect.Location = new Point(600, 140);
            BT_Connect.Size = new Size(100, 50);
            BT_Connect.Visible = true;
            Controls.Add(BT_Connect);
            BT_Connect.Click += new EventHandler(Connect);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            
            RTB_ChatContent.AppendText("(聊天室 版本 - V1.3 )\n增加上↑功能\n(查看特殊音效功能: /help)\n");
            Client.Clientstartup(TB_SendIP.Text);
            timer1.Start();
            //*******************************************************
            
        }

        //*************
        //按鈕觸發以下功能

        private void SendMsg()
        {
            string text = "("+Client.LocalIP+")[" + Client.Name + "]" + TB_SendMsg.Text +"\r\n";
            Client.SendMsg(text);
            lastmsg = TB_SendMsg.Text;
        //*************
        }
        
        private void Connect(object o, EventArgs e)
        {
            if(!Client.Ready)
            {
                byte[] sendname = Encoding.Default.GetBytes(TB_Name.Text);
                if(Client.Connect(sendname, TB_SendIP.Text) == -1)
                {
                    connectfail = true;
                    MessageBox.Show("無法連線!","Error");
                    RTB_ChatContent.AppendText("[錯誤]連線失敗：您的IP是空號，請查明後再撥\n");

                }
            }
        }
        
        private void BrowseFile(object o , EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPG Image|*.jpg|PNG Image|*.png|ALL File|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TB_SendFile.Text = ofd.FileName;
            }
        }//以按鈕":BT_BrowseFile瀏覽檔案"觸發

        private void SendFile(object o, EventArgs e)
        {
            if (Client.Ready)
            {
                string filepath = TB_SendFile.Text;

                string[] filenamedivision = filepath.Split('.');
                string filenameExtention = filenamedivision[filenamedivision.Length - 1];

                byte[] extension = Encoding.Default.GetBytes(filenameExtention);
               // MessageBox.Show(filenameExtention);
                byte[] buffer = ReadImageFile(filepath);
                Client.SendFile(extension,buffer);
            }
            else
            {
                Client.ChatContent.Add("[錯誤]Failed to send a picture: Not Connected!");
            }
            
        }

        //*************

        //按下Enter送出，按下上，跑出上次訊息可再次輸入
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && TB_SendMsg.Text!="")
            {
                SendMsg();
                TB_SendMsg.Clear();
                e.SuppressKeyPress = true;//Stop the warning "Ding "sound when pressing enter
            }
            else if(e.KeyCode == Keys.Up)
            {
                TB_SendMsg.Text = lastmsg;
            }
            TB_SendMsg.Focus();
        }
        
        private static byte[] ReadImageFile(string img)
        {
            FileInfo fileinfo = new FileInfo(img);
            byte[] buf = new byte[fileinfo.Length];
            FileStream fs = new FileStream(img, FileMode.Open, FileAccess.Read);
            fs.Read(buf, 0, buf.Length);
            fs.Close();
            GC.ReRegisterForFinalize(fileinfo);
            GC.ReRegisterForFinalize(fs);
            return buf;
        }

        
        private void UpdateChatContent()
        {
            RTB_ChatContent.AppendText(Client.ChatContent[Client.ChatContent.Count - 1]);
            RTB_ChatContent.SelectionStart = RTB_ChatContent.TextLength;
            //scroll to the caret
            RTB_ChatContent.ScrollToCaret();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "NewPicture.jpg";
                sfd.Filter = "JPG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp|GIF Image|*.gif";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    switch (sfd.FilterIndex)
                    {
                        case 1:
                            pictureBox1.Image.Save(sfd.FileName, ImageFormat.Jpeg);
                            break;
                        case 2:
                            pictureBox1.Image.Save(sfd.FileName, ImageFormat.Png);
                            break;
                        case 3:
                            pictureBox1.Image.Save(sfd.FileName, ImageFormat.Bmp);
                            break;
                        case 4:
                            pictureBox1.Image.Save(sfd.FileName, ImageFormat.Gif);
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("沒有圖片", "警告");
            }
        }

        //若有檔案可下載檔案
        private void FileBox_Click(object sender, EventArgs e)
        {
            if (FileBox.Image != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "NewFile."+Client.fileExtension;
                sfd.Filter = "Specific File|*."+Client.fileExtension;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = File.Create(sfd.FileName);
                    fs.Write(Client.CurrentFile, 0, Client.CurrentFile.Length);
                    Client.ChatContent.Add("(" + Client.LocalIP + ")[" + Client.Name+"] 已建立檔案 "+sfd.FileName+" 大小 = " + fs.Length);
                    fs.Close();
                }
            }
            else
            {
                MessageBox.Show("沒有檔案", "警告");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Client.CurrentPicture != null)
                SetPic(Client.CurrentPicture);
            if (Client.CurrentFile != null)
                SetFile();
            TB_SendMsg.Focus();
            if (currentchatsize != Client.ChatContent.Count)
            {
                UpdateChatContent();
                currentchatsize = Client.ChatContent.Count;
            }
            
        }

        private void SetPic(byte[] pic)
        {
            pictureBox1.Image = (Bitmap)((new ImageConverter()).ConvertFrom(pic));
        }
        private void SetFile()
        {
            FileBox.Image = Properties.Resources.File;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client.Stop();
            timer1.Stop();
            Application.Exit();
        }

        public static void DisplayIPAddresses()//測試用，顯示所有使用中網卡位址
        {
            StringBuilder sb = new StringBuilder();

            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection)
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface network in networkInterfaces)
            {
                // Read the IP configuration for each network
                IPInterfaceProperties properties = network.GetIPProperties();

                // Each network interface may have multiple IP addresses
                foreach (IPAddressInformation address in properties.UnicastAddresses)
                {
                    // We're only interested in IPv4 addresses for now
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    // Ignore loopback addresses (e.g., 127.0.0.1)
                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    sb.AppendLine(address.Address.ToString() + " (" + network.Name + ")");
                }
            }

            MessageBox.Show(sb.ToString());
        }

    }
}
