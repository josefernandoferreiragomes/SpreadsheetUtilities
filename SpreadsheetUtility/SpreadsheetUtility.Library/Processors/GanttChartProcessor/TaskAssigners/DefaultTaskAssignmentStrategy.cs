using SpreadsheetUtility.Library.Calculators;
using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Domain;

namespace SpreadsheetUtility.Library.TaskAssigners;
public class DefaultTaskAssignmentStrategy : TaskAssignmentStrategyBase
{
    public DefaultTaskAssignmentStrategy(IDateCalculator dateCalculator) : base(dateCalculator) { }
    public override void AssignTasks(ProjectGroup projectGroupList, List<Developer> developerList, List<GanttTask> projectTasks, DateTime projectStartDate)
    {

        DateTime startDate = projectStartDate;
        startDate = _dateCalculator.GetNextWorkingDay(startDate);

        AssignTasksToDevelopers(developerList, projectTasks, startDate);
    }
}
