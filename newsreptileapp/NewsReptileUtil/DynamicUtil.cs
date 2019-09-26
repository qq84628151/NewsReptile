using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewsReptileUtil.Util
{
    public static class DynamicUtil
    {
         static readonly Dictionary<Type , Dictionary<String, PropertyInfo>> cache = new Dictionary<Type, Dictionary<String, PropertyInfo>>();

        public static dynamic Attr(this Object obj, string p)
        {
            Type _type = obj.GetType();
            lock (cache)
            {
                if (cache.ContainsKey(_type) && cache[_type].ContainsKey(p))
                {
                    return cache[_type][p] == null ? null : cache[_type][p].GetValue(obj);
                }
                var pro = _type.GetProperties().Where(v => v.Name == p).FirstOrDefault();
                if (!cache.ContainsKey(_type))
                {
                    cache.Add(_type, new Dictionary<String, PropertyInfo>());
                }
                cache[_type].Add(p, pro);
                return pro == null ? null : pro.GetValue(obj);
            }
        }
    }
}
