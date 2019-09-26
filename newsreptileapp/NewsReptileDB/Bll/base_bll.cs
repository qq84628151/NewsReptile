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
        /// <summary>
        /// 添加
        /// </summary>
        public Int32 Add(T t)
        {
            return _base_dal.Add(t);
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        public List<T> GetAll()
        {
            return _base_dal.GetAll();
        }
        /// <summary>
        /// 根据id获取数据
        /// </summary>
        public T GetById(Int32 id)
        {
            return _base_dal.GetById(id);
        }
    }
}
