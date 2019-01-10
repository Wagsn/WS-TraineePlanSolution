#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：WS.Text
* 项目描述 ：文本格式化
* 类 名 称 ：Format
* 类 描 述 ：字符串格式化
* 所在的域 ：DESKTOP-KA4M82K
* 命名空间 ：WS.Text
* 机器名称 ：DESKTOP-KA4M82K 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：wagsn
* 创建时间 ：2018/11/22 1:27:26
* 更新时间 ：2018/11/22 1:27:26
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/
#endregion
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WS.Text
{
    /// <summary>
    /// 字符串格式化
    /// </summary>
    public static class Format
    {
        /// <summary>
        /// 去除首尾空格，再合并连续空格
        /// </summary>
        /// <param name="src">原始字符串</param>
        /// <returns></returns>
        public static string NormalSpace(string src)
        {
            return Regex.Replace(src.Trim(), "\\s{2,}", " ");
        }

        /// <summary>
        /// 使文件路径标准化（首先剔除掉文件路径不能包含的特殊字符）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalPath(string path)
        {
            // 剔除掉不能存在的特殊字符
            // 归并掉相对路径 （"/p1/./p2/p3/../p4" -> "/p1/p2/p4"）
            return "";
        }

        /// <summary>
        /// 是否是路径
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool IsPath(string src)
        {

            return false;
        }

        /// <summary>
        /// 测试模板替换，vs2017->鼠标方法名->右键交互执行->需要把那些依赖的方法和using都交互执行过才行
        /// </summary>
        public static void Test()
        {
            Dictionary<string, object> map = new Dictionary<string, object>
            {
                ["username"] = "wagsn",
                ["password"] = "123456"
            };
            Console.WriteLine(ReplacePlaceholder("${username}: ${password}", map));
        }

        /// <summary>
        /// 用于EL表达式正则
        /// </summary>
        public static string pattern = @"(\$\{)(\w*)(\})";
        
        /// <summary>
        /// 用于EL表达式
        /// </summary>
        public static Regex elRagex = new Regex(pattern);

        /// <summary>
        /// 占位符替换: ${}
        /// ${TagName} 如果TagName找不到将整体消去${TagName}.
        /// "${Date} ${NotFoundTagName}end" -> "2018-11-28 end".
        /// </summary>
        /// <param name="template">模板字符串</param>
        /// <returns></returns>
        public static string ReplacePlaceholder<TValue>(string template, SafeMap<TValue> pairs)
        {
            return elRagex.Replace(template, new MatchEvaluator(m => pairs[m.Groups[2].ToString()]?.ToString()));
        }

        /// <summary>
        /// 占位符替换: ${}
        /// ${TagName} 如果TagName找不到将整体消去${TagName}.
        /// "${Date} ${NotFoundTagName}end" -> "2018-11-28 end".
        /// </summary>
        /// <param name="template">模板字符串</param>
        /// <returns></returns>
        public static string ReplacePlaceholder<TValue>(string template, Dictionary<string, TValue> pairs)
        {
            return elRagex.Replace(template, new MatchEvaluator(m => pairs[m.Groups[2].ToString()]?.ToString()));
        }

        /// <summary>
        /// 会自动替换 变量   把形如 "{{varName}}" 替换成对应的数值
        /// </summary>
        private static string ReplaceStringVar(string str)
        {
            Regex reg = new Regex(@"\{\{(.*?)\}\}");
            //var mat = reg.Matches(webcofnigstring2);

            str = reg.Replace(str,
                new MatchEvaluator(m =>
                     InstallContext.Get(m.Groups[1].Value) == string.Empty ? m.Value : InstallContext.Get(m.Groups[1].Value)
                ));
            return str;
        }
    }

    /// <summary>
    /// 键值对
    /// </summary>
    public static class InstallContext
    {

        private static Dictionary<string, string> kvs = new Dictionary<string, string>();


        public static string Get(string index)
        {
            if (kvs.ContainsKey(index))
            {
                return kvs[index];
            }
            else
            {
                return string.Empty;
            }
        }
        public static void Set(string index, string value)
        {
            kvs[index] = value;
        }
    }
}

