using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using NewsReptileUtil.Util.DBUtil;

namespace NewsReptileDB.DB.Dal
{
    public class base_dal<T>
    {
        protected const int CONTET_WRITE_MAX_LENGTH = 3800;

        private readonly String sqlite_conn = @"server=DESKTOP-9FV5R3K\SQLEXPRESS;database=NewsReptileApp;Integrated Security =true;";
        private readonly Type _type = typeof(T);
        private readonly PropertyInfo[] pros = typeof(T).GetProperties();

        public Int32 Add(T model)
        {
            String table_name = _type.Name.Replace("Model", "");
            using (SqlConnection conn = new SqlConnection(sqlite_conn))
            {
                conn.Open();
                String add_sql = String.Format("insert into {0}({1}) values({2});select @identityV=@@IDENTITY;", table_name, String.Join(",", pros.Where(v => v.Name != "id" && v.GetValue(model) != null).Select(v => v.Name)), String.Join(",", pros.Where(v => v.Name != "id" && v.GetValue(model) != null).Select(v => "@" + v.Name)));
                using (SqlCommand cmd = new SqlCommand(add_sql, conn))
                {
                    var pars_list = pros.Where(v => v.Name != "id" && v.GetValue(model) != null).Select(v=> new SqlParameter("@" + v.Name, v.GetValue(model))).ToList();
                    pars_list.Add(new SqlParameter("@identityV", SqlDbType.Int));
                    pars_list.Last().Direction = ParameterDirection.Output;
                    cmd.Parameters.AddRange(pars_list.ToArray());
                    cmd.ExecuteNonQuery();
                    return (Int32)pars_list.Last().Value;
                }
            }
        }

        public Int32 Update(T model)
        {
            using (SqlConnection conn = new SqlConnection(sqlite_conn))
            {
                conn.Open();
                String update_sql = String.Format("update {0} set {1} where id = @id", _type.Name, String.Join(",", "set = @" + pros.Select(v => v.Name)));
                using (SqlCommand cmd = new SqlCommand(update_sql, conn))
                {
                    var pars_list = pros.Select(v => new SqlParameter("@" + v.Name, v.GetValue(model, null))).ToArray();
                    cmd.Parameters.AddRange(pars_list);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public Int32 Delete(Int32 id)
        {
            String table_name = _type.Name.Replace("Model", "");
            using (SqlConnection conn = new SqlConnection(sqlite_conn))
            {
                conn.Open();
                String delete_sql = String.Format("delete from {0} where id = {1}", table_name, id);
                using (SqlCommand cmd = new SqlCommand(delete_sql, conn))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public List<T> GetAll(String order_col = "create_datetime", String order = "desc")
        {
            String table_name = _type.Name.Replace("Model", "");
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(sqlite_conn))
            {
                String get_all_sql = String.Format("select * from {0} order by {1} {2}", table_name, order_col, order);
                SqlDataAdapter adapter = new SqlDataAdapter(get_all_sql, conn);
                adapter.Fill(dt);
            }
            return DBUtil.DataTableToList<T>(dt);
        }

        public T GetById(Int32 id)
        {
            String table_name = _type.Name.Replace("Model", "");
            SqlParameter[] _params = new SqlParameter[] {
                new SqlParameter("@id", id)
            };
            return GetOne(String.Format("select top 1 * from {0} where id = @id", table_name), _params);
        }

        protected List<T> GetList(String sql, SqlParameter[] _params = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(sqlite_conn))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                if (_params != null && _params.Length > 0)
                {
                    adapter.SelectCommand.Parameters.AddRange(_params);
                }
                adapter.Fill(dt);
            }
            return DBUtil.DataTableToList<T>(dt);
        }

        protected T GetOne(String sql, SqlParameter[] _params = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(sqlite_conn))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                if (_params != null && _params.Length > 0)
                {
                    adapter.SelectCommand.Parameters.AddRange(_params);
                }
                adapter.Fill(dt);
            }
            List<T> list = DBUtil.DataTableToList<T>(dt);
            return list == null || list.Count == 0 ? default : list[0];
        }
    }
}
