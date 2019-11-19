using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Folly.Auth;
using GokesRentals.Services;
using GokesRentals.Objects;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationTemplate.Controllers
{
    public class AuthController : Controller
    {
        //Tenant Auth Methods
        [HttpGet("~/login")]
        public IActionResult Login()
        {
            var cookie = Request.Cookies["AuthToken"];

            //Is Tenant already authenticated?
            if ( cookie != null && CookieHandler.GetTokenRole(cookie) == "member")
                return Redirect("~/");

            return View("~/Views/Home/login.cshtml");
        }

        [HttpPost("~/login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            IAuthUser user = null;

            //TODO check if null

            try
            {
                user = await Auth.Authenticate(email, password);

            }
            catch(UnauthorizedAccessException ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }

            Response.CreateAuthCookie(user.ID, "DBID");

            return Json(new
            {
                success = true,
                redirect = "/"
            });
        }

        [HttpGet("~/logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return Redirect("/login");
        }

        //[HttpGet("~/signup")]
        //public IActionResult Signup()
        //{
        //    return View("~/Views/Auth/Signup.cshtml");
        //}

        [HttpGet("~/signup")]
        public async Task<IActionResult> Signup(string email, string password)
        {

            IAuthUser user = new AuthUser()
            {
                Email = email,
                Password = password,
                Role = "member"
            };

            user.HashPassword(); //do this before saving to DB

            try
            {
                user = await Auth.CreateAccount(user);
            }
            catch(CreateAccountException ex)
            {

                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = "An unknown error occurred. Please try again later."
                });
            }

            Response.CreateAuthCookie(user.ID, "DBID");

            return Json(new
            {
                success = true,
                redirect = "/dashboard"
            });
        }
        [HttpGet("~/reset-password")]
        public IActionResult Reset(string token)
        {
            try
            {
                var user = new Tenant();
                var tokenIsValid = AccountRecoveryService.ValidateResetToken(token);
                if (tokenIsValid)
                {
                    return View("~/Views/Auth/ResetPassword.cshtml");
                }

                //If invalid token, prompt user for email.
                return View("~/Views/Auth/RequestToken.cshtml");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost("~/reset-password")]
        public IActionResult Reset(string token, string password)
        {
            bool success = false;
            try
            {
                success = AccountRecoveryService.RedeemResetToken(password, token);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = "An unknown error occured"
                });
            }

            if (success)
            {
                return Json(new { success = true, redirect = "/" });
            }

            return Json(new
            {
                success = false,
                error = "An unknown error occured"
            });
        }

        [HttpPost("~/request-token")]
        public IActionResult RequestToken(string email)
        {
            try
            {
                AccountRecoveryService.RequestResetToken(email.ToLower());
                return Json(new { success = true, data = "Success! Check your inbox for further instructions." });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message });
            }

        }

        //Admin Auth Methods
        [HttpGet("~/admin/login")]
        public IActionResult AdminLogin()
        {
            var cookie = Request.Cookies["AuthToken"];

            //Is Admin already authenticated?
            if (cookie != null && CookieHandler.GetTokenRole(cookie) == "admin")
                return Redirect("~/admin/");

            return View("~/Views/Admin/login.cshtml");
        }
        
        [HttpPost("~/admin/login")]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            IAuthUser user = null;
            bool isAdmin = true;

            //TODO check if null

            try
            {
                user = await Auth.Authenticate(email, password, isAdmin);

            }
            catch (UnauthorizedAccessException ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }

            Response.CreateAuthCookie(user.ID, "DBID", false, "admin");

            return Json(new
            {
                success = true,
                redirect = "/admin"
            });
        }

        [HttpGet("~/admin/reset-password")]
        public IActionResult ResetAdmin(string token)
        {
            try
            {
                var user = new Tenant();
                var tokenIsValid = AccountRecoveryService.ValidateResetToken(token, true);
                if (tokenIsValid)
                {
                    return View("~/Views/Auth/ResetAdminPassword.cshtml");
                }

                //If invalid token, prompt user for email.
                return View("~/Views/Auth/RequestAdminToken.cshtml");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost("~/admin/reset-password")]
        public IActionResult ResetAdmin(string token, string password)
        {
            bool success = false;
            try
            {
                success = AccountRecoveryService.RedeemResetToken(password, token, true);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = "An unknown error occured"
                });
            }

            if (success)
            {
                return Json(new { success = true, redirect = "/admin/login" });
            }

            return Json(new
            {
                success = false,
                error = "An unknown error occured"
            });
        }

        [HttpPost("~/admin/request-token")]
        public IActionResult RequestAdminToken(string email)
        {
            try
            {
                AccountRecoveryService.RequestResetToken(email.ToLower(), true);
                return Json(new { success = true, data = "Success! Check your inbox for further instructions." });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message });
            }

        }
    }
}