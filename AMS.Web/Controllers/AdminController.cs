using AMS.Web.Models;
using AMS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserStore _userStore; 
        private readonly RoleStore _roleStore;   

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            UserStore userStore, 
            RoleStore roleStore)   
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _roleStore = roleStore;
        }

        
        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
         
            var users = await _userStore.GetAllUsersAsync(CancellationToken.None);
            return View(users);
        }

       
        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            
            var allRoles = (await _roleStore.GetAllRolesAsync(CancellationToken.None)).Select(r => r.Name).ToList();

            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRoles = userRoles,
                AllRoles = allRoles,
                SelectedRoles = userRoles.ToList() 
            };

            return View(model);
        }

  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                
                model.AllRoles = (await _roleStore.GetAllRolesAsync(CancellationToken.None)).Select(r => r.Name).ToList();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            
            var rolesToRemove = userRoles.Except(model.SelectedRoles ?? new List<string>()).ToList();
            foreach (var role in rolesToRemove)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

          
            var rolesToAdd = (model.SelectedRoles ?? new List<string>()).Except(userRoles).ToList();
            foreach (var role in rolesToAdd)
            {
          
                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            var signInManager = HttpContext.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
            await signInManager.RefreshSignInAsync(user);


            return RedirectToAction(nameof(ManageUsers));
        }
    }
}
