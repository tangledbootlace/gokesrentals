using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Objects
{
    public class Invoice
    {
           public int InvoiceID { get; set; }
           public Guid InvoiceNumber { get; set; }
           public decimal AmountDue { get; set; }
           public DateTime BillDate { get; set; }
           public DateTime DueDate { get; set; }
           public int PropertyID { get; set; }
           public bool PaidInFull { get; set; }
    }
}
