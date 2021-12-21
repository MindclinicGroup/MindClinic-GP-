using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MindClinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MindClinic.Controllers
{
    public class MyProfileController : Controller
    {
        private readonly UserManager<User> _usermanager;
        private readonly IWebHostEnvironment _Host;

        public MyProfileController(UserManager<User> usermanager, IWebHostEnvironment Host)
        {
            _usermanager = usermanager;
            _Host = Host;
        }
        public IActionResult Index()
        {

            var userid = _usermanager.GetUserId(HttpContext.User);

            if (userid == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            User user = _usermanager.FindByIdAsync(userid).Result;

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Index(int age, string phoneNumber, IFormFile Img)
        {
            var userid = _usermanager.GetUserId(HttpContext.User);

            if (userid == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            User user = _usermanager.FindByIdAsync(userid).Result;
            if (user.Age != age)
            {
                if (age > 0) user.Age = age;
            }
            if (user.PhoneNumber != phoneNumber) user.PhoneNumber = phoneNumber;
            if (Img != null)
            {

                user.ImageFile = Img;
                string wwwRootPath = _Host.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + user.ImageFile.FileName;
                string extension = Path.GetExtension(user.ImageFile.FileName);
                string path = Path.Combine(wwwRootPath + "/Doctorsimage/" + fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await user.ImageFile.CopyToAsync(fileStream);
                }
                user.image = fileName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _usermanager.UpdateAsync(user);

                }
                catch
                {

                }


               

            }
             return View(user);

        }
    }
}
