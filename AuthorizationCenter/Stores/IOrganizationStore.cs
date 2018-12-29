﻿using AuthorizationCenter.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    /// <summary>
    /// 组织存储
    /// </summary>
    public interface IOrganizationStore : INameStore<Organization>
    {

        /// <summary>
        /// 通过组织ID找到所有子组织（包括间接子组织children.children）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<Organization> FindChildrenById(string id);
    }
}
