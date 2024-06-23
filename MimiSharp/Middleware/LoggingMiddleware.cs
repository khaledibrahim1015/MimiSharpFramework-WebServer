using MimiSharp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimiSharp.Middleware
{
    public class LoggingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(MimiContext context, Func<Task> next)
        {
            Console.WriteLine($"[{DateTime.UtcNow}] {context.Request.Method} {context.Request.Path}");
            await next();
            Console.WriteLine($"[{DateTime.UtcNow}] {context.Response.StatusCode}");
        }
    }
}
