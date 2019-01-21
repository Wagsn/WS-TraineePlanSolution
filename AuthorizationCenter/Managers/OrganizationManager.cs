using AuthorizationCenter.Define;
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
    public class OrganizationManager : IOrganizationManager
    {
        /// <summary>
        /// 组织存储
        /// </summary>
        public IOrganizationStore OrganizationStore { get; set; }

        /// <summary>
        /// 用户存储
        /// </summary>
        public IUserStore UserStore { get; set; }

        /// <summary>
        /// 角色存储
        /// </summary>
        public IRoleStore RoleStore { get; set; }

        /// <summary>
        /// 角色组织权限存储
        /// </summary>
        public IRoleOrgPerStore RoleOrgPerStore { get; set; }
        
        /// <summary>
        /// 类型映射
        /// </summary>
        public IMapper Mapper { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger Logger = LoggerManager.GetLogger<OrganizationManager>();

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="store"></param>
        /// <param name="userStore"></param>
        /// <param name="roleStore"></param>
        /// <param name="roleOrgPerStore"></param>
        /// <param name="mapper"></param>
        public OrganizationManager(IOrganizationStore store, IUserStore userStore ,IRoleStore roleStore , IRoleOrgPerStore roleOrgPerStore, IMapper mapper)
        {
            OrganizationStore = store ?? throw new ArgumentNullException(nameof(store));
            UserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            RoleStore = roleStore ?? throw new ArgumentNullException(nameof(roleStore));
            RoleOrgPerStore = roleOrgPerStore ?? throw new ArgumentNullException(nameof(roleOrgPerStore));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 手动类型映射
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
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
        /// <param name="organizationJson"></param>
        /// <returns></returns>
        public async Task Create(OrganizationJson organizationJson)
        {
            var org = Mapper.Map<Organization>(organizationJson);
            org.Id = Guid.NewGuid().ToString();
            await OrganizationStore.Create(org);
        }

        /// <summary>
        /// 用户(userId)创建组织(organizationJson)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="organizationJson">组织</param>
        /// <returns></returns>
        public async Task CreateByUserId(string userId, OrganizationJson organizationJson)
        {
            var org = Mapper.Map<Organization>(organizationJson);
            org.Id = Guid.NewGuid().ToString();
            await OrganizationStore.CreateByUserId(userId, org);
        }

        /// <summary>
        /// 用户(userId)删除组织(orgId)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string orgId)
        {
            using(var trans = await OrganizationStore.Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 查询所有组织ID
                    var orgIds = await OrganizationStore.FindChildrenFromOrgRelById(orgId).Select(org => org.Id).AsNoTracking().ToListAsync();
                    // 删除组织下的所有用户
                    await UserStore.DeleteByUserIdOrgId(userId, org => orgIds.Contains(org.Id));
                    // 删除组织下的所有角色
                    await RoleStore.DeleteByUserIdOrgId(userId, org => orgIds.Contains(org.Id));
                    // 删除组织下的所有角色组织权限
                    await RoleOrgPerStore.DeleteByUserId(userId, rop => orgIds.Contains(rop.OrgId));
                    // 递归删除组织（先删子组织，再删父组织）
                    await OrganizationStore.DeleteRecursionByUserId(userId, orgId);
                    trans.Commit();
                }
                catch(Exception e)
                {
                    Logger.Error($"用户({userId})删除组织({orgId})失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception($"用户({userId})删除组织({orgId})失败", e);
                }
            }
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<bool> Exist(Func<OrganizationJson, bool> predicate)
        {
            return OrganizationStore.Exist(org => predicate(Mapper.Map<OrganizationJson>(org)));
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public IQueryable<OrganizationJson> Find()
        {
            return OrganizationStore.Find().Select(org => Mapper.Map<OrganizationJson>(org));
        }

        /// <summary>
        /// 查询通过ID
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public IQueryable<OrganizationJson> FindById(string orgId)
        {
            return OrganizationStore.Find(org => org.Id == orgId).Include(org => org.Parent).Select(org => Mapper.Map<OrganizationJson>(org));
        }

        ///// <summary>
        ///// 将组织树扩展成组织列表
        ///// </summary>
        ///// <param name="organization">组织树节点</param>
        ///// <returns></returns>
        //public static List<Organization> TreeToList(Organization organization)
        //{
        //    List<Organization> result = new List<Organization>
        //    {
        //        organization
        //    };
        //    if (organization == null|| organization.Children == null)
        //    {
        //        return result;
        //    }
        //    foreach (var org in organization.Children)
        //    {
        //        result.AddRange(TreeToList(org));
        //    }
        //    return result;
        //}

        /// <summary>
        /// 查询用户(userId)具有组织查询的组织
        /// U.ID->R.ID->O.ID-O.ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrganizationJson>> FindPerOrgsByUserId(string userId)
        {
            // 1. 查询用户的有权组织集合
            var orgs = (await RoleOrgPerStore.FindOrgByUserIdPerName(userId, Constants.ORG_QUERY)).ToList();
            orgs.ForEach(org => org.Children = null);
            return orgs.Select(org => Mapper.Map<OrganizationJson>(org));
        }

        /// <summary>
        /// 查询通过用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> FindFromUOByUserId(string userId)
        {
            return await OrganizationStore.FindByUserId(userId).ToListAsync();
        }

        /// <summary>
        /// 用户(userId)查询以组织(orgId)为根的所有组织
        /// 通过用户ID和组织ID查询 -代码编写中
        /// U.ID->R.ID|P.ID->O.ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrganizationJson>> FindByUserIdOrgId(string userId, string orgId)
        {
            return (await OrganizationStore.FindChildrenById(orgId)).Select(org => Mapper.Map<OrganizationJson>(org));
            //return (await OrganizationStore.FindChildrenFromOrgRelById(orgId)).Select(org => Mapper.Map<OrganizationJson>(org));
        }

        /// <summary>
        /// 用户(userId)更新组织(organizationJson)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="organizationJson">组织</param>
        /// <returns></returns>
        public async Task UpdateByUserId(string userId, OrganizationJson organizationJson)
        {
            var org = Mapper.Map<Organization>(organizationJson);
            await OrganizationStore.UpdateByUserId(userId, org);
        }
    }
}
