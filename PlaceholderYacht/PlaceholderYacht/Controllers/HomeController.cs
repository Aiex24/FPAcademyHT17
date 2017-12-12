using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlaceholderYacht.Models.ViewModels;
using PlaceholderYacht.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace PlaceholderYacht.Controllers
{
    public class HomeController : Controller
    {
        // obs håll koll på reposen, den ska vara interface
        IBoatRepository repository;
        UserManager<IdentityUser> userManager;

        public HomeController(IBoatRepository repository, UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            this.repository = repository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            double latitude1 = 59.39496;
            double longitude1 = 19.33388;
            repository.GetTime(latitude1, longitude1, 90, 1, 45);
            return View();
        }
        public IActionResult Route()
        {
            return View();
        }
        public IActionResult Vpp()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult BoatPage(int id)
        {
            if (id > 0)
            {
                BoatPageVM boat = repository.GetBoatPageVM(id);
                if (boat.VppList.Count() < 1)
                    
                    boat = repository.AddEmptyVPP(boat);


                ViewBag.ActionName = "UpdateBoat";
                ViewBag.SaveBtnName = "Update Boat";
                return View(boat);
            }
            else
            {
                ViewBag.SaveBtnName = "Add Boat";
                ViewBag.ActionName = "AddBoatToDatabase";
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddBoatToDatabase(BoatPageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(BoatPage), model);
            }

            model.Uid = userManager.GetUserId(HttpContext.User);

            repository.InterpolateVpp(model);
            repository.SaveBoat(model);
            return RedirectToAction(nameof(Route));
        }

        [Authorize]
        [HttpPost]
        public IActionResult UpdateBoat(BoatPageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(BoatPage), model);
            }
            repository.UpdateBoat(model);
            return RedirectToAction(nameof(BoatPage), new { id = model.BoatID });
        }

        [Authorize]
        public IActionResult DeleteBoat(int id)
        {
            repository.DeleteBoat(id);
            return RedirectToAction(nameof(AccountController.UserPage), "Account", new { title = "test" });
        }
    }
}
