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
    public class TodoItemStore : StoreBase<TodoItem>, ITodoItemStore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public TodoItemStore(ApplicationDbContext context) : base(context) { }

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IEnumerable<TodoItem>> DeleteById(string id)
        {
            return Delete(ti => ti.Id == id);
        }

        /// <summary>
        /// 通过名称删除
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<IEnumerable<TodoItem>> DeleteByName(string name)
        {
            return Delete(ti => ti.Name == name);
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<TodoItem> FindById(string id)
        {
            return Find(ti => ti.Id == id);
        }

        /// <summary>
        /// 通过名称查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<TodoItem> FindByName(string name)
        {
            return Find(ti => ti.Name == name);
        }
    }
}
