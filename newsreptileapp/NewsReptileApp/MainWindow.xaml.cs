using HtmlAgilityPack;
using NewsReptileApp.Reptile;
using NewsReptileDB.DB.Bll;
using NewsReptileDB.DB.Model;
using NewsReptileUtil.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace NewsReptileApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        protected static readonly article_contentBll _article_contentBll = new article_contentBll();
        protected static readonly web_dataBll _web_dataBll = new web_dataBll();
        protected static readonly web_imgBll _web_imgBll = new web_imgBll();
        //采集标识
        private static List<Boolean> reptile_flag_list = new List<Boolean>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            if ((String)start_btn.Content == "停止采集")
            {
                reptile_flag_list.Clear();
                start_btn.Content = "开始采集";
                return;
            }
            start_btn.Content = "停止采集";
            var configs = JObject.Parse(File.ReadAllText("Config/test.json"));
            Boolean reptile_flag = true;
            reptile_flag_list.Add(reptile_flag);

            new Thread(()=> {
                while (reptile_flag_list.Contains(reptile_flag) && reptile_flag)
                {
                    #region CCTV-各大板块
                    CCTV_News((JObject)configs["CCTV_Focus_News_Article"], "http://news.cctv.com/2019/07/gaiban/cmsdatainterface/page/news_{0}.jsonp?cb=t&cb=news", "CCTV-新闻板块要闻", new String[] { "央视" });
                    CCTV_News((JObject)configs["CCTV_Focus_News_Article"], "http://news.cctv.com/2019/07/gaiban/cmsdatainterface/page/tech_{0}.jsonp?cb=t&cb=tech", "CCTV-科技板块要闻", new String[] { "央视", "科技" });
                    CCTV_News((JObject)configs["CCTV_Focus_News_Article"], "http://news.cctv.com/2019/07/gaiban/cmsdatainterface/page/china_{0}.jsonp?cb=t&cb=china", "CCTV-经济板块要闻", new String[] { "央视", "经济" });
                    CCTV_News((JObject)configs["CCTV_Focus_News_Article"], "http://news.cctv.com/2019/07/gaiban/cmsdatainterface/page/china_{0}.jsonp?cb=t&cb=china", "CCTV-国内板块要闻", new String[] { "央视" });
                    CCTV_News((JObject)configs["CCTV_Focus_News_Article"], "http://news.cctv.com/2019/07/gaiban/cmsdatainterface/page/world_{0}.jsonp?cb=t&cb=china", "CCTV-国际板块要闻", new String[] { "央视" });
                    #endregion

                    #region CCTV首页焦点新闻
                    CommonReptile.Start("http://www.cctv.com/", configs["CCTV_Focus_News"], null, (result, state) =>
                    {
                        List<Task> task_list = new List<Task>();
                        List<Hashtable> focus_news = (List<Hashtable>)result["focus news"];
                        foreach (Hashtable hash in focus_news)
                        {
                            if (((String)hash["title"]).Length <= 4)
                            {
                                return;
                            }
                            task_list.Add(Handle_News((String)hash["url"], (JObject)configs["CCTV_Focus_News_Article"], "CCTV首页-焦点新闻", new String[] { "央视" }, (String)hash["title"], (String)hash["title_html"]));
                        }
                        AddTask("正在采集：CCTV首页-焦点新闻");
                        Task.WaitAll(task_list.ToArray());
                        task_list.Clear();
                        SuccessTask("采集完成：CCTV首页-焦点新闻 ");
                    });
                    #endregion

                    #region 新华网-国际新闻板块
                    CommonReptile.Start("http://www.xinhuanet.com/world/index.htm", (JObject)configs["XinHuaWang_GuoJiBanKuai"], null, (result, state) =>
                    {
                        List<Task> task_list = new List<Task>();
                        List<Hashtable> focus_news = (List<Hashtable>)result["focus news"];
                        foreach (Hashtable hash in focus_news)
                        {
                            if (((String)hash["title"]).Length <= 4)
                            {
                                continue;
                            }
                            task_list.Add(CommonReptile.Start((String)hash["url"], (JObject)configs["XinHuaWang_Focus_News_Article"], null, (result2, state2) =>
                            {
                                if (result2["title_text"] == null)
                                {
                                    return;
                                }
                                var source = StringUtil.NotEmpty((String)result2["source1"], (String)result2["source2"]);

                                web_dataModel model = new web_dataModel();
                                model.top_title = (String)result2["title_text"];
                                model.toptitle_original = (String)result2["title_html"];
                                model.title = (String)result2["title_text"];
                                model.title_original = (String)result2["title_html"];
                                model.source = "新华网-国际板块热闻";
                                model.source2 = source != null ? source : ((String)result2["source3"]).Split(new Char[] { '：' })[1];
                                model.article_time = DateTime.Parse((String)result2["time"]);
                                model.url = (String)hash["url"];

                                if (!model.source2.Contains("新华") || model.article_time.Value.ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd") || _web_dataBll.GetByTitleAndSource(model.title, "新华网-国际板块热闻") != null)
                                {
                                    return;
                                }
                                //文章数据入库
                                Int32 insert_id = _web_dataBll.Add(model);
                                _article_contentBll.Add((String)result2["content_text"], insert_id, 1);
                                _article_contentBll.Add((String)result2["content_html"], insert_id);
                                //图片入库
                                List<Hashtable> img_list = (List<Hashtable>)result2["imgs"];
                                if (img_list != null && img_list.Count > 0)
                                {
                                    foreach (var img in img_list)
                                    {
                                        _web_imgBll.Add(new web_imgModel()
                                        {
                                            url = img["img"].Attr("url"),
                                            img_path = img["img"].Attr("path"),
                                            web_data_id = insert_id
                                        });
                                    }
                                }
                            }));
                        }
                        AddTask("正在采集： 新华网-国际板块热闻");
                        Task.WaitAll(task_list.ToArray());
                        task_list.Clear();
                        SuccessTask("采集完成： 新华网-国际板块热闻 ");
                    });
                    #endregion

                    #region 新华网-科技板块
                    CommonReptile.Start("http://www.xinhuanet.com/tech/index.htm", (JObject)configs["XinHua_KejiBanKuai"], null, (result, state) =>
                    {
                        List<Task> task_list = new List<Task>();
                        List<Hashtable> focus_news = (List<Hashtable>)result["focus news"];
                        if (focus_news == null)
                        {
                            return;
                        }
                        foreach (Hashtable hash in focus_news)
                        {
                            if (((String)hash["title"]).Length <= 4)
                            {
                                continue;
                            }
                            task_list.Add(CommonReptile.Start((String)hash["url"], (JObject)configs["XinHuaWang_Focus_News_Article"], null, (result2, state2) =>
                            {
                                if (result2["title_text"] == null)
                                {
                                    return;
                                }

                                var source = StringUtil.NotEmpty((String)result2["source1"], (String)result2["source2"]);
                                web_dataModel model = new web_dataModel();
                                model.top_title = (String)result2["title_text"];
                                model.toptitle_original = (String)result2["title_html"];
                                model.title = (String)result2["title_text"];
                                model.title_original = (String)result2["title_html"];
                                model.source = "新华网-科技板块热闻";
                                model.source2 = source != null ? source : ((String)result2["source3"]).Split(new Char[] { '：' })[1];
                                model.article_time = DateTime.Parse((String)result2["time"]);
                                model.url = (String)hash["url"];
                                if ((!model.source2.Contains("新华") && !model.source2.Contains("科技")) || model.article_time.Value.ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd") || _web_dataBll.GetByTitleAndSource(model.title, "新华网-科技板块热闻") != null)
                                {
                                    return;
                                }
                                //文章数据入库
                                Int32 insert_id = _web_dataBll.Add(model);
                                _article_contentBll.Add((String)result2["content_text"], insert_id, 1);
                                _article_contentBll.Add((String)result2["content_html"], insert_id);
                                //图片入库
                                List<Hashtable> img_list = (List<Hashtable>)result2["imgs"];
                                if (img_list != null && img_list.Count > 0)
                                {
                                    foreach (var img in img_list)
                                    {
                                        _web_imgBll.Add(new web_imgModel()
                                        {
                                            url = img["img"].Attr("url"),
                                            img_path = img["img"].Attr("path"),
                                            web_data_id = insert_id
                                        });
                                    }
                                }
                            }));
                        }
                        AddTask("正在采集： 新华网-科技板块热闻");
                        Task.WaitAll(task_list.ToArray());
                        task_list.Clear();
                        SuccessTask("采集完成： 新华网-科技板块热闻 ");
                    });
                    #endregion

                    #region 新华网-财经板块
                    new Thread(() =>
                    {
                        JObject json_data = HttpUtil.GetString("http://qc.wa.news.cn/nodeart/list?nid=11147664&pgnum=1&cnt=16&tp=1&orderby=1").Trim('(').Trim(')').JsonToJObject();
                        List<Task> task_list = new List<Task>();
                        foreach (JObject jObj in json_data["data"]["list"])
                        {
                            task_list.Add(CommonReptile.Start((String)jObj["LinkUrl"], (JObject)configs["XinHuaWang_Focus_News_Article"], null, (result2, state2) =>
                            {
                                if (result2["title_text"] == null)
                                {
                                    return;
                                }

                                var source = StringUtil.NotEmpty((String)result2["source1"], (String)result2["source2"]);
                                web_dataModel model = new web_dataModel();
                                model.top_title = (String)result2["title_text"];
                                model.toptitle_original = (String)result2["title_html"];
                                model.title = (String)result2["title_text"];
                                model.title_original = (String)result2["title_html"];
                                model.source = "新华网-财经板块";
                                model.source2 = source != null ? source : ((String)result2["source3"]).Split(new Char[] { '：' })[1];
                                model.article_time = DateTime.Parse((String)result2["time"]);
                                model.url = (String)jObj["LinkUrl"];
                                if ((!model.source2.Contains("新华") && !model.source2.Contains("科技")) || model.article_time.Value.ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd") || _web_dataBll.GetByTitleAndSource(model.title, "新华网-财经板块") != null)
                                {
                                    return;
                                }
                                //文章数据入库
                                Int32 insert_id = _web_dataBll.Add(model);
                                _article_contentBll.Add((String)result2["content_text"], insert_id, 1);
                                _article_contentBll.Add((String)result2["content_html"], insert_id);
                                //图片入库
                                List<Hashtable> img_list = (List<Hashtable>)result2["imgs"];
                                if (img_list != null && img_list.Count > 0)
                                {
                                    foreach (var img in img_list)
                                    {
                                        _web_imgBll.Add(new web_imgModel()
                                        {
                                            url = img["img"].Attr("url"),
                                            img_path = img["img"].Attr("path"),
                                            web_data_id = insert_id
                                        });
                                    }
                                }
                            }));
                        }
                        AddTask("正在采集： 新华网-财经板块");
                        Task.WaitAll(task_list.ToArray());
                        task_list.Clear();
                        SuccessTask("采集完成：新华网-财经板块 ");
                    }).Start();
                    #endregion

                    #region 人民日报
                    CommonReptile.Start("http://www.people.com.cn/", (JObject)configs["RenMinRiBao_ShouYeBanKuai"], null, (result, state) =>
                    {
                        List<Task> task_list = new List<Task>();
                        List<Hashtable> focus_news = (List<Hashtable>)result["focus news"];
                        if (focus_news == null)
                        {
                            return;
                        }
                        foreach (Hashtable hash in focus_news)
                        {
                            if (((String)hash["title"]).Length <= 4)
                            {
                                continue;
                            }
                            task_list.Add(CommonReptile.Start((String)hash["url"], (JObject)configs["RenMinRiBao_Focus_News_Article"], null, (result2, state2) =>
                            {
                                if (result2["title_text"] == null)
                                {
                                    return;
                                }
                                var source = ((String)result2["source_time"]).Split(new String[] { "来源：" }, StringSplitOptions.RemoveEmptyEntries);

                                web_dataModel model = new web_dataModel();
                                model.top_title = (String)result2["title_text"];
                                model.toptitle_original = (String)result2["title_html"];
                                model.title = (String)result2["title_text"];
                                model.title_original = (String)result2["title_html"];
                                model.source = "人民日报-首页热闻热闻";
                                model.source2 = source[1];
                                model.article_time = DateTime.Parse(source[0].Replace("&nbsp;", ""));
                                model.url = (String)hash["url"];
                                if (!model.source2.Contains("人民网") || model.article_time.Value.ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd") || _web_dataBll.GetByTitleAndSource(model.title, "人民日报-首页热闻热闻") != null)
                                {
                                    return;
                                }
                                //文章数据入库
                                Int32 insert_id = _web_dataBll.Add(model);
                                _article_contentBll.Add((String)result2["content_text"], insert_id, 1);
                                _article_contentBll.Add((String)result2["content_html"], insert_id);
                                //图片入库
                                List<Hashtable> img_list = (List<Hashtable>)result2["imgs"];
                                if (img_list != null && img_list.Count > 0)
                                {
                                    foreach (var img in img_list)
                                    {
                                        _web_imgBll.Add(new web_imgModel()
                                        {
                                            url = img["img"].Attr("url"),
                                            img_path = img["img"].Attr("path"),
                                            web_data_id = insert_id
                                        });
                                    }
                                }
                            }, "gb2312"));
                        }
                        AddTask("正在采集： 人民日报-首页热闻热闻");
                        Task.WaitAll(task_list.ToArray());
                        task_list.Clear();
                        SuccessTask("采集完成： 人民日报-首页热闻热闻 ");
                    }, "gb2312");
                    #endregion
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
                reptile_flag_list.Remove(reptile_flag);
                StopTask();
            }).Start();
        }

        /// <summary>
        /// 添加一个采集任务
        /// </summary>
        public void AddTask(String text_line)
        {
            this.Dispatcher.Invoke(() =>
            {
                progress.Maximum++;
                html_str.Text += text_line + Environment.NewLine;
                html_str.ScrollToEnd();
                task_count.Text = String.Format("总任务数:{0}\n已完成数:{1}", progress.Maximum, progress.Value);
            });
        }
        /// <summary>
        /// 完成一个采集任务
        /// </summary>
        public void SuccessTask(String success_line)
        {
            this.Dispatcher.Invoke(() =>
            {
                progress.Value++;
                html_str.Text += success_line + Environment.NewLine;
                html_str.ScrollToEnd();
                task_count.Text = String.Format("总任务数:{0}\n已完成数:{1}", progress.Maximum, progress.Value);
            });
        }
        /// <summary>
        /// 停止采集任务
        /// </summary>
        public void StopTask()
        {
            this.Dispatcher.Invoke(() =>
            {
                html_str.Text += "已停止采集" + Environment.NewLine;
                html_str.ScrollToEnd();
            });
        }

        #region 统一处理CCTV不同板块数据
        private Task Handle_News(String url, JObject jObj, String source, String[] filter = null, String top_title = null, String toptitle_original = null)
        {
            return CommonReptile.Start(url, jObj, null, (result, state) =>
            {
                web_dataModel model = new web_dataModel();
                String content = null;
                String content_html = null;

                if (result.ContainsKey("title_text"))
                {
                    //旧版处理
                    String[] source_time = null;
                    if (((String)result["source_time"]).Contains("来源："))
                    {
                        source_time = ((String)result["source_time"]).Replace("来源：", "").Split(new char[] { ' ' }, 2);
                    }
                    else
                    {
                        source_time = ((String)result["source_time"]).Split(new char[] { ' ' }, 3).Where(v => v.Length > 1).ToArray();
                    }
                    model.top_title = top_title == null ? (String)result["title_text"] : top_title;
                    model.toptitle_original = toptitle_original == null ? (String)result["title_html"] : toptitle_original;
                    model.title = (String)result["title_text"];
                    model.title_original = (String)result["title_html"];
                    model.source = source;
                    model.source2 = source_time[0];
                    model.article_time = DateTime.Parse(source_time[1]);
                    content = (String)result["content_text"];
                    content_html = (String)result["content_html"];
                }
                else if (result.ContainsKey("title_text2"))
                {
                    //新版处理
                    var source_time = ((String)result["source_time2"]).Replace("来源：", "").Split(new char[] { '|' }, 2);
                    model.top_title = top_title == null ? (String)result["title_text2"] : top_title;
                    model.toptitle_original = toptitle_original == null ? (String)result["title_html2"] : toptitle_original;
                    model.title = (String)result["title_text2"];
                    model.title_original = (String)result["title_html2"];
                    model.source = source;
                    model.source2 = source_time[0].Trim();
                    model.article_time = DateTime.Parse(source_time[1].Trim());
                    content = (String)result["content_text2"];
                    content_html = (String)result["content_html2"];
                }
                else
                {
                    return;
                }
                model.url = url;

                if (filter != null && filter.All(v => !model.source2.Contains(v)) || _web_dataBll.GetByTitleAndSource(model.title, source) != null)
                {
                    return;
                }
                //添加入库
                Int32 insert_id = _web_dataBll.Add(model);
                _article_contentBll.Add(content, insert_id, 1);
                _article_contentBll.Add(content_html, insert_id);

                //处理视频，可能会有多种版本的结构
                List<Hashtable> video_list = new List<Hashtable>();
                if (result["videos"] != null)
                {
                    video_list.AddRange((List<Hashtable>)result["videos"]);
                }
                if (result["videos2"] != null)
                {
                    video_list.AddRange((List<Hashtable>)result["videos2"]);
                }
                if (video_list != null && video_list.Count > 0)
                {
                    foreach (Hashtable hash in video_list)
                    {
                        Object file_info = null;
                        String video_id = (String)hash["script"];

                        //处理视频，js的guid可能会存在不同结构
                        if (video_id.Contains("guid"))
                        {
                            var _id = video_id.FindJSVarValue("guid");
                            if (String.IsNullOrEmpty(_id) || _id.Length < 5)
                            {
                                _id = video_id.FindJSVarValue("videoCenterId");
                            }
                            if (String.IsNullOrEmpty(_id) || _id.Length < 10)
                            {
                                continue;
                            }
                            JObject video_info = HttpUtil.GetString("https://vdn.apps.cntv.cn/api/getIpadVideoInfo.do?pid=" + _id + "&tai=ipad&from=html5").JsonpToJObject("'", "'");
                            file_info = ((String)video_info["hls_url"]).SaveFile();
                        }
                        else if (video_id.Contains("videoCenterId"))
                        {
                            var _id = video_id.FindJSVarValue("videoCenterId");
                            if (String.IsNullOrEmpty(_id) || _id.Length < 5)
                            {
                                _id = video_id.FindJSVarValue("guid");
                            }
                            if (String.IsNullOrEmpty(_id) || _id.Length < 10)
                            {
                                continue;
                            }
                            JObject video_info_JObj = HttpUtil.GetString("http://vdn.apps.cntv.cn/api/getHttpVideoInfo.do?pid=" + _id).JsonToJObject();
                            file_info = ((String)video_info_JObj["hls_url"]).SaveFile();
                        }
                        if (file_info != null)
                        {
                            _web_imgBll.Add(new web_imgModel()
                            {
                                url = file_info.Attr("url"),
                                img_path = file_info.Attr("path"),
                                web_data_id = insert_id
                            });
                        }
                    }
                }

                //处理图片，可能会有多种版本的结构
                List<Hashtable> img_list = new List<Hashtable>();
                if (result["imgs"] != null)
                {
                    img_list.AddRange((List<Hashtable>)result["imgs"]);
                }
                if (result["imgs2"] != null)
                {
                    img_list.AddRange((List<Hashtable>)result["imgs2"]);
                }
                if (img_list != null && img_list.Count > 0)
                {
                    foreach (var img in img_list)
                    {
                        _web_imgBll.Add(new web_imgModel()
                        {
                            url = img["img"].Attr("url"),
                            img_path = img["img"].Attr("path"),
                            web_data_id = insert_id
                        });
                    }
                }
            });
        }
        #endregion

        #region VVTC首页新闻版本焦点新闻
        private void CCTV_News(JObject config_obj, String url, String source, String[] filter)
        {
            new Thread(() =>
            {
                //该板块是通过web api获取数据，遍历100页数据，直至日期非今天
                List<Task> task_list = new List<Task>();
                for (int i = 1; i <= 100; ++i)
                {
                    JObject jObj = HttpUtil.GetString(String.Format(url, i)).JsonpToJObject();
                    foreach (JObject data in jObj["data"]["list"])
                    {
                        if (DateTime.Parse((String)data["focus_date"]).ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd"))
                        {
                            //非今天新闻则跳出外循环，作用是后续显示完成采集
                            i = 101;
                            break;
                        }
                        task_list.Add(Handle_News((String)data["url"], config_obj, source, filter));
                    }
                }
                AddTask("正在采集： " + source);
                Task.WaitAll(task_list.ToArray());
                task_list.Clear();
                SuccessTask("采集已完成： " + source);
            }).Start();
        }
        #endregion
    }
}
