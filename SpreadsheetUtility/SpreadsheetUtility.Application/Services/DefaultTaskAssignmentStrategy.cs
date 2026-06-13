using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Services;

public class DefaultTaskAssignmentStrategy : TaskAssignmentStrategyBase
{
    public DefaultTaskAssignmentStrategy(IDateCalculator dateCalculator) : base(dateCalculator) { }
    public override void AssignTasks(ProjectGroup projectGroup, List<Developer> developers, List<GanttTask> tasks, DateTime projectStartDate)
    {
        DateTime startDate = projectStartDate;
        startDate = _dateCalculator.GetNextWorkingDay(startDate);

        AssignTasksToDevelopers(developers, tasks, startDate);
    }
}
