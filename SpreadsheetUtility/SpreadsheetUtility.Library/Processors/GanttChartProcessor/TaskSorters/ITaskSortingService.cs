using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.TaskSorters;
public interface ITaskSortingStrategy
{
    (List<GanttTask> ganttTaskList, int newMaximumTaskID) SortTasks(List<GanttTask> tasks, int currentMaximumTaskID);
}
