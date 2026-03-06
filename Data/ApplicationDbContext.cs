using Microsoft.EntityFrameworkCore;
using NexusPay.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NexusPay.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tables representing your core entities
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Enforce Unique Account Numbers
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.AccountNumber)
                .IsUnique();

            // 2. Set Financial Precision (Crucial for banking)
            // Prevents rounding errors by using 18 digits with 2 decimal places
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            // 3. Define Relationships (One User -> Many Accounts)
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Restrict: Don't delete a user if they have active accounts

            // 4. Configure the Audit Log (Transactions)
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Reference)
                .IsRequired()
                .HasMaxLength(50);
            DbSeeder.Seed(modelBuilder);
        }
    }
}