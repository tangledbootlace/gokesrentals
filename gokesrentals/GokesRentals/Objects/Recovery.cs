using GokesRentals.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Objects
{
    public class Recovery
    {
        public int userID { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }

        public bool IsExpired()
        {
            DateTime today = DateTime.UtcNow;
            if (ExpirationDate == null)
            {
                throw new RecoveryException("Invalid Expiration Date");
            }

            if (today > this.ExpirationDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
