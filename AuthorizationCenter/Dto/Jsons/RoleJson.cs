using AuthorizationCenter.Define;
using System.ComponentModel.DataAnnotations;

namespace AuthorizationCenter.Dto.Jsons
{
    /// <summary>
    /// 角色Dto
    /// </summary>
    public class RoleJson
    {
        /// <summary>
        /// 角色GUID
        /// </summary>
        [Key]
        //[MaxLength(36)]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        //[MaxLength(15)]
        [Required(ErrorMessage = "角色名称不能为空")]
        [StringLength(15, MinimumLength = 2)]
        [RegularExpression(Constants.VISIBLE_REG, ErrorMessage = Constants.VISIBLE_ERR)]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [MaxLength(255)]
        public string Decription { get; set; }
    }
}
