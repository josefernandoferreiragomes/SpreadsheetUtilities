using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpreadsheetUtility.Library.Models;

public class CalculateGanttChartAllocationOutputLogCommand : ILogCommand
{
    private readonly ILogger _logger;
    private readonly CalculateGanttChartAllocationOutput _calculateGanttChartAllocationOutput;

    public CalculateGanttChartAllocationOutputLogCommand(ILogger logger, CalculateGanttChartAllocationOutput calculateGanttChartAllocationOutput)
    {
        _logger = logger;
        _calculateGanttChartAllocationOutput = calculateGanttChartAllocationOutput;
    }

    public void Execute()
    {
        _logger.LogDebug(JsonConvert.SerializeObject(_calculateGanttChartAllocationOutput, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        }));
    }
}
