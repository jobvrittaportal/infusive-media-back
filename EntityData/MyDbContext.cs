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
        public DbSet<Company> Company { get; set; }
        public DbSet<IndustryType> IndustryType { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<CompanyPoc> CompanyPoc { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Source> Source { get; set; }
        public DbSet<Status>Status { get; set; }
        public DbSet<Lead> Lead { get; set; }
    }
}
