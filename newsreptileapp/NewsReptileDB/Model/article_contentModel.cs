using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileDB.DB.Model
{
    public class article_contentModel
    {
        public Int32? id { get; set; }
        /// <summary>
        /// 网站数据表id
        /// </summary>
        public Int32? web_data_id { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public String data { get; set; }
        /// <summary>
        /// 原始数据（暂时没用）
        /// </summary>
        public String data_original { get; set; }
        /// <summary>
        /// 0为原始  1为文本
        /// </summary>
        public Int32? _type { get; set; }
        /// <summary>
        /// 数据索引
        /// </summary>
        public Int32? _index { get; set; }
    }
}
