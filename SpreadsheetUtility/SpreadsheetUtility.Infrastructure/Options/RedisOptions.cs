namespace SpreadsheetUtility.Infrastructure.Options;

public sealed class RedisOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Redis";

    /// <summary>
    /// StackExchange.Redis connection string.
    /// Format: "host:port,password=yourpassword,ssl=false,abortConnect=false"
    /// Example: "localhost:6379,password=yourpassword,abortConnect=false"
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Optional prefix applied to every key written by this service.
    /// Useful for namespacing when the Redis instance is shared.
    /// Example: "spreadsheet-utility" → keys become "spreadsheet-utility:session:email:..."
    /// </summary>
    public string KeyPrefix { get; init; } = "spreadsheet-utility";

    /// <summary>
    /// How long a session lives in Redis without any activity (sliding window).
    /// After this period with no reads or writes the keys are evicted automatically.
    /// Default: 60 minutes.
    /// </summary>
    public int SessionSlidingExpirationMinutes { get; init; } = 60;

    /// <summary>
    /// Hard ceiling on session lifetime regardless of activity.
    /// Prevents sessions from living forever if someone keeps touching them.
    /// Default: 8 hours.
    /// </summary>
    public int SessionAbsoluteExpirationHours { get; init; } = 8;
}
