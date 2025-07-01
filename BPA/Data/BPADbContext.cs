using Microsoft.EntityFrameworkCore;
using BPA.Models;

namespace BPA.Data
{
    public class BPADbContext : DbContext
    {
        public BPADbContext(DbContextOptions<BPADbContext> options)
            : base(options)
        {
            Database.EnsureCreated();  // 确保数据库被创建
        }

        public DbSet<BloodPressureRecord> BloodPressureRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BloodPressureRecord>()
                .HasKey(b => b.Id);  // 使用Id作为主键

            base.OnModelCreating(modelBuilder);
        }
    }
} 