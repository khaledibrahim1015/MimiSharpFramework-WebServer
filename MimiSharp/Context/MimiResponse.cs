using MimiSharp.Serialization;
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
    /// Represents an HTTP response.
    /// </summary>
    public class MimiResponse
    {
        private readonly NetworkStream _networkStream;

        public string HttpVersion { get; set; } = "HTTP/1.1";
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string StatusDescription { get; set; } = HttpStatusCode.OK.ToString();
        public string CacheControl { get; set; } = "no-cache";
        public string? Server { get; set; }
        public string Date { get; set; } = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
        //public string[] ContentType { get; set; } = new string[] { "application/json" };
        public string ContentType { get; set; } = "application/json";

        public long ContentLength { get; set; }
        public string Connection { get; set; } = "Keep-Alive";
        public string Body { get; set; } = string.Empty;



        /// <summary>
        /// Initializes a new instance of the MimiResponse class.
        /// </summary>
        /// <param name="networkStream">The network stream to write the response to.</param>
        public MimiResponse(NetworkStream networkStream)
        {
            this._networkStream = networkStream;
        }

        /// <summary>
        /// Sets the response body and serializes it based on the content type.
        /// </summary>
        /// <param name="body">The response body object.</param>
        /// <param name="requestContentType">The content type of the request.</param>
        public void SetBody(object body, string requestContentType = "application/json")
        {
            ContentType = requestContentType;
            Body = Serializer.Serialize(body, ContentType);
            ContentLength = Encoding.UTF8.GetByteCount(Body);
        }
        /// <summary>
        /// Sets the response body and serializes it based on the content type.
        /// </summary>
        /// <param name="body">The response body object.</param>
        /// <param name="requestContentType">The content type of the request.</param>
        public void SetBody<T>(T body, string requestContentType = "application/json")
        {
            ContentType = requestContentType;
            Body = Serializer.Serialize<T>(body, ContentType);
            ContentLength = Encoding.UTF8.GetByteCount(Body);
        }


        /// <summary>
        /// Sends the response to the client.
        /// </summary>
        public async Task SendAsync()
        {


            StringBuilder responseBuilder = new StringBuilder();
            responseBuilder.AppendLine($"{HttpVersion} {(int)StatusCode} {StatusDescription}");
            responseBuilder.AppendLine($"Date: {Date}");
            responseBuilder.AppendLine($"Server: {Server}");
            responseBuilder.AppendLine($"Cache-Control: {CacheControl}");
            responseBuilder.AppendLine($"Content-Type: {ContentType}");
            responseBuilder.AppendLine($"Content-Length: {ContentLength}");
            responseBuilder.AppendLine($"Connection: {Connection}");
            responseBuilder.AppendLine();
            responseBuilder.Append(Body);

            byte[] responseBuffer =  Encoding.UTF8.GetBytes(responseBuilder.ToString());

           await _networkStream.WriteAsync(responseBuffer, 0, responseBuffer.Length);



        }



    }
}
