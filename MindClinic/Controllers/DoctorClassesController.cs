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


       



        // GET: DoctorClasses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Doctors.Include(d => d.User);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> DoctorViewProfile(string id)
        {
            var user = _usermanager.Users.Where(x => x.Id == id).First();
            return View(user);
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
        public async Task<IActionResult> DoctorEducation(string?create)
        {

            var userid = _usermanager.GetUserId(HttpContext.User);
            var doctorClass = _context.Doctors.Where(x => x.userID == userid).First();
            var educations = _context.Educations.Where(x => x.doctorId == doctorClass.id);
            try
            {
                if (create != null)
                {
                    if (create.Equals("true"))
                    {
                        var education = new Education();
                        education.doctorId = doctorClass.id;
                        education.Degree = "";
                        education.College = "";
                        education.yearOfCompletion = "";
                        _context.Add(education);
                        await _context.SaveChangesAsync();
                        educations = _context.Educations.Where(x => x.doctorId == doctorClass.id);
                        return View(educations);
                    }
                }       
                if (educations.Any())
                {
                    return View(educations);
                }
                else 
                {
                        var education = new Education();
                        education.doctorId = doctorClass.id;
                        education.Degree = "";
                        education.College = "";
                        education.yearOfCompletion = "";
                        _context.Add(education);       
                    await _context.SaveChangesAsync();
                   
                }




            }
            catch
            {
              
              
            }
            return null;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorEducation(int id, string Degree,string College,string yearOfCompletion,string? delete)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            var doctor = _context.Doctors.Where(x => x.userID == userid).First();
            var education = _context.Educations.Where(x => x.doctorId == doctor.id && x.id == id).First();

            if (delete != null)
            {
                if (delete.Equals("true"))
                {
                   
                    _context.Educations.Remove(education);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DoctorProfile));
                }
            }
            else
            {

              
                if(Degree!=null)education.Degree = Degree;
               if(College!=null) education.College = College;
               if(yearOfCompletion!=null) education.yearOfCompletion = yearOfCompletion;
                _context.Update(education);
                await _context.SaveChangesAsync();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorClassExists(education.id))
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
         //   ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", doctorClass.userID);
            return View();
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
               // var education = new Education { Degree="",College="",doctorId=doctor.id,yearOfCompletion=""};
                //education.Degree = "";
                //education.College = "";
                //education.yearOfCompletion = "";
                //education.doctorId = doctor.id;
                // education.doctorId = doctorClass.id;
            //    _context.Add(education);
            //    await _context.SaveChangesAsync();

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

        [HttpGet]
        public async Task<IActionResult> DoctorAwards(string?create)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            var doctorClass = _context.Doctors.Where(x => x.userID == userid).First();
            var awards = _context.Awards.Where(x => x.doctorId == doctorClass.id);

            if (create != null)
            {
                if (create.Equals("true"))
                {
                    var award = new Awards();
                    award.doctorId = doctorClass.id;
                    award.award = "";
                    award.Year = "";
                    _context.Add(award);
                    await _context.SaveChangesAsync();
                    awards = _context.Awards.Where(x => x.doctorId == doctorClass.id);
                    return View(awards);
                }
            }
            try
            {
               
                if (awards.Any())
                {
                    return View(awards);
                }
                else
                {
                   
                        var award = new Awards();
                        award.doctorId = doctorClass.id;
                        award.award = "";
                        award.Year = "";
                        _context.Add(award);
                    
                    await _context.SaveChangesAsync();
                   
                    awards = _context.Awards.Where(x => x.doctorId == doctorClass.id);
                    return View(awards);
                }
            }
            catch(Exception e)
            {


            }
            return null;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorAwards(int id, string award, string year,string?delete)
        {

            var userid = _usermanager.GetUserId(HttpContext.User);
            var doctor = _context.Doctors.Where(x => x.userID == userid).First();
            var Award = _context.Awards.Where(x => x.doctorId == doctor.id && x.id == id).First();
            if (delete != null)
            {
                if (delete.Equals("true"))
                {

                    _context.Awards.Remove(Award);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DoctorProfile));
                }
            }
            else
            {
                if (award != null) Award.award = award;
                if (year != null) Award.Year = year;
                _context.Update(Award);
                await _context.SaveChangesAsync();
            }

            if (ModelState.IsValid)
            {
                try
                {
                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorClassExists(Award.id))
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
            //   ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", doctorClass.userID);
            return View();
        }

        private bool DoctorClassExists(int id)
        {
            return _context.Doctors.Any(e => e.id == id);
        }
        [HttpGet]
        public string getAboutMe(string id) 
        {
            
            return _context.Doctors.Where(x => x.userID == id).First().AboutMe;
        }
        [HttpGet]
        public string getPrice(string id)
        {

            return _context.Doctors.Where(x => x.userID == id).First().pricePerSession.ToString();
        }
        [HttpGet]
        public async Task<IActionResult> getEducation(string id)
        {
            var doctor = _context.Doctors.Where(x => x.userID == id).First();
            return View(_context.Educations.Where(x => x.doctorId == doctor.id).ToList()) ;

        }
        [HttpGet]
        public async Task<IActionResult> getAwards(string id)
        {
            var doctor = _context.Doctors.Where(x => x.userID == id).First();
            return View(_context.Awards.Where(x => x.doctorId == doctor.id).ToList());

        }


        [HttpGet]
        public IActionResult Reviews(string ?id)
        {


            var reviews = _context.Reviews.Where(x => x.DoctorUserId == id).First();

            return View(reviews);
        }


        public async Task<IActionResult> CreateReview(string DoctorId, string txt)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);
            var appointment = _context.Appointments.Where(x => x.patientId == userid && x.doctorId == DoctorId);


            if (appointment.Any())
            {
                var Review = new Reviews
            {
                WriterUserId = userid,
                DoctorUserId = DoctorId,
                Text = txt,
                TimeOfReview = DateTime.Now,
            };

            _context.Add(Review);
            await _context.SaveChangesAsync();

            return View();
            }

            else
            {

                return View();
            }
        }
    }
}
