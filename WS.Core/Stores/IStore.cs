using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WS.Core.Models;

namespace WS.Core.Stores
{
    /// <summary>
    /// Store接口，CRUD操作通用接口
    /// </summary>
    public interface IStore<TModel> where TModel : ITraceUpdate
    {
        /// <summary>
        /// 新增一条数据，建议与新增多条数据合并
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TModel> CreateAsync(TModel model, CancellationToken cancellationToken);

        /// <summary>
        /// 新增多条数据
        /// </summary>
        /// <param name="models">模型</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TModel>> CreateListAsync(List<TModel> models, CancellationToken cancellationToken);

        /// <summary>
        /// 查询一条数据，建议与查询多条数据合并
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TModel> ReadAsync(TModel model, CancellationToken cancellationToken);

        /// <summary>
        /// 条件查询一条数据，建议与查询多条数据合并
        /// </summary>
        /// <typeparam name="TResult">模型</typeparam>
        /// <param name="query">函数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> ReadAsync<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        /// <summary>
        /// 条件查询多条数据
        /// </summary>
        /// <typeparam name="TResult">模型</typeparam>
        /// <param name="query">函数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<TModel>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        /// <summary>
        /// 更新一条数据，通过ID匹配
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(TModel model, CancellationToken cancellationToken);

        /// <summary>
        /// 更新多条数据，通过ID匹配
        /// </summary>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateList(List<TModel> models, CancellationToken cancellationToken);

        /// <summary>
        /// 条件更新
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateIfAsync(Func<IQueryable<TModel>, IQueryable<TModel>> query, CancellationToken cancellationToken);

        /// <summary>
        /// 删除一条数据，通过ID匹配
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(TModel model, CancellationToken cancellationToken);

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="models"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteListAsync(List<TModel> models, CancellationToken cancellationToken);

        /// <summary>
        /// 条件删除，建议与删除多条数据合并
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteIfAsync(Func<IQueryable<TModel>, IQueryable<TModel>> query, CancellationToken cancellationToken);
    }
}
