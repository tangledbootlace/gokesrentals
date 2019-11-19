using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Objects
{
    public class MaintenanceRequest
    {
        public MaintenanceRequest() { }
        public int TicketID { get; set; }
        public int TenantID { get; set; }
        public int PropertyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Cost { get; set; }
        public string OpenDate { get; set; }
        public string EndDate { get; set; }
        public bool ActiveRequest { get; set; }
    }
}
