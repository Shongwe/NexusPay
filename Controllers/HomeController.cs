using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusPay.Models;
using NexusPay.Services;
using NexusPay.ViewModels;
using System.Diagnostics;

namespace NexusPay.Controllers
{
    public class WalletController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(ITransactionService transactionService, ILogger<WalletController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accounts = await _transactionService.GetAllAccountsAsync();
            return View(accounts);
        }

        [HttpGet]
        public async Task<IActionResult> History(int id)
        {
            var history = await _transactionService.GetTransactionHistoryAsync(id);
            return View(history);
        }

        [HttpGet]
        public IActionResult Transfer()
        {
            return View(new TransferViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
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