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
        }
        private bool GetIdentifyingNumber()
        {
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
    }
}
