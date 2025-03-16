using Newtonsoft.Json;

namespace SpreadsheetUtility.Test.Helpers
{
    internal static class JsonTestHelper
    {
        public static T ProcessMethodJson<T>(string method, string parameterType)
        {
            var holidayList = Activator.CreateInstance<T>();
            var filePath = Path.Combine(AppContext.BaseDirectory, @"TestData\GanttService", $"{method}{parameterType}.json");

            try
            {
                var jsonString = File.ReadAllText(filePath);
                holidayList = JsonConvert.DeserializeObject<T>(jsonString) ?? Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, JSON parsing errors)
                Console.WriteLine($"An error occurred processing {typeof(T)}file: {ex.Message}");
            }

            return holidayList;
        }
    }
}
