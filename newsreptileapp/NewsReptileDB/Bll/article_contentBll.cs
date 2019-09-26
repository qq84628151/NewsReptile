using NewsReptileDB.DB.Dal;
using NewsReptileDB.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileDB.DB.Bll
{
    public class article_contentBll : base_bll<article_contentModel>
    {
        private article_contentDal dal = new article_contentDal();

        public void Add(String content, Int32 web_data_id, Int32 _type = 0)
        {
            dal.Add(content, web_data_id, _type);
        }

        public String ReadContent(Int32 web_data_id, Int32 _type)
        {
            return dal.ReadContent(web_data_id, _type);
        }
    }
}
