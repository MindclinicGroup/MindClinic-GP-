using System;
using System.Collections.Generic;
using System.Dynamic;
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
    public class DoctorClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _usermanager;


        public DoctorClassesController(ApplicationDbContext context, UserManager<User> usermanager)
        {
            _usermanager = usermanager;
            _context = context;
            _usermanager = usermanager;
        }


        public IActionResult DoctorProfile()
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            if (userid == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            User user = _usermanager.FindByIdAsync(userid).Result;
            var doctor = _context.Doctors.Where(x => x.userID == userid);

            dynamic UUUser = new System.Dynamic.ExpandoObject(); ;

            UUUser.Ghaith = user;
            UUUser.SSS = doctor;



            return View(UUUser);
        }



        // GET: DoctorClasses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Doctors.Include(d => d.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DoctorClasses/Details/5
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            var doctorClass = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.userID == userid);
            if (doctorClass == null)
            {
                return NotFound();
            }

            return View(doctorClass);
        }

        public IActionResult DoctorProfile()
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            return View();
        }
        // GET: DoctorClasses/Create
        public IActionResult Create()
        {
            var userid = _usermanager.GetUserId(HttpContext.User);

           
            return View();
        }

        // POST: DoctorClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,AboutMe,pricePerSession")] DoctorClass doctorClass)
        {
            if (ModelState.IsValid)
            {
                doctorClass.userID = _usermanager.GetUserId(HttpContext.User);
                _context.Add(doctorClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", doctorClass.userID);
            return View(doctorClass);
        }

        // GET: DoctorClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorClass = await _context.Doctors.FindAsync(id);
            if (doctorClass == null)
            {
                return NotFound();
            }
            ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", doctorClass.userID);
            return View(doctorClass);
        }

        // POST: DoctorClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,AboutMe,pricePerSession,userID")] DoctorClass doctorClass)
        {
            if (id != doctorClass.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorClassExists(doctorClass.id))
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
            ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", doctorClass.userID);
            return View(doctorClass);
        }

        // GET: DoctorClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorClass = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.id == id);
            if (doctorClass == null)
            {
                return NotFound();
            }

            return View(doctorClass);
        }

        // POST: DoctorClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctorClass = await _context.Doctors.FindAsync(id);
            _context.Doctors.Remove(doctorClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult GetDoctorInfoForDashboardCard()
        {

            var userid = _usermanager.GetUserId(HttpContext.User);

            if (userid == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            User user = _usermanager.FindByIdAsync(userid).Result;

            return View(user);
        }
        private bool DoctorClassExists(int id)
        {
            return _context.Doctors.Any(e => e.id == id);
        }
    }
}
