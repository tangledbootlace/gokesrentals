using Folly.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Services
{
    public static class CookieHandler
    {
        public static int GetCurrentUserID(string cookie)
        {
            var decodedToken = JWT.Decode<JWT.Container<LoginToken>>(cookie);

            return int.Parse(decodedToken.Data.ID);
        }

        public static string GetTokenRole(string cookie)
        {
            var decodedToken = JWT.Decode<JWT.Container<LoginToken>>(cookie);

            return decodedToken.Data.Role;
        }
    }
}
