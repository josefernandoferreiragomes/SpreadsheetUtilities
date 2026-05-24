using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpreadsheetUtility.Application.Behaviors;
using SpreadsheetUtility.Application.Mappers;
using SpreadsheetUtility.Application.Services;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IGanttChartMapper, GanttChartMapper>();
        services.AddScoped<IDateCalculator, DateCalculator>();
        services.AddScoped<IDeveloperHoursCalculator, DeveloperHoursCalculator>();
        services.AddScoped<ICalculatorFacade, CalculatorFacade>();
        services.AddScoped<ITaskAssignmentStrategyFactory, TaskAssignmentStrategyFactory>();
        services.AddScoped<ITaskSortingStrategyFactory, TaskSortingStrategyFactory>();
        services.AddScoped<DefaultTaskAssignmentStrategy>();
        services.AddScoped<DefaultTaskSortingStrategy>();
        services.AddScoped<TaskSortingStrategyEffortBased>();
        services.AddScoped<IListGenerator<GanttTask, Project>, GanttTaskProjectListGenerator>();
        services.AddScoped<IListGenerator<GanttTask, GanttTask>, GanttTaskListGenerator>();
        services.AddScoped<IListGenerator<Developer, List<GanttTask>>, DeveloperTaskListGenerator>();
        services.AddScoped<GroupProjectsByProjectGroupQuery>();

        return services;
    }
}
