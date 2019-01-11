using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Define
{
    /// <summary>
    /// 公共函数集 扩展函数
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="data">数据来源</param>
        /// <param name="pageIndex">分页索引，从0开始</param>
        /// <param name="pageSize">每页数量{0,}</param>
        /// <returns></returns>
        public static IQueryable<E> Page<E>(this IQueryable<E> data, int pageIndex, int pageSize)
        {
            //// 总数
            //int count = data.Count();
            //// 判断索引有效
            //int pIndex = pageIndex;
            //int pSize = pageSize > 50 ? 10 : pageSize;
            //if (pageSize <= 0) pSize = 10;
            //int pageNum = (int)Math.Ceiling((double)count / pSize);
            //pIndex = (pageIndex % pageNum + pageNum) % pageNum;
            // 获取数据
            return data.Skip(pageIndex * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string Encrypt(string src)
        {
            return null;
        }
        
        /// <summary>
        /// 输入集合是当前集合的子集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="theCollection"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> theCollection, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                if (!theCollection.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
