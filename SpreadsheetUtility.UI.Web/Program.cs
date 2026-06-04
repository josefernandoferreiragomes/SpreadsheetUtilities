using MediatR;
using SpreadsheetUtility.Application;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Infrastructure;
using SpreadsheetUtility.UI.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDataProtection();

builder.Services.AddRazorComponents(options =>
        options.DetailedErrors = builder.Environment.IsDevelopment()
    )
    .AddInteractiveServerComponents();
builder.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Debug);
    logging.AddConsole();
});

builder.Services.AddApplication();

builder.Services.AddInfrastructure();

var app = builder.Build();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var servicesToValidate = new Type[]
    {
        typeof(IMediator),
        typeof(IDateTimeProvider),
        typeof(IHolidayProvider),
    };

    foreach (var serviceType in servicesToValidate)
    {
        scope.ServiceProvider.GetRequiredService(serviceType);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
