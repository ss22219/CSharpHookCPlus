using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        [DllImport("DemoNativeLib.dll")]
        public static extern int NativeMethod();
        [DllImport("DemoNativeLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SendMsg(ushort identifier);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NativeMethod();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMsg(0x555);
            Console.WriteLine("button1 Clicked!");
        }
    }
}
