using AuthorizationCenter.Dto.Jsons;
using AuthorizationCenter.Entitys;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Injections
{
    /// <summary>
    /// 映射
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public MappingProfile()
        {
            CreateMap<User, UserJson>();
            CreateMap<UserJson, User>();

            CreateMap<Organization, OrganizationJson>(); //.ForMember(o => o.Parent, m => m.Ignore());
            CreateMap<OrganizationJson, Organization>(); //.ForMember(o => o.Parent, m => m.Ignore());

            CreateMap<Role, RoleJson>();
            CreateMap<RoleJson, Role>();

            CreateMap<PermissionJson, Permission>();
            CreateMap<Permission, PermissionJson>();

            //CreateMap
        }
    }
}
