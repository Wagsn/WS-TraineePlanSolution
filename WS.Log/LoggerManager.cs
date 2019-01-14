using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Text;

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
                LogOutTemplate = "./log" ,
                FileNameTemplate = "${Date}",
                TimeFormat = "HH:mm:ss.FFFFFFK",
                DateFormat = "yyyy-MM-dd",
                DynanicMap = new Dictionary<string, Func<object, string>>
                {
                    ["Date"] = delegate (object entity)
                    {
                        return (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd");
                    },
                    ["DateTime"] = delegate (object entity)
                    {
                        return (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd yyyy-MM-dd");
                    },
                    ["LoggerName"] = delegate (object entity)
                    {
                        return (entity as LogEntity)?.LoggerName;
                    },
                    ["LoggerLevel"] = delegate (object entity)
                    {
                        return (entity as LogEntity).LogLevel.ToString();
                    },
                    ["Message"] = delegate (object entity)
                    {
                        return (entity as LogEntity)?.Message;
                    },
                    ["ErrOut"] = delegate (object entity)
                    {
                        return "./log/"+(entity as LogEntity)?.LoggerName+"/error/" +(entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd")+".log";
                    },
                    ["LogOut"] = delegate (object entity)
                    {
                        return "./log/" + (entity as LogEntity)?.LoggerName + "/" + (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd") + ".log";
                    }
                }
            });
        }

        ///// <summary>
        ///// 获取日志器
        ///// </summary>
        ///// <param name="loggerName"></param>
        ///// <returns></returns>
        //public static ILogger GetLogger(string loggerName)
        //{
        //    // 日志项数据 
        //    // item: {name, value, type, format} 
        //    // { name: "today", value: DateTime.Now, type: DateTime, format: "yyyy-MM-dd HH:mm:ss.FFFFFFK"}
        //    // { name: "price", value: new Money(15.6, unit:"dollar"), type: Money, format: "udddd.ff"} => "$0015.60"
        //    return new DefaultLogger(new LoggerConfig
        //    {
        //        LogOutTemplate = "./log/"+loggerName,
        //        LoggerName = loggerName,
        //        FileNameTemplate = "${Date}.log",
        //        TimeFormat = "HH:mm:ss.FFFFFFK",
        //        DateFormat = "yyyy-MM-dd",
        //        LogItemTemplate = "[${DateTime}] [${LoggerLevel}] [${LoggerName}] ${Message}",
        //        DynanicMap = new Dictionary<string, Func<object, string>>
        //        {
        //            ["Date"] = delegate (object entity)
        //            {
        //                return (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd");
        //            },
        //            ["DateTime"] = delegate (object entity)
        //            {
        //                return (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd yyyy-MM-dd");
        //            },
        //            ["LoggerName"] = delegate (object entity)
        //            {
        //                return (entity as LogEntity)?.LoggerName;
        //            },
        //            ["LoggerLevel"] = delegate (object entity)
        //            {
        //                return (entity as LogEntity).LogLevel.ToString();
        //            },
        //            ["Message"] = delegate (object entity)
        //            {
        //                return (entity as LogEntity)?.Message;
        //            },
        //            ["ErrOut"] = delegate (object entity)
        //            {
        //                // return EL.Parse((entity as LogEntity)?.ErrOut, )
        //                return "./log/" + (entity as LogEntity)?.LoggerName + "/error/" + (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd") + ".log";
        //            },
        //            ["LogOut"] = delegate (object entity)
        //            {
        //                return "./log/" + (entity as LogEntity)?.LoggerName + "/" + (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd") + ".log";
        //            }
        //        }
        //    });
        //}

        /// <summary>
        /// 获取日志器
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        public static ILogger GetLogger(Type srcType)
        {
            // 日志项数据 
            // item: {name, value, type, format} 
            // { name: "today", value: DateTime.Now, type: DateTime, format: "yyyy-MM-dd HH:mm:ss.FFFFFFK"}
            // { name: "price", value: new Money(15.6, unit:"dollar"), type: Money, format: "udddd.ff"} => "$0015.60"
            return new DefaultLogger(new LoggerConfig
            {
                LogOutTemplate = "./log/${LoggerName}/${Date}.log",
                ClassFullName = srcType.FullName,
                LoggerName = srcType.Name,
                FileNameTemplate = "${Date}.log",
                TimeFormat = "HH:mm:ss.FFFFFFK",
                DateFormat = "yyyy-MM-dd",
                LogItemTemplate = "[${DateTime}] [${LogLevel}] [${LoggerName}] ${Message}",
                DynanicMap = new Dictionary<string, Func<object, string>>
                {
                    ["Date"] = delegate (object entity)
                    {
                        return (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd");
                    },
                    ["DateTime"] = delegate (object entity)
                    {
                        return (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd HH:mm:ss");
                    },
                    ["LoggerName"] = delegate (object entity)
                    {
                        return (entity as LogEntity)?.LoggerName;
                    },
                    ["LogLevel"] = delegate (object entity)
                    {
                        return (entity as LogEntity).LogLevel.ToString();
                    },
                    ["Message"] = delegate (object entity)
                    {
                        return (entity as LogEntity)?.Message;
                    },
                    ["ErrOut"] = delegate (object entity)
                    {
                        // return EL.Parse((entity as LogEntity)?.ErrOut, )
                        return EL.Parse("./log/${LoggerName}/error/${Date}.log", new Dictionary<string, object> { ["Date"] = (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd"), ["LoggerName"] = (entity as LogEntity)?.LoggerName });
                    },
                    ["LogOut"] = delegate (object entity)
                    {
                        return EL.Parse("./log/${LoggerName}/${Date}.log", new Dictionary<string, object> { ["Date"] = (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd"), ["LoggerName"] = (entity as LogEntity)?.LoggerName });
                    },
                    ["LogItem"] = delegate (object entity)
                    {
                        return EL.Parse("[${DateTime}] [${LogLevel}] [${LoggerName}] ${Message}", new { DateTime = (entity as LogEntity)?.LogTime.ToString("yyyy-MM-dd HH:mm:ss"), (entity as LogEntity)?.LogLevel, (entity as LogEntity)?.LoggerName, (entity as LogEntity)?.Message });
                    }
                }
            });
        }

        /// <summary>
        /// 获取日志器
        /// </summary>
        /// <typeparam name="CategoryName"></typeparam>
        /// <returns></returns>
        public static ILogger GetLogger<CategoryName>()
        {
            return GetLogger(typeof(CategoryName));
        }

        ///// <summary>
        ///// 通过日志器配置获取Logger
        ///// </summary>
        ///// <param name="config">日志器配置</param>
        ///// <returns></returns>
        //public static ILogger GetLogger(LoggerConfig config)
        //{
        //    return new DefaultLogger(config);
        //}

        ///// <summary>
        ///// 通过日志器配置获取Logger
        ///// </summary>
        ///// <param name="config">日志器配置</param>
        ///// <returns></returns>
        //public static ILogger GetLogger<CategoryName>(LoggerConfig config)
        //{
        //    return new DefaultLogger(new LoggerConfig
        //    {
        //        LogOutTemplate = config.LogOutTemplate,
        //        ClassFullName = typeof(CategoryName).FullName,
        //        LoggerName = typeof(CategoryName).Name,
        //        FileNameTemplate = config.FileNameTemplate,
        //        TimeFormat = config.TimeFormat,
        //        DateFormat = config.DateFormat,
        //        LogItemTemplate = config.LogItemTemplate,
        //        DynanicMap = config.DynanicMap
        //    });
        //}

        ///// <summary>
        ///// 初始化日志管理
        ///// </summary>
        ///// <param name="config">日志管理配置</param>
        //public static void InitLog(LogConfig config) { }

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
