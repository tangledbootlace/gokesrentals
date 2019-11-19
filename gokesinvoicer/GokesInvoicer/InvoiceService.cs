using GokesInvoicer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GokesInvoicer
{
    public static class InvoiceService
    {
        public static void Run()
        {
            Discord.AlertDiscord("Starting...");

            List<int> invoiceablePropertyIDs = GetInvoicableProperties();
            Discord.AlertDiscord(invoiceablePropertyIDs.Count.ToString() + " properties need invoicing.");

            if (invoiceablePropertyIDs.Count > 0)
            {
                CreateInvoice(invoiceablePropertyIDs);
            }
            
        }

        public static void CreateInvoice(List<int> propertyIDs)
        {
            List<Property> properties = GetPropertyModels(propertyIDs);
            var UtcNow = DateTime.UtcNow;

            foreach (var property in properties)
            {
                var invoice = new Invoice();
                invoice.InvoiceNumber = Guid.NewGuid();
                invoice.AmountDue = property.RentCharge;
                invoice.BillDate = DateTime.UtcNow;
                invoice.DueDate = UtcNow.AddDays(5);
                invoice.PropertyID = property.PropertyID;
                invoice.PaidInFull = false;

                SQL.CreateInvoice(invoice);
            }

        }

        private static List<int> GetInvoicableProperties()
        {
            var tenants = new List <Tenant>();
            var propertyIDs = new List<int>();
            var currentMonthsInvoices = new List<Invoice>();

            DateTime firstDate = new DateTime(),
            lastDate = new DateTime(),
            now = DateTime.UtcNow;

            /*Gets all Tenants where Property IS NOT NULL*/
            tenants = SQL.GetTenantsWithProperty();

            /*Extract PropertyIDs, remove duplicates*/
            var distinctProperties = tenants.Select(x => x.PropertyID).Distinct();
            propertyIDs = distinctProperties.ToList();

            /*Calculate the first and last date of a the current month*/
            var daysInCurrentMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var daysToSubtract = 0 - (now.Day - 1);
            firstDate = now.AddDays(daysToSubtract);
            lastDate = firstDate.AddDays(daysInCurrentMonth);

            firstDate = GetDateZeroTime(firstDate);
            lastDate = GetDateZeroTime(lastDate);


            /*If invoice exists for current month, remove property from list*/
            currentMonthsInvoices = SQL.GetInvoicesBetweenDates(firstDate, lastDate);

            if (currentMonthsInvoices != null)
            {
                return RemovePropertiesFromList(propertyIDs, currentMonthsInvoices);
            }
            return propertyIDs;


        }
        private static List<int> RemovePropertiesFromList(List<int> propertyIDs, List<Invoice> currentMonthsInvoices)
        {
            var propertyIDsToExclude = new List<int>();

            foreach (var propertyID in propertyIDs)
            {
                foreach (var invoice in currentMonthsInvoices)
                {
                    if (invoice.PropertyID == propertyID)
                    {
                        propertyIDsToExclude.Add(propertyID);
                    }
                }

            }

            foreach (var propertyID in propertyIDsToExclude)
            {
                propertyIDs.Remove(propertyID);
            }

            return propertyIDs;
        }
        private static DateTime GetDateZeroTime(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        private static List<Property> GetPropertyModels(List<int> propertyIDs)
        {
            List<Property> properties = new List<Property>();
            foreach (var ID in propertyIDs)
            {
                var p = SQL.GetProperty(ID);
                properties.Add(p);
            }
            return properties;
        }
    }

}
