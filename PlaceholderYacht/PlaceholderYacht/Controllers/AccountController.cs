using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PlaceholderYacht.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlaceholderYacht.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        RoleManager<IdentityRole> roleManager;
        IdentityDbContext identityContext;

        public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IdentityDbContext identityContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.identityContext = identityContext;
        }

        [Route("account/index/{userName}")]
        public IActionResult Index(string userName)
        {
            return View((object)userName);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginVM viewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(AccountLoginVM.UserName), "Wrong username or password");
                return View();
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize]
        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }

        [HttpGet]
        public IActionResult NewUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewUser(AccountNewuserVM viewModel)
        {
            if (!ModelState.IsValid)
                return View();

            //Kontrollera om användaren redan finns
            IdentityUser user = await userManager.FindByNameAsync(viewModel.UserName);
            if (user != null)
            {
                ModelState.AddModelError(nameof(AccountLoginVM.UserName), "Username already exists");
                return View();
            }

            //Skapa användare
            var newUser = new IdentityUser(viewModel.UserName);
            var result = await userManager.CreateAsync(newUser, viewModel.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(AccountLoginVM.UserName), "Username or password was not valid: ");
                return View();
            }

            //Lägg till E-post om användaren matat in det
            if (viewModel.Email != null)
            {
                newUser.Email = viewModel.Email;
                await userManager.UpdateAsync(newUser);
            }
            return RedirectToAction(nameof(Index), new { userName = viewModel.UserName });
        }

    }
}
