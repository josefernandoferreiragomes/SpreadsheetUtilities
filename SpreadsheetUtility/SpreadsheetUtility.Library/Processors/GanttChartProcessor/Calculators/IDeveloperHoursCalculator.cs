using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Domain;

namespace SpreadsheetUtility.Library.Calculators;
public interface IDeveloperHoursCalculator
{
    void CalculateDeveloperHours(List<GanttTask> ganttTaskList, List<Developer> developerList, IDateCalculator dateCalculator);
}
