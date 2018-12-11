﻿using AuthorizationCenter.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Jsons
{
    /// <summary>
    /// 组织Dto
    /// </summary>
    public class OrganizationJson
    {
        /// <summary>
        /// 组织ID（GUID）
        /// </summary>
        [Key]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 父组织ID
        /// </summary>
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string ParentId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        [StringLength(15, MinimumLength = 2)]
        [RegularExpression(Constants.VISIBLE_REG, ErrorMessage = Constants.VISIBLE_ERR)]
        public string Name { get; set; }

        /// <summary>
        /// 组织描述
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }
    }
}
