using AuthorizationCenter.Define;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Jsons
{
    /// <summary>
    /// 权限Dto
    /// </summary>
    public class PermissionJson
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        [Key]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 父权限ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 父权限
        /// </summary>
        [JsonIgnore]
        public PermissionJson Parent { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [Required(ErrorMessage = "权限名称不能为空")]
        [StringLength(15, MinimumLength = 2)]
        [RegularExpression(Constants.VISIBLE_REG, ErrorMessage = Constants.VISIBLE_ERR)]
        public string Name { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }
    }
}
