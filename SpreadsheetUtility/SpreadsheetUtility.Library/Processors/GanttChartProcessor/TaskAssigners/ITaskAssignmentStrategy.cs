using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Library.TaskAssigners;

public interface ITaskAssignmentStrategy
{
    void AssignTasks(
        ProjectGroup projectGroup,
        List<Developer> developers,
        List<GanttTask> tasks,
        DateTime projectStartDate
    );
}
