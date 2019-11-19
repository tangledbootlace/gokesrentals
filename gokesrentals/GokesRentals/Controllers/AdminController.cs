using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Folly.Auth;
using GokesRentals.Objects;
using Microsoft.AspNetCore.Mvc;

namespace GokesRentals.Controllers
{
    [FollyAuthorize("[admin]")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Tennants()
        {
            return View();
        }
        public IActionResult FetchAllTenants()
        {
            try
            {
                var Tenants = Methods.Methods.RetrieveAllTenants();
                return Json(new { success = true, data = Tenants });
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }
        //Allows admins to create new tenants for the portal, password is set as default
        [HttpPost]
        public IActionResult MakeTenant(string firstName, string lastName, int propertyID, string emailAddress, string phoneNumber, string contract)
        {
            Tenant tenant = new Tenant();

            if (firstName != null && lastName != null && propertyID > 0 && emailAddress != null && phoneNumber != null)
            {
                tenant = Methods.Methods.CreateTenantClass(firstName, lastName, propertyID, emailAddress.ToLower(), phoneNumber, contract);
                var alreadyRegistered = SQL.SQLStatements.ValidateTenantEmail(emailAddress);

                if (alreadyRegistered)
                    return Json(new { success = false, error = "A tenant with this email address already exists!" });

                tenant.PasswordHash = PasswordHash.CreateHash("changeme123");
                if (SQL.SQLStatements.AddTenant(tenant))
                    return Json(new { success = true });

                return Json(new { success = false, error = "DB Error" });
            }
            else
            {
                return Json(new { success = false, error = "Required fields are missing." });
            }

        }

        [HttpPost]
        public IActionResult EditTenant(string firstName, string lastName, int propertyID, string emailAddress, string phoneNumber, string contractLink, string tenantId)
        {
            var tenant = new Tenant()
            {
                FirstName = firstName,
                LastName = lastName,
                PropertyID = propertyID,
                EmailAddress = emailAddress,
                PhoneNumber = phoneNumber,
                ContractLink = contractLink,
                TenantID = int.Parse(tenantId)
            };
            try
            {
                var success = SQL.SQLStatements.SaveTenantByAdministrator(tenant);
                if (success)
                    return Json(new { success = true });

                return Json(new { success = false, error = "DB error" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "An error occured." });
            }
        }

        [HttpPost]
        public IActionResult DeleteTenant(int tenantId)
        {
            try
            {
                var success = SQL.SQLStatements.DeleteTenant(tenantId);

                if (success)
                    return Json(new { success = true });

                return Json(new { success = false, error = "DB error" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }


        }
        public IActionResult Properties()
        {
            return View();
        }
        public IActionResult FetchAllProperties()
        {
            try
            {
                var Properties = Methods.Methods.RetrieveAllProperties();
                return Json(new { success = true, data = Properties });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public IActionResult MakeProperty(string address1, string address2, string city, string state, int zipcode, decimal? propertyValue, decimal? rentCharge)
        {
            Property property = new Property();

            if (address1 != null && address2 != null && city != null && state != null && propertyValue != null && rentCharge != null)
            {
                property = Methods.Methods.CreatePropertyClass(address1, address2, city, state, zipcode, propertyValue, rentCharge);

                if (SQL.SQLStatements.AddProperty(property))
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, error = "DB Error" });
            }
            else
            {
                return Json(new { success = false, error = "Property must have the listed information" });
            }

        }
        [HttpPost]
        public IActionResult EditProperty(string address1, string address2, int propertyId, string city, string state, int zipcode, decimal propertyValue, decimal rentCharge)
        {

            var property = new Property()
            {
                Address1 = address1,
                Address2 = address2,
                PropertyID = propertyId,
                City = city,
                State = state,
                Zipcode = zipcode,
                PropertyValue = propertyValue,
                RentCharge = rentCharge,

            };
            try
            {
                var success = SQL.SQLStatements.SavePropertyByAdministrator(property);
                if (success)
                    return Json(new { success = true });

                return Json(new { success = false, error = "DB error" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "An error occured." });
            }
        }

        [HttpPost]
        public IActionResult DeleteProperty(int propertyId)
        {
            try
            {
                //Dont allow system property to be deleted
                if (propertyId == 10)
                    return Json(new { success = false, error = "This property cannot be deleted." });

                if (SQL.SQLStatements.DoesPropertyHaveTenants(propertyId))
                    return Json(new { success = false, error = "There are tenants associated with this property. Please re-assign or delete tenants and try again." });



                if (SQL.SQLStatements.ReassignMaintenanceTickets(propertyId) && SQL.SQLStatements.ReassignInvoices(propertyId) && SQL.SQLStatements.DeleteProperty(propertyId))
                    return Json(new { success = true });

                return Json(new { success = false, error = "DB error" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message.ToString() });
            }
            
        }
        public IActionResult Maintenance()
        {
            return View();
        }
        public IActionResult FetchAllMaitenanceRequest()
        {
            try
            {

                var activeRequest = new List<MaintenanceRequest>();
                var inactiveRequest = new List<MaintenanceRequest>();
                var Requests = Methods.Methods.RetrieveAllMaitenanceRequests();

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
        [HttpPost]
        public IActionResult CloseMaitenanceRequest(int ticketID, decimal cost)
        {
            var TicketID = ticketID;

            if (ticketID != 0 && cost >= 0)
            {
                if (SQL.SQLStatements.CloseMaitenanceRequest(ticketID, cost))
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, error = "DB Error" });
            }
            else
            {
                return Json(new { success = false, error = "Request must have a cost to close" });
            }
        }
        [HttpPost]
        public IActionResult GetReport(int propertyID)
        {
            try
            {
                var ID = propertyID;
                var expense = SQL.SQLStatements.GetPropertyExpense(ID);
                var revenue = SQL.SQLStatements.GetPropertyRevenue(ID);

                return Json(new { success = true, expense = expense, revenue = revenue });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "DB Error" });
            }

        }
    }
}