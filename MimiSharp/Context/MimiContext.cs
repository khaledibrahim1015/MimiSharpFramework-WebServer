using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MimiSharp.Context
{

    /// <summary>
    /// Represents the context for an HTTP request and response.
    /// </summary>
    public class MimiContext
    {
        public MimiRequest  Request{ get; set; }
        public MimiResponse Response { get; set; }

        // "/users/{id}" adding Routing Parameter
        public Dictionary<string, string> RouteParams { get; set; }



        /// <summary>
        /// Initializes a new instance of the MimiContext class.
        /// </summary>
        /// <param name="networkStream">The network stream for the request and response.</param>    
        public MimiContext(NetworkStream networkStream , EndPoint remoteEndpoint)
        {
            Request = new MimiRequest(networkStream , remoteEndpoint);
            Response = new MimiResponse(networkStream);
            RouteParams = new Dictionary<string, string>();
        }
    }
}
