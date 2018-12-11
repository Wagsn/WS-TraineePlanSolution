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
            SafeMap<object> map = new SafeMap<object>();
            map["username"] = "wagsn";
            map["password"] = "123456";

            Console.WriteLine(ReplacePlaceholder("${username}: ${password}", map));
        }


        /// <summary>
        /// 占位符替换: ${}
        /// ${TagName} 如果TagName找不到将整体消去${TagName}.
        /// "${Date} ${NotFoundTagName}end" -> "2018-11-28 end".
        /// </summary>
        /// <param name="template">模板字符串</param>
        /// <returns></returns>
        public static string ReplacePlaceholder<TValue>(string template, SafeMap<TValue> pairs)
        {
            string result = new string(template.ToCharArray());

            // 需要优化为，匹配到 \$\{\S*?\} 后按照匹配到的内容作为Key在Map中寻找Value替换
            foreach(var key in pairs.Keys)
            {
                Regex regex = new Regex(@"\$\{"+key+@"\}");
                result =regex.Replace(result, pairs[key].ToString()); // 

            }
            return result;
        }

        /// <summary>
        /// 占位符替换: ${}.
        /// ${TagName} 如果TagName找不到将保留${TagName}. 
        /// "${Date} ${NotFoundTagName}" -> "2018-11-28 ${NotFoundTagName}"
        /// Created by Wagsn on 2018/11/28 11:29.
        /// </summary>
        /// <param name="template">模板字符串</param>
        /// <returns></returns>
        public static string ReplacePlaceholderByIgnore<TValue>(string template, SafeMap<TValue> pairs)
        {
            string result = new string(template.ToCharArray());

            // 需要优化为，匹配到 \$\{\S*?\} 后按照匹配到的内容作为Key在Map中寻找Value替换
            foreach (var key in pairs.Keys)
            {
                Regex regex = new Regex(@"\$\{" + key + @"\}");  // 如果Key也是正则表达式
                result = regex.Replace(result, pairs[key].ToString());
                result = regex.Replace(result, new MatchEvaluator(m => pairs.ContainsKey(key) ? pairs.UnSafeGet(key).ToString() : m.Groups[1].Value));  // ?? 这里的m. m.Groups[1] 难道指的是一个值，而不是所有匹配项
            }
            return result;
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

