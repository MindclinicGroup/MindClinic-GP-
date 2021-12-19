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


        //public IActionResult DoctorProfile()
        //{
        //    var userid = _usermanager.GetUserId(HttpContext.User);
        //    if (userid == null)
        //    {
        //        return RedirectToPage("/Account/Login", new { area = "Identity" });
        //    }
        //    User user = _usermanager.FindByIdAsync(userid).Result;
        //    var doctor = _context.Doctors.Where(x => x.userID == userid);

        //    dynamic UUUser = new System.Dynamic.ExpandoObject(); ;

        //    UUUser.Ghaith = user;
        //    UUUser.SSS = doctor;



        //    return View(UUUser);
        //}



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
        [HttpGet]
        public async Task<IActionResult> DoctorEducation()
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            
            try
            {
                var doctorClass = _context.Doctors.Where(x => x.userID == userid).First();
                var education = _context.Educations.Where(x => x.doctorId == doctorClass.id);
                return View(education);
            }
            catch
            {
                var doctorClass = _context.Doctors.Where(x => x.userID == userid).First();
                var education = new Education();
                education.doctorId = doctorClass.id;
                education.Degree = "";
                education.College = "";
                education.yearOfCompletion = "";

                _context.Add(education);
                await _context.SaveChangesAsync();
                return View(education);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorEducation(int id, [Bind("id,Degree,College,yearOfCompletion")] Education educationUpdate)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            var doctor = _context.Doctors.Where(x => x.userID == userid).First();
            var education = _context.Educations.Where(x => x.doctorId == doctor.id && x.id == id).First();
            education.Degree = educationUpdate.Degree;
            education.College = educationUpdate.College;
            education.yearOfCompletion = educationUpdate.yearOfCompletion;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(education);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorClassExists(educationUpdate.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(DoctorProfile));
            }
      //      ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", doctorClass.userID);
            return View(educationUpdate);
        }

        // GET: /DoctorClasses/DoctorProfile
        public async Task<IActionResult> DoctorProfile()
        {
            var userid = _usermanager.GetUserId(HttpContext.User);

            try
            {
                var doctorClass = _context.Doctors.Where(x => x.userID == userid).First();
                return View(doctorClass);
            }
            catch
            {

                var doctorClass = new DoctorClass();
                doctorClass.AboutMe = "";
                doctorClass.pricePerSession = 0;
                doctorClass.userID = userid;
              
                _context.Add(doctorClass);
                await _context.SaveChangesAsync();
                //
                var doctor = _context.Doctors.Where(x => x.userID == userid).First();
                var education = new Education { Degree="",College="",doctorId=doctor.id,yearOfCompletion=""};
                //education.Degree = "";
                //education.College = "";
                //education.yearOfCompletion = "";
                //education.doctorId = doctor.id;
                // education.doctorId = doctorClass.id;
                _context.Add(education);
                await _context.SaveChangesAsync();

                return View(doctorClass);
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorProfile(int id, [Bind("id,AboutMe,pricePerSession,userID")] DoctorClass doctorClass)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            var doctor = _context.Doctors.Where(x => x.userID == userid).First();
            doctor.AboutMe = doctorClass.AboutMe;
            doctor.pricePerSession = doctorClass.pricePerSession;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
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
                return RedirectToAction(nameof(DoctorProfile));
            }
            ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", doctorClass.userID);
            return View(doctorClass);
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
        public async Task<IActionResult> Edit(string doctorId)
        {
            if (doctorId == null)
            {
                return NotFound();
            }

            var doctorClass = _context.Doctors.Where(x => x.userID == doctorId).First();
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
