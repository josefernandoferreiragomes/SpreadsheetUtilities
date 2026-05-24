using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Services;

public class TaskSortingStrategyEffortBased : ITaskSortingStrategy
{
    public (List<GanttTask> ganttTaskList, int newMaximumTaskID) SortTasks(List<GanttTask> projectTasks, int currentMaximumTaskID)
    {
        List<GanttTask> sortedTasks = projectTasks;
        foreach (var task in projectTasks)
        {
            if (string.IsNullOrEmpty(task.Dependencies))
            {
                task.Dependencies = "0";
            }
        }

        sortedTasks = projectTasks.OrderBy(t => t.Dependencies).ThenByDescending(t => t.EffortHours).ToList();

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
