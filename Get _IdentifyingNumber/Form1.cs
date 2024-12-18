﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.IO;
using Get__IdentifyingNumber.Properties;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;
using HPSocket.Base;
using SocketClientBase;

namespace Get__IdentifyingNumber
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string IdentifyingNumber=string.Empty;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(System.Environment.CurrentDirectory + "\\BadIdentifyingNumber.txt"))
            {
                File.Create(System.Environment.CurrentDirectory + "\\BadIdentifyingNumber.txt").Close();
            }
            GetIdentifyingNumber();
            this.toolStripStatusLabel1.Visible = false;
            //OpenWeChatXCX();
        }
        private bool GetIdentifyingNumber()
        {
            //HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS\BIOSSerialNumber
            var a = new ManagementObjectSearcher("select IdentifyingNumber from Win32_ComputerSystemProduct").Get().GetEnumerator();
            foreach (ManagementBaseObject item in new ManagementObjectSearcher("select IdentifyingNumber from Win32_ComputerSystemProduct").Get())
            {
                ManagementObject managementObject = item as ManagementObject;
                if (managementObject != null) 
                {
                    IdentifyingNumber = managementObject.GetPropertyValue("IdentifyingNumber").ToString();
                    this.label1.Text = "序列号：" + IdentifyingNumber;
                    this.linkLabel1.Text = "序列号：" + IdentifyingNumber;
                    Comparison(IdentifyingNumber);
                }
            }
            return true;
        }
        private bool Comparison(string idnNum)
        {
            if (File.Exists(System.Environment.CurrentDirectory + "\\BadIdentifyingNumber.txt"))
            {
                IEnumerable<string> ret = File.ReadAllLines(System.Environment.CurrentDirectory + "\\BadIdentifyingNumber.txt").Where(x => x.Equals(idnNum));
                if (ret.Any()) 
                {
                    this.button1.Visible = false;
                    this.pictureBox1.Image = Image.FromStream(new MemoryStream(Resources.zhadan, 0, Resources.zhadan.Length));
                    return true;
                }
                else
                {
                    this.button1.Visible = true;
                    //string imagepath =System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace + ".Images.zhengque.jpg";
                    //string[] res = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                    //Image obj = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(res[3]));
                    Stream imagestream = Assembly.GetExecutingAssembly().GetManifestResourceStream(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace+ ".Images.zhengque.jpg");
                    this.pictureBox1.Image = Image.FromStream (imagestream);
                    return false;
                }
            }
            else 
            {
                File.Create(System.Environment.CurrentDirectory + "\\BadIdentifyingNumber.txt").Close();
                return false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(IdentifyingNumber);
            //Process.Start("IExplore", $"https://newsupport.lenovo.com.cn/deviceGuarantee.html?fromsource=deviceGuarantee&selname={IdentifyingNumber}");
            Process.Start($"https://newsupport.lenovo.com.cn/deviceGuarantee.html?fromsource=deviceGuarantee&selname={IdentifyingNumber}");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(IdentifyingNumber);
            this.linkLabel1.LinkVisited = true;
            //Process.Start("IExplore", $"https://newsupport.lenovo.com.cn/deviceGuarantee.html?fromsource=deviceGuarantee&selname={IdentifyingNumber}");
            Process.Start($"https://newsupport.lenovo.com.cn/deviceGuarantee.html?fromsource=deviceGuarantee&selname={IdentifyingNumber}");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FileStream fs = new FileStream(System.Environment.CurrentDirectory + "\\BadIdentifyingNumber.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.Position = fs.Length;
                    sw.WriteLine(this.IdentifyingNumber);
                    sw.Close();
                    fs.Close();
                }
            }
            this.pictureBox1.Image = Image.FromStream(new MemoryStream(Resources.zhadan, 0, Resources.zhadan.Length));
            this.button1.Visible = false;
            
        }

        private void OpenWeChatXCX()
        {
            //weixin://dl/business/?t=  weixin://dl/business/?ticket=
            Process[] WeChat = Process.GetProcessesByName("WeChat"); //WeChatPlayer.exe    WeChatUtility.exe
            if (WeChat.Length > 0)
            {
                Process[] WeChatUtility = Process.GetProcessesByName("WeChatUtility");
                if (WeChatUtility.Length > 0)
                {
                    //reg add "HKEY_CURRENT_USER\Software\Tencent\WeChat" /v DesktopApps /t REG_SZ /d "联想百应" /f
                    var RegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Tencent\\WeChat", true);
                    if (RegistryKey.GetValue("DesktopApps") == null)
                    {
                        RegistryKey.SetValue("DesktopApps", "联想百应", RegistryValueKind.String);
                    }
                    else
                    {
                        if (!RegistryKey.GetValue("DesktopApps").ToString().Contains("联想百应"))
                        {
                            string newvlaue = RegistryKey.GetValue("DesktopApps").ToString() + "|联想百应";
                            RegistryKey.SetValue("DesktopApps", newvlaue, RegistryValueKind.String);
                        }
                    }
                    //string filename =WeChat[0].StartInfo.FileName;
                    Process.Start(WeChat[0].MainModule.FileName.Replace("WeChat.exe", "WechatAppLauncher.exe"), "-launch_appid=wx54834006424fda0b").Close(); //wxd98a20e429ce834b   wx54834006424fda0b  wxddc6d889b65e6e33
                }
                else
                {
                    Console.WriteLine("清先运行登录微信");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("清先运行微信并登录");
                Console.ReadLine();
            }
        }

        private  void 上传数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SocketClientUP();
        }
        private async void SocketClientUP()
        {
            this.上传数据ToolStripMenuItem.Enabled = false;
            SocketClient client = new SocketClient();
            client.OnConnected += () => Console.WriteLine("Connected to server.");
            client.OnDisconnected += () => Console.WriteLine("Disconnected from server.");
            client.OnTextSent += text => 
            { 
                Console.WriteLine($"Text sent: {text}"); 
            };
            client.OnTextReceived += (text, from) => 
            { 
                Console.WriteLine($"Text received from {from}: {text}");
                this.statusStrip1.Invoke(new Action(() => { this.toolStripStatusLabel1.Text = text; }));
                if (text.Equals("上传完成"))
                {
                    this.上传数据ToolStripMenuItem.Enabled = true;
                    client.Disconnect();
                }
                
            };
            client.OnFileProgress += (fileName, current, total) =>
            {
                Console.WriteLine($"File progress: {fileName} ({current}/{total})");
                this.toolStripStatusLabel1.Visible = true;
                this.statusStrip1.Invoke(new Action(() => { this.toolStripStatusLabel1.Text = $"File progress: {current}/{total}"; }));
            };
                
            client.OnFileSent += fileName => Console.WriteLine($"File sent: {fileName}");

            await client.ConnectAsync("47.107.248.159", 8899);//47.107.248.159
            //await client.SendTextAsync("Hello, Server!");
            await client.SendFileAsync("BadIdentifyingNumber.txt");
        }

        private void HpSocketClientUP()
        {
            //this.上传数据ToolStripMenuItem.Enabled = false;
            HpSocketClient client = new HpSocketClient();
            client.Connect("47.107.248.159", 8899);
            client.UploadFile(System.Environment.CurrentDirectory + "\\BadIdentifyingNumber.txt");
            client.OnReceiveEvent += (response) =>
            {
                Console.WriteLine(response.Message);
                this.toolStripStatusLabel1.Visible = true;
                //this.statusStrip1.Invoke(new Action(() => { this.toolStripStatusLabel1.Text= response.Message; }));
                this.toolStripStatusLabel1.Text = response.Message;
                if (response.Message.Equals("上传完成"))
                {
                    client.CloseCliend();
                    //this.menuStrip1.Invoke(new Action(() => { this.上传数据ToolStripMenuItem.Enabled = true; }));
                }
            };
            //client.CloseCliend();
        }

        private void 下载数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SocketClientDown();

        }

        private async void SocketClientDown()
        {
            this.下载数据ToolStripMenuItem.Enabled = false;
            SocketClient client = new SocketClient();
            client.OnConnected += () => Console.WriteLine("Connected to server.");
            client.OnDisconnected += () => Console.WriteLine("Disconnected from server.");
            client.OnTextSent += text =>
            {
                Console.WriteLine($"Text sent: {text}");
            };
            client.OnTextReceived += (text, from) =>
            {
                Console.WriteLine($"Text received from {from}: {text}");
                this.toolStripStatusLabel1.Visible = true;
                this.statusStrip1.Invoke(new Action(() => { this.toolStripStatusLabel1.Text = text; }));
                if (text.Equals("下载完成")|| text.Contains("请求文件不存在"))//请求文件不存在，请先上传或联系服务器管理员处理
                {
                    base.Invoke(new Action(() => {
                        GetIdentifyingNumber(); //更新检测炸弹
                    }));

                    this.下载数据ToolStripMenuItem.Enabled = true;
                    client.Disconnect();
                }

            };
            client.OnFileProgress += (fileName, current, total) =>
            {
                Console.WriteLine($"File progress: {fileName} ({current}/{total})");
                this.toolStripStatusLabel1.Visible = true;
                this.statusStrip1.Invoke(new Action(() => { this.toolStripStatusLabel1.Text = $"File progress: {current}/{total}"; }));
            };

            await client.ConnectAsync("47.107.248.159", 8899);//47.107.248.159
            await client.RequestFileAsync("ReceivedFiles\\BadIdentifyingNumber.txt");
        }

        private void  HpSocketDown()
        {
            //this.下载数据ToolStripMenuItem.Enabled = false;
            HpSocketClient client = new HpSocketClient();
            client.Connect("127.0.0.1", 8899);
            client.DownloadFile("BadIdentifyingNumber.txt");
            client.OnReceiveEvent += (response) =>
            {
                Console.WriteLine(response.Message);
                this.toolStripStatusLabel1.Visible = true;
                //this.statusStrip1.Invoke(new Action(() => { this.toolStripStatusLabel1.Text = response.Message; }));
                this.toolStripStatusLabel1.Text = response.Message;
                if (response.Message.Equals("下载完成"))
                {
                    client.CloseCliend();
                    //this.menuStrip1.Invoke(new Action(() => { this.下载数据ToolStripMenuItem.Enabled = true; }));
                }
            };
            //client.CloseCliend();
        }
    }
}
