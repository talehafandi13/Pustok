using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("Member"));
            return Ok();
        }

        public async Task<IActionResult> CreateAdmin()
        {
            AppUser admin = new AppUser
            {
                UserName = "SuperAdmin",
                Fullname = "Super Admin",
                IsAdmin =true
            };
            var result = await _userManager.CreateAsync(admin, "SuperAdmin123");
            if (!result.Succeeded)
            {
                return Ok(result.Errors);
            }
            await _userManager.AddToRoleAsync(admin, "SuperAdmin");

            return RedirectToAction("index", "home");
        }
        //public IActionResult Index()
        //{

        //    return View();
        //}
        public IActionResult Register()
        {
            MemberRegisterViewModel registerVM = new MemberRegisterViewModel();
            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel memberVM)
        {
            if (!ModelState.IsValid)
                return View();

            AppUser appUser = new AppUser
            {
                Fullname = memberVM.Fullname,
                UserName = memberVM.Username,
                Email = memberVM.Email,
                IsAdmin =false
            };

            var result =  await _userManager.CreateAsync(appUser, memberVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            string url = Url.Action("ConfirmEmail", "Account", new { email = appUser.Email, token = token }, Request.Scheme);

            await _userManager.AddToRoleAsync(appUser, "Member");

            return Ok(new { URL = url });

            return RedirectToAction("index","home");
        }

        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            AppUser member = await _userManager.FindByEmailAsync(email);
            if (member == null)
                return RedirectToAction("error", "home");
            var result = await _userManager.ConfirmEmailAsync(member, token);
            if (result.Succeeded)
                return RedirectToAction("login");
            else
                return RedirectToAction("error", "home");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel memberVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser member =  _userManager.Users.FirstOrDefault(x=>!x.IsAdmin && x.UserName == memberVM.LoginUsername);
            if (member == null)
            {
                ModelState.AddModelError("", "username or password incorrect");
                return View();
            }
            if (!member.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please, confirm email");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(member, memberVM.LoginPassword, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "username or password incorrect");
                return View();
            }

            return RedirectToAction("index","home");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        public  IActionResult Forgot()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Forgot(ForgotPasswordViewModel forgotVM)
        {
            if (!ModelState.IsValid)
                return View();
            AppUser member = await _userManager.FindByEmailAsync(forgotVM.Email);
            if (member ==null)
            {
                ModelState.AddModelError("Email", "Email is incorrect");
                return View();
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(member);
            string url = Url.Action("ResetPassword", "Account", new { email = member.Email, token = token }, Request.Scheme);
            return Ok(new { URL = url });
        }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            var member = _userManager.Users.FirstOrDefault(x => !x.IsAdmin && x.Email == email);
            if (member ==null)
            {
                return RedirectToAction("error", "home");
            }
            if (!await _userManager.VerifyUserTokenAsync(member,_userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token))
            {
                return RedirectToAction("error", "home");
            }

            MemberResetPasswordViewModel vm = new MemberResetPasswordViewModel
            {
                Email = email,
                Token= token
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(MemberResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View();
            AppUser member = _userManager.Users.FirstOrDefault(x => !x.IsAdmin && x.NormalizedEmail == vm.Email.ToUpper());
            if (member == null)
            {
                return RedirectToAction("error", "home");
            }

            var result = await _userManager.ResetPasswordAsync(member, vm.Token, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            return RedirectToAction("login");
        }
    }
}
