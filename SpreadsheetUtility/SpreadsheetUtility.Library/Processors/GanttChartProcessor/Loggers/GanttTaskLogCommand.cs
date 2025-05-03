using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpreadsheetUtility.Library.Models;

public class GanttTaskLogCommand : ILogCommand
{
    private readonly ILogger _logger;
    private readonly List<GanttTask> _ganttTasks;

    public GanttTaskLogCommand(ILogger logger, List<GanttTask> ganttTasks)
    {
        _logger = logger;
        _ganttTasks = ganttTasks;
    }

    public void Execute()
    {
        _logger.LogDebug(JsonConvert.SerializeObject(_ganttTasks, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        }));
    }
}
