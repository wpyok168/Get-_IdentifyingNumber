using System;
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

namespace Get__IdentifyingNumber
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

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
                    string IdentifyingNumber = managementObject.GetPropertyValue("IdentifyingNumber").ToString();
                    this.label1.Text = "本机序列号：" + IdentifyingNumber;
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
                    this.pictureBox1.Image = Image.FromStream(new MemoryStream(Resources.zhadan, 0, Resources.zhadan.Length));
                    return true;
                }
                else
                {
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
    }
}
