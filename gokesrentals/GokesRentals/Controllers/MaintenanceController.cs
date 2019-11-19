using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using GokesRentals.Services;
using GokesRentals.Objects;
using GokesRentals.SQL;
using Folly.Auth;

namespace GokesRentals.Controllers
{
    [FollyAuthorize("[member]")]
    public class MaintenanceController : Controller
    {
        public IActionResult Index()
        {
            return View(); 
        }
        public IActionResult FetchMaitenanceRequest()
        {
            try
            {
                int tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);

                var activeRequest = new List<MaintenanceRequest>();
                var inactiveRequest = new List<MaintenanceRequest>();
                var tenant = Methods.Methods.GetTenant(tenantID);
                var Requests = Methods.Methods.RetrieveMaitenanceRequests(tenant.PropertyID);

                foreach (var request in Requests)
                {
                    if (request.ActiveRequest)
                    {
                        activeRequest.Add(request);
                    }
                    else
                    {
                        inactiveRequest.Add(request);
                    }
                }
                return Json(new { active = activeRequest, inactive = inactiveRequest });
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        //Submits maintenance request from the request pop up, this validates that 
        //both dialog boxes contain a string

        [HttpPost]
        public IActionResult MakeMaintenanceRequest(string summary, string description)
        {
                int TenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);

                MaintenanceRequest MaintenanceRequest = new MaintenanceRequest();

                if (summary != null && description != null)
                {
                    MaintenanceRequest = Methods.Methods.CreateMaintenanceRequest(TenantID, summary, description);

                    if (SQL.SQLStatements.AddMaintenanceRequest(MaintenanceRequest))
                    {
                        EmailService.SendEmail(MaintenanceRequest);
                        return Json(new { success = true });
                    }

                    return Json(new { success = false, error = "DB Error" });
                }
                else
                {
                    return Json(new { success = false, error = "Request must have a description and summary" });
                }
            
        }
    }
}