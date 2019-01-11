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
    public class OrganizationManager : IOrganizationManager<OrganizationJson>
    {
        /// <summary>
        /// 组织存储
        /// </summary>
        public IOrganizationStore OrganizationStore { get; set; }

        /// <summary>
        /// 用户组织存储
        /// </summary>
        public IUserOrgStore UserOrgStore { get; set; }

        /// <summary>
        /// 角色组织存储
        /// </summary>
        public IRoleOrgStore RoleOrgStore { get; set; }

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
        public ILogger Logger = LoggerManager.GetLogger(nameof(OrganizationManager));

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="store"></param>
        /// <param name="userOrgStore"></param>
        /// <param name="roleOrgStore"></param>
        /// <param name="roleOrgPerStore"></param>
        /// <param name="mapper"></param>
        public OrganizationManager(IOrganizationStore store, IUserOrgStore userOrgStore, IRoleOrgStore roleOrgStore, IRoleOrgPerStore roleOrgPerStore, IMapper mapper)
        {
            OrganizationStore = store ?? throw new ArgumentNullException(nameof(store));
            UserOrgStore = userOrgStore ?? throw new ArgumentNullException(nameof(userOrgStore));
            RoleOrgStore = roleOrgStore ?? throw new ArgumentNullException(nameof(roleOrgStore));
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
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task Create(OrganizationJson json)
        {
            var entity = Mapper.Map<Organization>(json);
            entity.Id = Guid.NewGuid().ToString();
            await OrganizationStore.Create(entity);
        }

        /// <summary>
        /// 删除通过ID
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task DeleteById(string orgId)
        {
            // 1. 删除组织
            await OrganizationStore.DeleteById(orgId);
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
            return OrganizationStore.Find()
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
            return OrganizationStore.Find(org => org.Id == id)
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
            var org = OrganizationStore.Find(o => o.Id == id).Include(o => o.Children).SingleOrDefault();
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
        public static List<Organization> TreeToList(Organization organization)
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
        /// 查询通过用户ID -先找角色-再找权限组织-再找子组织
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
        public async Task<IEnumerable<Organization>> FindFromUserOrgByUserId(string userId)
        {
            return await OrganizationStore.FindByUserId(userId).ToListAsync();
        }

        /// <summary>
        /// 通过用户ID和组织ID查询 -代码编写中
        /// U.ID->R.ID|P.ID->O.ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrganizationJson>> FindByUserIdOrgId(string userId, string orgId)
        {
            throw new NotImplementedException("未实现");
            // 1. 查询有权组织集合
            var orgs = await RoleOrgPerStore.FindOrgByUserIdPerName(userId, Constants.ORG_QUERY);
            // 2. 查询组织ID所在组织
            return null;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task Update(OrganizationJson json)
        {
            var organization = Mapper.Map<Organization>(json);
            await OrganizationStore.Update(organization);
        }
    }
}
