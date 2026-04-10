//using Microsoft.AspNetCore.Http;

//namespace VirtualClassroom.Web.Middleware
//{
//    public class SessionMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public SessionMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            var path = context.Request.Path.Value;

//            // Allow public pages
//            if (path.Contains("/Account/Login") ||
//                path.Contains("/Account/Register") ||
//                path.Contains("/Account/GoogleLogin") ||
//                path.Contains("/Account/GoogleResponse") ||
//                path.Contains("/Account/SelectRole"))
//            {
//                await _next(context);
//                return;
//            }

//            var userId = context.Session.GetInt32("UserId");

//            if (userId == null)
//            {
//                context.Response.Redirect("/Account/Login");
//                return;
//            }

//            await _next(context);
//        }
//    }
//}

using Microsoft.AspNetCore.Http;

namespace VirtualClassroom.Web.Middleware
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // ✅ Public pages
            if (path.StartsWith("/account/login") ||
                path.StartsWith("/account/register") ||
                path.StartsWith("/account/googlelogin") ||
                path.StartsWith("/account/googleresponse") ||
                path.StartsWith("/account/selectrole") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/lib"))
            {
                await _next(context);
                return;
            }

            var userId = context.Session.GetInt32("UserId");

            if (userId == null)
            {
                context.Response.Redirect("/Account/Login");
                return;
            }

            await _next(context);
        }
    }
}