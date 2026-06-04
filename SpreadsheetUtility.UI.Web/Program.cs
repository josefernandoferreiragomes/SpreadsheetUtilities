using SpreadsheetUtility.Bootstrapper;
using SpreadsheetUtility.Infrastructure.Services;
using SpreadsheetUtility.UI.Web.ViewModels;
using SpreadsheetUtility.UI.Web.Components;
using SpreadsheetUtility.UI.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDataProtection();
builder.Services.AddMemoryCache();

builder.Services.AddRazorComponents(options =>
        options.DetailedErrors = builder.Environment.IsDevelopment()
    )
    .AddInteractiveServerComponents();
builder.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Debug);
    logging.AddConsole();
});

builder.Services.AddSpreadsheetUtilities();
builder.Services.AddScoped<SessionService>();

builder.Services.AddScoped<GanttGeneratorViewModel>();

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    options.ValidateOnBuild = context.HostingEnvironment.IsDevelopment();
});

var app = builder.Build();

app.MapDefaultEndpoints();

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

app.MapExampleFilesEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
