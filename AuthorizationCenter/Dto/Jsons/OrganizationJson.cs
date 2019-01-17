using AuthorizationCenter.Define;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Jsons
{
    /// <summary>
    /// 组织Dto -ViewModel
    /// </summary>
    public class OrganizationJson
    {
        //Code: Digit{12}
        /// <summary>
        /// 组织ID（GUID）
        /// </summary>
        [Key]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 父组织ID TODO:删除PanrentId
        /// </summary>
        //[ForeignKey("ParentId")]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string ParentId { get; set; }

        /// <summary>
        /// 父组织
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public OrganizationJson Parent { get; set; }

        /// <summary>
        /// 子组织
        /// </summary>
        [NotMapped]
        public List<OrganizationJson> Children { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        [Required(ErrorMessage = "组织名不能为空")] // 在添加和修改时为必填字段
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
