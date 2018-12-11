#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：WS.Text
* 项目描述 ：
* 类 名 称 ：StringExtension
* 类 描 述 ：
* 所在的域 ：DESKTOP-KA4M82K
* 命名空间 ：WS.Text
* 机器名称 ：DESKTOP-KA4M82K 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：wagsn
* 创建时间 ：2018/11/27 19:36:44
* 更新时间 ：2018/11/27 19:36:44
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WS.Text
{
    /// <summary>
    /// 原生String扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 去除首尾空格，再合并连续空格
        /// </summary>
        /// <param name="src">原始字符串</param>
        /// <returns></returns>
        public static string NormalSpace(this string src)
        {
            return Regex.Replace(src.Trim(), "\\s{2,}", " ");
        }

        /// <summary>
        /// 使文件路径标准化（首先剔除掉文件路径不能包含的特殊字符）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalPath(this string path)
        {
            return string.Empty;
        }

        /// <summary>
        /// 是否是路径，TODO
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool IsPath(this string src)
        {

            return false;
        }
    }
}
