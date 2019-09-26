using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileUtil.Util
{
    /// <summary>
    /// 文件处理工具类
    /// </summary>
    public static class FileUtil
    {
        public static Boolean temp_dir_checked = false;

        public static Object SaveFile(this String url)
        {
            if (url == null)
            {
                return null;
            }
            if (url.Contains("?"))
            {
                url = url.Split(new char[] { '?' })[0];
            }
            if (!temp_dir_checked && !Directory.Exists("./temp"))
            {
                if (!Directory.Exists("./temp"))
                {
                    Directory.CreateDirectory("./temp");
                }
                temp_dir_checked = true;
            }

            String file_name = Guid.NewGuid().ToString("N") + Path.GetExtension(url);
            try
            {
                File.WriteAllBytes("./temp/" + file_name, HttpUtil.GetGetBytes(url));
                return new
                {
                    url,
                    file_name,
                    path = "./temp/" + file_name
                };
            }
            catch (Exception Ex)
            {
                return null;
            }
        }
    }
}
