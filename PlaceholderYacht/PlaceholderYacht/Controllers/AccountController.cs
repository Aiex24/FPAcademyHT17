using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PlaceholderYacht.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using PlaceholderYacht.Models;

namespace PlaceholderYacht.Controllers
{
    public class AccountController : Controller
    {
        IBoatRepository repository;
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        RoleManager<IdentityRole> roleManager;
        IdentityDbContext identityContext;

        public AccountController(
        IBoatRepository repository,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IdentityDbContext identityContext)
        {
            this.repository = repository;
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
            };

            return RedirectToAction(nameof(AccountController.UserPage), "Account");
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

        [Authorize]
        [HttpGet]
        [Route("account/UserPage/{title}")]
        public async Task<IActionResult> UserPage(string title)
        {
            //Hämta info om användaren som är inloggad
            string userID = userManager.GetUserId(HttpContext.User);
            IdentityUser user = await userManager.FindByIdAsync(userID);

            if (title == "Updated")
                title = $"{user.UserName} has been updated";
            else if (title == user.UserName)
                title = $"Userpage {user.UserName}";
            else if (title == "error")
                title = $"Something went wrong, {user.UserName} was not updated";

            return View(new AccountUserpageVM
            {
                BoatItem = repository.GetUsersBoatsByUID(userID),
                UserName = user.UserName,
                Email = user.Email,
                Title = title
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(AccountUserpageVM viewModel)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(UserPage), new { title = "error" });

            //Hämta info om användaren som är inloggad
            string userID = userManager.GetUserId(HttpContext.User);
            IdentityUser user = await userManager.FindByIdAsync(userID);

            //Uppdatera användaren om det finns någon förändring i username eller mail
            if (viewModel.UserName != user.UserName || viewModel.Email != user.Email)
            {
                user.UserName = viewModel.UserName;
                user.Email = viewModel.Email;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return RedirectToAction(nameof(UserPage), new { title = "error" });

                return RedirectToAction(nameof(UserPage), new{title = "Updated"});
            }
            return RedirectToAction(nameof(UserPage), new { title = "error" });
        }
    }
}
