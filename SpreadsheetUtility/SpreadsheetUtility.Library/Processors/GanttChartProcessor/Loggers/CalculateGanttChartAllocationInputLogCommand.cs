using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpreadsheetUtility.Library.Models;

public class CalculateGanttChartAllocationInputLogCommand : ILogCommand
{
    private readonly ILogger _logger;
    private readonly CalculateGanttChartAllocationInput _calculateGanttChartAllocationInput;

    public CalculateGanttChartAllocationInputLogCommand(ILogger logger, CalculateGanttChartAllocationInput calculateGanttChartAllocationInput)
    {
        _logger = logger;
        _calculateGanttChartAllocationInput = calculateGanttChartAllocationInput;
    }

    public void Execute()
    {
        _logger.LogDebug(JsonConvert.SerializeObject(_calculateGanttChartAllocationInput, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        }));
    }
}
