using System.ComponentModel.DataAnnotations.Schema;

namespace Infusive_back.Models
{
    public class UserRole : Base 
    {
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User_Details? User_Details { get; set; }
        public int RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public virtual Role? Role { get; set; }
    }
}
