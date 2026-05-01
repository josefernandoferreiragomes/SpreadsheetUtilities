using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI services
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

// Add Scalar UI services
//builder.Services.AddScalar();

// Build app
var app = builder.Build();

// Enable OpenAPI JSON endpoint
app.MapOpenApi();

// Enable Scalar UI at /scalar
app.MapScalarApiReference(options =>
{
    options.Title = "Minimal API with Scalar";
    options.Theme = ScalarTheme.Default; // Options: Light, Dark, Midnight, etc.
});

app.MapGet("/initiateSession", (IMemoryCache memoryCache) =>
{
    var guid = Guid.NewGuid();
    memoryCache.Set(guid, "SomeValue");
    return guid;
})
.WithName("InitiateSession");

app.MapGet("/getSession", (IMemoryCache memoryCache, Guid guid) =>
{
    if (memoryCache.TryGetValue(guid, out var value))
    {
        return value;
    }
    throw new AuthenticationException("Invalid session.");
})
.WithName("GetSession");

app.Run();

//using Microsoft.Extensions.Caching.Memory;
//using Scalar.AspNetCore;
//using System.Security.Authentication;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//builder.Services.AddMemoryCache();
//var app = builder.Build();

//app.MapOpenApi().WithDescription("API for managing sessions");

//// Paste the API endpoints and records from the "API versioning for Minimal APIs" section here

//// MapScalarApiReference sets up the Scalar UI at /scalar
//// AddDocuments registers all known API versions so Scalar shows a dropdown to switch between them.
//// You can enrich your OpenAPI document with Scalar specific integrations if you wish.
//// To learn more: https://scalar.com/products/api-references/integrations/aspnetcore/openapi-extensions
//app.MapScalarApiReference();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.MapGet("/initiateSession", (IMemoryCache memoryCache) =>
//{
//    var guid = Guid.NewGuid();
//    memoryCache.Set(guid, "SomeValue");
//    return guid;
//})
//.WithName("InitiateSession");

//app.MapGet("/getSession", (IMemoryCache memoryCache, Guid guid) =>
//{    
//    if (memoryCache.TryGetValue(guid, out var value))
//    {
//        return value;
//    }
//    throw new AuthenticationException("Invalid session.");
//})
//.WithName("GetSession");

//app.Run();