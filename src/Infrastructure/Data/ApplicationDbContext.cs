using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación entre Permission y PermissionType
            modelBuilder.Entity<Permission>()
                .HasOne(p => p.PermissionType)
                .WithMany(pt => pt.Permissions)
                .HasForeignKey(p => p.PermissionTypeId);
        }
    }
}
