using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Services;

public interface IDeveloperHoursCalculator
{
    void CalculateDeveloperHours(List<GanttTask> ganttTaskList, List<Developer> developerList, IDateCalculator dateCalculator);
}
