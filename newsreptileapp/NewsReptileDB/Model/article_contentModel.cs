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
        public Int32? web_data_id { get; set; }
        public String data { get; set; }
        public String data_original { get; set; }
        /// <summary>
        /// 0为原始  1为文本
        /// </summary>
        public Int32? _type { get; set; }
        public Int32? _index { get; set; }
    }
}
