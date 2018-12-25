using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 待办项存储
    /// </summary>
    public interface ITodoItemStore : INameStore<TodoItem>
    {
    }
}
