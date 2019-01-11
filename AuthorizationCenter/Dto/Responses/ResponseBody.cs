using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Responses
{
    /// <summary>
    /// 携带消息的响应体
    /// </summary>
    public class ResponseBody
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        public ResponseBody()
        {
            Code = "0";
            Message = "成功";
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <returns></returns>
        public ResponseBody<D> ToResponseBody<D>()
        {
            return new ResponseBody<D>
            {
                Code = Code,
                Message = Message
            };
        }

        /// <summary>
        /// 填装数据
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseBody<D> WrapData<D>(D data)
        {
            if (data == null)
            {
                return NotFound<D>("找不到"+typeof(D).Name);
            }
            return new ResponseBody<D>
            {
                Data = data
            };
        }


        /// <summary>
        /// 找不到资源
        /// </summary>
        /// <param name="msg">错误提示信息</param>
        /// <returns></returns>
        public static ResponseBody NotFound(string msg ="找不到资源")
        {
            return new ResponseBody
            {
                Code = "404",
                Message = msg
            };
        }

        /// <summary>
        /// 找不到资源
        /// </summary>
        /// <param name="msg">错误提示信息</param>
        /// <returns></returns>
        public static ResponseBody<D> NotFound<D>(object msg = null)
        {
            return new ResponseBody<D>
            {
                Code = "404",
                Message = msg == null ? "找不到资源" : msg.ToString()
            };
        }
        
        /// <summary>
        /// 服务器错误
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ResponseBody<D> ServerError<D>(object msg = null)
        {
            return new ResponseBody<D>
            {
                Code = "500",
                Message = msg == null ? "服务器错误" : msg.ToString()
            };
        }

        /// <summary>
        /// 包装
        /// </summary>
        /// <param name="msg"></param>
        public virtual ResponseBody ServerError(object msg = null)
        {
            Code = "500";
            Message = msg == null ? "服务器错误" : msg.ToString();
            return this;
        }
    }

    /// <summary>
    /// 携带数据的响应体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseBody<T> : ResponseBody
    {
        /// <summary>
        /// 携带的数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 包装
        /// </summary>
        /// <param name="msg"></param>
        public new ResponseBody<T> ServerError(object msg = null)
        {
            base.ServerError(msg);
            return this;
        }
    }

    /// <summary>
    /// 携带列表数据的响应体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResponesBody<T> : ResponseBody<List<T>>
    {
        /// <summary>
        /// 分页索引，当前页码，从0开始
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页记录数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 记录总数
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 包装
        /// </summary>
        /// <param name="msg"></param>
        public new PageResponesBody<T> ServerError(object msg = null)
        {
            base.ServerError(msg);
            return this;
        }
    }
}
