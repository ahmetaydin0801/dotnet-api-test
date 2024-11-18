using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
    public class DataContextEF(IConfiguration config) : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSalary> UserSalary { get; set; }
        public virtual DbSet<UserJobInfo> UserJobInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(config.GetConnectionString("DefaultConnection"),
                        optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                        );
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema");
            
            modelBuilder.Entity<User>()
                .ToTable("Users","TutorialAppSchema")
                .HasKey(u => u.UserId);
            
            modelBuilder.Entity<UserSalary>()
                .HasKey(u => u.UserId);
            
            modelBuilder.Entity<UserJobInfo>()
                .HasKey(u => u.UserId);
        }
    }
}