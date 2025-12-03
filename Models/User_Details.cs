using System.ComponentModel.DataAnnotations;

namespace Infusive_back.Models
{
    public class User_Details : Base
    {
        [MaxLength(255)]
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Mobile { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }

    // use for login time
    public class UserLogin
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
