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
        const string AdminRoleName = "Admin";

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

            return RedirectToAction(nameof(AccountController.UserPage), "Account", new { title = $"Userpage {viewModel.UserName}" });
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
            List<AccountUserItemVM> userList = new List<AccountUserItemVM>();
            AccountBoatItemVM[] boatList;

            //Hämta info om användaren som är inloggad
            string userID = userManager.GetUserId(HttpContext.User);
            IdentityUser user = await userManager.FindByIdAsync(userID);

            //Om användaren är Admin, hämta lista med alla båtar och alla användare
            if (await userManager.IsInRoleAsync(user, AdminRoleName))
            {
                boatList = repository.GetAllBoats();
                foreach (AccountBoatItemVM boat in boatList)
                {
                    if (boat.Owner != null)
                    {
                        user = await userManager.FindByIdAsync(boat.Owner);
                        boat.Owner = user.UserName;
                    }
                }

                foreach (var u in userManager.Users)
                {
                    bool isAdmin = false;
                    if (await userManager.IsInRoleAsync(u, AdminRoleName))
                        isAdmin = true;

                    userList.Add(new AccountUserItemVM { UserID = u.Id, Email = u.Email, UserName = u.UserName, Admin = isAdmin });
                }
            }
            else
                boatList = repository.GetUsersBoatsByUID(userID);

            return View(new AccountUserpageVM
            {
                BoatItem = boatList,
                UserName = user.UserName,
                Email = user.Email,
                Title = title,
                UserItem = userList.ToArray()
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(AccountUserpageVM viewModel)
        {
            string errorMess = $"Something went wrong, user was not updated";
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(UserPage), new { title = errorMess });

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
                    return RedirectToAction(nameof(UserPage), new { title = errorMess });

                return RedirectToAction(nameof(UserPage), new { title = $"{user.UserName} has been updated" });
            }
            return RedirectToAction(nameof(UserPage), new { title = errorMess });
        }

        [Authorize]
        [Route("account/MakeAdmin/{userId}")]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            IdentityUser user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Content($"Kunde inte hitta någon användare med ID: {userId}");
            }

            await userManager.AddToRoleAsync(user, AdminRoleName);
            return RedirectToAction(nameof(UserPage), new { title = $"Userpage {user.UserName}" });
        }

        [Authorize]
        [Route("account/RemoveAdminRole/{userId}")]
        public async Task<IActionResult> RemoveAdminRole(string userId)
        {
            IdentityUser user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Content($"Kunde inte hitta någon användare med ID: {userId}");
            }

            await userManager.RemoveFromRoleAsync(user, AdminRoleName);
            return RedirectToAction(nameof(UserPage), new { title = $"Userpage {user.UserName}" });
        }

    }
}
