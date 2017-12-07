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
        IBoatRepository repository;
        public HomeController(IBoatRepository repository)
        {
            this.repository = repository;
        }
        public IActionResult Index()
        {
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
                return View(repository.GetBoatPageVM(id));

            return View();
        }
    }
}
