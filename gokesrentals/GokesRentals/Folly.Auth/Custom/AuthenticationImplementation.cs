using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Folly;
using Folly.Auth;
using GokesRentals.Objects;

namespace GokesRentals.Folly.Auth.Custom
{
    public class AuthenticationImplementation : IAuthenticationImplementation
    {
        ///NOTE: This is a hack for this template. Don't actually do this, the credentials will be cleared 
        ///everytime the website is restarted. You should be storing these in a database instead
        //public List<FakeUser> FakeDBCredStore = new List<FakeUser>();

        public TimeSpan GetAuthTimeout()
        {
            return TimeSpan.FromDays(365);
        }

        /// <summary>
        /// 32-character random key. GENERATE YOUR OWN.
        /// You can run StringHelper.RandomString(32) on a one-off basis to generate one
        /// </summary>
        /// <returns></returns>
        public string GetEncryptionKey()
        {
            return "yRjFvg9keJCsgSfbOcctxaEqlZRJMoav";
        }

        public async Task<PermissionResult> CheckPermission(LoginToken token, string roles = null)
        {
            if (roles == null)
                return PermissionResult.Ok;

            Tenant user = null;
            Administrator admin = null;

            var id = int.Parse(token.ID);

            if(token.IDSource == "DBID" && token.Role.ToLower() == "member")
                user = Methods.Methods.GetTenant(id);

            if (token.IDSource == "DBID" && token.Role.ToLower() == "admin")
                admin = Methods.Methods.GetAdministrator(token.ID);

            if (user == null && admin == null)
                return PermissionResult.Invalid;

            if (!roles.ToLower().Contains("[" + token.Role.ToLower() + "]"))
                return PermissionResult.Denied;

            return PermissionResult.Ok;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IAuthUser> Authenticate(string email, string password,bool isAdmin)
        {
            if (isAdmin)
            {
                var admin = SQL.SQLStatements.GetAdminByEmail(email.ToLower());
                if (admin == null)
                    throw new UnauthorizedAccessException("There is no account with that email address.");

                if (!PasswordHash.ValidatePassword(password, admin.PasswordHash))
                    throw new UnauthorizedAccessException("Wrong Password.");

                AuthUser u = new AuthUser()
                {
                    ID = admin.ID.ToString(),
                    PasswordHash = admin.PasswordHash,
                    Role = admin.Role,
                };
                return u;
            }
            else
            {
                var user = SQL.SQLStatements.GetTenantByEmail(email.ToLower());
                if (user == null)
                    throw new UnauthorizedAccessException("There is no account with that email address.");

                if (!PasswordHash.ValidatePassword(password, user.PasswordHash))
                    throw new UnauthorizedAccessException("Wrong password.");

                AuthUser u = new AuthUser()
                {
                    ID = user.TenantID.ToString(),
                    PasswordHash = user.PasswordHash,
                    Role = user.Role,
                };

                return u;
            }

        }

        /// <summary>
        /// Assigns the user and ID and saves it to the "database"
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public async Task<IAuthUser> CreateAccount(IAuthUser u)
        {
            AuthUser user = (AuthUser)u;
            if (SQL.SQLStatements.ValidateTenantEmail(user.Email.ToLower()) == true)
            {
                throw new CreateAccountException("A user with this email address already exists!");
            }

            //if (FakeDBCredStore.Any(c => c.Email.ToLower() == user.Email.ToLower()))
            //{
            //    throw new CreateAccountException("A user with this email address already exists!");
            //}
            //user.ID = StringHelper.RandomString(32);

            var tenant = new Tenant()
            {
                EmailAddress = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role,
            };
            SQL.SQLStatements.AddTenant(tenant);
            //FakeDBCredStore.Add(user);

            return user;
        }
    }
}
