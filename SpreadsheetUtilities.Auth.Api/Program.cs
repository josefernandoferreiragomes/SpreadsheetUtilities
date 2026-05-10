using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add OpenAPI services
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

// Add Scalar UI services
//builder.Services.AddScalar();

// Build app
var app = builder.Build();

app.MapDefaultEndpoints();

// Enable OpenAPI JSON endpoint
app.MapOpenApi();

// Enable Scalar UI at /scalar
app.MapScalarApiReference(options =>
{
    options.Title = "Minimal API with Scalar";
    options.Theme = ScalarTheme.Default; // Options: Light, Dark, Midnight, etc.
});

app.MapGet("/initiateSession", (IMemoryCache memoryCache, string eMail, Guid? guid = null) =>
{
    string cacheKey = string.Empty;
    string? cacheValue = string.Empty;

    if(guid is null)
    {
        guid = Guid.NewGuid();
        cacheKey = eMail;
        cacheValue = guid.ToString();
        memoryCache.Set(cacheKey, cacheValue);
        return cacheValue;
    }
    //TODO: We should validate the email and guid combination before returning the cache key. For now, we will just check if the cache key exists.
    if (memoryCache.TryGetValue<string>(cacheKey, out cacheValue))
    {        
        if(cacheValue is not null)
            return cacheValue;
    }
    throw new AuthenticationException("Invalid session.");
})
.WithName("InitiateSession");

app.MapGet("/getSession", (IMemoryCache memoryCache, string eMail, Guid guid) =>
{
    var cacheKey = eMail; // Use email as the cache key
    if (guid == Guid.Empty)
    {
        if (memoryCache.TryGetValue<string>(cacheKey, out var cacheValue))
        {
            return cacheValue;
        }
    }
    else
    {
        if (memoryCache.TryGetValue<string>(cacheKey, out var cacheValue))
        {
            if(cacheValue is not null && cacheValue == guid.ToString())
            {
                var guidCacheKey = guid.ToString(); 
                if (memoryCache.TryGetValue<string>(guidCacheKey, out var guidCacheValue))
                {
                    return guidCacheValue;
                }
            }
        }
    }
    throw new AuthenticationException("Invalid session.");
})
.WithName("GetSession");

//TODO: We should validate the email and guid combination before updating the cache value. For now, we will just check if the cache key exists.
app.MapGet("/updateSession", (IMemoryCache memoryCache, string eMail, Guid guid, string newValue) =>
{
    if (guid == Guid.Empty)
    {
        var cacheKey = eMail; // Use email as the cache key
        if (memoryCache.TryGetValue<string>(cacheKey, out var cacheValue))
        {
            return cacheValue;
        }
    }
    else
    {
        var cacheKey = eMail; // Use email as the cache key
        if (memoryCache.TryGetValue<string>(cacheKey, out var cacheValue))
        {
            if (cacheValue is not null && cacheValue == guid.ToString())
            {
                var guidCacheKey = guid.ToString();
                if (memoryCache.TryGetValue<string>(guidCacheKey, out var guidCacheValue))
                {
                    //TODO: validate if the email and guid match before updating the cache value. If not, return an error.
                    var updatedCacheValue = $"{eMail}:{guid}:{newValue}";
                    memoryCache.Set(cacheKey, updatedCacheValue);
                    return updatedCacheValue;
                }
            }
        }
    }
    //var cacheKey = $"{eMail}:{guid}"; // Use email and GUID as the cache key
    //if (memoryCache.TryGetValue<string>(cacheKey, out var cacheValue))
    //{
    //    var updatedCacheValue = $"{eMail}:{guid}:{newValue}";
    //    memoryCache.Set(cacheKey, updatedCacheValue);
    //    return updatedCacheValue;
    //}
    throw new AuthenticationException("Invalid session.");
})
.WithName("UpdateSession");

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
//    if (memoryCache.TryGetValue(guid, out var cacheValue))
//    {
//        return cacheValue;
//    }
//    throw new AuthenticationException("Invalid session.");
//})
//.WithName("GetSession");

//app.Run();