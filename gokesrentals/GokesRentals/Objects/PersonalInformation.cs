using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Objects
{
    public class PersonalInformation
    {
        public PersonalInformation() { }
        public int TenantID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
