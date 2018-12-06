﻿using AuthorizationCenter.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    /// <summary>
    /// 角色模型
    /// </summary>
    public class Role
    {
        /// <summary>
        /// 角色GUID
        /// </summary>
        [Key]
        [MaxLength(36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage =Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [MaxLength(15)]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [MaxLength(255)]
        public string Decription { get; set; }
    }
}
