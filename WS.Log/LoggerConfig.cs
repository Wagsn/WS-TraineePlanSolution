#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：WS.Core.Log
* 项目描述 ：
* 类 名 称 ：LoggerConfig
* 类 描 述 ：
* 所在的域 ：DESKTOP-KA4M82K
* 命名空间 ：WS.Core.Log
* 机器名称 ：DESKTOP-KA4M82K 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：wagsn
* 创建时间 ：2018/11/23 9:14:28
* 更新时间 ：2018/11/23 9:14:28
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/
#endregion
using Newtonsoft.Json;
using WS.Text;

namespace WS.Log
{
    /// <summary>
    /// 单独一个日志器的配置
    /// LoogerName
    /// LoggerRoot
    /// FileFormat
    /// TimeFormat
    /// </summary>
    public class LoggerConfig
    {
        /// <summary>
        /// 日志器名称
        /// </summary>
        public string LogName { get; set; }
        
        /// <summary>
        /// 日志器日志文件根路径（./log/loggerName）
        /// </summary>
        public string LogOut { get; set; }

        /// <summary>
        /// 日志文件名模板
        /// 暂时支持标签（LoggerName：TodoContriller、DateTime：yyy-MM-dd）
        /// TODO：模板化  "${LoggerName} ${Year} ${Month} ${Day}"  
        /// 花括号里面的是日志器识别的标签，如果在标签库存在则将 ${TagName} -> TagValue 否则就将 ${TagName} 消去
        /// </summary>
        public string FileNameFormat { get; set; }

        /// <summary>
        /// 日志项模板
        /// "[${DateTime}] [${LoggerLevel}] [${LoggerName}] ${Message}" -> "[2018-11-18 17:15.452154+8:00] [Trace] [TodoController] logging content"
        /// </summary>
        public string ItemFormat { get; set; }

        /// <summary>
        /// 日期格式（yyyy-MM-dd）
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// 时间格式（09:53:12.154451+8:00）
        /// </summary>
        public string TimeFormat { get; set; }

        /// <summary>
        /// 键值对，占位符与实际值的映射（LoggerName：TodoContriller）
        /// 不在JSON中映射
        /// </summary>
        [JsonIgnore]
        public SafeMap<object> KVs = new SafeMap<object>();
    }
}
