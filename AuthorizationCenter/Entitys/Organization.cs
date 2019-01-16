using AuthorizationCenter.Define;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationCenter.Entitys
{
    /// <summary>
    /// 组织模型 TODO: 将关系分离成关系表（ParentId）
    /// </summary>
    public class Organization
    {
        //Code: Digit{12}
        /// <summary>
        /// 组织ID（GUID）
        /// </summary>
        [Key]
        [StringLength(36, MinimumLength =36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage =Constants.GUID_ERR)]
        public string Id { get; set; }

        /// <summary>
        /// 父组织ID -在没有标注为外键时必须要与Parent属性名称对应才能被自动作为外键
        /// </summary>
        //[ForeignKey("Parent")]
        [StringLength(36, MinimumLength = 36)]
        [RegularExpression(Constants.GUID_REG, ErrorMessage = Constants.GUID_ERR)]
        public string ParentId { get; set; }

        /// <summary>
        /// 父组织
        /// </summary>
        //[ForeignKey("ParentId")]  // 指定外键
        [NotMapped]
        [JsonIgnore]
        public Organization Parent { get; set; }

        /// <summary>
        /// 子组织
        /// </summary>
        [NotMapped]
        public List<Organization> Children { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        [StringLength(15, MinimumLength = 2)]
        [RegularExpression(Constants.VISIBLE_REG, ErrorMessage = Constants.VISIBLE_ERR)]
        public string Name { get; set; }

        ///// <summary>
        ///// 组织类型 -个人、俱乐部、协会等等
        ///// </summary>
        //public string Type { get; set; }

        /// <summary>
        /// 组织描述
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Organization> Sons { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Organization> Parents { get; set; }
    }
}
