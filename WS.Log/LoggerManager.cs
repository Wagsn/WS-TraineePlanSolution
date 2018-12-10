using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS.Log
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public class LoggerManager
    {

        /// <summary>
        /// 是否能够进行日志记录事件
        /// </summary>
        public static bool IsLoggingEnabled { get; }

        /// <summary>
        /// 日志记录事件
        /// </summary>
        //public static event EventHandler<LoggingEventArgs> Logging;

        /// <summary>
        /// 删除日志文件
        /// </summary>
        /// <param name="days"></param>
        /// <param name="logFolder"></param>
        /// <param name="clearLogger"></param>
        public static void DeleteLogs(int days, string logFolder, ILogger clearLogger) { }

        /// <summary>
        /// 取消日志事件
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="logLevel"></param>
        public static void DisableLogEvent(string loggerName, LogLevels logLevel) { }

        /// <summary>
        /// 取消日志器
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="logLevel"></param>
        public static void DisableLogger(string loggerName, LogLevels logLevel) { }

        /// <summary>
        /// 取消日志记录事件
        /// </summary>
        public static void DisableLogging() { }

        /// <summary>
        /// 开启某个日志器的记录事件
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="logLevel"></param>
        public static void EnableLogEvent(string loggerName, LogLevels logLevel) { }

        /// <summary>
        /// 开始日志器
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="logLevel"></param>
        public static void EnableLogger(string loggerName, LogLevels logLevel) { }

        /// <summary>
        /// 开启日志记录事件
        /// </summary>
        public static void EnableLogging() { }

        /// <summary>
        /// 获取根日志器(直接将文件写入log文件夹下)
        /// </summary>
        /// <returns></returns>
        public static ILogger GetLogger()
        {
            return new DefaultLogger(new LoggerConfig
            {
                LogOut = "./log" ,
                FileNameFormat = "${Date}",
                TimeFormat = "HH:mm:ss.FFFFFFK",
                DateFormat = "yyyy-MM-dd"
            });
        }

        /// <summary>
        /// 获取日志器
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        public static ILogger GetLogger(string loggerName)
        {
            return new DefaultLogger(new LoggerConfig
            {
                LogOut = "./log/"+loggerName,
                LogName = loggerName,
                FileNameFormat = "${Date}",
                TimeFormat = "HH:mm:ss.FFFFFFK",
                DateFormat = "yyyy-MM-dd"
            });
        }

        /// <summary>
        /// 通过日志器配置获取Logger
        /// </summary>
        /// <param name="config">日志器配置</param>
        /// <returns></returns>
        public static ILogger GetLogger(LoggerConfig config)
        {
            return new DefaultLogger(config);
        }

        /// <summary>
        /// 初始化日志管理
        /// </summary>
        /// <param name="config">日志管理配置</param>
        public static void InitLog(LogConfig config) { }

        //public static void MapLogger(string loggerName, LogLevels logLevel, string logFileName, Layout layout = null) { }

        /// <summary>
        /// 设置日志层级
        /// </summary>
        /// <param name="logLevel"></param>
        public static void SetLoggerAboveLevels(LogLevels logLevel) { }

        /// <summary>
        /// 设置日志层级
        /// </summary>
        /// <param name="logLevels"></param>
        public static void SetLoggerLevel(LogLevels logLevels) { }

        /// <summary>
        /// 清理日志
        /// </summary>
        /// <param name="days"></param>
        /// <param name="logFolder"></param>
        /// <param name="clearLogger"></param>
        public static void StartClear(int days, string logFolder, ILogger clearLogger) { }
    }
}
