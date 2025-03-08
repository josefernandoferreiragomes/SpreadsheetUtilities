using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.JSInterop;
using SpreadsheetUtility.Library;
using SpreadsheetUtility.UI.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents(options =>
        options.DetailedErrors = builder.Environment.IsDevelopment()
    )
    .AddInteractiveServerComponents();

builder.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Debug);
    logging.AddConsole();
});

//add GanttChartProcessor to middleware services:
builder.Services.AddScoped<IGanttChartProcessor, GanttChartProcessor>();


var app = builder.Build();

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
