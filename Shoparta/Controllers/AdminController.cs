using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shoparta.Repositories;
using Shoparta.ViewModels;

namespace Shoparta.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(IUserOrderRepository userOrderRepository, UserManager<IdentityUser> userManager)
        {
            _userOrderRepo = userOrderRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> AdminPanel()
        {
            var orders = await _userOrderRepo.UserOrders();
            return View(orders);
        }

        // GET: Admin/Users
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // GET: Admin/UserDetails/5
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // GET: Admin/CreateUser
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("Email,Password")] CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Users));
                }

                AddErrors(result);
            }
            return View(model);
        }

        // GET: Admin/EditUser/5
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email
            };

            return View(model);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, EditUserViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return NotFound();

                user.Email = model.Email;
                user.UserName = model.Email;

                var emailUpdateResult = await _userManager.UpdateAsync(user);

                if (!emailUpdateResult.Succeeded)
                {
                    AddErrors(emailUpdateResult);
                    return View(model);
                }

               
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    
                    var passwordChangeResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (!passwordChangeResult.Succeeded)
                    {
                        AddErrors(passwordChangeResult);
                        return View(model);
                    }
                }

                return RedirectToAction(nameof(Users));
            }
            return View(model);
        }

        // GET: Admin/DeleteUser/5
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Users));
            }

            AddErrors(result);

            return View(user);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}