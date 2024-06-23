using MimiSharp.Context;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MimiSharp.Middleware
{
    /// <summary>
    /// Middleware to limit the number of requests a client can make within a specified time frame.
    /// </summary>
    public class RateLimitingMiddleware : IMiddleware
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _timeSpan;
        private readonly ConcurrentDictionary<string, (int count, DateTime resetTime)> _clientRequests;

        public RateLimitingMiddleware(int maxRequests, TimeSpan timeSpan)
        {
            _maxRequests = maxRequests;
            _timeSpan = timeSpan;
            _clientRequests = new ConcurrentDictionary<string, (int, DateTime)>();
        }

        public async Task InvokeAsync(MimiContext context, Func<Task> next)
        {
            var clientIp = context.Request.Headers.ContainsKey("X-Forwarded-For")
                ? context.Request.Headers["X-Forwarded-For"]
                : context.Request.RemoteEndPoint.ToString();

            if (_clientRequests.TryGetValue(clientIp, out var clientData))
            {
                if (DateTime.UtcNow > clientData.resetTime)
                {
                    _clientRequests[clientIp] = (1, DateTime.UtcNow.Add(_timeSpan));
                }
                else if (clientData.count >= _maxRequests)
                {
                    context.Response.StatusCode = HttpStatusCode.TooManyRequests;
                    context.Response.StatusDescription = "Too Many Requests";
                    context.Response.SetBody(new { message = "Too Many Requests" }, "application/json");
                    await context.Response.SendAsync();
                    return;
                }
                else
                {
                    _clientRequests[clientIp] = (clientData.count + 1, clientData.resetTime);
                }
            }
            else
            {
                _clientRequests[clientIp] = (1, DateTime.UtcNow.Add(_timeSpan));
            }

            await next();
        }
    }
}
