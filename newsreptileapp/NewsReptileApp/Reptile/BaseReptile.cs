using NewsReptileDB.DB.Bll;
using NewsReptileUtil.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileApp.Reptile
{
    public class BaseReptile
    {
        protected static readonly article_contentBll _article_contentBll = new article_contentBll();
        protected static readonly web_dataBll _web_dataBll = new web_dataBll();
        protected static readonly web_imgBll _web_imgBll = new web_imgBll();
        protected const int CONTET_WRITE_MAX_LENGTH = 3800;
        /// <summary>
        /// 执行cmd，用于适用ffmpeg处理视频
        /// </summary>
        public static void ExecuteCmd(String[] lines)
        {
            Process myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.RedirectStandardInput = true;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.Start();
            foreach (var line in lines)
            {
                myProcess.StandardInput.WriteLine(line);
            }
            myProcess.StandardInput.AutoFlush = true;
            myProcess.WaitForExit();
            myProcess.Close();
        }
        /// <summary>
        /// 以制定编码获取html内容
        /// </summary>
        /// <param name="url">html地址</param>
        /// <param name="encode">解析编码</param>
        public static String GetHtmlData(String url, String encode)
        {
            if (encode == null)
            {
                return HttpUtil.GetString(url);
            }
            else
            {
                return HttpUtil.GetString(url, "text/html", encode);
            }
        }
    }
}
