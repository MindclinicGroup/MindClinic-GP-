using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MindClinic.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MindClinic.Data;
using Microsoft.AspNetCore.Identity;

namespace MindClinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;
        private readonly UserManager<User> _usermanager;

        public HomeController(ILogger<HomeController> logger , ApplicationDbContext context, UserManager<User> usermanager)
        {
            _logger = logger;
            _context = context;
            _usermanager = usermanager;
        }

        public IActionResult Index()
        {
            var Doctor = _context.Users.Where(x => x.RoleId == "2").ToList();

            return View(Doctor);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SearchForDoctors()
        {
            var Doctor = _context.Users.Where(x => x.RoleId == "2").ToList();

            return View(Doctor);
        }



    }
}
