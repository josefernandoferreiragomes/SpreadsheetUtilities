using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Services;

public interface ITaskSortingStrategy
{
    (List<GanttTask> ganttTaskList, int newMaximumTaskID) SortTasks(List<GanttTask> tasks, int currentMaximumTaskID);
}
