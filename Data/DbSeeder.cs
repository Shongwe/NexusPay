using Microsoft.EntityFrameworkCore;
using NexusPay.Models;
using System.Reflection.Emit;

namespace NexusPay.Data
{
    public static class DbSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FullName = "Senzosenkosi Shongwe", Email = "senzo@nexuspay.com" },
                new User { Id = 2, FullName = "BBD Recruiter", Email = "recruiter@bbd.co.za" }
            );

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    AccountNumber = "NX-1001",
                    Balance = 5000.00m,
                    UserId = 1
                },
                new Account
                {
                    Id = 2,
                    AccountNumber = "NX-2002",
                    Balance = 1500.00m,
                    UserId = 2
                }
            );

            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    Id = 1,
                    SenderAccountId = 1,
                    ReceiverAccountId = 2,
                    Amount = 100.00m,
                    Timestamp = DateTime.UtcNow.AddDays(-1),
                    Reference = "INIT-TX-001",
                    Type = TransactionType.Transfer
                }
            );
        }
    }
}