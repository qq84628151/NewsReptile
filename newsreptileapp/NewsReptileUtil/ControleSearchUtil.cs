using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NewsReptileUtil.Util
{
    /// <summary>
    /// 子元素和父元素查询查找工具类
    /// </summary>
    public static class ControleSearchUtil
    {
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name || string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    return GetChildObject<T>(child, name);
                }
            }
            return null;
        }

        public static List<T> GetChildObjects<T>(DependencyObject obj, string name, List<T> childList = null) where T : FrameworkElement
        {
            childList = childList ?? new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }
                GetChildObjects<T>(child, null, childList);
            }
            return childList;

        }
    }
}
