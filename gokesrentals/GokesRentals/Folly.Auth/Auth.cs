using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Folly.Auth
{
    public static class Auth
    {
        public static IAuthenticationImplementation Instance { get; set; }
        public static string ENCRYPTION_KEY
        {
            get
            {
                return Instance.GetEncryptionKey();
            }
        }

        public static Task<PermissionResult> CheckPermission(LoginToken token, string roles)
        {
            return Instance.CheckPermission(token, roles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task<IAuthUser> Authenticate(string email, string password, bool isAdmin = false)
        {
            return Instance.Authenticate(email, password, isAdmin);
        }

        public static async Task<IAuthUser> CreateAccount(IAuthUser user)
        {
            user = await Instance.CreateAccount(user);
            return user;
        }

        public static void CreateAuthCookie(this HttpResponse response, string id, string source, bool isAPIToken = false, string role = "member")
        {
            var container = new JWT.Container<LoginToken>(new LoginToken()
            {
                ID = id,
                IDSource = source,
                IsAPIToken = isAPIToken,
                Role = role
            }, TimeSpan.FromDays(365));

            response.Cookies.Append("AuthToken", JWT.Encode(container), new CookieOptions()
            {
                Expires = container.Expires
            });
        }

        public static void HashPassword(this IAuthUser user)
        {
            if(user.Password == null)
            {
                return;
            }

            user.PasswordHash = PasswordHash.CreateHash(user.Password);
            user.Password = null;
        }
    }
}
