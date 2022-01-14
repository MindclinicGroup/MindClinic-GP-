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
using AspNetCoreHero.ToastNotification.Abstractions;
namespace MindClinic.Controllers
{
    public class MyProfileController : Controller
    {
        private readonly UserManager<User> _usermanager;
        private readonly IWebHostEnvironment _Host;
        private readonly INotyfService _notyf;
        public MyProfileController(UserManager<User> usermanager, IWebHostEnvironment Host, INotyfService notyf)
        {
            _usermanager = usermanager;
            _Host = Host;
            _notyf = notyf;
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
                else _notyf.Error("Wrong age input");
                 
            }
            if (user.PhoneNumber != phoneNumber) {
                bool isNumeric = int.TryParse(phoneNumber, out int n);
                if (isNumeric)
                    user.PhoneNumber = phoneNumber;
                else _notyf.Error("Wrong phone number input");
                
            }
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
                    _notyf.Success("Profile updated.");
                }
                catch
                {
                    
                }


               

            }
             return View(user);

        }
         [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {

            return View();

        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string? oldPassword, string? confirm, string? newPassword)
        {

            if (!newPassword.Equals(confirm) || newPassword == null || newPassword == null || oldPassword == null)
            {
                _notyf.Error("Passwords does not match!");
                return RedirectToAction("ChangePassword", "MyProfile");
            }
            else
            {

                var userid = _usermanager.GetUserId(HttpContext.User);
                if (userid == null)
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    var user = await _usermanager.FindByIdAsync(userid);

                    var result = _usermanager.ChangePasswordAsync(user, oldPassword, newPassword);
                    if (result.Result.Succeeded)
                    {
                        _notyf.Success("Password changed.");
                        
                    }
                    else
                    {
                        _notyf.Error("Old Password is wrong");
                       
                    }
                  
                }
              

            }
            return RedirectToAction("ChangePassword", "MyProfile");


        }
    }
}
