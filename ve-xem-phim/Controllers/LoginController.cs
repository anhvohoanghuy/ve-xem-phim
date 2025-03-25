using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ve_xem_phim.Models;
using ve_xem_phim.ViewModel;

namespace ve_xem_phim.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                return Ok("Đăng nhập thành công!");
            }
            return Unauthorized("Sai email hoặc mật khẩu");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            User user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                LastName = model.LastName,
                FirstName = model.FirstName
            };
            var result = await _userManager.CreateAsync(user,model.Password);

            if (result.Succeeded)
            {
                if (user != null && !await _userManager.IsInRoleAsync(user, "User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok("Đăng ký thành công!");
            }
            return BadRequest(result.Errors);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

    }
}
