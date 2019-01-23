using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Jsons
{
    /// <summary>
    /// 选择框
    /// </summary>
    public class CheckBox<D>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public D Data { get; set; }

        /// <summary>
        /// 是否被选择
        /// </summary>
        public bool IsChecked { get; set; }
    }
}
