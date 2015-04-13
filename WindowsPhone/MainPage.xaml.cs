using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Storage.Streams;
using Windows.Storage;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace UdpPPTPhone
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Object ip = localSettings.Values["IP"];
            if (ip != null)
            {
                TB_IP.Text = ip.ToString();
            }
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            try
            {
                new HostName(TB_IP.Text);
            }
            catch
            {
                return;
            }
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["IP"] = TB_IP.Text;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: 准备此处显示的页面。
            
            // TODO: 如果您的应用程序包含多个页面，请确保
            // 通过注册以下事件来处理硬件“后退”按钮:
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed 事件。
            // 如果使用由某些模板提供的 NavigationHelper，
            // 则系统会为您处理该事件。
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new HostName(TB_IP.Text);
            }
            catch
            {
                return;
            }
            SendMsg(TB_IP.Text, ((Button)sender).Tag.ToString()); 
        }

        private async void SendMsg(string ip,string msg)
        {
            DatagramSocket udp = new DatagramSocket();
            HostName host = new HostName(ip);
            var outStream = await udp.GetOutputStreamAsync(host, "8788");
            DataWriter writer = new DataWriter(outStream);
            // 往流里面写数据
            writer.WriteString(msg);
            await writer.StoreAsync();
            writer.DetachStream();
        }
    }
}
