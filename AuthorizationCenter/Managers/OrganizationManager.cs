using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    /// <summary>
    /// 组织管理
    /// </summary>
    public class OrganizationManager : IOrganizationManager<OrganizationJson>
    {
        /// <summary>
        /// 存储
        /// </summary>
        public IOrganizationStore Store { get; set; }

        /// <summary>
        /// 类型映射
        /// </summary>
        public IMapper Mapper { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="store"></param>
        /// <param name="mapper"></param>
        public OrganizationManager(IOrganizationStore store, IMapper mapper)
        {
            Store = store;
            Mapper = mapper;
        }



        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task Create(OrganizationJson json)
        {
            await Store.Create(Mapper.Map<Organization>(json));
        }

        /// <summary>
        /// 删除通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteById(string id)
        {
            await Store.DeleteById(id);
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<OrganizationJson, bool> predicate)
        {
            return Store.Exist(org => predicate(Mapper.Map<OrganizationJson>(org)));
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<OrganizationJson> Find()
        {
            return Store.Find().Include(org => org.Parent).Select(org => Mapper.Map<OrganizationJson>(org));
        }

        /// <summary>
        /// 查询通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<OrganizationJson> FindById(string id)
        {
            return Store.Find(org => org.Id == id).Include(org => org.Parent).Select(org => Mapper.Map<OrganizationJson>(org));
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task Update(OrganizationJson json)
        {
            await Store.Update(Mapper.Map<Organization>(json));
        }
    }
}
