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

namespace MindClinic.Controllers
{
   

    
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private ApplicationDbContext _context;
        private readonly UserManager<User> _usermanager;
        public PaymentController(ApplicationDbContext context, UserManager<User> usermanager, ILogger<PaymentController> logger )
        {   _logger = logger;
            _context = context;
            _usermanager = usermanager;
        }
        public async Task<IActionResult> Index(string id , DateTime time)
        {
            ViewBag.Time = time;
            var userid = _usermanager.GetUserId(HttpContext.User);
            var doctor = _context.Doctors.Where(x => x.userID == id).ToList();
            var DoctorUser = _context.Users.Where(x => x.Id == id).ToList();
            var model1 = Tuple.Create<IEnumerable<MindClinic.Models.User>, IEnumerable<MindClinic.Models.DoctorClass>>(DoctorUser, doctor);
            return View(model1);
        }
        [HttpPost]
        public async Task<IActionResult> Payment(string Name, string CardNum, int ExpMon, int ExpYear, int CVV, string id , DateTime a)
        {
            var Doctor = _context.Doctors.Where(x => x.userID == id).FirstOrDefault();
            var Payment = _context.PaymentMethods.Where(x =>
                x.NameOnCard == Name && x.CCV == CVV && x.CardNumber == CardNum && x.ExpiryMonth == ExpMon &&
                x.ExpiryYear == ExpYear).SingleOrDefault();
            Payment.Amount = Payment.Amount - Doctor.pricePerSession;
            _context.Update(Payment);
            var userid = _usermanager.GetUserId(HttpContext.User);
            var TimeSelected = new Appointment
            {
                Price = Doctor.pricePerSession,
                patientId = userid,
                doctorId = id,
                status = "True",
                Time = (DateTime)a
            };

            _context.Add(TimeSelected);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }
    }
}
