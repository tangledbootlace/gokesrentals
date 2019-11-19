using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GokesInvoicer.Objects
{
    public class Tenant
    {
        public Tenant() { }
        public int TenantID { get; set; }
        public int PropertyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string PaymentPreference { get; set; }
        public string StripeID { get; set; }
        public bool StripeIsVerified { get; set; }
        public string ContractLink { get; set; }
        public string Employer { get; set; }
        public string JobTitle { get; set; }
    }
}
