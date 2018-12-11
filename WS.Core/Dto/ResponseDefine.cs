#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：WS.Core.Dto
* 项目描述 ：
* 类 名 称 ：ResponseDefine
* 类 描 述 ：
* 所在的域 ：DESKTOP-KA4M82K
* 命名空间 ：WS.Core.Dto
* 机器名称 ：DESKTOP-KA4M82K 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：wagsn
* 创建时间 ：2018/11/7 17:46:14
* 更新时间 ：2018/11/7 17:46:14
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace WS.Core.Dto
{
    /// <summary>
    /// 响应体常量池
    /// </summary>
    public class ResponseDefine
    {
        #region 请求成功
        /// <summary>
        /// 成功
        /// </summary>
        public static readonly string SuccessCode = "0";
        public static readonly string SuccessMsg = "成功";

        #endregion

        #region POST

        /// <summary>
        /// 成功但是重复
        /// </summary>
        public static readonly string PostRepeat = "601";
        public static readonly string PostRepeatMsg = "创建资源重复，服务器的资源已经被创建";

        #endregion

        #region 模型错误

        /// <summary>
        /// 模型验证失败
        /// </summary>
        public static readonly string ModelStateInvalid = "100";
        public static readonly string ModelStateInvalidMsg = "模型验证失败";

        /// <summary>
        /// 参数（请求体）不能为空
        /// </summary>
        public static readonly string ArgumentNullError = "101";
        public static readonly string ArgumentNullErrorMsg = "参数不能为空";

        #endregion

        #region 请求成功

        public static readonly string Created = "201";
        /// <summary>
        /// 新的资源已经依据请求的需要而建立
        /// </summary>
        public static readonly string CreatedMsg = "新的资源已经依据请求的需要而建立";

        #endregion

        // 206 PartialContent 部分内容

        #region 请求错误

        /// <summary>
        /// 请求体错误 400
        /// </summary>
        public static readonly string BadRequset = "400";
        /// <summary>
        /// 请求体错误
        /// </summary>
        public static readonly string BadRequsetMsg = "请求体错误";

        /// <summary>
        /// 找不到你要的资源
        /// </summary>
        public static readonly string NotFound = "404";
        public static readonly string NotFoundMsg = "找不到你要的资源";

        /// <summary>
        /// 你没有权限访问资源
        /// </summary>
        public static readonly string NotAllow = "403";
        public static readonly string NotAllowMsg = "你没有权限访问资源";

        #endregion

        #region 服务器错误

        /// <summary>
        /// 服务器出现了异常
        /// </summary>
        public static readonly string ServiceError = "500";
        public static readonly string ServiceErrorMsg = "服务器出现了异常";

        /// <summary>
        /// 服务器不支持完成请求所需的功能
        /// </summary>
        public static readonly string NotSupport = "501";
        /// <summary>
        /// 服务器不支持完成请求所需的功能
        /// </summary>
        public static readonly string NotSupportMsg = "服务器不支持完成请求所需的功能";

        #endregion
    }

    namespace Response
    {
        namespace Defines
        {
            /// <summary>
            /// 响应状态基类，StateBase
            /// </summary>
            public class Base
            {
                public string Code = ResponseDefine.SuccessCode;
                public string Msg = ResponseDefine.SuccessMsg;
            }
            /// <summary>
            /// 响应成功状态
            /// </summary>
            public static class Success
            {
                public static readonly string Code = ResponseDefine.SuccessCode;
                public static readonly string Msg = ResponseDefine.SuccessMsg;
            }
        }
    }
}
