using System.ComponentModel.DataAnnotations;

namespace Infusive_back.Models
{
    public class Role : Base
    {
        [MaxLength(255)]
        public required string Name { get; set; }
        public string? Desc { get; set; }
        public virtual ICollection<UserRole>? RoleUsers { get; set; }
        public virtual ICollection<Permission>? RolePermissions { get; set; }
    }
}
