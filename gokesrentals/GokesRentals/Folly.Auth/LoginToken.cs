using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Folly.Auth
{
    public class LoginToken
    { 
        public LoginToken()
        {
            IsAPIToken = false;
        }

        public string IDSource { get; set; }
        public string ID { get; set; }
        public bool IsAPIToken { get; set; }
        public string Role { get; set; }
    }
}
