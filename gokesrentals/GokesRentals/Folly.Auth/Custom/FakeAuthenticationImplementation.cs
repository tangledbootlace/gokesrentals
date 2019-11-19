using Folly.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Folly.Auth
{
    //TODO: Implement your own Auth Engine, similar to this
    public class FakeAuthenticationImplementation : IAuthenticationImplementation
    {
        ///NOTE: This is a hack for this template. Don't actually do this, the credentials will be cleared 
        ///everytime the website is restarted. You should be storing these in a database instead
        public List<FakeUser> FakeDBCredStore = new List<FakeUser>();

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
            //TODO: DEFINITELY change this.
            return "*GENERATEYOUROWNKEYANDPUTITHERE*";
        }

        public async Task<PermissionResult> CheckPermission(LoginToken token, string roles = null)
        {
            if (roles == null)
                return PermissionResult.Ok;

            FakeUser user = null;

            if (token.IDSource == "DBID")
                user = FakeDBCredStore.SingleOrDefault(c => c.ID == token.ID);

            if (user == null)
                return PermissionResult.Invalid;

            if (!roles.ToLower().Contains("[" + user.Role.ToLower() + "]"))
                return PermissionResult.Denied;

            return PermissionResult.Ok;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IAuthUser> Authenticate(string email, string password, bool isAdmin = false)
        {
            var user = FakeDBCredStore.SingleOrDefault(c => c.Email.ToLower() == email.ToLower());

            if (user == null)
                throw new UnauthorizedAccessException("There is no account with that email address."); ;

            if (!PasswordHash.ValidatePassword(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Wrong password.");

            return user;
        }

        /// <summary>
        /// Assigns the user and ID and saves it to the "database"
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public async Task<IAuthUser> CreateAccount(IAuthUser u)
        {
            FakeUser user = (FakeUser)u;

            if(FakeDBCredStore.Any(c => c.Email.ToLower() == user.Email.ToLower()))
            {
                throw new CreateAccountException("A user with this email address already exists!");
            }
            user.ID = StringHelper.RandomString(32);


            FakeDBCredStore.Add(user);

            return user;
        }
    }
}
