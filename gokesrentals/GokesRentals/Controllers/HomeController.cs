using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GokesRentals.Models;
using Folly.Auth;
using GokesRentals.Services;

namespace GokesRentals.Controllers
{
    //[FollyAuthorize("[member]")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var cookie = Request.Cookies["AuthToken"];

            //Is Tenant already Authenticated?
            if (cookie != null && CookieHandler.GetTokenRole(cookie) == "member")
                return View();

            return View("~/Views/Home/login.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Unauthorized()
        {
            return View();
        }

        public IActionResult CountRequests()
        {
            try
            {
                var tenantId = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
                var tenant = Methods.Methods.GetTenant(tenantId);
                var Count = SQL.SQLStatements.CountMtnceRequests(tenant.PropertyID);
                return Json(new { success = true, data = Count });
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }
        public IActionResult GetLastPayment()
        {
            var tenantId = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
            var tenant = Methods.Methods.GetTenant(tenantId);

            if (tenant == null || tenant.PropertyID == 10 || tenant.PropertyID == 0)
                return Json(new { success = false, error = "invalid tenant" });

            var payment = SQL.SQLStatements.GetLastPayment(tenant.TenantID);

            return Json(new { success = true, data = payment });
        }
    }
}
