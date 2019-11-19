using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Folly.Auth
{
    public interface IAuthUser
    {
        string ID { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string PasswordHash { get; set; }
        string Role { get; set; }
    }
}
