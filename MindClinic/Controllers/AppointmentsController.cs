using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MindClinic.Data;
using MindClinic.Models;

namespace MindClinic.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _usermanager;

        public AppointmentsController(ApplicationDbContext context, UserManager<User> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }


        [HttpGet]
        public async Task<IActionResult> getslots(string? id)
        {
            DateTime current= DateTime.Now;
            var applicationDbContext = _context.Schedules.Include(s => s.doctor).Where(s => s.doctorID == id&& s.startTime>=current);
            var appointments = new List<Appointment>();

            var doctor = _context.Doctors.Where(x => x.userID == id).First();

            foreach (var item in applicationDbContext)
            {

                while (item.endtime.Hour > item.startTime.Hour)
                {
                    Appointment app = new Appointment();

                    app.doctorId = id;
                    app.patientId = null;
                    app.Time = item.startTime;
                    app.Price = doctor.pricePerSession;

                    var Appointment = from contextAppointment in _context.Appointments
                                      where (app.Time == contextAppointment.Time && app.doctorId == contextAppointment.doctorId)

                                      select contextAppointment;

                    if (Appointment.Any())
                    {

                    }
                    else
                    {
                        appointments.Add(app);
                    }


                    item.startTime = item.startTime.AddHours(1);
                    item.startTime = item.startTime.AddMinutes(15);



                }
            }
            return View(appointments);
        }




        [HttpGet]
        public async Task<IActionResult> TimeSelected(DateTime a, string DoctorId)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);

            if (userid == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var TimeSelected = new Appointment
            {
                patientId = userid,
                doctorId = DoctorId,
                status = "True",
                Time = (DateTime)a

            };

            _context.Add(TimeSelected);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");

        }


        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments.Include(a => a.doctor).Include(a => a.patient);
            return View(await applicationDbContext.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.doctor)
                .Include(a => a.patient)
                .FirstOrDefaultAsync(m => m.id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }


        public IActionResult Create()
        {
            ViewData["doctorId"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["patientId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Time,status,doctorId,patientId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["doctorId"] = new SelectList(_context.Users, "Id", "Id", appointment.doctorId);
            ViewData["patientId"] = new SelectList(_context.Users, "Id", "Id", appointment.patientId);
            return View(appointment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["doctorId"] = new SelectList(_context.Users, "Id", "Id", appointment.doctorId);
            ViewData["patientId"] = new SelectList(_context.Users, "Id", "Id", appointment.patientId);
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Time,status,doctorId,patientId")] Appointment appointment)
        {
            if (id != appointment.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["doctorId"] = new SelectList(_context.Users, "Id", "Id", appointment.doctorId);
            ViewData["patientId"] = new SelectList(_context.Users, "Id", "Id", appointment.patientId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.doctor)
                .Include(a => a.patient)
                .FirstOrDefaultAsync(m => m.id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.id == id);
        }

        public async Task<IActionResult> Cancel(int id, string description)
        {
            try
            {
                var userid = _usermanager.GetUserId(HttpContext.User);

                Appointment appointment = _context.Appointments.Where(x => x.id == id).FirstOrDefault();
               
                if (userid == appointment.doctorId)
                {
                    appointment.Description = "Cancelled by doctor";
                }
                else if (userid == appointment.patientId)
                {
                    appointment.Description = "Cancelled by patient";
                }
                else if ((_usermanager.Users.Where(x => x.Id == userid).First().RoleId) == "1") // admin 
                    appointment.Description = "Cancelled by Admin";

                if (description != null)
                { 
                    appointment.Description += "\nNotes: " + description;
                }
                appointment.status = "False";

                _context.Update(appointment);
                await _context.SaveChangesAsync();
                if (userid == appointment.doctorId)
                {
                    return RedirectToAction("GetDoctorAppointments", "Appointments");
                }
                else if (userid == appointment.patientId)
                {
                    return RedirectToAction("PatientAppointments", "Home");
                }
              
        

                return RedirectToAction("index", "Home");
            }
            catch (Exception e) 
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public async Task<IActionResult> GetDoctorAppointments()
        {
         
                var userid = _usermanager.GetUserId(HttpContext.User);
                ViewBag.CountOfAppointments = _context.Appointments.Where(x => x.doctorId == userid).Count();
                ViewBag.TotalPrice = _context.Appointments.Where(x => x.doctorId == userid).Sum(x=>x.Price);
                var appointment = _context.Appointments.Where(x => x.doctorId == userid).Include(x => x.patient);
                return View(appointment);
           
        }
    }
}
