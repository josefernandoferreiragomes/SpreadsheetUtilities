using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Domain;

namespace SpreadsheetUtility.Library.Calculators;
public class DeveloperHoursCalculator : IDeveloperHoursCalculator
{
    public void CalculateDeveloperHours(List<GanttTask> ganttTaskList, List<Developer> developerList, IDateCalculator dateCalculator)
    {
        if (ganttTaskList.Count == 0 || developerList.Count == 0) return;

        DateTime minDate = ganttTaskList.Min(t => t.StartDate);
        DateTime maxDate = ganttTaskList.Max(t => t.EndDate);

        foreach (var developer in developerList)
        {
            if (developer != null)
            {
                var calculatedWorkDays = dateCalculator.CalculateWorkDays(minDate, maxDate, developer?.VacationPeriods);
                developer!.AllocatedHours = developer.Tasks?.Sum(t => t.EffortHours) ?? 0;
                developer.AllocatedDays = developer.Tasks?.Sum(t => t.WorkDays) ?? 0;
                developer!.TotalHours = calculatedWorkDays * developer.DailyWorkHours;
                var slackHours = developer.TotalHours - developer.AllocatedHours;
                developer.SlackHours = slackHours >= 0 ? slackHours : 0;
            }
        }
    }
}
