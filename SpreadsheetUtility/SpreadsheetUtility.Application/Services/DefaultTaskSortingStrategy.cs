using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Services;

public class DefaultTaskSortingStrategy : ITaskSortingStrategy
{
    public (List<GanttTask> ganttTaskList, int newMaximumTaskID) SortTasks(List<GanttTask> tasks, int currentMaximumTaskID)
    {
        return (tasks, currentMaximumTaskID);
    }
}
