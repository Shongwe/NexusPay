using NexusPay.Models;

namespace NexusPay.Services
{
    public interface ITransactionService
    {
        Task<bool> TransferFundsAsync(int fromAccountId, int toAccountId, decimal amount);
        Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(int accountId);
        Task<decimal> GetBalanceAsync(int accountId);

        Task<IEnumerable<Account>> GetAllAccountsAsync();
    }
}

