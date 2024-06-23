using MimiSharp.Context;
using MimiSharp.Middleware;
using MimiSharp.Routing;
using System.Text.RegularExpressions;

namespace MimiSharp
{
    internal class Program
    {   
            static async Task Main(string[] args)
            {
                var app = new WebServer("127.0.0.1", 8580);

            // Add Middleware to the pipeline
                app.Use(new ErrorHandlingMiddleware());
                app.Use(new AuthenticationMiddleware());
                app.Use(new LoggingMiddleware());
                app.Use(new RateLimitingMiddleware(5, TimeSpan.FromMinutes(1)));



            app.Get("/", async ctx =>
                {
                    ctx.Response.SetBody("<h1>Hello, World!</h1>", "text/html");
                    await ctx.Response.SendAsync();
                });

                app.Post("/users/create", async ctx =>
                {
                    ctx.Response.SetBody(new { message = "User created" }, ctx.Request.Headers.ContainsKey("Content-Type") ? ctx.Request.Headers["Content-Type"] : "application/json");
                    await ctx.Response.SendAsync();
                });
                app.Post("/users/create/test", async ctx =>
                {
                    ctx.Response.SetBody<test>(new test { id=1 , name="khaled"}, ctx.Request.Headers.ContainsKey("Content-Type") ? ctx.Request.Headers["Content-Type"] : "application/json");
                    await ctx.Response.SendAsync();
                });
                app.Post("/users/create/testRequestBody", async ctx =>
                {
                    ctx.Response.SetBody(ctx.Request.Body, ctx.Request.Headers.ContainsKey("Content-Type") ? ctx.Request.Headers["Content-Type"] : "application/json");
                    await ctx.Response.SendAsync();
                });
                app.Put("/users/{id}", async ctx =>
                {
                    var userId = ctx.RouteParams["id"];
                    ctx.Response.SetBody(new { message = $"User {userId} updated" }, ctx.Request.Headers.ContainsKey("Content-Type") ? ctx.Request.Headers["Content-Type"] : "application/json");
                    await ctx.Response.SendAsync();
                });

                app.Delete("/users/{id}", async ctx =>
                {
                    var userId = ctx.RouteParams["id"];
                    ctx.Response.SetBody(new { message = $"User {userId} deleted" }, ctx.Request.Headers.ContainsKey("Content-Type") ? ctx.Request.Headers["Content-Type"] : "application/json");
                    await ctx.Response.SendAsync();
                });

                app.Get("/search", async ctx =>
                {
                    var query = ctx.Request.QueryParams.Any() ? ctx.Request.QueryParams  : new Dictionary<string, string> { { "NoQuery", "NoQuery" } };
                    ctx.Response.SetBody(new { message = $"Search query: {query["id"]} , {query["name"]}" }, ctx.Request.Headers.ContainsKey("Content-Type") ? ctx.Request.Headers["Content-Type"] : "application/json");
                    await ctx.Response.SendAsync();
                });

               await app.Start();
                Console.ReadLine();
            }



       
            public static async Task handle (MimiContext ctx )
            {
                ctx.Response.ContentType = "text/html";
                ctx.Response.SetBody("<h1>Hello, World!</h1>", "text/html");
               await ctx.Response.SendAsync();

            }


        public class test
        {
            public int id { get; set; }
            public string name { get; set; }
        }
    }
}
