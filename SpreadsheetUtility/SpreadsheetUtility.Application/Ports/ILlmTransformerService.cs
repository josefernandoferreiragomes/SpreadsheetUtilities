namespace SpreadsheetUtility.Application.Ports;

public enum TargetFormat
{
    Projects,
    Tasks,
    Team
}

public record LlmTransformationResult(
    bool IsPossible,
    string Output,
    string? ErrorReason
);

public interface ILlmTransformerService
{
    Task<LlmTransformationResult> TransformAsync(
        string inputData,
        TargetFormat targetFormat,
        CancellationToken cancellationToken = default
    );
}
