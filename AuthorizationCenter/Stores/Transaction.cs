using AuthorizationCenter.Entitys;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 事务
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class Transaction<TContext> : ITransaction where TContext : ApplicationDbContext
    {
        private readonly ApplicationDbContext dbContext;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="_dbContext"></param>
        public Transaction(ApplicationDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }
    }
}
