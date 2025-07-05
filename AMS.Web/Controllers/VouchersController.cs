using AMS.Web.Services;
using AMS.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Web.Controllers
{
    public class VouchersController : Controller
    {
        private readonly IVoucherService _voucherService;
        private readonly IAccountService _accountService;
        private readonly UserManager<IdentityUser> _userManager;

        public VouchersController(IVoucherService voucherService, IAccountService accountService, UserManager<IdentityUser> userManager)
        {
            _voucherService = voucherService;
            _accountService = accountService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var vouchers = await _voucherService.GetVouchersAsync();
            return View(vouchers);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new VoucherViewModel
            {
                VoucherDate = DateTime.Today,
                VoucherTypes = await _voucherService.GetVoucherTypesAsync(),
                Accounts = await _accountService.GetAccountsAsync(),
                Details = new List<VoucherDetailViewModel> { new VoucherDetailViewModel() }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoucherViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Validate that debits equal credits
                var totalDebits = viewModel.Details.Sum(d => d.DebitAmount);
                var totalCredits = viewModel.Details.Sum(d => d.CreditAmount);

                if (totalDebits != totalCredits)
                {
                    ModelState.AddModelError("", "Total debits must equal total credits.");
                }
                else
                {
                    var user = await _userManager.GetUserAsync(User);
                    var voucherId = await _voucherService.SaveVoucherAsync(viewModel, user.Id);

                    if (voucherId > 0)
                    {
                        TempData["Success"] = "Voucher saved successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            viewModel.VoucherTypes = await _voucherService.GetVoucherTypesAsync();
            viewModel.Accounts = await _accountService.GetAccountsAsync();
            return View(viewModel);
        }
    }
}
