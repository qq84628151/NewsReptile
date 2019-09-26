using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileDB.DB.Model
{
    public class web_dataModel
    {
        public Int32? id { get; set; }
        /// <summary>
        /// 爬取来源
        /// </summary>
        public String source { get; set; }
        /// <summary>
        /// 文章来源
        /// </summary>
        public String source2 { get; set; }
        /// <summary>
        /// 开始标题
        /// </summary>
        public String top_title { get; set; }
        /// <summary>
        /// 开始标题的html数据
        /// </summary>
        public String toptitle_original { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public String title { get; set; }
        /// <summary>
        /// 文章标题html数据
        /// </summary>
        public String title_original { get; set; }
        /// <summary>
        /// 文章地址
        /// </summary>
        public String url { get; set; }
        /// <summary>
        /// 文章发布时间
        /// </summary>
        public DateTime? article_time { get; set; }
        /// <summary>
        /// 文章图片路径
        /// </summary>
        public String img_path { get; set; }
        /// <summary>
        /// 1为特殊类型，内容为原始带标签
        /// </summary>
        public Int32? _type { get; set; }
        /// <summary>
        /// 文章描述
        /// </summary>
        public String descript { get; set; }
        public DateTime? create_datetime { get; set; }
        public Boolean? del { get; set; }
    }
}
