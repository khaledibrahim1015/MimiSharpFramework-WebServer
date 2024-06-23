using MimiSharp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MimiSharp.Routing
{

    /// <summary>
    /// Add routing functionality.
    /// Store the handlers for each route and method.
    /// 
    /// Manages route registration and handler retrieval.
    /// </summary>
    public class Router
    {
        //  method , { path , handler }
        private readonly Dictionary<string, Dictionary<string, Func<MimiContext, Task>>> _routes;


        /// <summary>
        /// Initializes a new instance of the Router class.
        /// </summary>
        public Router()
        {
            _routes = new Dictionary<string, Dictionary<string, Func<MimiContext, Task>>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Registers a route and its handler.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void RegisterRoute(string method, string path, Func<MimiContext, Task> handler)
        {
            if (!_routes.ContainsKey(method))
            {
                _routes[method] = new Dictionary<string, Func<MimiContext, Task>>(StringComparer.OrdinalIgnoreCase);
            }
            _routes[method][path] = handler;
        }

        /// <summary>
        /// Retrieves the handler for a given method and path.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The request path.</param>
        /// <param name="routeParams">The route parameters extracted from the path.</param>
        /// <returns>The handler function if found; otherwise, null.</returns>
        public Func<MimiContext, Task>? GetHandler(string method, string path, out Dictionary<string, string> routeParams)
        {
            routeParams = new Dictionary<string, string>();
            if (_routes.TryGetValue(method, out var routes))
            {
                foreach (var route in routes)
                {
                    var routeTemplate = "^" + Regex.Replace(route.Key, "{[^/]+}", "([^/]+)") + "$";
                    var match = Regex.Match(path, routeTemplate);
                    if (match.Success)
                    {
                        var paramNames = Regex.Matches(route.Key, "{([^/]+)}");
                        for (int i = 0; i < paramNames.Count; i++)
                        {
                            routeParams[paramNames[i].Groups[1].Value] = match.Groups[i + 1].Value;
                        }
                        return route.Value;
                    }
                }
            }
            return null;
        }

        //public Func<MimiContext, Task>? GetHandler(string method, string path)
        //{
        //    if (_routes.TryGetValue(method, out var routes) && routes.TryGetValue(path, out var handler))
        //    {
        //        return handler;
        //    }
        //    return null;
        //}







    }
}

