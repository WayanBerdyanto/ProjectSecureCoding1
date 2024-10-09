using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProjectSecureCoding1.Data;
using ProjectSecureCoding1.Models;
using ProjectSecureCoding1.ViewModels;
using BC = BCrypt.Net.BCrypt;


namespace ProjectSecureCoding1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUser _userData;

        public AccountController(IUser user)
        {
            _userData = user;
        }

        private Users GetCurrentUser()
        {
            var username = User.Identity.Name; // Use Name to get the logged-in user's username
            if (string.IsNullOrEmpty(username))
            {
                return null; // Return null if the user is not authenticated
            }

            // Retrieve the user from the database
            return _userData.GetUserByUsername(username);
        }

        public ActionResult Index()
        {
            return View("Login");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new Users
                    {
                        Username = loginViewModel.Username,
                        Password = loginViewModel.Password
                    };

                    var loginUser = _userData.Login(user);
                    if (loginUser == null)
                    {
                        ViewBag.Error = "Invalid login Attempted"; // Pesan kesalahan jika login gagal
                        return View(loginViewModel);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginUser.Username), // Pastikan menggunakan loginUser
                        new Claim(ClaimTypes.Role, loginUser.Role) // Pastikan role ditambahkan
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = loginViewModel.RememberLogin
                        });

                    // Login berhasil, redirect ke dashboard
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.Error = "ModelState not valid"; // Jika model tidak valid
                }
            }
            catch (Exception ex)
            {
                // Login gagal, tampilkan error
                ViewBag.Error = ex.Message;
            }
            return View(loginViewModel);
        }

        [HttpGet("register")]
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync(RegistrationViewModel registrationViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new Users
                    {
                        Username = registrationViewModel.Username,
                        Password = registrationViewModel.Password, // Pastikan ini password yang sudah di-hash
                        Role = "contributor"
                    };
                    var registerUser = _userData.Registration(user);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, registerUser.Username), // Pastikan menggunakan loginUser
                        new Claim(ClaimTypes.Role, registerUser.Role) // Pastikan role ditambahkan
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal
                        );

                    return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                // Registration failed, show error
                ViewBag.Error = ex.Message;
            }
            return View(registrationViewModel);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet("password")]
        public IActionResult ChangePassword()
        {
            return View("ChangePassword");
        }

        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // Assume you have a method to get the current user
            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                ViewBag.Error = "User not found. Please log in.";
                return RedirectToAction("Login");
            }

            if (!BC.Verify(model.CurrentPassword, currentUser.Password))
            {
                ViewBag.Error = "Current password is incorrect.";
                return View(model);
            }

            if (model.NewPassword.Length < 12)
            {
                ViewBag.Error = "Password must be at least 12 characters";
                return View(model);
            }

            // Update the user
            var updatedUser = new Users
            {
                Username = currentUser.Username,
                Password = model.NewPassword,
                Role = currentUser.Role
            };

            try
            {
                _userData.UpdateUser(updatedUser);
                ViewBag.SuccessMessage = "Password updated successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }

    }
}
