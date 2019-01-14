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
        #region << 正则表达式及其错误提示 >>

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
        public const string GUID_ERR = "GUID格式错误";

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
        /// Id的字符数不能超过36 const静态常量（编译期决定值） readonly动态常量
        /// </summary>
        public const string IdLengthErrMsg = "Id的字符数不能超过36";

        #endregion

        #region << 变量名 -用于ViewData >>

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
        /// 角色 复数
        /// </summary>
        public const string ROLES = "Roles";

        /// <summary>
        /// 用户角色 复数
        /// </summary>
        public const string USERROLES = "UserRoles";

        #endregion

        #region << 文件路径 >>

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public const string CONFIG_PATH = "./cfg/config.json";

        /// <summary>
        /// 文档文件夹
        /// </summary>
        public const string DOCDIR = "./doc/";

        #endregion

        #region << 权限项名称 >>

        /// <summary>
        /// 根权限 -系统所有操作所有
        /// </summary>
        public const string ROOT = "ROOT";

        /// <summary>
        /// 用户管理 -用户增删查改 -ROOT>ORG_MANAGE>(ORG_SAVE>ORG_CREATE|ORG_UPDATE)|ORG_DELETE|ORG_QUERY
        /// </summary>
        public const string USER_MANAGE = "USER_MANAGE";

        /// <summary>
        /// 用户管理 -用户增改
        /// </summary>
        public const string USER_SAVE = "USER_SAVE";

        /// <summary>
        /// 用户创建
        /// </summary>
        public const string USER_CREATE = "USER_CREATE";

        /// <summary>
        /// 用户更新
        /// </summary>
        public const string USER_UPDATE = "USER_UPDATE";

        /// <summary>
        /// 用户删除
        /// </summary>
        public const string USER_DELETE = "USER_DELETE";

        /// <summary>
        /// 用户查询
        /// </summary>
        public const string USER_QUERY = "USER_QUERY";

        /// <summary>
        /// 用户详情
        /// </summary>
        public const string USER_DETAILS = "USER_DETAILS";

        /// <summary>
        /// 组织管理 -组织的增删查改
        /// </summary>
        public const string ORG_MANAGE = "ORG_MANAGE";

        /// <summary>
        /// 组织保存 -组织增改
        /// </summary>
        public const string ORG_SAVE = "ORG_SAVE";

        /// <summary>
        /// 组织创建
        /// </summary>
        public const string ORG_CREATE = "ORG_CREATE";

        /// <summary>
        /// 组织更新
        /// </summary>
        public const string ORG_UPDATE = "ORG_UPDATE";

        /// <summary>
        /// 组织删除
        /// </summary>
        public const string ORG_DELETE = "ORG_DELETE";

        /// <summary>
        /// 组织查询 ORG_SELECT
        /// </summary>
        public const string ORG_QUERY = "ORG_QUERY";

        /// <summary>
        /// 角色管理 -角色项的增删查改
        /// </summary>
        public const string ROLE_MANAGE = "ROLE_MANAGE";

        /// <summary>
        /// 用户保存
        /// </summary>
        public const string ROLE_SAVE = "ROLE_SAVE";

        /// <summary>
        /// 角色添加
        /// </summary>
        public const string ROLE_CREATE = "ROLE_CREATE";

        /// <summary>
        /// 角色添加界面
        /// </summary>
        public const string ROLE_CREATE_VIEW = "ROLE_CREATE_VIEW";

        /// <summary>
        /// 角色查询
        /// </summary>
        public const string ROLE_QUERY = "ROLE_QUERY";

        /// <summary>
        /// 权限管理 -权限项写死了的，这里指的是数据库
        /// </summary>
        public const string PER_MANAGE = "PER_MANAGE";

        /// <summary>
        /// 权限查询
        /// </summary>
        public const string PER_QUERY = "PER_QUERY";

        /// <summary>
        /// 角色绑定 -用户角色关联的增删查改
        /// </summary>
        public const string USERROLE_MANAGE = "USERROLE_MANAGE";

        /// <summary>
        /// 授权管理 -角色组织权限关联的增删查改
        /// </summary>
        public const string AUTH_MANAGE = "AUTH_MANAGE";

        #endregion
    }
}
