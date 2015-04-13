using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Reflection;
using System.Net;

namespace UdpPPT
{
    public partial class Form1 : Form
    {
        public delegate void MessageHandler(string msg);

        static MessageHandler handler;

        public event MessageHandler ReceviedFromPhone
        {
            add
            {
                handler += new MessageHandler(value);
            }
            remove
            {
                handler -= new MessageHandler(value);
            }
        }

        Assembly asm;
        private NotifyIcon CM_notifyIcon;  //右下角提示图标
        private ContextMenu CM_contextMenu;  //右下角图标右键菜单


        UdpClient udp = new UdpClient(8788);
        public Form1()
        {
            InitializeComponent();
            
            Start_Serv();
            this.ReceviedFromPhone += Form1_ReceviedFromPhone;

            asm = GetType().Assembly;

            this.FormClosing += Form1_FormClosing;
            this.components = new Container();

            CM_contextMenu = new ContextMenu();
            CM_notifyIcon = new NotifyIcon(this.components);
            CM_notifyIcon.Visible = true;

            MenuItem exitall = new MenuItem { Name = "ExitAll", Text = "退出" };
            exitall.Click += exitall_Click;
            
            

            CM_notifyIcon.ContextMenu = CM_contextMenu;

            Icon icon = new System.Drawing.Icon(asm.GetManifestResourceStream("UdpPPT.icons.wait_48.ico"));
            CM_notifyIcon.Icon = icon;
            CM_notifyIcon.BalloonTipText = "PPT翻页器已启动！";
            CM_notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            CM_notifyIcon.BalloonTipTitle = "PPT翻页器";
            CM_notifyIcon.ShowBalloonTip(0);
            
            IPAddress[] ips = GetIPs();
            //StringBuilder sb = new StringBuilder();
            //sb.Append("IP地址：\n");
            
            foreach(var ipitem in ips)
            {
                CM_contextMenu.MenuItems.Add(new MenuItem { Text = ipitem.ToString(), Enabled = false });
            }
            CM_contextMenu.MenuItems.Add(exitall);
            //CM_notifyIcon.Text = sb.ToString();
        }

        IPAddress[] GetIPs()
        {
            string hostname = Dns.GetHostName();
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            return localhost.AddressList;
        }

        void exitall_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            CM_notifyIcon.ShowBalloonTip(0, "PPT翻页器", "PPT翻页器已最小化", ToolTipIcon.Info);
            e.Cancel = true;
        }

        void Form1_ReceviedFromPhone(string msg)
        {
            switch(msg)
            {
                case "ESC": SendKeys.SendWait("{ESC}"); break;
                case "PGUP": SendKeys.SendWait("{PGUP}"); break;
                case "START": SendKeys.SendWait("+{F5}"); break;
                default:
                case "PGDN": SendKeys.SendWait("{PGDN}"); break;                
            }
        }

        async void Start_Serv()
        {
            while (true)
            {
                UdpReceiveResult result = await udp.ReceiveAsync();
                string str = Encoding.UTF8.GetString(result.Buffer);
                handler(str);
                Console.WriteLine(str);
            }
        }
    }
}
