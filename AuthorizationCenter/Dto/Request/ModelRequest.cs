﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Request
{
    /// <summary>
    /// 元数据操作请求
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ModelRequest<TData> : UserBaseRequest
    {
        /// <summary>
        /// 模型 为单实体操作
        /// </summary>
        public TData Data { get; set; }
    }

    /// <summary>
    /// 元数据批量操作
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ModelListRequest<TData> : ModelRequest<List<TData>>
    {

    }
}
