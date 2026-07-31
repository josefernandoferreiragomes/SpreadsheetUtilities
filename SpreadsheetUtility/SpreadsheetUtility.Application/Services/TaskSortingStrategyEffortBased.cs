using SpreadsheetUtility.Domain.Models;
using System.Globalization;

namespace SpreadsheetUtility.Application.Services;

public class TaskSortingStrategyEffortBased : ITaskSortingStrategy
{
    public (List<GanttTask> ganttTaskList, int newMaximumTaskID) SortTasks(List<GanttTask> tasks, int currentMaximumTaskID)
    {
        List<GanttTask> sortedTasks = tasks;
        foreach (var task in tasks)
        {
            if (string.IsNullOrEmpty(task.Dependencies))
            {
                task.Dependencies = "0";
            }
        }

        sortedTasks = tasks.OrderBy(t => t.Dependencies).ThenByDescending(t => t.EffortHours).ToList();

        var idMapping = new Dictionary<string, string>();
        for (int i = currentMaximumTaskID; i < sortedTasks.Count; i++)
        {
            var originalTaskId = sortedTasks[i].Id;
            var newTaskId = (i + 1).ToString(CultureInfo.InvariantCulture);
            idMapping[originalTaskId] = newTaskId;
            sortedTasks[i].Id = newTaskId;

            if (sortedTasks[i].Dependencies == "0")
            {
                sortedTasks[i].Dependencies = "";
            }
            currentMaximumTaskID = int.Parse(newTaskId, CultureInfo.InvariantCulture);
        }

        foreach (var task in sortedTasks)
        {
            if (!string.IsNullOrEmpty(task.Dependencies) && idMapping.TryGetValue(task.Dependencies, out string? newDependencyId))
            {
                task.Dependencies = newDependencyId;
            }
        }

        tasks = sortedTasks;

        return (tasks, currentMaximumTaskID);
    }
}
