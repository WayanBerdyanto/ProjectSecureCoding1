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

namespace ProjectSecureCoding1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUser _userData;

        public AccountController(IUser user)
        {
            _userData = user;
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
        public ActionResult Register(RegistrationViewModel registrationViewModel)
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
                    _userData.Registration(user);
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

    }
}
