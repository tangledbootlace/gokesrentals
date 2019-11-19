using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Objects
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int InvoiceID { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Receipt { get; set; }
        public DateTime Date { get; set; }
        public int TenantId { get; set; }
    }
}
