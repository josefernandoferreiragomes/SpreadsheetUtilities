using Microsoft.Extensions.DependencyInjection;
using SpreadsheetUtility.Library.TaskSorters;

namespace SpreadsheetUtility.Library.TaskAssigners;
public interface ITaskSortingStrategyFactory
{
    ITaskSortingStrategy GetStrategy(bool preSortTasks);
}

public enum TaskSortingStrategyType
{
    Default
}

public class TaskSortingStrategyFactory : ITaskSortingStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TaskSortingStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITaskSortingStrategy GetStrategy(bool preSortTasks)
    {
        return preSortTasks switch
        {   true => _serviceProvider.GetRequiredService<TaskSortingStrategyEffortBased>(),
            _ => _serviceProvider.GetRequiredService<DefaultTaskSortingStrategy>()
        };
    }
}
