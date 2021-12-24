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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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


        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            var Doctor = (from doctor in _context.Users
                    where doctor.Name.StartsWith(prefix)

                    select new 
                    {
                        label=doctor.Name,
                        val =doctor.Id,
                    }
                ).ToList();

            return Json(Doctor);
        }

        [HttpPost]
        public IActionResult Index(string DoctorName,string DoctorId)
        {
            var Doctor = _context.Users.Where(x => x.RoleId == "2").ToList();
            ViewBag.Message = "DoctorName: " + DoctorName + "DoctorId:" + DoctorId;

            return View(Doctor);

        }

        public IActionResult Index()
        {
            var Doctor = _context.Users.Where(x => x.RoleId == "2").ToList();

            return View(Doctor);
        }

        public IActionResult PatientAppointments()
        {
            var userid = _usermanager.GetUserId(HttpContext.User);

            if (userid == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            //var applicationDbContext = _context.Reviews.Include(r => r.DoctorUser).Include(r => r.WriterUser);

            string user = _usermanager.FindByIdAsync(userid).Result.Id;

            var App = _context.Appointments.Where(x => x.patientId == user).Include(x => x.doctor).ToList();

            return View(App);
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
