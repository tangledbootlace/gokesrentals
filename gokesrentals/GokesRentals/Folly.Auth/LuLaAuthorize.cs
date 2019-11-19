using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Folly.Auth
{
    public class FollyAuthorize : TypeFilterAttribute
    {
        public FollyAuthorize(string roles = null, string unauthorizedRedirectURL = "/login") : base(typeof(FollyAuthorizeImplementation))
        {
            Arguments = new[] { roles, unauthorizedRedirectURL };
        }

        private class FollyAuthorizeImplementation : IAsyncAuthorizationFilter
        {
            //private LuLaDB DB;
            private string Roles { get; set; }
            private string UnauthorizedRedirectURL { get; set; }
            public FollyAuthorizeImplementation(string role, string unauthorizedRedirectURL)
            {
                Roles = role;
                UnauthorizedRedirectURL = unauthorizedRedirectURL;
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                if (context.Filters.Any(c => c is AllowAnonymousFilter))
                    return;

                var roles = Roles;
                var authFilter = context.ActionDescriptor.FilterDescriptors.Where(c => c.Filter is FollyAuthorize).OrderByDescending(c => c.Scope).FirstOrDefault();

                if(authFilter != null)
                {
                    roles = (string)((FollyAuthorize)authFilter.Filter).Arguments[0];
                }

                var Request = context.HttpContext.Request;
                var Response = context.HttpContext.Response;

                var cookie = Request.Cookies["AuthToken"];

                if (cookie == null)
                {
                    context.Result = new RedirectResult("/login?return=" + context.HttpContext.Request.Path);
                    return;
                }

                var token = JWT.Decode<JWT.Container<LoginToken>>(cookie);

                if (token == null || token.IsExpired)
                {
                    Response.Cookies.Delete("AuthToken");
                    context.Result = new RedirectResult("/login?return=" + context.HttpContext.Request.Path);
                    return;
                }

                if (Roles == null)
                    return;

                if (token.Data.IsAPIToken && !Roles.ToLower().Contains("[api]"))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var permission = await Auth.CheckPermission(token.Data, Roles);

                if(permission == PermissionResult.Invalid)
                {
                    Response.Cookies.Delete("AuthToken");
                    context.Result = new RedirectResult("/login?return=" + context.HttpContext.Request.Path);
                    return;
                }

                if (permission == PermissionResult.Denied)
                {
                    if(string.IsNullOrWhiteSpace(UnauthorizedRedirectURL))
                    {
                        throw new UnauthorizedAccessException();
                    }

                    context.Result = new RedirectResult(UnauthorizedRedirectURL);
                    return;
                }
            }
        }
    }
}
