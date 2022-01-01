using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.Extensions.Logging;
using MindClinic.Data;

namespace MindClinic.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;

        public AdminDashboardController(ApplicationDbContext context, INotyfService notyf, ILogger<HomeController> logger)
        {
            _context = context;
            _notyf = notyf;
            _logger = logger;
        }

        public IActionResult Index()
        {
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
            var Doctors = _context.Users.Where(r => r.RoleId == "2").ToList();
            return View(Doctors);
        }
        public IActionResult PatientList()
        {
            var Patients = _context.Users.Where(r => r.RoleId == "3").ToList();

            return View(Patients);
        }

        public IActionResult Appointment()
        {
            var Appointments = _context.Appointments.ToList();
            var Doctors = _context.Users.Where(r => r.RoleId == "2").ToList();
            var Patients = _context.Users.Where(r => r.RoleId == "3").ToList();
            var model1 = Tuple.Create<IEnumerable<MindClinic.Models.User>, IEnumerable<MindClinic.Models.Appointment>, IEnumerable<MindClinic.Models.User>>(Doctors, Appointments, Patients);
            return View(model1);
        }

        public IActionResult Reviews()
        {
            var reviews = _context.Reviews.ToList();
            return View(reviews);

        }

        

        public IActionResult ContactUs()
        {
            var contact = _context.ContactUs.ToList();
            return View(contact);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteContactUs(int id)
        {
            var Contact = _context.ContactUs.Where(x => x.Id == id).First();

            _notyf.Error("Deleted");
            _context.ContactUs.Remove(Contact);
            await _context.SaveChangesAsync();

            return RedirectToAction("ContactUs");

        }
    }
}
