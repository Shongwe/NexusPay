using Microsoft.EntityFrameworkCore;
using NexusPay.Data;
using NexusPay.Models;

namespace NexusPay.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TransferFundsAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var sender = await _context.Accounts.FindAsync(fromAccountId);
                var receiver = await _context.Accounts.FindAsync(toAccountId);

                if (sender == null || receiver == null || sender.Balance < amount)
                {
                    return false; 
                }

                sender.Balance -= amount;

                receiver.Balance += amount;

                var auditEntry = new Transaction
                {
                    SenderAccountId = fromAccountId,
                    ReceiverAccountId = toAccountId,
                    Amount = amount,
                    Timestamp = DateTime.UtcNow,
                    Type = TransactionType.Transfer,
                    Reference = $"TX-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
                };

                _context.Transactions.Add(auditEntry);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new Exception("Concurrency conflict detected. Please retry.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(int accountId)
        {
            return await _context.Transactions
                .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<decimal> GetBalanceAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            return account?.Balance ?? 0;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts
                .Include(a => a.User)
                .ToListAsync();
        }
    }
}