using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MindClinic.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
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
        private readonly INotyfService _notyf;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,
            UserManager<User> usermanager, INotyfService notyf)
        {
            _logger = logger;
            _context = context;
            _usermanager = usermanager;
            _notyf = notyf;
        }


        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            var Doctor = (from doctor in _context.Users
                    where doctor.Name.StartsWith(prefix)

                    select new
                    {
                        label = doctor.Name,
                        val = doctor.Id,
                    }
                ).ToList();

            return Json(Doctor);
        }

        [HttpPost]
        public IActionResult Index(string DoctorName, string DoctorId)
        {
            var Doctor = _context.Users.Where(x => x.RoleId == "2").ToList();
            ViewBag.Message = "DoctorName: " + DoctorName + "DoctorId:" + DoctorId;
            ViewBag.Doctor = DoctorId;

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
                return RedirectToPage("/Account/Login", new {area = "Identity"});
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

        [HttpGet]
        public async Task<IActionResult> ContactUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(string name,string email,string subject,string msg)
        {

            var userid = _usermanager.GetUserId(HttpContext.User);
           

            if (userid != null)
            {
                var User = _context.Users.Where(x => x.Id == userid).First();
                var Contact = new ContactUs
                {
                    Name = User.Name,
                    Email = User.Email,
                    Subject = subject,
                    Message = msg

                };
                _notyf.Success("Thanks for Contacting Us");
                _context.Add(Contact);
                await _context.SaveChangesAsync();
               
            }
            else
            {
                var Contact = new ContactUs
                {
                    Name = name,
                    Email = email,
                    Subject = subject,
                    Message = msg

                };
                _notyf.Success("Thanks for Contacting Us");
                _context.Add(Contact);
                await _context.SaveChangesAsync();
                
            }


            return RedirectToAction("Index","Home");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public async Task<IActionResult> SearchForDoctors(string? search, string? orderby, int? pageNumber)
        {
            //ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";  
            ViewData["search"] = search;

            var Doctors = _context.Users.Where(x => x.RoleId == "2");
            if (!String.IsNullOrEmpty(search))
            {
                Doctors = Doctors.Where(s => s.Name.Contains(search)
                                             || s.Email.Contains(search)
                );
                ViewData["search"] = search;
            }
            if (orderby != null)
            {
                ViewData["orderBy"] = orderby;
                switch (orderby)
                {

                    case "Name_desc":
                        Doctors = Doctors.OrderByDescending(s => s.Name);
                        break;
                    case "Age":
                        Doctors = Doctors.OrderBy(s => s.Age);
                        break;
                    case "Male Dcotors":
                        Doctors = Doctors.Where(s => s.Gender == "Male");
                        break;
                    case "Female Doctors":
                        Doctors = Doctors.Where(s => s.Gender == "Female");
                        break;
                    default:
                        Doctors = Doctors.OrderBy(s => s.Name);
                        break;

                }
            }

            int pageSize = 10;

            return View(await PaginatedList<User>.CreateAsync(Doctors.AsNoTracking(), pageNumber ?? 1, pageSize));
        }





    }
}
