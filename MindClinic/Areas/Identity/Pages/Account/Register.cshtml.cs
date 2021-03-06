using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MindClinic.Data;
using MindClinic.Models;

namespace MindClinic.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _Host;
        private readonly ApplicationDbContext _context;


        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IWebHostEnvironment Host,
             ApplicationDbContext context,


            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _Host = Host;
            _context = context;

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]

            [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
            ErrorMessage = "Minimum eight characters, at least one letter, one number and one special character.")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Gender { get; set; }
            [Required]
            public int Age { get; set; }
            [Required]
            public string Name { get; set; }

            public string role { get; set; }

            public IFormFile Imagefile { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            string RoleRole="";
           

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                if (Input.role == "ADMIN"){RoleRole = "1";}else if (Input.role == "DOCTOR"){ RoleRole = "2";}else if (Input.role == "PATIENT") { RoleRole = "3";} else if
                    (Input.role == "Secretary")
                {
                    RoleRole = "4";}


                var user = new User { UserName = Input.Email, Email = Input.Email, Age = Input.Age, Gender = Input.Gender, Name = Input.Name, ImageFile = Input.Imagefile, RoleId = RoleRole,EmailConfirmed=true };

              


                if (user.ImageFile != null)
                {
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
            




                var result = await _userManager.CreateAsync(user, Input.Password);



                var result2 = await _userManager.AddToRoleAsync(user, Input.role);



                if (result.Succeeded)
                {
                    if (Input.role == "DOCTOR")
                    {
                        var doctorClass = new DoctorClass();
                        doctorClass.AboutMe = "";
                        doctorClass.pricePerSession = 0;
                        doctorClass.userID = user.Id;

                        _context.Add(doctorClass);
                        await _context.SaveChangesAsync();
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    if(_signInManager.IsSignedIn(User) && User.IsInRole("ADMIN")) return RedirectToAction("Index","Home");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                 //   }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
