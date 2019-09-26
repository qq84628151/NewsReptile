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
        public String source { get; set; }
        public String source2 { get; set; }
        public String top_title { get; set; }
        public String toptitle_original { get; set; }
        public String title { get; set; }
        public String title_original { get; set; }
        public String url { get; set; }
        public DateTime? article_time { get; set; }
        public String img_path { get; set; }
        /// <summary>
        /// 1为特殊类型，内容为原始带标签
        /// </summary>
        public Int32? _type { get; set; }
        public String descript { get; set; }
        public DateTime? create_datetime { get; set; }
        public Boolean? del { get; set; }
    }
}
