using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

namespace NewsReptileUtil.Util.DBUtil
{
    public static class DBUtil
    {
        public static List<T> DataTableToList<T>(DataTable dt)
        {
            List<T> list = new List<T>(dt.Rows.Count);
            for (Int32 row_index = 0; row_index < dt.Rows.Count; ++row_index)
            {
                T model = (T)Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo pro in typeof(T).GetProperties())
                {
                    pro.SetValue(model, dt.Rows[row_index][pro.Name] == DBNull.Value ? null : dt.Rows[row_index][pro.Name], null);
                }
                list.Add(model);
            }
            return list;
        }
    }
}
