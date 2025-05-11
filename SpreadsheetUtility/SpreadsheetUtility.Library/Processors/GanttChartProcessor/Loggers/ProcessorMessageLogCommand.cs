using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class ProcessorMessageLogCommand : ILogCommand
{
    private readonly ILogger _logger;
    private readonly string _processorStartedLogMessage;

    public ProcessorMessageLogCommand(ILogger logger, string processorStartedLogMessage)
    {
        _logger = logger;
        _processorStartedLogMessage = processorStartedLogMessage;
    }

    public void Execute()
    {
        _logger.LogDebug(JsonConvert.SerializeObject(_processorStartedLogMessage, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        }));
    }
}
