using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewReaderWpfApp.Windows
{
    /// <summary>
    /// ViewInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ViewInfoWindow : Window
    {
        private ChromiumWebBrowser web_bro = new ChromiumWebBrowser();

        public String _title { get; set; }
        public String _content { get; set; }
        public String _url { get; set; }

        public ViewInfoWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            web_container.Children.Add(web_bro);
            web_bro.Address = _url;
            web_bro.FrameLoadEnd += Web_bro_FrameLoadEnd;
        }

        private void Web_bro_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            if (e.Url.Contains("xinhuanet.com"))
            {
                //过滤新华网无用数据
                e.Frame.ExecuteJavaScriptAsync("$('div.nav, div.h-p1, div.h-p2, div.part.part2.clearfix,#bottom,.footer,.fllow3-wap,.fllow1-wap.left,.net-logo').hide();$('.fixhead.domPC.fixhead-show,.fixhead.domPC').remove();");
            }
            //e.Frame.ExecuteJavaScriptAsync("$('div.clearfix,#ops_share,#rwb_bbstop').not('div.text_title, div.text_con').hide();");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            web_bro.Dispose();
        }
    }
}
