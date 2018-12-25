using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 待办项管理接口
    /// </summary>
    public interface ITodoItemManager
    {
        Task<IQueryable<TodoItem>> Create(IQueryable<TodoItem> todos);
    }
}
