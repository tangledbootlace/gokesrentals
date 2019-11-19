using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Folly.Auth
{
    //public static class PermissionResult
    //{
    //    /// <summary>
    //    /// This user has permission to access this endpoint.
    //    /// </summary>
    //    /// <returns>OkResult</returns>
    //    public static IActionResult Ok()
    //    {
    //        return new OkResult();
    //    }

    //    /// <summary>
    //    /// User should not be able to access this endpoint.
    //    /// </summary>
    //    /// <param name="redirect">The page that the user should be redirected to. Setting to null (default) will show 401 page.</param>
    //    /// <returns>UnauthorizedRedirectResult if redirect is supplied. Throws UnauthorizedAccessException if no redirect.</returns>
    //    public static IActionResult Denied(string redirect = null)
    //    {
    //        if (redirect == null)
    //            throw new UnauthorizedAccessException();
    //        else
    //            return new UnauthorizedRedirectResult(redirect);
    //    }

    //    public static IAsyncResult InvalidUser()
    //    {

    //    }

    //    /// <summary>
    //    /// User should not be able to access this endpoint.
    //    /// </summary>
    //    /// <param name="redirect">The page that the user should be redirected to.</param>
    //    /// <returns>RedirectResult containing the supplied url</returns>
    //    public static IActionResult Redirect(string redirect)
    //    {
    //        return new RedirectResult(redirect);
    //    }
    //}

    public enum PermissionResult
    {
        Ok,
        Invalid,
        Denied
    }

    public class UnauthorizedRedirectResult : RedirectResult
    {
        public UnauthorizedRedirectResult(string url) : base( url )
        {

        }

        public UnauthorizedRedirectResult(string url, bool permanent) : base (url, permanent)
        {

        }

        public UnauthorizedRedirectResult(string url, bool permanent, bool preserveMethod) : base (url, permanent, preserveMethod)
        {

        }
    }
}
