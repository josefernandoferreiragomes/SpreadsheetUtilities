using SpreadsheetUtility.Library;

namespace SpreadsheetUtility.UI.Web.Helpers
{
    public static class GanttColorHelper
    {
        private static List<string> colorClasses = new List<string>()
        {
            "task-red",
            "task-green",
            "task-blue",
            "task-purple",
            "task-brown",
            "task-orange",
            "task-yellow",
            "task-cyan",
            "task-magenta",
            "task-lime",
            "task-pink",
        };

        public static void SetupProjectColor(IQueryable<GanttTask>? ganttProjectListOutput, IQueryable<GanttTask>? ganttTaskListOutput)
        {
            if (ganttProjectListOutput == null || ganttTaskListOutput == null)
            {
                return;
            }
            var random = new Random();
            foreach (var project in ganttProjectListOutput)
            {
                int count = 0;
                string color = "";
                do
                {
                    color = GetRandomColorClass();
                    count++;
                } while (ganttProjectListOutput.ToList().Any(p => p.CustomClass == color) && count < (colorClasses.Count()));

                project.CustomClass = color;
                var tempTasks = ganttTaskListOutput.Where(t => t.ProjectID == project.ProjectID);
                foreach (var task in tempTasks)
                {
                    task.CustomClass = color;
                }
            }
        }

        public static void SetupDeveloperColor(IQueryable<DeveloperAvailability>? developerListOutput, IQueryable<GanttTask>? ganttTaskListOutput)
        {
            if (developerListOutput == null || ganttTaskListOutput == null)
            {
                return;
            }
            var random = new Random();
            foreach (var developer in developerListOutput)
            {
                int count = 0;
                string color = "";
                do
                {
                    color = GetRandomColorClass();
                    count++;
                } while (developerListOutput.ToList().Any(p => p.CustomClass == color) && count < (colorClasses.Count()));

                developer.CustomClass = color;
                var tempTasks = ganttTaskListOutput.Where(t => t.AssignedDeveloperId == developer.DeveloperId);
                foreach (var task in tempTasks)
                {
                    task.CustomClass = color;
                }
            }
        }

        private static string GetRandomColorClass()
        {
            var random = new Random();
            //colors from gantt-style.css classes

            return colorClasses[random.Next(colorClasses.Count - 1)];
        }
    }
}
