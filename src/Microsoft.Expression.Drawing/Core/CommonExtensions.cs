using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Core
{
    public static class CommonExtensions
    {
        public static void ForEach(this IEnumerable items, Action<object> action)
        {
            foreach (object obj in items)
            {
                action(obj);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T obj in items)
            {
                action(obj);
            }
        }

        public static void ForEach<T>(this IList<T> list, Action<T, int> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                action(list[i], i);
            }
        }

        public static bool EnsureListCount<T>(this IList<T> list, int count, Func<T> factory = null)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (list.EnsureListCountAtLeast(count, factory))
            {
                return true;
            }
            if (list.Count > count)
            {
                List<T> list2 = list as List<T>;
                if (list2 != null)
                {
                    list2.RemoveRange(count, list.Count - count);
                }
                else
                {
                    for (int i = list.Count - 1; i >= count; i--)
                    {
                        list.RemoveAt(i);
                    }
                }
                return true;
            }
            return false;
        }

        public static bool EnsureListCountAtLeast<T>(this IList<T> list, int count, Func<T> factory = null)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (list.Count < count)
            {
                List<T> list2 = list as List<T>;
                if (list2 != null && factory == null)
                {
                    list2.AddRange(new T[count - list.Count]);
                }
                else
                {
                    for (int i = list.Count; i < count; i++)
                    {
                        list.Add((factory == null) ? default(T) : factory());
                    }
                }
                return true;
            }
            return false;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            List<T> list = collection as List<T>;
            if (list != null)
            {
                list.AddRange(newItems);
                return;
            }
            foreach (T item in newItems)
            {
                collection.Add(item);
            }
        }

        public static T Last<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        public static void RemoveLast<T>(this IList<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        internal static T DeepCopy<T>(this T obj) where T : class
        {
            if (obj == null)
            {
                return default(T);
            }
            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }
            if (typeof(IList).IsAssignableFrom(type))
            {
                IList collection = (IList)Activator.CreateInstance(type);
                ((IList)((object)obj)).ForEach(delegate (object o)
                {
                    collection.Add(o.DeepCopy<object>());
                });
                return (T)((object)collection);
            }
            if (type.IsClass)
            {
                object obj2 = Activator.CreateInstance(obj.GetType());
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo propertyInfo in properties)
                {
                    if (propertyInfo.CanRead && propertyInfo.CanWrite)
                    {
                        object value = propertyInfo.GetValue(obj, null);
                        object value2 = propertyInfo.GetValue(obj2, null);
                        if (value != value2)
                        {
                            propertyInfo.SetValue(obj2, value.DeepCopy<object>(), null);
                        }
                    }
                }
                return (T)((object)obj2);
            }
            throw new NotImplementedException();
        }

        public static bool SetIfDifferent(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, object value)
        {
            object value2 = dependencyObject.GetValue(dependencyProperty);
            if (!object.Equals(value2, value))
            {
                dependencyObject.SetValue(dependencyProperty, value);
                return true;
            }
            return false;
        }

        public static bool ClearIfSet(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            object obj = dependencyObject.ReadLocalValue(dependencyProperty);
            if (obj != DependencyProperty.UnsetValue)
            {
                dependencyObject.ClearValue(dependencyProperty);
                return true;
            }
            return false;
        }

        public static IEnumerable<T> FindVisualDesendent<T>(this DependencyObject parent, Func<T, bool> condition) where T : DependencyObject
        {
            Queue<DependencyObject> queue = new Queue<DependencyObject>();
            parent.GetVisualChildren().ForEach(delegate (DependencyObject child)
            {
                queue.Enqueue(child);
            });
            while (queue.Count > 0)
            {
                DependencyObject next = queue.Dequeue();
                next.GetVisualChildren().ForEach(delegate (DependencyObject child)
                {
                    queue.Enqueue(child);
                });
                T candidate = next as T;
                if (candidate != null && condition(candidate))
                {
                    yield return candidate;
                }
            }
            yield break;
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            int N = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < N; i++)
            {
                yield return VisualTreeHelper.GetChild(parent, i);
            }
            yield break;
        }
    }
}
