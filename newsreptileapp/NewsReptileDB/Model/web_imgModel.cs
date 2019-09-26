using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileDB.DB.Model
{
    public class web_imgModel
    {
        public Int32? id { get; set; }
        /// <summary>
        /// 图片本地路径
        /// </summary>
        public String img_path { get; set; }
        /// <summary>
        /// 网站数据表id
        /// </summary>
        public Int32? web_data_id { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public String url { get; set; }
        public DateTime? create_datetime { get; set; }
        public Boolean? del { get; set; }
    }
}
