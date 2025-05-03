using Microsoft.Extensions.DependencyInjection;

namespace SpreadsheetUtility.Library.TaskAssigners;
public interface ITaskAssignmentStrategyFactory
{
    ITaskAssignmentStrategy GetStrategy(TaskAssignmentStrategyType strategyType);
}

public enum TaskAssignmentStrategyType
{
    Default
}

public class TaskAssignmentStrategyFactory : ITaskAssignmentStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TaskAssignmentStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITaskAssignmentStrategy GetStrategy(TaskAssignmentStrategyType strategyType)
    {
        return strategyType switch
        {            
            _ => _serviceProvider.GetRequiredService<DefaultTaskAssignmentStrategy>()
        };
    }
}
