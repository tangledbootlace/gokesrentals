using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Folly.Auth
{
    public class CreateAccountException : Exception
    {
        public CreateAccountException() : base()
        {

        }

        public CreateAccountException(string message) : base(message)
        {

        }
    }
}
