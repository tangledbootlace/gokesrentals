using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Folly.Auth;

namespace Folly.Auth
{
    public class AuthUser : IAuthUser
    {
        public string ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

    }
}
