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
        public static PageBody<E> Page<E>(this IQueryable<E> data, int pageIndex, int pageSize)
        {
            if (pageSize < 1 || pageIndex <0)
            {
                throw new ArgumentOutOfRangeException("参数范围错误");
            }
            // 总数
            int count = data.Count();
            // 判断索引有效
            int pageNum = (int)Math.Ceiling((double)count / pageSize);
            //pIndex = (pageIndex % pageNum + pageNum) % pageNum;
            // 获取数据
            return new PageBody<E>
            {
                Data = data.Skip(pageIndex * pageSize).Take(pageSize).ToList(),
                Total = count,
                PageIndex = pageIndex,
                PageCount = pageNum,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="data">数据来源</param>
        /// <param name="pageIndex">分页索引，从0开始</param>
        /// <param name="pageSize">每页数量{0,}</param>
        /// <returns></returns>
        public static PageBody<E> Page<E>(this IEnumerable<E> data, int pageIndex, int pageSize)
        {
            //if (pageSize < 1 || pageIndex < 0)
            //{
            //    throw new ArgumentOutOfRangeException("参数范围错误");
            //}
            // 总数
            int count = data.Count();
            int pCount = 0;
            //int pSize = 0;
            //int pIndex = 0;
            if (pageSize > 0)
            {
                pCount = (int)Math.Ceiling((double)count / pageSize);
            }
            //pIndex = (pageIndex % pageNum + pageNum) % pageNum;
            // 获取数据
            return new PageBody<E>
            {
                Data = data.Skip(pageIndex * pageSize).Take(pageSize).ToList(),
                Total = count,
                PageIndex = pageIndex,
                PageCount = pCount,
                PageSize = pageSize
            };
            ////// 总数
            ////int count = data.Count();
            ////// 判断索引有效
            ////int pIndex = pageIndex;
            ////int pSize = pageSize > 50 ? 10 : pageSize;
            ////if (pageSize <= 0) pSize = 10;
            ////int pageNum = (int)Math.Ceiling((double)count / pSize);
            ////pIndex = (pageIndex % pageNum + pageNum) % pageNum;
            //// 获取数据
            //return data.Skip(pageIndex * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string Encrypt(string src)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string Decrypt(string src)
        {
            throw new NotImplementedException();
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
                if (!theCollection.Contains(item)) // theCollection.Any(entity => item.Equals(entity))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
