using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.TaskSorters;

public class DefaultTaskSortingStrategy : ITaskSortingStrategy
{
    public (List<GanttTask> ganttTaskList, int newMaximumTaskID) SortTasks(List<GanttTask> tasks, int currentMaximumTaskID)
    {
        return (tasks, currentMaximumTaskID);
    }
}
