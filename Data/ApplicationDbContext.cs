using Microsoft.EntityFrameworkCore;
using snap_backend.Models;

namespace snap_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Package>()
                .HasOne(p => p.Courier)
                .WithMany(u => u.AssignedPackages)
                .HasForeignKey(p => p.CourierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Package>()
                .Property(p => p.Status)
                .HasConversion<string>();
        }


    }
}
