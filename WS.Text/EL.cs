#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：WS.Text
* 项目描述 ：
* 类 名 称 ：EL
* 类 描 述 ：
* 所在的域 ：DESKTOP-KA4M82K
* 命名空间 ：WS.Text
* 机器名称 ：DESKTOP-KA4M82K 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：wagsn
* 创建时间 ：2018/11/28 11:46:31
* 更新时间 ：2018/11/28 11:46:31
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/
#endregion

using System.Text.RegularExpressions;

namespace WS.Text
{
    /// <summary>
    /// EL 表达式解析
    /// </summary>
    public class EL
    {
        /// <summary>
        /// 全局唯一的键值映射
        /// </summary>
        public static SafeMap<object> AppKVs = new SafeMap<object>();
        
        /// <summary>
        /// 对象的键值映射
        /// </summary>
        public SafeMap<object> KVs = new SafeMap<object>();

        /// <summary>
        /// 对象解析模板
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public string Parse (string template)
        {
            return Parse(template, KVs);
        }

        /// <summary>
        /// 表达式解析
        /// </summary>
        /// <param name="template"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static string Parse(string template, SafeMap<object> pairs, ELOption option = default(ELOption))
        {
            string result = new string(template.ToCharArray());

            // 忽略不匹配项
            if (option.Ignore)
            {
                // 需要优化为，匹配到 \$\{\S*?\} 后按照匹配到的内容作为Key在Map中寻找Value替换
                foreach (var key in pairs.Keys)
                {
                    Regex regex = new Regex(@"\$\{" + key + @"\}");  // 如果Key也是正则表达式??
                    result = regex.Replace(result, pairs[key].ToString());
                    result = regex.Replace(result, new MatchEvaluator(m => pairs.ContainsKey(key) ? pairs.UnSafeGet(key).ToString() : m.Groups[1].Value));  // ?? 这里的m. m.Groups[1] 难道指的是一个匹配项，而不是所有匹配项
                }
            }
            // 将未找到匹配项的占位符删除
            else
            {
                // 需要优化为，匹配到 \$\{\S*?\} 后按照匹配到的内容作为Key在Map中寻找Value替换
                foreach (var key in pairs.Keys)
                {
                    Regex regex = new Regex(@"\$\{" + key + @"\}");
                    result = regex.Replace(result, pairs[key].ToString()); // 

                }
            }
            return result;
        }

        /// <summary>
        /// EL 解析选项
        /// </summary>
        public class ELOption
        {
            /// <summary>
            /// 是否忽略不匹配项
            /// false: "${Date} ${NotFoundTagName}" -> "2018-11-28 ${NotFoundTagName}"
            /// Created by Wagsn on 2018/11/28 11:29.
            /// 默认false
            /// </summary>
            public bool Ignore { get; set; }
        }
    }
}
