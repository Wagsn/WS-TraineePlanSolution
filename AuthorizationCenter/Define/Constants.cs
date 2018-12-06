﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Define
{
    /// <summary>
    /// 常量
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// GUID 正则表达式
        /// </summary>
        public const string GUID_REG = @"^([0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12})?$";

        /// <summary>
        /// GUID 格式错误消息
        /// </summary>
        public const string GUID_ERR = "GUID输入错误";

        /// <summary>
        /// 签名 正则表达式
        /// </summary>
        public const string SIGNNAME_REG = @"^[a-zA-Z0-9]{3,15}$";

        /// <summary>
        /// 签名 格式错误消息
        /// </summary>
        public const string SIGNNAME_ERR = "签名格式错误，长度为3-15，包含字母数字";

        /// <summary>
        /// 密码 正则表达式
        /// </summary>
        public const string PASSWORD_REG = @"^[a-zA-Z0-9_\.]{6,63}$";

        /// <summary>
        /// 密码 格式错误消息
        /// </summary>
        public const string PASSWORD_ERR = "密码格式错误，长度在6-63位，包含字母数字下划线和点";
    }
}
