using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimiSharp.Context;

namespace MimiSharp
{
    /// <summary>
    /// Represents a group of routes with a common prefix.
    /// </summary>
    public class RouteGroup
    {
        private readonly string _prefix;
        private readonly Router _router;

        public RouteGroup(string prefix, Router router)
        {
            _prefix = prefix;
            _router = router;
        }

        /// <summary>
        /// Registers a GET route within the group.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Get(string path, Func<MimiContext, Task> handler)
        {
            _router.RegisterRoute("GET", CombinePaths(_prefix, path), handler);
        }

        /// <summary>
        /// Registers a POST route within the group.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Post(string path, Func<MimiContext, Task> handler)
        {
            _router.RegisterRoute("POST", CombinePaths(_prefix, path), handler);
        }

        /// <summary>
        /// Registers a PUT route within the group.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Put(string path, Func<MimiContext, Task> handler)
        {
            _router.RegisterRoute("PUT", CombinePaths(_prefix, path), handler);
        }

        /// <summary>
        /// Registers a DELETE route within the group.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Delete(string path, Func<MimiContext, Task> handler)
        {
            _router.RegisterRoute("DELETE", CombinePaths(_prefix, path), handler);
        }

        private static string CombinePaths(string prefix, string path)
        {
            if (string.IsNullOrEmpty(prefix))
                return path;

            if (string.IsNullOrEmpty(path))
                return prefix;

            return $"{prefix.TrimEnd('/')}/{path.TrimStart('/')}";
        }
    }
}
