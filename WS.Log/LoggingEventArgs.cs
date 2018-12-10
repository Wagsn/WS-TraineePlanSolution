using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS.Log
{
    /// <summary>
    /// 日志事件（）
    /// </summary>
    public class LoggingEventArgs : EventArgs
    {
        public LoggingEventArgs(LogEntity logEntity) { }

        public LogEntity LogEntity { get; set; }
    }
}
