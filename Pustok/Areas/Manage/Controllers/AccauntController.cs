using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Manage.ViewModels;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccauntController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccauntController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }
        //public async Task<IActionResult> Index()
        //{
        //    //AppUser admin = new AppUser
        //    //{
        //    //    Fullname = "Super Admin",
        //    //    UserName = "SuperAdmin"
        //    //};

        //    //var result = await _userManager.CreateAsync(admin, "newAdmin123");
        //    //if (!result.Succeeded)
        //    //{
        //    //    return Ok(result.Errors);
        //    //}

        //    return View();
        //}

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel adminVM)
        {
            if (!ModelState.IsValid)
                return View();

            var user = _userManager.Users.FirstOrDefault(x =>x.IsAdmin && x.UserName == adminVM.Username);
            if (user==null)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }

            var result =await _signInManager.PasswordSignInAsync(user, adminVM.Password,false,false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }

            return RedirectToAction("index", "dashboard");
        }

        public IActionResult GetUser()
        {
            if (User.Identity.IsAuthenticated)
                return Content(User.Identity.Name);
            else return Content("login edin!");
        }

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "accaunt");
        }
    }
}
