using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Folly.Auth
{
    public interface IAuthenticationImplementation
    {
        TimeSpan GetAuthTimeout();
        string GetEncryptionKey();
        Task<PermissionResult> CheckPermission(LoginToken token, string roles);
        Task<IAuthUser> Authenticate(string email, string password, bool isAdmin);
        Task<IAuthUser> CreateAccount(IAuthUser user);
    }
}
