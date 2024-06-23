using MimiSharp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MimiSharp.Middleware
{
    /// <summary>
    /// Middleware to handle authentication.
    /// </summary>
    public class AuthenticationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(MimiContext context, Func<Task> next)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                // Perform your authentication logic here
                await next();
            }
            else
            {
                context.Response.StatusCode = HttpStatusCode.Unauthorized;
                context.Response.StatusDescription = "Unauthorized";
                context.Response.SetBody(new { message = "Unauthorized" }, "application/json");
                await context.Response.SendAsync();
            }
        }
    }
}
