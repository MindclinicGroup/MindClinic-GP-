using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MindClinic.Data;
using MindClinic.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace MindClinic.Controllers
{



    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private ApplicationDbContext _context;
        private readonly UserManager<User> _usermanager;
        private readonly INotyfService _notyf;
        public PaymentController(ApplicationDbContext context, UserManager<User> usermanager, ILogger<PaymentController> logger, INotyfService notyf)
        {
            _logger = logger;
            _context = context;
            _usermanager = usermanager;
            _notyf = notyf;
        }
        public async Task<IActionResult> Index(string id, DateTime time)
        {
            if (HttpContext.User.IsInRole("DOCTOR"))
            {
                _notyf.Error("Doctors can not book appointments");
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.User.IsInRole("ADMIN"))
            {
                _notyf.Error("Admins can not book appointments");
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.User.IsInRole("Secretary"))
            {
                _notyf.Error("Secretaries can not book appointments");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Time = time;
            var userid = _usermanager.GetUserId(HttpContext.User);
            var doctor = _context.Doctors.Where(x => x.userID == id).ToList();
            var DoctorUser = _context.Users.Where(x => x.Id == id).ToList();
            var model1 = Tuple.Create<IEnumerable<MindClinic.Models.User>, IEnumerable<MindClinic.Models.DoctorClass>>(DoctorUser, doctor);
            return View(model1);
        }
        [HttpPost]
        public async Task<IActionResult> Payment(string Name, string CardNum, int ExpMon, int ExpYear, int CVV, string id, DateTime a)
        {
            var Doctor = _context.Doctors.Where(x => x.userID == id).FirstOrDefault();
            if (Doctor != null)
            {
                
                var schedule = _context.Schedules.Where(x => x.startTime.Date == a.Date && x.doctorID == Doctor.userID);
                bool tag = false;
                foreach (var item in schedule) 
                {
                    var booked = _context.Appointments.Where(x => x.doctorId == Doctor.userID && x.Time == a).FirstOrDefault();
                    if (schedule != null && booked == null &&a.Minute == item.startTime.Minute && a.TimeOfDay <item.endtime.TimeOfDay)
                    {
                        tag = true;
                        break;
                    }
                }
                if (tag == false)
                {
                    _notyf.Error("Bad Request!");
                    return RedirectToAction("Index", "Home");
                }
              
                var Payment = _context.PaymentMethods.Where(x =>
                    x.NameOnCard == Name && x.CCV == CVV && x.CardNumber == CardNum && x.ExpiryMonth == ExpMon &&
                    x.ExpiryYear == ExpYear).FirstOrDefault();
                if (Payment != null)
                {
                    if (Payment.Amount >= Doctor.pricePerSession)
                    {
                        Payment.Amount = Payment.Amount - Doctor.pricePerSession;
                        _context.Update(Payment);
                    }
                    else
                    {
                        _notyf.Error("insufficient Funds");
                        return RedirectToAction("Index", "Home");
                    }
                    var userid = _usermanager.GetUserId(HttpContext.User);
                    var TimeSelected = new Appointment
                    {
                        Price = Doctor.pricePerSession,
                        patientId = userid,
                        doctorId = id,
                        status = "True",
                        Time = (DateTime)a,
                        MeetingLink = Doctor.DefaultMeetingLink,
                        Duration = 60,
                        PaymentId = Payment.Id  
                    }; 
                    _notyf.Success("Appointment is booked successfully");
                    _context.Add(TimeSelected);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("PatientAppointments", "Home");
                }
                else
                    _notyf.Error("Unable to validate card."); 
                return RedirectToAction("Index", "Home"); 
            } 
            else
            { 
                _notyf.Error("Bad Request!");
                return RedirectToAction("Index", "Home");
            }
        }




    }
}
