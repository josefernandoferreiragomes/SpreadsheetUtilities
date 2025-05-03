using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.Calculators;
public interface IDeveloperHoursCalculator
{
    void CalculateDeveloperHours(List<GanttTask> ganttTaskList, List<Developer> developerList, IDateCalculator dateCalculator);
}
