using NewReaderWpfApp.Windows;
using NewsReptileDB.DB.Bll;
using NewsReptileDB.DB.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewReaderWpfApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        web_dataBll _web_dataBll = new web_dataBll();
        article_contentBll _article_contentBll = new article_contentBll();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WinLoaded(object sender, RoutedEventArgs e)
        {
            LoadWebData();
        }

        private void LoadWebData()
        {
            web_data_dg.ItemsSource = _web_dataBll.GetAll();
        }

        private void ViewInfoWebData(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Int32 id = (Int32)btn.Tag;

            web_dataModel model = _web_dataBll.GetById(id);
            ViewInfoWindow view_info_win = new ViewInfoWindow();
            view_info_win._title = model.title.Trim();
            view_info_win._content = _article_contentBll.ReadContent(id, 1);
            view_info_win._url = model.url;
            view_info_win.ShowDialog();
        }
    }
}
