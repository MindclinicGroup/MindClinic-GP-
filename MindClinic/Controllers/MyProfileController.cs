using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MindClinic.Models;

namespace MindClinic.Controllers
{
    public class MyProfileController : Controller
    {
        private readonly UserManager<User> _usermanager;

        public MyProfileController(UserManager<User> usermanager)
        {
            _usermanager = usermanager;
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

    }
}
