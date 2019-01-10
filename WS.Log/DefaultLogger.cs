using System;
using System.Collections.Generic;
using WS.IO;
using WS.Text;

namespace WS.Log
{
    /// <summary>
    /// 日志器，每个日志器一个配置，还有一个总配置
    /// </summary>
    class DefaultLogger : ILogger
    {

        /// <summary>
        /// 配置文件
        /// </summary>
        public LoggerConfig Config { get; set; }
        
        /// <summary>
        /// 配置文件包含日志器名以及文件位置和日志格式等内容
        /// </summary>
        /// <param name="config">日志器配置</param>
        public DefaultLogger(LoggerConfig config)
        {
            Config = config;
            KeyValues = new Dictionary<string, Func<object, string>>
            {
                ["LogOut"] = delegate (object entity)
                {
                    return EL.Parse(config.LogOutTemplate, new Dictionary<string, object> { ["Date"] = (entity as LogEntity).LogTime.ToString(Config.DateFormat) });
                    //return "./log/" + entity.LoggerName + ".log";
                }
            };
        }

        public void Debug(string message)
        {
            Log(Config, LogLevels.Debug, message);
        }

        public void Debug(string formatString, params object[] args)
        {
            Log(Config, LogLevels.Debug, formatString, args);
        }

        public void Error(string message)
        {
            Log(Config, LogLevels.Error, message);
        }

        public void Error(object message)
        {
            Error(message?.ToString());
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <typeparam name="ActionName"></typeparam>
        /// <param name="message"></param>
        public void Error<ActionName>(object message)
        {
            Error($"[{nameof(ActionName)}] {message?.ToString()}");
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="message"></param>
        public void Error(object tagName, object message)
        {
            Error($"[{tagName}] {message?.ToString()}");
        }

        public void Error(string formatString, params object[] args)
        {

            Error(string.Format(formatString, args));
        }

        public void Fatal(string message)
        {
            Log(Config, LogLevels.Fatal, message);
        }

        public void Fatal(string formatString, params object[] args)
        {
            Log(Config, LogLevels.Fatal, formatString, args);
        }

        public void Info(string message)
        {
            Log(Config, LogLevels.Info, message);
        }

        public void Info(string formatString, params object[] args)
        {
            Log(Config, LogLevels.Info, formatString, args);
        }

        public void Log(LogLevels logLevel, string message)
        {
            Log(Config, logLevel, message);
        }

        public void Log(LogLevels logLevel, string formatString, params object[] args)
        {
            Log(Config, logLevel, formatString, args);
        }

        public void Trace(string message)
        {
            Log(Config, LogLevels.Trace, message);
        }

        public void Trace(string formatString, params object[] args)
        {
            Log(Config, LogLevels.Trace, formatString, args);
        }

        public void Warn(string message)
        {
            Log(Config, LogLevels.Warn, message);
        }

        public void Warn(string formatString, params object[] args)
        {
            Log(Config, LogLevels.Warn, formatString, args);
        }

        /// <summary>
        /// 带格式化的
        /// </summary>
        /// <param name="config">日志器配置文件</param>
        /// <param name="level">日志层级</param>
        /// <param name="formatString">模板字符串</param>
        /// <param name="args">填充字符串数组</param>
        public static void Log(LoggerConfig config, LogLevels level, string formatString, params object[] args)
        {
            Log(config, level, string.Format(formatString, args));
        }

        /// <summary>
        /// 不带格式化的
        /// </summary>
        /// <param name="config">日志器配置文件</param>
        /// <param name="level">日志层级</param>
        /// <param name="message">日志正文</param>
        public static void Log(LoggerConfig config, LogLevels level, string message)
        {
            Log(config, new LogEntity
            {
                LogLevel = level,
                LoggerName = config.LoggerName,
                LogTime = DateTime.Now,
                Message = message
            });
        }

        public Dictionary<string, Func<object, string>> KeyValues { get; set; }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="config">记录配置（文件保存路径）</param>
        /// <param name="entity">记录实体（记录包含信息）</param>
        public static void Log(LoggerConfig config,  LogEntity entity)
        {
            // 日志项占位符替换
            var ps = new Dictionary<string, Func<string>>
            {
                ["LogOut"] = delegate()
                {
                    return EL.Parse(config.LogOutTemplate, new Dictionary<string, object> { ["Date"] = entity.LogTime.ToString(config.DateFormat) });
                    //return "./log/" + entity.LoggerName + ".log";
                }
            };
            var logitem = EL.Parse(config.LogItemTemplate, entity, config.DynanicMap, @"\$\{", @"\}");
            // 文件名占位符替换
            var filename = EL.Parse(config.FileNameTemplate, entity, config.DynanicMap, @"\$\{", @"\}");
            // 输出->控制台
            Console.WriteLine(logitem);
            //Console.WriteLine(EL.Parse(config.FileNameFormat, new { Date = entity.LogTime.ToString(config.DateFormat), entity.LoggerName }));
            //string.Format("", )
            // {name, value, type, format, convertor: (format)=>string}
            // 输出->文件 TODO: 根据配置文件限制Trace等日志输出到文件
            switch (entity.LogLevel)
            {
                case LogLevels.Error:
                    // 是否错误日志输出独立，默认独立 ErrOut: "./log/${LoggerName}/error/${Date}.log"
                    File.WriteAllText(config.DynanicMap["ErrOut"](entity), logitem + "\r\n", true);
                    break;
                default:
                    File.WriteAllText(config.DynanicMap["LogOut"](entity), logitem + "\r\n", true);
                    break;
            }
        }
    }
}
