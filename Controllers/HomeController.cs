using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusPay.Models;
using NexusPay.Services;
using NexusPay.ViewModels;
using System.Diagnostics;

namespace NexusPay.Controllers
{
    // Removing [ApiController] and [Route] allows standard MVC View routing
    // or you can keep them and use specific [HttpGet] attributes.
    public class WalletController : Controller // Changed to Controller to support Views
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(ITransactionService transactionService, ILogger<WalletController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        // --- MVC VIEWS (UI) ---

        // GET: /Wallet/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // We'll add a method to our service to get all accounts for the dashboard
            var accounts = await _transactionService.GetAllAccountsAsync();
            return View(accounts);
        }

        // GET: /Wallet/History/1
        [HttpGet]
        public async Task<IActionResult> History(int id)
        {
            var history = await _transactionService.GetTransactionHistoryAsync(id);
            return View(history);
        }

        // GET: /Wallet/Transfer
        [HttpGet]
        public IActionResult Transfer()
        {
            return View(new TransferViewModel());
        }

        // --- API ENDPOINTS (Logic) ---

        [HttpPost]
        [ValidateAntiForgeryToken] // Security best practice for MVC forms
        public async Task<IActionResult> Transfer(TransferViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _transactionService.TransferFundsAsync(
                model.FromAccountId, model.ToAccountId, model.Amount);

            if (result) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Transfer failed. Check balance.");
            return View(model);
        }
    }
}