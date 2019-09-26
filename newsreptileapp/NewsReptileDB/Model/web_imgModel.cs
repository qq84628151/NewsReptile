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
        public String img_path { get; set; }
        public Int32? web_data_id { get; set; }
        public String url { get; set; }
        public DateTime? create_datetime { get; set; }
        public Boolean? del { get; set; }
    }
}
