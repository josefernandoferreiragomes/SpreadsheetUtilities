using SpreadsheetUtility.Library;
using SpreadsheetUtility.Library.Calculators;
using SpreadsheetUtility.Library.Grouppers;
using SpreadsheetUtility.Library.ListGenerators;
using SpreadsheetUtility.Library.Mappers;
using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Processors;
using SpreadsheetUtility.Library.Providers;
using SpreadsheetUtility.Library.Services;
using SpreadsheetUtility.Library.TaskAssigners;
using SpreadsheetUtility.Library.TaskSorters;
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
builder.Services.AddScoped<IGanttChartDataManager, GanttChartDataManager>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IGanttChartMapper, GanttChartMapper>();
builder.Services.AddScoped<IHolidayProvider, HolidayProvider>();
builder.Services.AddScoped<ITaskAssignmentStrategyFactory, TaskAssignmentStrategyFactory>();
builder.Services.AddScoped<ITaskSortingStrategyFactory, TaskSortingStrategyFactory>();
builder.Services.AddScoped<DefaultTaskAssignmentStrategy>();
builder.Services.AddScoped<DefaultTaskSortingStrategy>();
builder.Services.AddScoped<TaskSortingStrategyEffortBased>();
// Register the generalized ListGenerator implementations
builder.Services.AddScoped<IListGenerator<GanttTask, Project>, GanttTaskProjectListGenerator>();
builder.Services.AddScoped<IListGenerator<GanttTask, GanttTask>, GanttTaskListGenerator>();
builder.Services.AddScoped<IListGenerator<Developer, List<GanttTask>>, DeveloperTaskListGenerator>();

builder.Services.AddScoped<IDateCalculator, DateCalculator>();
builder.Services.AddScoped<IDeveloperHoursCalculator, DeveloperHoursCalculator>();
builder.Services.AddScoped<ICalculatorFacade, CalculatorFacade>();
builder.Services.AddScoped<LoggingInvoker>();
builder.Services.AddScoped<GroupProjectsByProjectGroupQuery>();



var app = builder.Build();

// Validate all scoped services (optional manual step)
using (var scope = app.Services.CreateScope())
{
    var servicesToValidate = new Type[]
    {
        typeof(IGanttChartProcessor),
        typeof(IGanttChartDataManager),
        typeof(IDateTimeProvider),
        typeof(IGanttChartMapper),
        typeof(IDateCalculator),
        typeof(IHolidayProvider),
        typeof(ITaskAssignmentStrategyFactory),
        typeof(ITaskSortingStrategyFactory),
        typeof(DefaultTaskAssignmentStrategy),
        typeof(DefaultTaskSortingStrategy),
        typeof(TaskSortingStrategyEffortBased),
        typeof(IListGenerator<GanttTask, Project>),
        typeof(IListGenerator<GanttTask, GanttTask>),
        typeof(IListGenerator<Developer, List<GanttTask>>),
        typeof(IDeveloperHoursCalculator),
        typeof(ICalculatorFacade),
        typeof(LoggingInvoker),
        typeof(GroupProjectsByProjectGroupQuery),
        
    };

    foreach (var serviceType in servicesToValidate)
    {
        scope.ServiceProvider.GetRequiredService(serviceType); // Throws if not registered
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
