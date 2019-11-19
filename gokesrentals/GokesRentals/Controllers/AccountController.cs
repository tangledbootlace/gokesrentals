using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using GokesRentals.Objects;
using GokesRentals.SQL;
using GokesRentals.Services;

using Stripe;
using Folly.Auth;

namespace GokesRentals.Controllers
{
    [FollyAuthorize("[member]")]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetWidgetInfo()
        {
            try
            {
                var tenantId = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
                Tenant CurrentTenant = Methods.Methods.GetTenant(tenantId);           
                Property FillWidget = SQL.SQLStatements.GetWidgetInfo(CurrentTenant.PropertyID); //hardcoded TenantID
                
                return Json(new { success = true, Data = FillWidget });
            }

            catch
            {
                return Json(new { success = false, error = "Bad TenantID" });
            }

        }

        [HttpPost]
        public IActionResult UpdatePersonalInfo(string firstName, string lastName, string emailAddress, string phoneNumber)        
        {


            var tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);

            if (firstName != null && lastName != null && emailAddress != null && phoneNumber != null)
            {
                PersonalInformation info = Methods.Methods.UserInputPersonalInformation(tenantID, firstName, lastName, phoneNumber, emailAddress);

                if (SQLStatements.UpdatePersonalInformation(info))
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, error ="DB Error" });
            }
            else
            {
                return Json(new { success = false, error = "Invalid URL input" });
            }

        }

        [HttpPost]
        public IActionResult UpdateBillingInfo(string StripeToken)
        {
            //FIRST: check if Stripe customer exists, 
            // If true: get stripe customer ID, send token, and store bank_id in DB
            // If false: Create stripe customer and send token (one call), then store bank_id in DB

            var tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
            var isTypeBank = true; //hardcoded type
            var isVerified = false;

            if (isTypeBank)
            {
                isVerified = false;
            }

            if (StripeToken == null)
                return Json(new { success = false, error = "No Stripe Token provided." });

            var currentTenant = Methods.Methods.GetTenant(tenantID);

            //Check if Stripe customer doesnt exist, create one. Else, update customer in Stripe.
            if (currentTenant.StripeID == "" || currentTenant.StripeID == null)
            {
                if (StripeService.CreateCustomer(currentTenant, StripeToken, isVerified))
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, error = "Could not create Stripe customer." });
                
            }
            else 
            {
                var tenant = Methods.Methods.GetTenant(tenantID);
                StripeService.UpdateCustomer(tenant.TenantID, tenant.StripeID, StripeToken, isVerified);

                return Json(new { success = true, isVerified = isVerified, hasBillingInfo = true });
            }
        }

        [HttpPost]
        public IActionResult VerifyBankAccount(string first, string second)
        {
            var tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
            var tenant = Methods.Methods.GetTenant(tenantID);

            var deposits = new List<string>();
            deposits.Add(first);
            deposits.Add(second);
            try
            {
                if (StripeService.VerifyBankAccount(tenant, deposits))
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = "Could not verify bank." });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message });
            }


        }

        public IActionResult GetPersonalInfo()
        {
            var tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
            var personalInfo = SQLStatements.GetPersonalInformation(tenantID);

            if (personalInfo != null)
            {
                return Json(new { success = true, data = personalInfo });
            }
            else
            {
                return Json(new { success = false, error = "No personal info found." });
            }
        }

        [HttpPost]
        public IActionResult GetPaymentInfo()
        {
            var tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
            var tenant = Methods.Methods.GetTenant(tenantID);

            //New tenants with no previous payment method
            if (tenant.StripeID == "")
            {
                return Json(new { success = false});
            }

            //Tenants who has a payment method on file, but needs to verify a bank account
            if (tenant.StripeIsVerified == false)
            {
                BankAccount bank = StripeService.GetBankAccount(tenant);
                return Json(new { success = true, isVerified = false, paymentInfo = bank });
            }
            
            //Tenant who has payment method on file, and ready to make payments.
            if (tenant.StripeIsVerified == true)
            {
                BankAccount bank = StripeService.GetBankAccount(tenant);
                return Json(new { success = true, isVerified = true, paymentInfo = bank });
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult UpdateEmploymentInfo(string employer, string jobTitle)
        {
            var tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
            EmploymentDetails details = new EmploymentDetails();

            details.TenantID = tenantID;
            details.Employer = employer;
            details.JobTitle = jobTitle;

            bool success = false;

            success = SQLStatements.UpdateEmploymentDetails(details);

            if(success)
            {
                return Json(new { success = true, });
            }
            else
            {
                return Json(new { success = false, error="An unknown error occured." });
            }
        }

        [HttpGet]
        public IActionResult GetEmploymentInfo()
        {
            var tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);

            var details = SQLStatements.GetEmploymentDetails(tenantID);

            return Json(new { success = true, data = details });
        }
    }
}