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
            var applicationDbContext = _context.Schedules.Include(s => s.doctor).Where(s => s.doctorID == id);
            var appointments = new List<Appointment>();
            
            foreach (var item in applicationDbContext)
            {

                while (item.endtime.Hour > item.startTime.Hour)
                {
                    Appointment app = new Appointment();

                    app.doctorId = id;
                    app.patientId = null;
                    app.Time = item.startTime;

                    var Appointment = from contextAppointment in _context.Appointments
                        where (app.Time == contextAppointment.Time && app.doctorId==contextAppointment.doctorId) 

                        select contextAppointment;

                    if (Appointment.Any())
                    {
                        
                    }
                    else
                    {
                        appointments.Add(app);
                    }

                             
                    item.startTime = item.startTime.AddHours(1);

                }
            }
            return View(appointments);
        }




        [HttpGet]
        public async Task<IActionResult> TimeSelected(DateTime a,string DoctorId)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);

            var TimeSelected = new Appointment
            {
                patientId = userid,
                doctorId = DoctorId,
                status = "True",
                Time = (DateTime)a

            };

            _context.Add(TimeSelected);
           await _context.SaveChangesAsync();

            return RedirectToAction("Index","Home");

        }


        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments.Include(a => a.doctor).Include(a => a.patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
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

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["doctorId"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["patientId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Appointments/Edit/5
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

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
    }
}
