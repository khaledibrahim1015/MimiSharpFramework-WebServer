using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MimiSharp.Context
{
    /// <summary>
    /// Represents an HTTP request.
    /// </summary>
    public class MimiRequest
    {


        public string Method { get; private set; }
        public string Path { get; private set; }
        public string HttpVersion { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }
        public string Body { get; private set; }
        public Dictionary<string, string> QueryParams { get; private set; }

        public EndPoint RemoteEndPoint { get; set; }

        /// <summary>
        /// Initializes a new instance of the MimiRequest class.
        /// </summary>
        /// <param name="networkStream">The network stream to read the request from.</param>
        public MimiRequest(NetworkStream networkStream , EndPoint remoteEndpoint)
        {
            Method = HttpMethod.Get.ToString();
            Path = string.Empty;
            HttpVersion = "HTTP/1.1";
            Body = string.Empty;
            Headers = new Dictionary<string, string>();
            QueryParams = new Dictionary<string, string>();
            RemoteEndPoint = remoteEndpoint;
            ParseRequestAsync(networkStream).Wait();
        }

        private async Task ParseRequestAsync(NetworkStream networkStream)
        {
            byte[] buffer = new byte[1024 * 4];
            int noOfBytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);

            if (noOfBytesRead > 0)
            {
                string request = Encoding.UTF8.GetString(buffer, 0, noOfBytesRead);
                var parts = request.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                var header = parts[0];
                var body = parts.Length > 1 ? parts[1] : string.Empty;

                ParseHeader(header);
                ParseBody(body);
            }
        }

        private void ParseHeader(string header)
        {
            string[] headerRequest = header.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < headerRequest.Length; i++)
            {
                if (i == 0)
                {
                    var requestLine = headerRequest[0].Split(new string[] { " " }, StringSplitOptions.None);
                    if (requestLine.Length == 3)
                    {
                        Method = requestLine[0];
                        var fullPath = requestLine[1];
                        var pathAndQuery = fullPath.Split(new string[] { "?" }, StringSplitOptions.None);
                        Path = pathAndQuery[0];
                        if (pathAndQuery.Length > 1)
                        {
                            var queryString = pathAndQuery[1];
                            foreach (var param in queryString.Split('&'))
                            {
                                var keyValue = param.Split('=');
                                if (keyValue.Length == 2)
                                {
                                    QueryParams[keyValue[0]] = keyValue[1];
                                }
                            }
                        }
                        HttpVersion = requestLine[2];
                    }
                }
                else
                {
                    int separatorIndex = headerRequest[i].IndexOf(":");
                    if (separatorIndex > 0)
                    {
                        var key = headerRequest[i].Substring(0, separatorIndex).Trim();
                        var value = headerRequest[i].Substring(separatorIndex + 1).Trim();
                        Headers.Add(key, value);
                    }
                }
            }
        }

        private void ParseBody(string body)
            => Body = body;
       

    }
}
