using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Define
{
    /// <summary>
    /// 公共常量集
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 字符 -十六进制
        /// </summary>
        public const string HEX_REG = @"[0-9a-fA-F]";

        /// <summary>
        /// GUID -正则表达式 -单行匹配
        /// </summary>
        public const string GUID_REG = "^("+HEX_REG+"{8}(-"+ HEX_REG + "{4}){3}-"+ HEX_REG + "{12})?$";

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
        public const string SIGNNAME_ERR = "签名格式错误，长度为3-15，仅包含字母数字";

        /// <summary>
        /// 密码 正则表达式
        /// </summary>
        public const string PASSWORD_REG = @"^[a-zA-Z0-9_\.]{6,63}$";

        /// <summary>
        /// 密码 格式错误消息
        /// </summary>
        public const string PASSWORD_ERR = "密码格式错误，长度在6-63位，包含字母数字下划线和点";

        /// <summary>
        /// 可见字符 正则表达式
        /// </summary>
        public const string VISIBLE_REG = @"\S+";

        /// <summary>
        /// 可见字符 格式错误，不能包含空格以及制表符等不可见字符"
        /// </summary>
        public const string VISIBLE_ERR = "格式错误，不能包含空格以及制表符等不可见字符";

        /// <summary>
        /// 大写字母 正则表达式
        /// </summary>
        public const string CAPSCASE_REG = "[A-Z]";

        /// <summary>
        /// 小写字母 正则表达式
        /// </summary>
        public const string LOWERCASE_REG = "[a-z]";

        /// <summary>
        /// 英文字母 正则表达式
        /// </summary>
        public const string LETTER_REG = "[a-zA-Z]";

        /// <summary>
        /// SignUser
        /// </summary>
        public const string SIGNUSER = "SignUser";

        /// <summary>
        /// UserId
        /// </summary>
        public const string USERID = "UserId";

        /// <summary>
        /// SignName
        /// </summary>
        public const string SIGNNAME = "SignName";
        /// <summary>
        /// PassWord
        /// </summary>
        public const string PASSWORD = "PassWord";


        /// <summary>
        /// 一些字符串常量 作废
        /// </summary>
        public static class Str
        {
            /// <summary>
            /// SignUser
            /// </summary>
            public const string SIGNUSER = "SignUser";

            /// <summary>
            /// UserId
            /// </summary>
            public const string USERID = "UserId";

            /// <summary>
            /// SignName
            /// </summary>
            public const string SIGNNAME = "SignName";
            /// <summary>
            /// PassWord
            /// </summary>
            public const string PASSWORD = "PassWord";
        }
    }
}
