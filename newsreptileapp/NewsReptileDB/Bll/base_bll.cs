using NewsReptileDB.DB.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewsReptileDB.DB.Bll
{
    public class base_bll<T>
    {
        private base_dal<T> _base_dal = new base_dal<T>();

        public Int32 Add(T t)
        {
            return _base_dal.Add(t);
        }
        public List<T> GetAll()
        {
            return _base_dal.GetAll();
        }
        public T GetById(Int32 id)
        {
            return _base_dal.GetById(id);
        }
    }
}
