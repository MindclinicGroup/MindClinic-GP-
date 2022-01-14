using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.Extensions.Logging;
using MindClinic.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MindClinic.Models;
using Microsoft.AspNetCore.Authorization;

namespace MindClinic.Controllers
{
    
    public class AdminDashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;
        private readonly UserManager<User> _usermanager;

        public AdminDashboardController(ApplicationDbContext context, INotyfService notyf, ILogger<HomeController> logger, UserManager<User> usermanager)
        {
            _context = context;
            _notyf = notyf;
            _logger = logger;
            _usermanager = usermanager;
        }
       
         
        public IActionResult Index()
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CountOfAppointments = _context.Appointments.Count();
            ViewBag.CountOfPatients = _context.Users.Where(x => x.RoleId == "3").Count();
            ViewBag.CountOfDoctors = _context.Users.Where(x => x.RoleId == "2").Count();


            var Appointments = _context.Appointments.ToList();
            var Doctors = _context.Users.Where(r => r.RoleId == "2").ToList();
            var Patients = _context.Users.Where(r => r.RoleId == "3").ToList();
            var model1= Tuple.Create<IEnumerable<MindClinic.Models.User>, IEnumerable<MindClinic.Models.Appointment>, IEnumerable<MindClinic.Models.User>>(Doctors, Appointments, Patients);
            return View(model1);


        }
        public IActionResult DoctorsList()
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            var Doctors = _context.Users.Where(r => r.RoleId == "2").ToList();
            return View(Doctors);
        }
        public IActionResult PatientList()
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            var Patients = _context.Users.Where(r => r.RoleId == "3").ToList();

            return View(Patients);
        }

        public IActionResult Appointment()
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            var Appointments = _context.Appointments.ToList();
            var Doctors = _context.Users.Where(r => r.RoleId == "2").ToList();
            var Patients = _context.Users.Where(r => r.RoleId == "3").ToList();
            var model1 = Tuple.Create<IEnumerable<MindClinic.Models.User>, IEnumerable<MindClinic.Models.Appointment>, IEnumerable<MindClinic.Models.User>>(Doctors, Appointments, Patients);
            return View(model1);
        }

        public IActionResult Reviews(string? id)
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            var reviews = _context.Reviews.Include(x=>x.DoctorUser).Include(z=>z.WriterUser).ToList();

            if (id != null)
            {
              
                 reviews = _context.Reviews.Where(x => x.DoctorUserId == id).ToList();
                 var user = _context.Users.Where(x => x.RoleId == "2").ToList();
                 var model1 = Tuple.Create<IEnumerable<MindClinic.Models.User>, IEnumerable<MindClinic.Models.Reviews>>(user, reviews);


                return View(model1);
            }
            else
            {
                reviews = _context.Reviews.ToList();
                var user = _context.Users.Where(x=>x.RoleId=="2").ToList();
                var model1 = Tuple.Create<IEnumerable<MindClinic.Models.User>, IEnumerable<MindClinic.Models.Reviews>>(user, reviews);

                return View(model1);
            }

        }

        [HttpGet]
        public async Task<IActionResult> DeleteReview(int id)
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            var reviews = _context.Reviews.Where(x => x.Id == id).First();

            _notyf.Error("Deleted");
            _context.Reviews.Remove(reviews);
            await _context.SaveChangesAsync();
            return RedirectToAction("Reviews");
        }



        public IActionResult ContactUs()
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            var contact = _context.ContactUs.ToList();
            return View(contact);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteContactUs(int id)
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            var Contact = _context.ContactUs.Where(x => x.Id == id).First();

            _notyf.Error("Deleted");
            _context.ContactUs.Remove(Contact);
            await _context.SaveChangesAsync();
            return RedirectToAction("ContactUs");

        }
        [HttpGet]
        public async Task<IActionResult> LockUser(string email, DateTime? endDate)
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            DateTime EndDate = new DateTime(2222, 06, 06);
            if (endDate == null)
                endDate = EndDate;

            var userTask = _usermanager.FindByEmailAsync(email);
            userTask.Wait();
            var user = userTask.Result;

            var lockUserTask = _usermanager.SetLockoutEnabledAsync(user, true);
            lockUserTask.Wait();

            var lockDateTask = _usermanager.SetLockoutEndDateAsync(user, endDate);
            lockDateTask.Wait();
            if(lockDateTask.Result.Succeeded && lockUserTask.Result.Succeeded)
            {
                _notyf.Success("locked: " + user.Email);
            }
            else 
            {
                _notyf.Error("Something is wrong");
            }
            if(user.RoleId=="2") return RedirectToAction("DoctorsList", "AdminDashboard");
            if(user.RoleId=="3") return RedirectToAction("PatientList", "AdminDashboard");
            return RedirectToAction("PatientList", "AdminDashboard");
        }
        public async Task<IActionResult> UnlockUser(string email)
        {
            if (!HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Access denied!");
                return RedirectToAction("Index", "Home");
            }
            var userTask = _usermanager.FindByEmailAsync(email);
            userTask.Wait();
            var user = userTask.Result;

            var lockDisabledTask = _usermanager.SetLockoutEnabledAsync(user, false);
            lockDisabledTask.Wait();

            var setLockoutEndDateTask = _usermanager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));
            setLockoutEndDateTask.Wait();
            var result = setLockoutEndDateTask.Result.Succeeded && lockDisabledTask.Result.Succeeded;
         
            _notyf.Success("Unlocked: " + user.Email);
            if (user.RoleId == "2") return RedirectToAction("DoctorsList", "AdminDashboard");
            if (user.RoleId == "3") return RedirectToAction("PatientList", "AdminDashboard");
            return RedirectToAction("PatientList", "AdminDashboard");
       
        }
    }
}
