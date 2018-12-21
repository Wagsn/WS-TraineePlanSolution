using AuthorizationCenter.Define;
using AuthorizationCenter.Dto.Jsons;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Controllers
{
    /// <summary>
    /// 登陆用户
    /// </summary>
    public class SignUser
    {
        /// <summary>
        /// Serssion
        /// </summary>
        public ISession Session { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SignUser() { }

        /// <summary>
        /// 登陆用户
        /// </summary>
        /// <param name="session"></param>
        public SignUser(ISession session)
        {
            Session = session;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="user"></param>
        public void Set(UserBaseJson user)
        {
            Id = user.Id;
            SignName = user.SignName;
            PassWord = user.PassWord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public void SetSession(ISession session)
        {
            Session = session;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            Session.Remove(Constants.USERID);
            Session.Remove(Constants.SIGNNAME);
            Session.Remove(Constants.PASSWORD);
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id
        {
            get
            {
                return Session.GetString(Constants.USERID);
            }
            set
            {
                Session.SetString(Constants.USERID, value);
            }
        }

        /// <summary>
        /// 用户签名
        /// </summary>
        public string SignName
        {
            get
            {
                return Session.GetString(Constants.SIGNNAME);
            }
            set
            {
                Session.SetString(Constants.SIGNNAME, value);
            }
        }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string PassWord
        {
            get
            {
                return Session.GetString(Constants.PASSWORD);
            }
            set
            {
                Session.SetString(Constants.PASSWORD, value);
            }
        }
    }
}
