using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Entitys;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;
using WS.Text;

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
        /// 日志器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger(nameof(OrganizationManager));

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

        private OrganizationJson Map(Organization organization)
        {
            var json = new OrganizationJson
            {
                Id = organization.Id,
                Name = organization.Name,
                Description = organization.Description,
                ParentId = organization.ParentId,
                Children = new List<OrganizationJson>()
            };
            foreach(var org in organization.Children)
            {
                json.Children.Add(Map(org));
            }
            return json;
        }


        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task Create(OrganizationJson json)
        {
            var entity = Mapper.Map<Organization>(json);
            entity.Id = Guid.NewGuid().ToString();
            await Store.Create(entity);
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
            return Store.Find()
                .Include(org => org.Parent)
                .Select(org => Mapper.Map<OrganizationJson>(org));
        }

        /// <summary>
        /// 查询通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<OrganizationJson> FindById(string id)
        {
            return Store.Find(org => org.Id == id)
                .Include(org => org.Parent)
                .Include(org => org.Children)
                .Select(org => Mapper.Map<OrganizationJson>(org));
        }

        /// <summary>
        /// 递归查询所有节点，构成一棵树返回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Organization FindTreeById(string id)
        {
            var org = Store.Find(o => o.Id == id).Include(o => o.Children).SingleOrDefault();
            for (int i = 0; i < org.Children.Count; i++)
            {
                org.Children[i] = FindTreeById(org.Children[i].Id);
            }
            return org;
        }

        /// <summary>
        /// 将组织树扩展成组织列表
        /// </summary>
        /// <param name="organization">组织树节点</param>
        /// <returns></returns>
        public List<Organization> TreeToList(Organization organization)
        {
            List<Organization> result = new List<Organization>
            {
                organization
            };
            if (organization == null|| organization.Children == null)
            {
                return result;
            }
            foreach (var org in organization.Children)
            {
                result.AddRange(TreeToList(org));
            }
            return result;
        }

        /// <summary>
        /// 查询通过用户ID -先找角色-再找组织-再找子组织
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<OrganizationJson>> FindByUserId(string id)
        {
            // 1. 查询角色相关的组织
            var orgs = await (from org in Store.Context.Organizations
                              where (from rop in Store.Context.RoleOrgPers
                                     where (from ur in Store.Context.UserRoles
                                            where ur.UserId == id
                                            select ur.RoleId).Contains(rop.RoleId)
                                     select rop.OrgId).Contains(org.Id)
                              select org).ToListAsync();
            // 2. 递归查询所有其下的子组织
            var orgList = new List<Organization>();
            foreach(var org in orgs)
            {
                orgList.AddRange(TreeToList(FindTreeById(org.Id)));
            }
            orgList =orgList.Distinct().ToList();
            orgList.ForEach(org => org.Children = null);
            return orgList.Select(org => Mapper.Map<OrganizationJson>(org));

            //return (from org in Store.Context.Organizations
            //        where (from rop in Store.Context.RoleOrgPers
            //               where (from ur in Store.Context.UserRoles
            //                      where ur.UserId == id
            //                      select ur.RoleId).Contains(rop.RoleId)
            //               select rop.OrgId).Contains(org.Id)
            //        select org).Select(org => Mapper.Map<OrganizationJson>(org));
            //select org).Include(org =>org.Parent).Select(org => Mapper.Map<OrganizationJson>(org));

            //return Store.Find(org => Store.Context.RoleOrgPers.Where(rop = rop))
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task Update(OrganizationJson json)
        {
            var organization = Mapper.Map<Organization>(json);
            await Store.Update(organization);
        }
    }
}
