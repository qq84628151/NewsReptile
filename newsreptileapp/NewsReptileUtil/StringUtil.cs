using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileUtil.Util
{
    public static class StringUtil
    {
        /// <summary>
        /// 转换jsonp字符串为JObject
        /// </summary>
        public static JObject JsonpToJObject(this String json_str, String start_str = "(", String end_str = ")")
        {
            Int32 start_index = json_str.IndexOf(start_str) + 1;
            Int32 end_index = json_str.LastIndexOf(end_str);
            return JObject.Parse(json_str.Substring(start_index, end_index - start_index));
        }

        /// <summary>
        /// 转换jsonp字符串为JArray
        /// </summary>
        public static JArray JsonpToJArray(this String json_str)
        {
            Int32 start_index = json_str.IndexOf("(") + 1;
            Int32 end_index = json_str.LastIndexOf(")");
            return JArray.Parse(json_str.Substring(start_index, end_index - start_index));
        }
        /// <summary>
        /// 转换json字符串为JObject
        /// </summary>
        public static JObject JsonToJObject(this String json_str)
        {
            return JObject.Parse(json_str);
        }

        /// <summary>
        /// 转换json字符串为JArray
        /// </summary>
        public static JArray JsonToJArray(this String json_str)
        {
            return JArray.Parse(json_str);
        }
        /// <summary>
        /// 从Javascript字符串获取变量的值
        /// </summary>
        /// <returns></returns>
        public static String FindJSVarValue(this String input, String var_name)
        {
            Int32 id_str_index = input.IndexOf(var_name);
            Int32 id_str_start_index = input.IndexOf("\"", id_str_index) + 1;
            Int32 id_str_end_index = input.IndexOf("\"", id_str_start_index);
            return input.Substring(id_str_start_index, id_str_end_index - id_str_start_index);
        }

        public static String NotEmpty(params String[] strs)
        {
            foreach (String str in strs)
            {
                if (!String.IsNullOrEmpty(str))
                {
                    return str;
                }
            }
            return null;
        }
    }
}
