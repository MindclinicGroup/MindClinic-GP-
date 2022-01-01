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
            var Doctor = _context.Doctors.Include(x => x.User).ToList();
            ViewBag.Message = "DoctorName: " + DoctorName + "DoctorId:" + DoctorId;
            ViewBag.Doctor = DoctorId;

            return View(Doctor);

        }

        public IActionResult Index()
        {
            var Doctor = _context.Doctors.Include(x=>x.User).ToList();

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

            int pageSize = 8;

            return View(await PaginatedList<User>.CreateAsync(Doctors.AsNoTracking(), pageNumber ?? 1, pageSize));
        }




        public async Task<IActionResult> SearchForSecretary(string? search, string? orderby, int? pageNumber)
        {
            //ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";  
            ViewData["search"] = search;

            var secretary = _context.Users.Where(x => x.RoleId == "4");
            if (!String.IsNullOrEmpty(search))
            {
                secretary = secretary.Where(s => s.Name.Contains(search)
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
                        secretary = secretary.OrderByDescending(s => s.Name);
                        break;
                    case "Age":
                        secretary = secretary.OrderBy(s => s.Age);
                        break;
                    case "Male Secretary":
                        secretary = secretary.Where(s => s.Gender == "Male");
                        break;
                    case "Female Secretary":
                        secretary = secretary.Where(s => s.Gender == "Female");
                        break;
                    default:
                        secretary = secretary.OrderBy(s => s.Name);
                        break;

                }
            }

            int pageSize = 8;

            return View(await PaginatedList<User>.CreateAsync(secretary.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> WorkWithMe()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>  WorkWithMe(string id,string Msg,double salary)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);

            var Request = new SecretaryRequests
            {
                SecretaryID = id,
                DoctorId = userid,
                Message = Msg,
                Salary = salary,
            };

            _notyf.Success("Request Sent");
            _context.Add(Request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Home");

        }


        public async Task<IActionResult> SecretaryViewProfile(string? id)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);




            if (id == null)
            {
                var Sacertary = _usermanager.Users.Where(x => x.Id == userid).First();
                return View(Sacertary);
            }
            else
            {
                var Sacertary = _usermanager.Users.Where(x => x.Id == id).First();
                return View(Sacertary);
            }
        }


        public IActionResult SecretaryRequests()
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            var Secretary = _context.SecretaryRequests.Where(x => x.SecretaryID == userid).Include(s=>s.Doctor).Include(y=>y.Secretary).ToList();

            return View(Secretary);

        }


       

        public async Task<IActionResult> AcceptRequest(string id,double salary,string status,int ReqId)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            var requset = _context.SecretaryRequests.Where(x => x.SecretaryID == userid).First();
            var Secretary = _context.Secretary.Where(x => x.DoctorId == id).Count();

            

            
            if (status == "Yes")
            {

                var req = _context.SecretaryRequests.Where(x => x.Id == ReqId).First();
                var secretary = new SecretaryClass
                {
                    SecretaryId = userid,
                    DoctorId = id,
                    Salary = salary,
                };
                _notyf.Success("Request Accepted");
                _context.Add(secretary);
                _context.Remove(req);
                await _context.SaveChangesAsync();

            }
            else
            {
                var Request = _context.SecretaryRequests.Where(x => x.Id == ReqId).First();
                _notyf.Error("Request Rejected");
                _context.Remove(Request);
                await _context.SaveChangesAsync();

            }

            

            return RedirectToAction("SecretaryRequests");

        }

    }


}


