using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS.Log
{
    /// <summary>
    /// 一条日志的实体，将会输出到日志文件中
    /// [2018-11-22 17:31.421428+8:00] [Trace] [LoggerName] Message
    /// </summary>
    public class LogEntity
    {
        ///// <summary>
        ///// 输出路径（"./log/sign/error/2019-01-10.log"）（URI）
        ///// </summary>
        //public string OutPath { get; set; }


        /// <summary>
        /// 日志项
        /// </summary>
        public string LogItem { get; set; }

        /// <summary>
        /// 日志器名
        /// </summary>
        public string LoggerName { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 日志层级，Trace Debug Info Warn Fatal All
        /// </summary>
        public LogLevels LogLevel { get; set; }
    }
}
