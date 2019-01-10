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

using System;
using System.Collections.Generic;
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
        public static Dictionary<string, Func<string>> AppKVs = new Dictionary<string, Func<string>>();
        
        /// <summary>
        /// 对象的键值映射
        /// </summary>
        public Dictionary<string, Func<string>> KVs = new Dictionary<string, Func<string>>();

        /// <summary>
        /// 对象解析模板
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public string Parse (string template)
        {
            return Parse(template, KVs, @"\$\{", @"\}");
        }

        /// <summary>
        /// 表达式解析 
        /// 默认边界符：${ }
        /// </summary>
        /// <param name="input">模板字符串</param>
        /// <param name="pairs">键值对</param>
        /// <returns></returns>
        public static string Parse(string input, Dictionary<string, object> pairs, ELOption option = default(ELOption))
        {
            return Parse(input, pairs, @"\$\{", @"\}", option);
        }

        /// <summary>
        /// 表达式解析 
        /// 默认边界符：${ }
        /// </summary>
        /// <param name="input">模板字符串</param>
        /// <param name="entity">键值对</param>
        /// <returns></returns>
        public static string Parse(string input, object entity, ELOption option = default(ELOption))
        {
            // 通过反射key做字段名
            var pairs = new Dictionary<string, object>();
            foreach (System.Reflection.PropertyInfo prop in entity.GetType().GetProperties())
            {
                pairs[prop.Name] = prop.GetValue(entity);
            }
            return Parse(input, pairs, @"\$\{", @"\}", option);
        }

        /// <summary>
        /// 表达式解析 
        /// 输入边界符：left rigth
        /// {Input:"${Who} ${Do} ${What}",KVs:{"Who":"Zhangsan","Do":"to eat","What":"an Apple"}}=>"Zhangsan to eat an Apple"
        /// http://www.runoob.com/csharp/csharp-regular-expressions.html
        /// </summary>
        /// <param name="input">模板字符串</param>
        /// <param name="pairs">键值对</param>
        /// <param name="leftReg">占位符左标识正则</param>
        /// <param name="rightReg">占位符右标识正则</param>
        /// <returns></returns>
        public static string Parse(string input, Dictionary<string, object> pairs, string leftReg, string rightReg, ELOption option = default(ELOption))
        {
            // left right 作为匹配字符串要正则表达式转义
            // @"(\$\{)(\w+)(\})"
            Regex regex = new Regex($"({leftReg}){@"(\w+)"}({rightReg})");

            // 忽略不匹配项 key不存在保留${key}
            if (option.Ignore)
            {
                return regex.Replace(input, new MatchEvaluator(m => pairs.ContainsKey(m.Groups[2].ToString()) ? pairs[m.Groups[2].ToString()].ToString() : m.Value));
            }
            // 将未找到匹配项值的占位符被删除
            else
            {
                // 这里的m. m.Groups[2]指的是第二个分组（1-9）
                return regex.Replace(input, new MatchEvaluator(m => pairs.ContainsKey(m.Groups[2].ToString()) ? pairs[m.Groups[2].ToString()].ToString() : ""));
            }
        }

        /// <summary>
        /// 表达式解析 动态计算Value(无参)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pairs"></param>
        /// <param name="leftReg"></param>
        /// <param name="rightReg"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string Parse(string input, Dictionary<string, Func<string>> pairs, string leftReg, string rightReg, ELOption option = default(ELOption))
        {
            // left right 作为匹配字符串要正则表达式转义
            // @"(\$\{)(\w+)(\})"
            Regex regex = new Regex($"({leftReg}){@"(\w+)"}({rightReg})");

            // 忽略不匹配项 key不存在保留${key}
            if (option.Ignore)
            {
                return regex.Replace(input, new MatchEvaluator(m => pairs.ContainsKey(m.Groups[2].ToString()) ? pairs[m.Groups[2].ToString()]() : m.Value));
            }
            // 将未找到匹配项值的占位符被删除
            else
            {
                // 这里的m. m.Groups[2]指的是第二个分组（1-9）
                return regex.Replace(input, new MatchEvaluator(m => pairs.ContainsKey(m.Groups[2].ToString()) ? pairs[m.Groups[2].ToString()]() : ""));
            }
        }

        /// <summary>
        /// 表达式解析 动态计算Value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pairs"></param>
        /// <param name="leftReg"></param>
        /// <param name="rightReg"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string Parse(string input, object entity, Dictionary<string, Func<object, string>> pairs, string leftReg, string rightReg, ELOption option = default(ELOption))
        {
            // left right 作为匹配字符串要正则表达式转义
            // @"(\$\{)(\w+)(\})"
            Regex regex = new Regex($"({leftReg}){@"(\w+)"}({rightReg})");

            // 忽略不匹配项 key不存在保留${key}
            if (option.Ignore)
            {
                return regex.Replace(input, new MatchEvaluator(m => pairs.ContainsKey(m.Groups[2].ToString()) ? pairs[m.Groups[2].ToString()](entity) : m.Value));
            }
            // 将未找到匹配项值的占位符被删除
            else
            {
                // 这里的m. m.Groups[2]指的是第二个分组（1-9）
                return regex.Replace(input, new MatchEvaluator(m => pairs.ContainsKey(m.Groups[2].ToString()) ? pairs[m.Groups[2].ToString()](entity) : ""));
            }
        }

        /// <summary>
        /// EL 解析选项
        /// </summary>
        public struct ELOption
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
