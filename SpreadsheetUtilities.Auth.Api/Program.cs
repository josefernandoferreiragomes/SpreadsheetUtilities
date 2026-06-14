using MediatR;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.Session;
using SpreadsheetUtility.Bootstrapper;
using SpreadsheetUtility.Infrastructure;
using SpreadsheetUtility.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

builder.AddRedis();

builder.Services.AddSpreadsheetUtilities();
builder.Services.AddScoped<IAuthServiceFactory, AuthServiceFactory>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "SpreadsheetUtilities Auth API";
    options.Theme = ScalarTheme.Default;
});

app.MapGet("/initiateSession", async (
    IMediator mediator, 
    string eMail, 
    Guid? guid = null,
    CacheBackend cache = CacheBackend.Memory) =>
{
    var result = await mediator.Send(new InitiateSessionCommand(eMail, guid, cache));
    return result.SessionId;
})
.WithName("InitiateSession");

app.MapGet("/getSession", async (IMediator mediator, string eMail, Guid guid) =>
{
    var result = await mediator.Send(new GetSessionQuery(eMail, guid));
    return result.SessionValue;
})
.WithName("GetSession");

app.MapPost("/updateSession", async (IMediator mediator, string eMail, Guid guid, [FromBody] string newValue) =>
{
    var result = await mediator.Send(new UpdateSessionCommand(eMail, guid, newValue));
    return result.UpdatedValue;
})
.WithName("UpdateSession");

app.MapGet("/listSessions", async (IMediator mediator) =>
{
    var result = await mediator.Send(new ListSessionsQuery());
    return result.Sessions;
})
.WithName("ListSessions");

app.Run();

