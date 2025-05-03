using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpreadsheetUtility.Library.Models;

public class ProjectInputLogCommand : ILogCommand
{
    private readonly ILogger _logger;
    private readonly List<Project> _projects;

    public ProjectInputLogCommand(ILogger logger, List<Project> projects)
    {
        _logger = logger;
        _projects = projects;
    }

    public void Execute()
    {
        _logger.LogDebug(JsonConvert.SerializeObject(_projects, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        }));
    }
}
