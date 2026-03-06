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
            // Start the ACID-compliant transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var sender = await _context.Accounts.FindAsync(fromAccountId);
                var receiver = await _context.Accounts.FindAsync(toAccountId);

                if (sender == null || receiver == null || sender.Balance < amount)
                {
                    return false; // Validation failed
                }

                // 1. Deduct from Sender
                sender.Balance -= amount;

                // 2. Add to Receiver
                receiver.Balance += amount;

                // 3. Create the Audit Log (The Ledger entry)
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

                // 4. Save changes to the database
                await _context.SaveChangesAsync();

                // 5. Commit the transaction (All or nothing)
                await transaction.CommitAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // This catches "Double Spending" attempts via the RowVersion/Timestamp
                await transaction.RollbackAsync();
                throw new Exception("Concurrency conflict detected. Please retry.");
            }
            catch (Exception)
            {
                // Rollback if anything else goes wrong (Network error, Disk error, etc.)
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
            // We include the User data so the Dashboard can show the account owner's name
            return await _context.Accounts
                .Include(a => a.User)
                .ToListAsync();
        }
    }
}