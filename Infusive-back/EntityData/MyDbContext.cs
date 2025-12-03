using Microsoft.EntityFrameworkCore;
using Infusive_back.Models;

namespace Infusive_back.EntityData
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<User_Details> User_Details { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Permission_Table> Permission_Table { get; set; }
        public DbSet<Page> Page { get; set; }
    }
}
