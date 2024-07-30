### MimiSharp - Custom Lightweight Web Server Framework
MimiSharp is a lightweight, flexible web server framework built from scratch in C#. It provides a simple yet powerful way to create web applications with routing, middleware support, and easy request/response handling.
Features
# 1. Basic HTTP Server

Built on top of TcpListener for handling incoming connections
Supports asynchronous request handling

# 2. Routing System

Flexible routing with support for route parameters
HTTP method-based routing (GET, POST, PUT, DELETE)
Route groups for organizing and prefixing routes

# 3. Middleware Pipeline

Global middleware support
Route-specific middleware
Built-in middleware components:

Error Handling
Authentication
Logging
Rate Limiting



# 4. Request Handling

Parses incoming HTTP requests
Extracts method, path, headers, and body
Supports query parameter parsing

# 5. Response Handling

Customizable HTTP responses
Support for different content types
Easy body setting with automatic serialization

# 6. Serialization

JSON serialization support
XML serialization support
Plain text and HTML support

# 7. Content Negotiation

Automatic content type detection and handling

# 8. Route Parameters

Extracts and provides easy access to route parameters

# 9. Query Parameters

Parses and provides access to query string parameters

# 10. Extensibility

Easy to add custom middleware
Flexible routing system allows for complex applications

# 11. Asynchronous Programming

Built with async/await pattern for efficient handling of concurrent requests

# 12. Port Management

Automatic port selection if the preferred port is in use

# 13. Grouping Routes

Ability to group routes with a common prefix

Getting Started
Installation
[Provide instructions on how to include the framework in a project]

# Basic Usage
```csharp
using MimiSharp;

var app = new WebServer("127.0.0.1", 8080);

app.Get("/", async ctx =>
{
    ctx.Response.SetBody("<h1>Hello, World!</h1>", "text/html");
    await ctx.Response.SendAsync();
});

app.Start();
Console.ReadLine();
```

# Adding Middleware
```csharp
using MimiSharp.Middleware;

app.Use(new ErrorHandlingMiddleware());
app.Use(new LoggingMiddleware());
app.Use(new RateLimitingMiddleware(5, TimeSpan.FromMinutes(1)));


```

# Defining Routes
```csharp
app.Post("/users/create", async ctx =>
{
    ctx.Response.SetBody(new { message = "User created" }, "application/json");
    await ctx.Response.SendAsync();
});

app.Get("/users/{id}", async ctx =>
{
    var userId = ctx.RouteParams["id"];
    ctx.Response.SetBody(new { message = $"User {userId} details" }, "application/json");
    await ctx.Response.SendAsync();
});


```

# Using Route Groups
```csharp

var v1 = app.Group("/api/v1");
v1.Get("/user", async ctx =>
{
    ctx.Response.SetBody(new { message = "User endpoint" }, "application/json");
    await ctx.Response.SendAsync();
});

```

# Handling Query Parameters
```csharp
app.Get("/search", async ctx =>
{
    var query = ctx.Request.QueryParams;
    var searchTerm = query.ContainsKey("q") ? query["q"] : "No search term provided";
    ctx.Response.SetBody(new { message = $"Searching for: {searchTerm}" }, "application/json");
    await ctx.Response.SendAsync();
});


```

# Custom Middleware
```csharp

public class CustomMiddleware : IMiddleware
{
    public async Task InvokeAsync(MimiContext context, Func<Task> next)
    {
        // Pre-processing logic
        await next();
        // Post-processing logic
    }
}

app.Use(new CustomMiddleware());

```

# Advanced Features
Content Negotiation
```csharp
app.Get("/data", async ctx =>
{
    var data = new { id = 1, name = "John Doe" };
    ctx.Response.SetBody(data);
    await ctx.Response.SendAsync();
});


```

# Error Handling
The ErrorHandlingMiddleware catches exceptions and returns appropriate error responses:
```csharp
app.Use(new ErrorHandlingMiddleware());

app.Get("/error", async ctx =>
{
    throw new Exception("Something went wrong");
});


```

# Rate Limiting
Protect your routes from abuse with rate limiting:Rate Limiting
Protect your routes from abuse with rate limiting:
```csharp
app.Use("GET", "/api", new RateLimitingMiddleware(100, TimeSpan.FromHour(1)));


```





