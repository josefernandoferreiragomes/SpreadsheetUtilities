   public interface IObserver
   {
       void Update();
   }

   public class DeveloperObserver : IObserver
   {
       public void Update()
       {
           // React to changes in GanttChartProcessor
       }
   }
   