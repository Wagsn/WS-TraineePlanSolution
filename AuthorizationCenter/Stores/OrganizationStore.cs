
using AuthorizationCenter.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS.Log;
using WS.Text;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 组织存储 -Create Update Delete Find
    /// </summary>
    public class OrganizationStore : StoreBase<Organization>, IOrganizationStore
    {
        /// <summary>
        /// 组织存储
        /// </summary>
        /// <param name="context"></param>
        public OrganizationStore(ApplicationDbContext context):base(context){}

        /// <summary>
        /// [组织扩展表] 用户(userId)创建组织(organization)
        /// 添加一个组织会在组织扩展表中添加数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="organization">组织</param>
        /// <returns></returns>
        public async Task CreateByUserId(string userId, Organization organization)
        {
            Logger.Trace($"[{nameof(CreateByUserId)}] 用户({userId})创建组织:\r\n{JsonUtil.ToJson(organization)}");
            if (organization == null || organization.Id == null || organization.ParentId == null)
            {
                throw new ArgumentNullException("参数不能为空");
            }
            using(var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 创建组织
                    await Create(organization);
                    // 2. 创建组织关系
                    await CreateRelById(organization.Id, organization.ParentId);
                    trans.Commit();
                }
                catch(Exception e)
                {
                    Logger.Trace($"[{nameof(CreateByUserId)}] 用户({userId})创建组织:\r\n{JsonUtil.ToJson(organization)}\r\n失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception("创建组织失败", e);
                }
            }
        }

        /// <summary>
        /// [组织扩展表] 用户(userId)更新组织(organization)
        /// ID和ParentId不可修改
        /// TODO：可以更改组织架构（即修改ParentId）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="organization">组织</param>
        /// <returns></returns>
        public async Task UpdateByUserId(string userId, Organization organization)
        {
            Logger.Trace($"[{nameof(CreateByUserId)}] 用户({userId})更新组织:\r\n{JsonUtil.ToJson(organization)}");
            if (organization == null || organization.Id == null || organization.ParentId == null)
            {
                throw new ArgumentNullException("参数不能为空");
            }
            using (var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 查询数据库中组织信息
                    var dbOrg = await Context.Set<Organization>().Where(org => org.Id == organization.Id).AsNoTracking().SingleOrDefaultAsync();
                    if(dbOrg == null)
                    {
                        throw new ArgumentException("找不到编辑的组织");
                    }
                    // 2. 是否修改组织架构
                    if(organization.ParentId == dbOrg.ParentId)
                    {
                        // 2.1 没有修改组织架构
                        Context.Attach(organization);
                        Context.Update(organization);
                    }
                    else
                    {
                        // 2.2 修改了组织架构
                        // 2.2.1 删除原始关系表
                        await DeleteRelById(dbOrg.Id, dbOrg.ParentId);
                        // 2.2.2 创建新的关系表
                        await CreateRelById(organization.Id, organization.ParentId);
                        // 2.2.3 更新组织
                        Context.Attach(organization);
                        Context.Update(organization);
                    }
                    await Context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    Logger.Trace($"[{nameof(CreateByUserId)}] 用户({userId})更新组织:\r\n{JsonUtil.ToJson(organization)}\r\n失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception("更新组织失败", e);
                }
            }
        }

        /// <summary>
        /// [组织扩展表] 删除关联
        /// </summary>
        /// <param name="sonId">子组织ID</param>
        /// <param name="parentId">父组织ID</param>
        /// <param name="isDirect">直接关系</param>
        /// <returns></returns>
        public async Task DeleteRelById(string sonId, string parentId, bool isDirect = true)
        {
            // 是否直接关系
            if (isDirect)
            {
                var newOrgRels = new List<OrganizationRelation>();
                // 2. 当组织(sonId)含有子组织时, 组织(parentId)含有父组织时, 要为每一个子组织添加每一个父组织
                // 2.1 获取组织(sonId)的子组织
                var allSonIds = await (from orgRel in Context.Set<OrganizationRelation>()
                                       where orgRel.ParentId == sonId
                                       select orgRel.SonId).AsNoTracking().ToListAsync();
                allSonIds.Add(sonId);
                // 2.2 获取组织(parentId)的父组织
                var allParentIds = await (from orgRel in Context.Set<OrganizationRelation>()
                                          where orgRel.SonId == parentId
                                          select orgRel.ParentId).AsNoTracking().ToListAsync();
                allParentIds.Add(parentId);
                // 2.3 为每一个子组织添加每一个父组织
                // 示例: 子组织04有05和06两个子组织，父组织03有01和02两个父组织，04为子组织，有子组织集合：01、02、03，父组织集合：04、05、06，两两删除关联
                var delOrgRels = from orgRel in Context.Set<OrganizationRelation>()
                                 where allSonIds.Contains(orgRel.SonId) && allParentIds.Contains(orgRel.ParentId)
                                 select orgRel;
                Context.AttachRange(delOrgRels);
                Context.RemoveRange(delOrgRels);
            }
            else
            {
                var delOrgRels = from orgRel in Context.Set<OrganizationRelation>()
                                 where orgRel.SonId == sonId && orgRel.ParentId == parentId
                                 select orgRel;
                Context.AttachRange(delOrgRels);
                Context.RemoveRange(delOrgRels);
            }
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// [组织扩展表] 删除与组织(orgId)有关的关联
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task DeleteRelById(string orgId)
        {
            // 1. 查询与orgId直接关联的orgRel
            var directOrgRels = await (from orgRel in Context.Set<OrganizationRelation>()
                                       where (orgRel.SonId == orgId || orgRel.ParentId == orgId) && orgRel.IsDirect == true
                                       select orgRel).AsNoTracking().ToListAsync();
            // 2. 断开所有直接关联的orgRel
            foreach (var directOrgRel in directOrgRels)
            {
                await DeleteRelById(directOrgRel.SonId, directOrgRel.ParentId);
            }
        }

        /// <summary>
        /// [组织扩展表] 添加关联
        /// </summary>
        /// <param name="sonId">子组织ID</param>
        /// <param name="parentId">父组织ID</param>
        /// <param name="isDirect">直接关系</param>
        /// <returns></returns>
        public async Task CreateRelById(string sonId, string parentId, bool isDirect = true)
        {
            // 是否直接关系
            if (isDirect)
            {
                var newOrgRels = new List<OrganizationRelation>();
                // 2. 当组织(sonId)含有子组织时, 组织(parentId)含有父组织时, 要为每一个子组织添加每一个父组织
                // 2.1 获取组织(sonId)的子组织
                var allSonIds =await (from orgRel in Context.Set<OrganizationRelation>()
                                  where orgRel.ParentId == sonId
                                  select orgRel.SonId).AsNoTracking().ToListAsync();
                allSonIds.Add(sonId);
                // 2.2 获取组织(parentId)的父组织
                var allParentIds =await (from orgRel in Context.Set<OrganizationRelation>()
                                    where orgRel.SonId == parentId
                                    select orgRel.ParentId).AsNoTracking().ToListAsync();
                allParentIds.Add(parentId);
                // 2.3 为每一个子组织添加每一个父组织
                // 示例: 子组织04有05和06两个子组织，父组织03有01和02两个父组织，04为子组织，有子组织集合：01、02、03，父组织集合：04、05、06，两两创建关联
                foreach(var sId in allSonIds)
                {
                    foreach(var pId in allParentIds)
                    {
                        newOrgRels.Add(new OrganizationRelation
                        {
                            Id = Guid.NewGuid().ToString(),
                            SonId = sId,
                            ParentId = pId,
                            IsDirect = sId==sonId&&pId==parentId
                        });
                    }
                }
                Context.AddRange(newOrgRels);
            }
            else
            {
                Context.Add(new OrganizationRelation
                {
                    Id = Guid.NewGuid().ToString(),
                    SonId = sonId,
                    ParentId = parentId,
                    IsDirect = isDirect
                });
            }
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// [组织扩展表] 用户(userId)删除组织(orgId)
        /// 删除关联表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task DeleteByUserId(string userId, string orgId)
        {
            Logger.Trace($"[{nameof(DeleteByUserId)}] 用户({userId})删除组织({orgId})");
            if (orgId == null)
            {
                throw new ArgumentNullException("参数不能为空");
            }
            using(var trans = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 查询所有组织
                    var orgIds = (await FindChildrenFromOrgRelById(orgId).AsNoTracking().ToListAsync()).Select(o => o.Id);
                    // 2. 删除所有用户
                    // 2.1 查询所有用户ID
                    var userIds = await (from uo in Context.Set<UserOrg>()
                                         where orgIds.Contains(uo.OrgId)
                                         select uo.UserId).AsNoTracking().ToListAsync();
                    // 2.2 删除所有用户组织关联
                    var userOrgs = await (from uo in Context.Set<UserOrg>()
                                          where userIds.Contains(uo.UserId)
                                          select uo).AsNoTracking().ToListAsync();
                    Context.RemoveRange(userOrgs);
                    // 2.3 删除所有用户角色关联
                    //var userRoles
                    // 2.4 删除所有用户本身
                    var users = await (from u in Context.Set<User>()
                                       where userIds.Contains(u.Id)
                                       select u).AsNoTracking().ToListAsync();
                    Context.RemoveRange(users);
                    
                    // 3. 删除所有角色
                    // 3.1 查询所有角色ID
                    // 3.2 删除所有角色组织关联
                    // 3.3 删除所有用户角色关联
                    // 3.4 删除所有角色本身
                    
                    // 4. 删除所有角色组织权限关联

                    // 5. 删除所有组织
                    // 5.1. 删除组织关系表 -删除条件: 以其为子组织以其为父组织
                    // await DeleteRelById(orgId);
                    // 5.2 删除所有组织本身

                    //await Delete(org => org.Id == orgId);
                    trans.Commit();
                }
                catch(Exception e)
                {
                    Logger.Trace($"[{nameof(DeleteByUserId)}] 用户({userId})删除组织({orgId})失败:\r\n{e}");
                    trans.Rollback();
                    throw new Exception("删除组织失败", e);
                }
            }
        }

        /// <summary>
        /// 删除递归删除组织，先删叶子组织
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task DeleteRecursionByUserId(string userId, string orgId)
        {
            var org = await FindById(orgId).Include(o => o.Children).AsNoTracking().SingleOrDefaultAsync();
            if (org == null)
            {
                return;
            }
            foreach(var o in org.Children)
            {
                await DeleteRecursionByUserId(userId, o.Id);
            }
            try
            {
                // 删除组织相关的组织关系
                await DeleteRelById(orgId);
                // 删除组织本身
                await DeleteById(orgId);
            }
            catch (Exception e)
            {
                Logger.Error($"[{nameof(DeleteRecursionByUserId)}] 用户({userId})递归删除组织({orgId})失败:\r\n{e}");
                throw new Exception($"用户({userId})递归删除组织({orgId})失败", e);
            }
        }

        /// <summary>
        /// 删除通过ID 
        /// 单元操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IEnumerable<Organization>> DeleteById(string id)
        {
            return Delete(org => org.Id == id);
        }

        /// <summary>
        /// 删除通过名称
        /// 单元操作
        /// </summary>
        /// <param name="name">组织名</param>
        /// <returns></returns>
        public Task<IEnumerable<Organization>> DeleteByName(string name)
        {
            return Delete(org => org.Name == name);
        }

        /// <summary>
        /// 查询通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<Organization> FindById(string id)
        {
            return Find(org => org.Id == id);
        }

        /// <summary>
        /// 查询通过用户ID在UserOrg表中
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public IQueryable<Organization> FindByUserId(string userId)
        {
            return from org in Context.Organizations
                   where (from uo in Context.UserOrgs
                          where uo.UserId == userId
                          select uo.OrgId).Contains(org.Id)
                   select org;
        }

        /// <summary>
        /// 查询资源所在组织 -级联查询
        /// TODO: 改成返回IQueryble
        /// </summary>
        /// <typeparam name="TResource">资源类型</typeparam>
        /// <param name="userId">用户ID</param>
        /// <param name="resourceId">资源ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> FindByUserIdSrcId<TResource>(string userId, string resourceId) where TResource: class
        {
            //var src= await Context.Set<TResource>().FindAsync(resourceId);
            List<string> orgIds = new List<string>();
            switch (typeof(TResource).Name)
            {
                case nameof(User):
                    orgIds =await (from uo in Context.Set<UserOrg>()
                                where uo.UserId == resourceId
                                select uo.OrgId).AsNoTracking().ToListAsync();
                    
                    break;
                case nameof(Role):
                    orgIds = await (from ro in Context.Set<RoleOrg>()
                                    where ro.RoleId == resourceId
                                    select ro.OrgId).ToListAsync();
                    break;
                default:
                    break;
            }
            return await (from org in Context.Set<Organization>()
                          where orgIds.Contains(org.Id)
                          select org).ToListAsync(); ;
        }

        /// <summary>
        /// 通过组织ID找到所有子组织（包含自身）
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> FindChildrenById(string orgId)
        {
            return await FindChildrenFromOrgRelById(orgId).Include(org => org.Parent).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// 通过组织ID找到所有子组织（包含自身）
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public async Task<List<Organization>> FindChildrenFromOrgById(string orgId)
        {
            List<Organization> result = new List<Organization>();
            //if(orgId == null)
            //{
            //    return result;
            //}
            var org = await Find(o => o.Id == orgId).SingleAsync();
            result.Add(org);
            // 根据 orgid查询其所有直接子组织
            var orgs = await Find(o => orgId == o.ParentId).ToListAsync();
            // 遍历其直接子组织
            foreach (var o in orgs)
            {
                result.AddRange(await FindChildrenFromOrgById(o.Id));
            }
            return result;
        }

        /// <summary>
        /// [组织关系表] 通过组织ID找到其所有子组织包含其自身
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public IQueryable<Organization> FindChildrenFromOrgRelById(string orgId)
        {
            return from org in Context.Set<Organization>()
                   where (from orgRel in Context.Set<OrganizationRelation>()
                          where orgRel.ParentId == orgId
                          select orgRel.SonId).Contains(org.Id)  // 所有子组织
                          || org.Id == orgId  // 组织自身
                   select org;
        }

        /// <summary>
        /// [组织关系表] 找到组织ID集合的所有子组织（包括自身）
        /// </summary>
        /// <param name="orgIds">组织ID集合</param>
        /// <returns></returns>
        public IQueryable<Organization> FindChildrenFromOrgRelById(List<string> orgIds)
        {
            return from org in Context.Set<Organization>()
                   where (from orgRel in Context.Set<OrganizationRelation>()
                          where orgIds.Contains(orgRel.ParentId)
                          select orgRel.SonId).Contains(org.Id)  // 所有子组织
                          || orgIds.Contains(org.Id)  // 组织自身
                   select org;
        }

        /// <summary>
        /// 通过组织找到所有子组织（不包含自身）
        /// </summary>
        /// <param name="organization">组织</param>
        /// <returns></returns>
        public async Task<List<Organization>> FindChildren(Organization organization)
        {
            List<Organization> result = new List<Organization>();
            if (organization == null)
            {
                return result;
            }
            // 根据 orgid查询其所有直接子组织
            var orgs = await Find(org => organization.Id == org.ParentId).Include(org => org.Parent).AsNoTracking().ToListAsync();
            result.AddRange(orgs);
            // 遍历其直接子组织
            foreach (var org in orgs)
            {
                result.AddRange(await FindChildren(org));
            }
            return result;
        }

        /// <summary>
        /// 递归查询所有节点，构成一棵树返回
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Organization FindTreeById(string orgId)
        {
            var org = (from o in Find()
                        where o.Id == orgId
                        select o).Include(o => o.Children).SingleOrDefault();
            if(org == null)
            {
                return null;
            }
            for (int i = 0; i < org.Children.Count; i++)
            {
                org.Children[i] = FindTreeById(org.Children[i].Id);
            }
            return org;
        }

        /// <summary>
        /// 递归查询子组织集合
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<List<Organization>> RecursionChildren(string orgId)
        {
            List<Organization> result = new List<Organization>();
            // 根据 orgid查询其所有直接子组织
            var orgs = await Find(o => orgId == o.ParentId).ToListAsync();
            // 将直接子组织加入结果集
            result.AddRange(orgs);
            // 遍历其直接子组织
            foreach(var org in orgs)
            {
                result.AddRange(await RecursionChildren(org.Id));
            }
            return result;
        }

        /// <summary>
        /// 查询通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IQueryable<Organization> FindByName(string name)
        {
            return Find(org => org.Id == name);
        }

        /// <summary>
        /// [组织表] 查询所有父组织
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns></returns>
        public async Task<List<Organization>> FindParentById(string id)
        {
            List<Organization> result = new List<Organization>();
            var org = Context.Set<Organization>().Where(o => o.Id == id).Single();
            while (org.ParentId != null)
            {
                var temp = await Context.Set<Organization>().Where(o => o.Id == org.ParentId).SingleOrDefaultAsync();
                result.Add(temp);
                org = temp;
            }
            return result;
        }

        /// <summary>
        /// [组织关系表] 查询组织(orgId)的所有父组织包含自身
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        public IQueryable<Organization> FindParentFromRelById(string orgId)
        {
            return from org in Context.Set<Organization>()
                   where (from orgRel in Context.Set<OrganizationRelation>()
                          where orgRel.SonId == orgId
                          select orgRel.SonId).Contains(org.Id)  // 所有子组织
                          || org.Id == orgId  // 组织自身
                   select org;
        }
    }
}
