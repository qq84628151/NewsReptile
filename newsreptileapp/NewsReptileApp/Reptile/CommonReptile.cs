using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using NewsReptileUtil.Util;

namespace NewsReptileApp.Reptile
{
    /// <summary>
    /// 通用多任务采集封装小型框架
    /// </summary>
    public class CommonReptile : BaseReptile
    {
        /// <summary>
        /// 开始爬取任务
        /// </summary>
        /// <param name="url">爬取的url</param>
        /// <param name="conf">内容分析规则</param>
        /// <param name="state">数据传递（用于多层爬取，互相传递数据）</param>
        /// <param name="action">任务结束，回调方法</param>
        /// <param name="encode">网页数据编码</param>
        public static Task Start(String url, Object conf, Object state, Action<Hashtable, JObject> action = null, String encode = "utf-8")
        {
            return Start(url, conf is JObject ? (JObject)conf : JObject.FromObject(conf), state == null ? null : JObject.FromObject(state), action, encode);
        }

        public static Task Start(String url, JObject conf, JObject state, Action<Hashtable, JObject> action = null, String encode = "utf-8")
        {
            return Task.Run(() =>
            {
                Hashtable result_list = new Hashtable();
                //配置对象的data_type为null或html，则表示以html方式分析
                if (conf["data_type"] == null || (String)conf["data_type"] == "html")
                {
                    String html_content = GetHtmlData(url, encode);
                    HtmlDocument html_doc = new HtmlDocument();
                    //加载html内容
                    html_doc.LoadHtml(html_content);
                    //过滤不需要的内容
                    if (conf["filters"] != null && ((JArray)conf["filters"]).Count > 0)
                    {
                        //临时整个文档标签内容
                        String html_str = html_doc.DocumentNode.OuterHtml;
                        foreach (String filter in conf["filters"])
                        {
                            HtmlNode filter_node = html_doc.DocumentNode.SelectSingleNode(filter);
                            if (filter_node != null)
                            {
                                //删除过滤的内容
                                html_str = html_str.Replace(filter_node.OuterHtml, "");
                            }
                        }
                        //重新加载html内容
                        html_doc.LoadHtml(html_str);
                    }
                    //遍历分析规则
                    foreach (JObject rule in conf["rules"])
                    {
                        //规则是否可以匹配到多个值
                        if (rule["is_multiple"] == null || (Boolean)rule["is_multiple"] == false)
                        {
                            if (rule["math_type"] == null || (String)rule["math_type"] == "xpath")
                            {
                                HtmlNode node = html_doc.DocumentNode.SelectSingleNode((String)rule["rule"]);
                                if (node == null)
                                {
                                    continue;
                                }
                                MathValAdd(result_list, node, rule, url);
                            }
                            else if ((String)rule["math_type"] == "regex")
                            {
                                Regex regex = rule["regex_options"] == null ? new Regex((String)rule["rule"]) : new Regex((String)rule["rule"], (RegexOptions)(Int32)rule["regex_options"]);
                                if (regex.IsMatch(html_content))
                                {
                                    result_list.Add((String)rule["rule_name"], RegexMatch(regex, html_content));
                                }
                                else
                                {
                                    result_list.Add((String)rule["rule_name"], null);
                                }
                            }
                        }
                        else
                        {
                            if (rule["math_type"] == null || (String)rule["math_type"] == "xpath")
                            {
                                HtmlNodeCollection nodes = html_doc.DocumentNode.SelectNodes((String)rule["rule"]);
                                if (nodes == null || nodes.Count == 0)
                                {
                                    continue;
                                }
                                List<Hashtable> mat_res_list = new List<Hashtable>();
                                foreach (HtmlNode node in nodes)
                                {
                                    Hashtable res_dict = new Hashtable();
                                    foreach (JObject match_rule in rule["match_rule"])
                                    {
                                        MathValAdd(res_dict, node, match_rule, url);
                                    }
                                    mat_res_list.Add(res_dict);
                                }
                                result_list.Add((String)rule["rule_name"], mat_res_list);
                            }
                            else if ((String)rule["math_type"] == "regex")
                            {
                                Regex regex = rule["regex_options"] == null ? new Regex((String)rule["rule"]) : new Regex((String)rule["rule"], (RegexOptions)(Int32)rule["regex_options"]);
                                if (regex.IsMatch(html_content))
                                {
                                    List<List<String>> root_list = new List<List<String>>();
                                    foreach (Match match in regex.Matches(html_content))
                                    {
                                        root_list.Add(RegexMatch(regex, html_content));
                                    }
                                    result_list.Add((String)rule["rule_name"], root_list);
                                }
                                else
                                {
                                    result_list.Add((String)rule["rule_name"], null);
                                }
                            }
                        }
                    }
                }
                action.Invoke(result_list, state);
            });
        }
        /// <summary>
        /// 正则爬取
        /// </summary>
        /// <param name="regex">正则表达式</param>
        /// <param name="html_content">html内容</param>
        public static List<String> RegexMatch(Regex regex, String html_content)
        {
            List<String> reg_match_list = new List<String>();
            foreach (Group group in regex.Match(html_content).Groups)
            {
                reg_match_list.Add(group.Value);
            }
            return reg_match_list;
        }
        /// <summary>
        /// 添加匹配的值
        /// </summary>
        /// <param name="dict">待添加数据的哈希表</param>
        /// <param name="node">网页元素</param>
        /// <param name="match_rule">匹配规则</param>
        /// <param name="web_url">当前爬取的地址</param>
        public static void MathValAdd(Hashtable dict, HtmlNode node, JObject match_rule, String web_url)
        {
            //如果匹配不到元素则添加null进哈希表
            if (node == null)
            {
                dict.Add((String)match_rule["rule_name"], null);
            }
            //如果值类型为null或text，则添加元素的文本内容
            else if ((String)match_rule["value_type"] == null || (String)match_rule["value_type"] == "text")
            {
                dict.Add((String)match_rule["rule_name"], node.InnerText);
            }
            //如果值类型为outer_html，则添加元素的html内容
            else if ((String)match_rule["value_type"] == "outer_html")
            {
                dict.Add((String)match_rule["rule_name"], node.OuterHtml);
            }
            //如果值类型为attr，则需要value_attr，用于添加元素的某属性
            else if ((String)match_rule["value_type"] == "attr")
            {
                dict.Add((String)match_rule["rule_name"], node.Attributes[(String)match_rule["value_attr"]].Value);
            }
            //如果值类型为normal_file，则下载文件的字节并保存成文件
            else if ((String)match_rule["value_type"] == "normal_file")
            {
                //根据value_attr获取元素的属性的值
                String img_src = node.Attributes[(String)match_rule["value_attr"]].Value;
                //处理分析文件地址
                img_src = !img_src.StartsWith("http") && !img_src.StartsWith("https") ? new Uri(new Uri(web_url), img_src).ToString() : img_src;
                //保存成文件
                Object file_obj = FileUtil.SaveFile(img_src);
                if (file_obj != null)
                {
                    dict.Add((String)match_rule["rule_name"], file_obj);
                }
            }
            else
            {
                dict.Add((String)match_rule["rule_name"], null);
            }
        }
    }
}
