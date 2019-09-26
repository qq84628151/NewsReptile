using NewsReptileDB.DB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileDB.DB.Dal
{
    public class web_dataDal : base_dal<web_dataModel>
    {
        public web_dataModel GetByTopTitleAndSource(String top_title, String source)
        {
            SqlParameter[] _params = new SqlParameter[] {
                new SqlParameter("@top_title", top_title),
                new SqlParameter("@source", source)
            };
            return base.GetOne("select * from web_data where top_title = @top_title and source = @source", _params);
        }

        public web_dataModel GetByTitleAndSource(String title, String source)
        {
            SqlParameter[] _params = new SqlParameter[] {
                new SqlParameter("@title", title),
                new SqlParameter("@source", source)
            };
            return base.GetOne("select * from web_data where title = @title and source = @source", _params);
        }

        public web_dataModel GetByUrlAndSource(String url, String source)
        {
            SqlParameter[] _params = new SqlParameter[] {
                new SqlParameter("@url", url),
                new SqlParameter("@source", source)
            };
            return base.GetOne("select * from web_data where url = @url and source = @source", _params);
        }
    }
}
