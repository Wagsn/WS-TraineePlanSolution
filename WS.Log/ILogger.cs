using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS.Log
{
    /// <summary>
    /// 日志器接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 日志器配置
        /// </summary>
        LoggerConfig Config { get; set; }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="args"></param>
        void Debug(string formatString, params object[] args);

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="args"></param>
        void Error(string formatString, params object[] args);

        /// <summary>
        /// 致命错误
        /// </summary>
        /// <param name="message"></param>
        void Fatal(string message);

        /// <summary>
        /// 致命错误
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="args"></param>
        void Fatal(string formatString, params object[] args);

        /// <summary>
        /// 信息（比如配置信息，系统环境信息之类的）
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// 信息（比如配置信息，系统环境信息之类的）
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="args"></param>
        void Info(string formatString, params object[] args);

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        void Log(LogLevels logLevel, string message);

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="formatString"></param>
        /// <param name="args"></param>
        void Log(LogLevels logLevel, string formatString, params object[] args);

        /// <summary>
        /// 痕迹
        /// </summary>
        /// <param name="message"></param>
        void Trace(string message);

        /// <summary>
        /// 痕迹
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="args"></param>
        void Trace(string formatString, params object[] args);

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="message"></param>
        void Warn(string message);

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="args"></param>
        void Warn(string formatString, params object[] args);
    }
}
