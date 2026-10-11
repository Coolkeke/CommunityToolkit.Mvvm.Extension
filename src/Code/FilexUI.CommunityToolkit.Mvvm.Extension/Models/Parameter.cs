using System.Collections.Generic;
using System.Linq;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 保存通过导航和对话框操作传递的具名值。
    /// </summary>
    public class Parameter : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly List<KeyValuePair<string, object>> entries = new List<KeyValuePair<string, object>>();

        /// <summary>
        /// 获取已保存条目的数量。
        /// </summary>
        public int Count => entries.Count;

        /// <summary>
        /// 获取集合中已保存的去重键名。
        /// </summary>
        public IEnumerable<string> Keys => entries.Select(x => x.Key).Distinct();

        /// <summary>
        /// 获取所有已保存的值。
        /// </summary>
        public IEnumerable<object> Values => entries.Select(x => x.Value);

        /// <summary>
        /// 获取与指定键关联的第一个值。
        /// </summary>
        /// <param name="key">要查找的键。</param>
        /// <returns>找到时返回匹配值；否则返回 <c>null</c>。</returns>
        public object this[string key]
        {
            get
            {
                foreach (var entry in entries)
                {
                    if (string.Compare(entry.Key, key, System.StringComparison.Ordinal) == 0)
                    {
                        return entry.Value;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 向集合添加一个具名值。
        /// </summary>
        /// <param name="key">用于取回值的非空键。</param>
        /// <param name="value">要保存的值。</param>
        public void Add(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new System.ArgumentNullException(nameof(key));
            }

            entries.Add(new KeyValuePair<string, object>(key, value));
        }

        /// <summary>
        /// 判断集合是否包含指定键。
        /// </summary>
        /// <param name="key">要检查的键。</param>
        /// <returns>键存在时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public bool ContainsKey(string key)
        {
            return entries.Any(x => string.Compare(x.Key, key, System.StringComparison.Ordinal) == 0);
        }

        /// <summary>
        /// 按键获取值，并在需要时转换为请求的类型。
        /// </summary>
        /// <typeparam name="T">请求的返回类型。</typeparam>
        /// <param name="key">要查找的键。</param>
        /// <returns>找到时返回转换后的值；否则返回 <typeparamref name="T"/> 的默认值。</returns>
        public T GetValue<T>(string key)
        {
            var value = this[key];
            if (value == null)
            {
                return default(T);
            }

            if (value is T typedValue)
            {
                return typedValue;
            }

            return (T)System.Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// 返回遍历所有键值条目的枚举器。
        /// </summary>
        /// <returns>用于遍历已保存条目的枚举器。</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
