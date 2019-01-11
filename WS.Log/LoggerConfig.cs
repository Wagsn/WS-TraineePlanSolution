#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：WS.Log
* 项目描述 ：.NET Standard 2.0
* 类 名 称 ：LoggerConfig
* 类 描 述 ：
* 所在的域 ：DESKTOP-KA4M82K
* 命名空间 ：WS.Log
* 机器名称 ：DESKTOP-KA4M82K 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：wagsn
* 创建时间 ：2018/11/23 09:14:28
* 更新时间 ：2018/01/10 13:15:00
* 版 本 号 ：v1.0.0.2
//----------------------------------------------------------------*/
#endregion
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WS.Log
{
    /// <summary>
    /// 单独一个日志器的配置
    /// </summary>
    public class LoggerConfig
    {
        /// <summary>
        /// 日志器名称
        /// </summary>
        public string LoggerName { get; set; }

        /// <summary>
        /// 全称命名
        /// </summary>
        public string ClassFullName { get; set; }

        /// <summary>
        /// 错误日志文件输出路径模板（"./log/${LoggerName}/${Date}.log"）
        /// </summary>
        public string LogOutTemplate { get; set; }

        /// <summary>
        /// 错误日志文件输出路径模板（"./log/${LoggerName}/error/${Date}.log"）
        /// </summary>
        public string ErrOutTemplate { get; set; }

        /// <summary>
        /// 日志文件名模板（"${Year} ${Month} ${Day}.log"）
        /// 暂时支持标签（LoggerName：TodoContriller，Date：yyy-MM-dd）
        /// </summary>
        public string FileNameTemplate { get; set; }

        /// <summary>
        /// 日志项模板
        /// "[${DateTime}] [${LoggerLevel}] [${LoggerName}] ${Message}" -> "[2018-11-18 17:15.452154+8:00] [Trace] [TodoController] Message"
        /// </summary>
        public string LogItemTemplate { get; set; }

        /// <summary>
        /// 日期格式（yyyy-MM-dd）
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// 时间格式（09:53:12.154451+8:00）
        /// </summary>
        public string TimeFormat { get; set; }

        /// <summary>
        /// 映射，键值对
        /// 占位符与实际值的映射（LoggerName：TodoContriller）
        /// 不在JSON中映射
        /// TODO：LoggerName可能只需要赋值一次，但是DateTime是每次打印都在刷新的，将object改造成委托，采用动态计算机制 KVs[key](entity):string
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> Map = new Dictionary<string, object>();

        /// <summary>
        /// 通过LogEntity动态计算LogItem
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, Func<object, string>> DynanicMap { get; set; }
    }
}
