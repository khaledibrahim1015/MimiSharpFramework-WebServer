using MimiSharp.Context;
using MimiSharp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MimiSharp.Routing;
using MimiSharp.Enums;
using MimiSharp.Middleware;
using static System.Net.Mime.MediaTypeNames;

namespace MimiSharp
{   
     /// <summary>
     /// Represents   web server.
     /// </summary>
    public class WebServer
    {
        public  bool isServerAlive { get; private set; } = true;
        private  TcpListener _listener;
        private Router _router;

        private readonly List<IMiddleware> _middlewares;

        /// <summary>
        /// Initializes a new instance of the WebServer class.
        /// </summary>
        /// <param name="ipaddress">The IP address to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        public WebServer(string ipaddress , int port )
        {
            _listener = new TcpListener(
                                    !string.IsNullOrEmpty(ipaddress) ? IPAddress.Parse(ipaddress) : IPAddress.Any
                                    , IPEndpointChecker.FindAvailablePort(port, 16));
            _router = new Router();
            _middlewares = new List<IMiddleware>();

        }
        /// <summary>
        /// Starts the web server.
        /// </summary>
        public async Task Start()
        {
            _listener.Start();
            Task.Run(() => HandlIncomingConnections());
            Console.WriteLine($"Web Server is Running on Port {_listener.LocalEndpoint}");
        }

        private async Task HandlIncomingConnections()
        {
            while (isServerAlive)
            { 
                TcpClient tcpClient = await  _listener.AcceptTcpClientAsync();
                Console.WriteLine($"CLIENT CONNECTED FROM PORT {tcpClient.Client.RemoteEndPoint}");
                Task.Run( ()=>  HandlingClient(tcpClient));
            }


        }
        private async Task HandlingClient(TcpClient tcpClient)
        {

            using var networkStream  = tcpClient.GetStream();
            var ctx = new MimiContext(networkStream , tcpClient.Client.RemoteEndPoint);

            //  GetHandler Functions that regiesters 
            Func<MimiContext, Task>?  handler = _router.GetHandler(ctx.Request.Method, ctx.Request.Path, out var routeParams);
            if (handler != null)
            {
                ctx.RouteParams = routeParams;
                //await handler(ctx);
                await ExecuteMiddleWareAsync(ctx , async ()=>  await  handler(ctx));
            }
            else
            {
                ctx.Response.StatusCode = HttpStatusCode.NotFound;
                ctx.Response.StatusDescription = "Not Found";
                await ctx.Response.SendAsync();
            }



        }

        private async Task ExecuteMiddleWareAsync(MimiContext ctx , Func<Task> endpoint)
        {

            var  middleWareEnumerator = _middlewares.GetEnumerator();


            async Task Next()
            {
                if (middleWareEnumerator.MoveNext())
                    await middleWareEnumerator.Current.InvokeAsync(ctx, Next);            
                else
                   await endpoint();              
            }
            await Next();
              
        }



        /// <summary>
        /// Registers a GET route and its handler.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Get(string path, Func<MimiContext, Task> handler)
            => _router.RegisterRoute(HttpMethods.GET.ToString(), path, handler);

        /// <summary>
        /// Registers a POST route and its handler.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Post(string path, Func<MimiContext, Task> handler)
            => _router.RegisterRoute(HttpMethods.POST.ToString(), path, handler);

        /// <summary>
        /// Registers a PUT route and its handler.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>
        public void Put(string path, Func<MimiContext, Task> handler)
            => _router.RegisterRoute(HttpMethods.PUT.ToString(), path, handler);

        /// <summary>
        /// Registers a DELETE route and its handler.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="handler">The handler function.</param>

        public void Delete(string path, Func<MimiContext, Task> handler)
            => _router.RegisterRoute(HttpMethods.DELETE.ToString(), path, handler);
        /// <summary>
        /// Registers a middleware component to the pipeline.
        /// </summary>
        /// <param name="middleware">The middleware to register.</param>
        public void Use(IMiddleware middleware)
            => _middlewares.Add(middleware);




    }


}
