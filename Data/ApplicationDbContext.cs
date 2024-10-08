using System;
using Microsoft.EntityFrameworkCore;
using ProjectSecureCoding1.Models;

namespace ProjectSecureCoding1.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        public ApplicationDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
        }

        public DbSet<Students> Students { get; set; } = null!;
        public DbSet<Users> Users { get; set; } = null!;
    }
}