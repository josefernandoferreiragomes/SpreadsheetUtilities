using SpreadsheetUtility.Library;

public interface ITaskAssignmentStrategy
   {
       void AssignTasks(ProjectGroup projectGroup, List<GanttTask> tasks, List<Developer> developers, DateTime projectStartDate);
   }

   public class DefaultTaskAssignmentStrategy : ITaskAssignmentStrategy
   {
       public void AssignTasks(ProjectGroup projectGroup, List<GanttTask> tasks, List<Developer> developers, DateTime projectStartDate)
       {
           // Task assignment logic here
       }
   }
   