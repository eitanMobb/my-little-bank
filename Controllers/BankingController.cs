using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MyLittleBank.Models;
using MyLittleBank.Services;

namespace MyLittleBank.Controllers
{
    [Authorize]
    public class BankingController : Controller
    {
        private readonly DatabaseService _databaseService;

        public BankingController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public IActionResult Dashboard()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var accounts = _databaseService.GetUserAccounts(userId);
            
            ViewBag.UserName = User.FindFirst("FirstName")?.Value + " " + User.FindFirst("LastName")?.Value;
            return View(accounts);
        }

        [HttpGet]
        public IActionResult Transfer()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var accounts = _databaseService.GetUserAccounts(userId);
            return View(accounts);
        }

        [HttpPost]
        public IActionResult Transfer(TransferRequest request)
        {
            // Process money transfer between accounts
            
            if (request.Amount <= 0)
            {
                ViewBag.ErrorMessage = "Amount must be greater than zero";
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var accounts = _databaseService.GetUserAccounts(userId);
                return View(accounts);
            }

            var success = _databaseService.TransferMoney(request.FromAccount, request.ToAccount, request.Amount);
            
            if (success)
            {
                ViewBag.SuccessMessage = $"Successfully transferred ${request.Amount:F2} from {request.FromAccount} to {request.ToAccount}";
            }
            else
            {
                ViewBag.ErrorMessage = "Transfer failed. Please check account numbers and balance.";
            }

            var userId2 = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var accounts2 = _databaseService.GetUserAccounts(userId2);
            return View(accounts2);
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            // Search for accounts based on user input
            var accounts = _databaseService.SearchAccounts(searchTerm);
            return View("SearchResults", accounts);
        }
    }
}
