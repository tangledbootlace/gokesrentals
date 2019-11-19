using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GokesRentals.Methods;
using GokesRentals.Services;
using GokesRentals.Objects;
using Folly.Auth;

namespace GokesRentals.Controllers
{
    [FollyAuthorize("[member]")]
    public class PaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetInvoices()
        {
            try
            {
                var tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
                List<Invoice> paidInvoices = new List<Invoice>();
                List<Invoice> unPaidInvoices = new List<Invoice>();
                var tenant = Methods.Methods.GetTenant(tenantID);
                var invoices = Methods.Methods.GetInvoices(tenant.PropertyID);

                foreach (var invoice in invoices)
                {
                    if (invoice.PaidInFull)
                    {
                        paidInvoices.Add(invoice);
                    }
                    else
                    {
                        unPaidInvoices.Add(invoice);
                    }
                }

                return Json(new { success = true, paidInvoices = paidInvoices, unPaidInvoices = unPaidInvoices });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message });
            }

        }

        public IActionResult ViewInvoiceDetails(string invoiceNumber)
        {
            var invoiceNum = new Guid(invoiceNumber);
            var invoice = new Invoice();
            List<Payment> payments = new List<Payment>();

            try
            {
                invoice = InvoiceService.GetInvoiceByInvoiceNumber(invoiceNum);

                if (invoice == null)
                    throw new Exception("Could not retrieve invoice");

                payments = InvoiceService.GetAllPayemntsForInvoice(invoice.InvoiceID);

                return Json(new { success = true, data = payments });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message });
            }


        }

        [HttpPost]
        public IActionResult NewInvoicePayment(decimal payment, decimal fee, int invoiceID)
        {
            try
            {
                var tenantID = CookieHandler.GetCurrentUserID(Request.Cookies["AuthToken"]);
                InvoiceService.PayInvoice(invoiceID, tenantID, payment, fee);
                return Json(new { success = true});
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new { success = false, error = ex.Message });
            }
        }

        public IActionResult CalculateACHFee(decimal amount)
        {
            try
            {
                var fee = StripeService.CalculateTransactionFee(amount);
                return Json(new { success = true, data = fee });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message });
            }

        }
    }
}