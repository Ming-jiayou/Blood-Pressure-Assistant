using Microsoft.EntityFrameworkCore;
using BPA.Models;

namespace BPA.Data
{
    public class BPADbContext : DbContext
    {
        public BPADbContext(DbContextOptions<BPADbContext> options)
            : base(options)
        {          
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