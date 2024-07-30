using MimiSharp.Context;
using MimiSharp.Middleware;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MimiSharp
{
    /// <summary>
    /// Manages route registration and handler retrieval.
    /// </summary>
    public class Router
    {
        private readonly Dictionary<string, Dictionary<string, Func<MimiContext, Task>>> _routes;
        private readonly Dictionary<string, Dictionary<string, List<IMiddleware>>> _routeMiddlewares;

        /// <summary>
        /// Initializes a new instance of the Router class.
        /// </summary>
        public Router()
        {
            _routes = new Dictionary<string, Dictionary<string, Func<MimiContext, Task>>>(StringComparer.OrdinalIgnoreCase);
            _routeMiddlewares = new Dictionary<string, Dictionary<string, List<IMiddleware>>>(StringComparer.OrdinalIgnoreCase);
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
                _routeMiddlewares[method] = new Dictionary<string, List<IMiddleware>>(StringComparer.OrdinalIgnoreCase);
            }
            _routes[method][path] = handler;
        }

        /// <summary>
        /// Registers middleware for a specific route.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The route path.</param>
        /// <param name="middleware">The middleware to register.</param>
        public void RegisterRouteMiddleware(string method, string path, IMiddleware middleware)
        {
            if (!_routeMiddlewares.ContainsKey(method) || !_routeMiddlewares[method].ContainsKey(path))
            {
                _routeMiddlewares[method][path] = new List<IMiddleware>();
            }
            _routeMiddlewares[method][path].Add(middleware);
        }

        /// <summary>
        /// Retrieves the handler for a given method and path.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The request path.</param>
        /// <param name="routeParams">The route parameters extracted from the path.</param>
        /// <returns>The handler function if found; otherwise, null.</returns>
        public Func<MimiContext, Task>? GetHandler(string method, string path, out Dictionary<string, string> routeParams, out List<IMiddleware> middlewares)
        {
            routeParams = new Dictionary<string, string>();
            middlewares = new List<IMiddleware>();
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
                        if (_routeMiddlewares[method].ContainsKey(route.Key))
                        {
                            middlewares = _routeMiddlewares[method][route.Key];
                        }
                        return route.Value;
                    }
                }
            }
            return null;
        }
    }
}
