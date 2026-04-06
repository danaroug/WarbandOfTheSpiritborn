using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WarbandOfTheSpiritborn.Models;

namespace WarbandOfTheSpiritborn.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Events> Events { get; set; }
        public DbSet<About> About { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<Builds> Builds { get; set; }
        public DbSet<Gallery> Gallery { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

