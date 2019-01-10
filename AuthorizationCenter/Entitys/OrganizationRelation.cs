using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Entitys
{
    /// <summary>
    /// 组织关系（自关系）双主键
    /// </summary>
    public class OrganizationRelation
    {
        /// <summary>
        /// 关系ID
        /// </summary>
        [Key]
        [StringLength(36, MinimumLength = 36)]
        public string Id { get; set; }

        /// <summary>
        /// 子组织ID
        /// </summary>
        [StringLength(36, MinimumLength =36)]
        public string SonId { get; set; }

        /// <summary>
        /// 子组织
        /// </summary>
        public Organization Son { get; set; }

        /// <summary>
        /// 父组织ID
        /// </summary>
        [StringLength(36, MinimumLength = 36)]
        public string ParentId { get; set; }

        /// <summary>
        /// 父组织
        /// </summary>
        public Organization Parent { get; set; }

        /// <summary>
        /// 直接关系
        /// </summary>
        public bool IsDirect { get; set; }
    }
}
