using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS.Log
{
    /// <summary>
    /// 日志层级
    /// </summary>
    public enum LogLevels
    {
        // 000011 痕迹与调试  curr 与 传入的 做且运算
        /// <summary>
        /// 痕迹 000001
        /// </summary>
        Trace = 1,
        /// <summary>
        /// 调试 000010
        /// </summary>
        Debug = 2,
        /// <summary>
        /// 信息 000100
        /// </summary>
        Info = 4,
        /// <summary>
        /// 警告 001000
        /// </summary>
        Warn = 8,
        /// <summary>
        /// 错误 010000
        /// </summary>
        Error = 16,
        /// <summary>
        /// 致命错误 100000
        /// </summary>
        Fatal = 32,
        /// <summary>
        /// 所有 111111
        /// </summary>
        All = 63
    }
}
