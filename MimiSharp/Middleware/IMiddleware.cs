using MimiSharp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimiSharp.Middleware
{
    /// <summary>
    /// Defines the interface for middleware components.
    /// </summary>
    public interface IMiddleware
    {
        /// <summary>
        /// Method to invoke the middleware.
        /// </summary>
        /// <param name="context">The HTTP context containing the request and response.</param>
        /// <param name="next">The delegate for the next middleware in the pipeline.</param>
        Task InvokeAsync(MimiContext context, Func<Task> next);

    }
}
