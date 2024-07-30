using MimiSharp.Context;
using MimiSharp.Middleware;
using MimiSharp.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MimiSharp
{
    /// <summary>
    /// Represents a simple web server.
    /// </summary>
    public class WebServer
    {
        private bool isServerAlive { get; set; } = true;
        private TcpListener _listener;
        private Router _router;
        private readonly List<IMiddleware> _globalMiddlewares;

        /// <summary>
        /// Initializes a new instance of the WebServer class.
        /// </summary>
        /// <param name="ipaddress">The IP address to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        public WebServer(string ipaddress, int port)
        {
            _listener = new TcpListener(
                !string.IsNullOrEmpty(ipaddress) ? IPAddress.Parse(ipaddress) : IPAddress.Any,
                IPEndpointChecker.FindAvailablePort(port, 2));
            _router = new Router();
            _globalMiddlewares = new List<IMiddleware>();
        }

        /// <summary>
        /// Starts the web server.
        /// </summary>
        public void Start()
        {
            _listener.Start();
            Task.Run(() => HandleIncomingConnections());
            Console.WriteLine($"Web Server is Running on Port {_listener.LocalEndpoint}");
        }

        private async Task HandleIncomingConnections()
        {
            while (isServerAlive)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"CLIENT CONNECTED FROM PORT {tcpClient.Client.RemoteEndPoint}");
                Task.Run(() => HandleClient(tcpClient));
            }
        }

        private async Task HandleClient(TcpClient tcpClient)
        {
            using var networkStream = tcpClient.GetStream();
            var ctx = new MimiContext(networkStream,tcpClient.Client.RemoteEndPoint);

            var handler = _router.GetHandler(ctx.Request.Method, ctx.Request.Path, out var routeParams, out var routeMiddlewares);
            if (handler != null)
            {
                ctx.RouteParams = routeParams;
                var middlewares = new List<IMiddleware>(_globalMiddlewares);
                middlewares.AddRange(routeMiddlewares);
                await ExecuteMiddleware(ctx, async () => await handler(ctx), middlewares);
            }
            else
            {
                ctx.Response.StatusCode = HttpStatusCode.NotFound;
                ctx.Response.StatusDescription = "Not Found";
                await ctx.Response.SendAsync();
            }
        }

        private async Task ExecuteMiddleware(MimiContext context, Func<Task> endpoint, List<IMiddleware> middlewares)
        {
            var middlewareEnumerator = middlewares.GetEnumerator();

            async Task Next()
            {
                if (middlewareEnumerator.MoveNext())
                {
                    await middlewareEnumerator.Current.InvokeAsync(context, Next);
                }
                else
                {
                    await endpoint();
                }
            }

            await Next();
        }

        /// <summary>
        /// Registers a global middleware component to the pipeline.
        /// </summary>
        /// <param name="middleware">The middleware to register.</param>
        public void Use(IMiddleware middleware) => _globalMiddlewares.Add(middleware);

        /// <summary>
        /// Registers a middleware component for a specific route.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The route path.</param>
        /// <param name="middleware">The middleware to register.</param>
        public void Use(string method, string path, IMiddleware middleware) => _router.RegisterRouteMiddleware(method, path, middleware);

        /// <summary>
        /// Registers a GET route and its handler.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Get(string path, Func<MimiContext, Task> handler) => _router.RegisterRoute("GET", path, handler);

        /// <summary>
        /// Registers a POST route and its handler.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Post(string path, Func<MimiContext, Task> handler) => _router.RegisterRoute("POST", path, handler);

        /// <summary>
        /// Registers a PUT route and its handler.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Put(string path, Func<MimiContext, Task> handler) => _router.RegisterRoute("PUT", path, handler);

        /// <summary>
        /// Registers a DELETE route and its handler.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Delete(string path, Func<MimiContext, Task> handler) => _router.RegisterRoute("DELETE", path, handler);

        /// <summary>
        /// Creates a new route group with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix for the route group.</param>
        /// <returns>A RouteGroup object to register routes within the group.</returns>
        public RouteGroup Group(string prefix) => new RouteGroup(prefix, _router);
    }
}
