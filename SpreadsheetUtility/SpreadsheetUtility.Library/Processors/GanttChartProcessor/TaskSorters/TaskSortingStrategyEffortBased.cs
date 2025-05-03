using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.TaskSorters;

public class TaskSortingStrategyEffortBased : ITaskSortingStrategy
{
    /// <summary>
    /// Sorts the tasks based on their dependencies and estimated effort hours.
    /// </summary>
    /// <param name="projectTasks"></param>
    /// <param name="currentMaximumTaskID"></param>
    /// <returns></returns>
    public (List<GanttTask> ganttTaskList, int newMaximumTaskID)  SortTasks(List<GanttTask> projectTasks, int currentMaximumTaskID)
    {
        List<GanttTask> sortedTasks = projectTasks;
        // Change Dependencies from empty string to "0" for sorting
        foreach (var task in projectTasks)
        {
            if (string.IsNullOrEmpty(task.Dependencies))
            {
                task.Dependencies = "0";
            }
        }

        // Sort tasks by dependencies and estimated effort hours
        sortedTasks = projectTasks.OrderBy(t => t.Dependencies).ThenByDescending(t => t.EstimatedEffortHours).ToList();

        // Create a mapping of original task IDs to new task IDs
        var idMapping = new Dictionary<string, string>();
        for (int i = currentMaximumTaskID; i < sortedTasks.Count; i++)
        {
            var originalTaskId = sortedTasks[i].Id;
            var newTaskId = (i + 1).ToString();
            idMapping[originalTaskId] = newTaskId;
            sortedTasks[i].Id = newTaskId;

            if (sortedTasks[i].Dependencies == "0")
            {
                sortedTasks[i].Dependencies = "";
            }
            currentMaximumTaskID = int.Parse(newTaskId);
        }

        // Update the dependencies to the new sorted order
        foreach (var task in sortedTasks)
        {
            if (!string.IsNullOrEmpty(task.Dependencies) && idMapping.ContainsKey(task.Dependencies))
            {
                task.Dependencies = idMapping[task.Dependencies];
            }
        }

        projectTasks = sortedTasks;
        
        return (projectTasks, currentMaximumTaskID);
    }
}
