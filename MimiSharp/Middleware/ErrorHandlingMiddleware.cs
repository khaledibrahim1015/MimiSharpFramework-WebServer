using MimiSharp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MimiSharp.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(MimiContext context, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Response.StatusDescription = "Internal Server Error";
                context.Response.SetBody(new { message = ex.Message }, "application/json");
                await context.Response.SendAsync();
            }
        }
    }
}
