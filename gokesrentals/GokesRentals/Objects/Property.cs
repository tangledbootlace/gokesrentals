using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Objects
{
    public class Property
    {
        public int PropertyID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zipcode { get; set; }
        public decimal PropertyValue { get; set; }
        public decimal RentCharge { get; set; }
    }
}
