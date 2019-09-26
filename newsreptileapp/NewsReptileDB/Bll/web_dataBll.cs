using NewsReptileDB.DB.Dal;
using NewsReptileDB.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileDB.DB.Bll
{
    public class web_dataBll : base_bll<web_dataModel>
    {
        web_dataDal _web_dataDal = new web_dataDal();

        public web_dataModel GetByTopTitleAndSource(String top_title, String source)
        {
            return _web_dataDal.GetByTopTitleAndSource(top_title, source);
        }

        public web_dataModel GetByUrlAndSource(String url, String source)
        {
            return _web_dataDal.GetByUrlAndSource(url, source);
        }

        public web_dataModel GetByTitleAndSource(String title, String source)
        {
            return _web_dataDal.GetByTitleAndSource(title, source);
        }
    }
}
