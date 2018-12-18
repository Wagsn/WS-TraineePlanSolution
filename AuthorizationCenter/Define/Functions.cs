using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Define
{
    /// <summary>
    /// 公共函数集
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEle"></typeparam>
        /// <param name="data">数据来源</param>
        /// <param name="pageIndex">分页索引，从0开始</param>
        /// <param name="pageSize">每页数量{0,}</param>
        /// <returns></returns>
        public static IQueryable<TEle> Page<TEle>(IQueryable<TEle> data, int pageIndex, [Range(1, 50, ErrorMessage ="每页数量在1到50之间")]int pageSize)
        {
            // 总数
            var count = data.Count();

            // 判断索引有效
            int pIndex = pageIndex;
            if (pageIndex > (int)Math.Ceiling((double)count / pageSize) - 1)
            {
                // 默认超限选择第一页
                pIndex = 0;
            }
            // 获取数据
            return data.Skip(pIndex * pageSize).Take(pageSize);
        }
    }
}
