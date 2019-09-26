using NewsReptileDB.DB.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileDB.DB.Dal
{
    public class article_contentDal : base_dal<article_contentModel>
    {
        public void Add(String content, Int32 web_data_id, Int32 _type = 0)
        {
            //防止数据长度溢出，进行分块处理
            var count_org = content.Length % CONTET_WRITE_MAX_LENGTH == 0 ? content.Length / CONTET_WRITE_MAX_LENGTH : content.Length / CONTET_WRITE_MAX_LENGTH + 1;
            for (int i = 0; i < count_org; ++i)
            {
                base.Add(new article_contentModel()
                {
                    _type = _type,
                    data = i == count_org - 1 ? content.Substring(i * CONTET_WRITE_MAX_LENGTH) : content.Substring(i * CONTET_WRITE_MAX_LENGTH, CONTET_WRITE_MAX_LENGTH),
                    web_data_id = web_data_id,
                    _index = i
                });
            }
        }

        public String ReadContent(Int32 web_data_id, Int32 _type)
        {
            SqlParameter[] _params = new SqlParameter[] {
                new SqlParameter("@web_data_id", web_data_id),
                new SqlParameter("@_type", _type)
            };
            List<article_contentModel> article_content_list = base.GetList("select * from article_content where web_data_id = @web_data_id and _type = @_type order by _index", _params);
            StringBuilder str_buil = new StringBuilder();
            foreach (var model in article_content_list)
            {
                str_buil.Append(model.data);
            }
            return str_buil.ToString();
        }
    }
}
