using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 用户存储
    /// </summary>
    public interface IUserBaseStore : INameStore<UserBase>
    {
        /// <summary>
        /// 存在与运算
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool IsExistAnd(UserBase user);
    }
}
