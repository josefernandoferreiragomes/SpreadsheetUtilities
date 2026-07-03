using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.Validators;

public static class EtlOutputValidator
{
    private static readonly Dictionary<TargetFormat, string[]> ExpectedHeaders = new()
    {
        [TargetFormat.Projects] = new[] { "ProjectID", "Project Name", "Project Group Id", "Team Id" },
        [TargetFormat.Tasks] = new[]
        {
            "ID", "Project Id", "ProjectName", "TaskName",
            "EstimatedEffortHours", "Dependencies", "Progress", "InternalID"
        },
        [TargetFormat.Team] = new[]
        {
            "Team ID", "Team Name", "Developer Id", "Developer Name",
            "Developer Vacation Date Intervals", "Daily Work Hours"
        },
    };

    public static ValidationResult Validate(string output, TargetFormat format)
    {
        if (string.IsNullOrWhiteSpace(output))
            return new ValidationResult(false, new[] { "Output is empty." });

        var lines = output.Split('\n', '\r')
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .ToArray();

        if (lines.Length == 0)
            return new ValidationResult(false, new[] { "Output is empty." });

        var expectedHeaders = ExpectedHeaders[format];
        var headerColumns = lines[0].Split('\t');

        // Check header
        var headerErrors = new List<string>();
        for (int i = 0; i < Math.Max(headerColumns.Length, expectedHeaders.Length); i++)
        {
            if (i >= expectedHeaders.Length)
                headerErrors.Add($"Extra column {i + 1}: \"{headerColumns[i]}\"");
            else if (i >= headerColumns.Length)
                headerErrors.Add($"Missing column {i + 1}: expected \"{expectedHeaders[i]}\"");
            else if (!string.Equals(headerColumns[i], expectedHeaders[i], StringComparison.Ordinal))
                headerErrors.Add($"Column {i + 1}: expected \"{expectedHeaders[i]}\", got \"{headerColumns[i]}\"");
        }

        if (headerErrors.Count > 0)
            return new ValidationResult(false, headerErrors.ToArray());

        // Check data rows
        var dataErrors = new List<string>();
        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split('\t');
            if (cols.Length != expectedHeaders.Length)
                dataErrors.Add($"Row {i + 1}: expected {expectedHeaders.Length} columns, got {cols.Length}");
        }

        if (dataErrors.Count > 0)
            return new ValidationResult(false, dataErrors.ToArray());

        return new ValidationResult(true, Array.Empty<string>());
    }
}

public record ValidationResult(bool IsValid, string[] Errors);
