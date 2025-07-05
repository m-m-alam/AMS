using AMS.Web.Models;
using AMS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AMS.Web.Controllers
{
    public class ChartOfAccountsController : Controller
    {
        private readonly IAccountService _accountService;

        public ChartOfAccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await _accountService.GetAccountsAsync();
            return View(accounts);
        }

        public async Task<IActionResult> Create()
        {
            var accounts = await _accountService.GetAccountsAsync();
            ViewBag.ParentAccounts = new SelectList(accounts.Where(a => (bool)a.IsActive), "Id", "AccountName");
            ViewBag.AccountTypes = new SelectList(new[] { "Asset", "Liability", "Equity", "Revenue", "Expense" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChartOfAccount account)
        {
            if (ModelState.IsValid)
            {
                account.CreatedBy = User.Identity.Name;
                var id = await _accountService.CreateAccountAsync(account);
                if (id > 0)
                {
                    TempData["Success"] = "Account created successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var accounts = await _accountService.GetAccountsAsync();
            ViewBag.ParentAccounts = new SelectList(accounts.Where(a => (bool)a.IsActive), "Id", "AccountName");
            ViewBag.AccountTypes = new SelectList(new[] { "Asset", "Liability", "Equity", "Revenue", "Expense" });
            return View(account);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound();

            var accounts = await _accountService.GetAccountsAsync();
            ViewBag.ParentAccounts = new SelectList(accounts.Where(a => (bool)a.IsActive && a.Id != id), "Id", "AccountName", account.ParentAccountId);
            ViewBag.AccountTypes = new SelectList(new[] { "Asset", "Liability", "Equity", "Revenue", "Expense" }, account.AccountType);
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ChartOfAccount account)
        {
            if (ModelState.IsValid)
            {
                var success = await _accountService.UpdateAccountAsync(account);
                if (success)
                {
                    TempData["Success"] = "Account updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var accounts = await _accountService.GetAccountsAsync();
            ViewBag.ParentAccounts = new SelectList(accounts.Where(a => (bool)a.IsActive && a.Id != account.Id), "Id", "AccountName", account.ParentAccountId);
            ViewBag.AccountTypes = new SelectList(new[] { "Asset", "Liability", "Equity", "Revenue", "Expense" }, account.AccountType);
            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _accountService.DeleteAccountAsync(id);
            if (success)
            {
                TempData["Success"] = "Account deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete account.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

