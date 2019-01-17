using AuthorizationCenter.Define;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Entitys
{
    /// <summary>
    /// 权限实体 -ROOT>ORG_MANAGE>(ORG_SAVE>ORG_CREATE|ORG_UPDATE)|ORG_DELETE|ORG_QUERY
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        [Key]
        [StringLength(36, MinimumLength =36)]
        public string Id { get; set; }

        /// <summary>
        /// 父权限ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 父权限
        /// </summary>
        [JsonIgnore]
        public Permission Parent { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [Required(ErrorMessage ="权限名称不能为空")]
        [StringLength(63, MinimumLength =2)]
        public string Name { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// 子权限
        /// </summary>
        public List<Permission> Children { get; set; }
    }
}
