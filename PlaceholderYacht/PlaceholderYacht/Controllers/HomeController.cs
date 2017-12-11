using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlaceholderYacht.Models.ViewModels;
using PlaceholderYacht.Models;

namespace PlaceholderYacht.Controllers
{
    public class HomeController : Controller
    {
        // obs håll koll på reposen, den ska vara interface
        IBoatRepository repository;

        public HomeController(IBoatRepository repository)
        {
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
        [HttpGet]
        public IActionResult BoatPage(int id)
        {
            if (id > 0)
            {
                ViewBag.ActionName = "UpdateBoat";
                ViewBag.SaveBtnName = "Update Boat";
                return View(repository.GetBoatPageVM(id));
            }

            ViewBag.SaveBtnName = "Add Boat";
            ViewBag.ActionName = "AddBoatToDatabase";
            return View();
        }
        [HttpPost]
        public IActionResult AddBoatToDatabase(BoatPageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(BoatPage), model);
            }
            repository.InterpolateVpp(model);
            repository.SaveBoat(model);
            return RedirectToAction(nameof(Route));
        }
    }
}
