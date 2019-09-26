using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileUtil.Util
{
    /// <summary>
    /// Http 请求工具类
    /// </summary>
    public static class HttpUtil
    {
        private static HttpClient http_client = null;

        public static HttpClient GetClient()
        {
            if (http_client == null)
            {
                http_client = new HttpClient(new HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                });
                http_client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                http_client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            }
            return http_client;
        }

        public static String GetString(string url, string contentType = "text/html", String encode = "utf-8")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 60000;

                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding(encode)))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                httpWebRequest.Abort();
            }
        }

        public static String PostGetString(string url, string body = "", string contentType = "text/html", String encode = "utf-8")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 60000;

                byte[] btBodys = Encoding.GetEncoding(encode).GetBytes(body);
                httpWebRequest.ContentLength = btBodys.Length;
                Stream req_stream = httpWebRequest.GetRequestStream();
                req_stream.Write(btBodys, 0, btBodys.Length);

                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    req_stream.Close();
                    return streamReader.ReadToEnd();
                }
            }
            finally
            {
                httpWebRequest.Abort();
            }
        }

        public static Byte[] GetGetBytes(string url, string contentType = "text/html", String encode = "utf-8")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 60000;

                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                using (Stream streamReader = httpWebResponse.GetResponseStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] bArr = new byte[1024];
                    int size = streamReader.Read(bArr, 0, (int)bArr.Length);
                    ms.Write(bArr, 0, size);
                    while (size > 0)
                    {
                        size = streamReader.Read(bArr, 0, (int)bArr.Length);
                        ms.Write(bArr, 0, size);
                    }
                    return ms.ToArray();
                }
            }
            finally
            {
                httpWebRequest.Abort();
            }
        }
    }
}
