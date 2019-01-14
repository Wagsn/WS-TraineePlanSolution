using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Define
{
    /// <summary>
    /// 分页实体
    /// </summary>
    public class PageBody<E>
    {
        /// <summary>
        /// 分页数据
        /// </summary>
        public IEnumerable<E> Data { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 分页索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 分页数量
        /// </summary>
        public int PageCount { get; set; }
    }
}
