using System.Security.Authentication;
using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Infrastructure.Options;

namespace SpreadsheetUtility.Infrastructure.Services;

/// <summary>
/// Redis-backed implementation of ISessionStore.
///
/// KEY LAYOUT IN REDIS
/// ───────────────────
/// All keys share the configured prefix (default "spreadsheet-utility").
///
///   {prefix}:session:email:{email}          STRING  →  sessionId (Guid as string)
///   {prefix}:session:data:{sessionId}       STRING  →  session payload (arbitrary string)
///   {prefix}:session-index                  HASH    →  email → SessionIndexEntry (JSON)
///
/// The Hash acts as the replacement for the in-memory ConcurrentDictionary index,
/// giving us atomic per-field writes and a single HGETALL for GetAllSessions().
///
/// EXPIRY STRATEGY
/// ───────────────
/// Both session keys (email and data) get a sliding expiry refreshed on every read/write.
/// The index hash entry is a plain JSON blob without its own TTL — it is removed explicitly
/// on session deletion. This mirrors the original in-memory design where the index was
/// never evicted independently.
/// </summary>
public sealed class RedisAuthService : ISessionStore
{
    private readonly IConnectionMultiplexer _redis;
    private readonly RedisOptions _options;

    // ── Key helpers ──────────────────────────────────────────────────────────

    private string EmailKey(string email)         => $"{_options.KeyPrefix}:session:email:{email}";
    private string DataKey(string sessionId)      => $"{_options.KeyPrefix}:session:data:{sessionId}";
    private string IndexKey()                     => $"{_options.KeyPrefix}:session-index";

    private TimeSpan SlidingExpiry =>
        TimeSpan.FromMinutes(_options.SessionSlidingExpirationMinutes);

    // ── Constructor ──────────────────────────────────────────────────────────

    public RedisAuthService(IConnectionMultiplexer redis, IOptions<RedisOptions> options)
    {
        _redis    = redis;
        _options  = options.Value;
    }

    // ── ISessionStore ────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a new session for the given email.
    /// Throws InvalidOperationException if a session already exists.
    /// Returns the new session ID as a string.
    /// </summary>
    public string InitiateSession(string email)
    {
        var db         = _redis.GetDatabase();
        var emailKey   = EmailKey(email);

        // SetNX (Set if Not eXists) is atomic — safe against concurrent calls.
        var sessionId  = Guid.NewGuid();
        var sessionStr = sessionId.ToString();

        bool created = db.StringSet(
            emailKey,
            sessionStr,
            SlidingExpiry,
            When.NotExists   // NX flag — only sets if key does not already exist
        );

        if (!created)
        {
            throw new InvalidOperationException(
                $"A session already exists for email '{email}'.");
        }

        // Initialise the data key with an empty payload.
        db.StringSet(DataKey(sessionStr), string.Empty, SlidingExpiry);

        // Write index entry — stores metadata for GetAllSessions().
        var indexEntry = new SessionIndexEntry(sessionId, DateTime.UtcNow, DateTime.UtcNow);
        db.HashSet(IndexKey(), email, JsonSerializer.Serialize(indexEntry));

        return sessionStr;
    }

    /// <summary>
    /// Retrieves session data.
    ///
    /// - If sessionId is Guid.Empty: returns the raw sessionId string stored under the email key
    ///   (i.e. "what session does this email have?").
    /// - If sessionId is provided: validates ownership then returns the session payload.
    ///
    /// Returns null if the session does not exist or the sessionId does not match.
    /// </summary>
    public string? GetSession(string email, Guid sessionId)
    {
        var db       = _redis.GetDatabase();
        var emailKey = EmailKey(email);

        var storedSessionId = (string?)db.StringGet(emailKey);
        if (storedSessionId is null)
            return null;

        // Refresh sliding expiry on read.
        db.KeyExpire(emailKey, SlidingExpiry);

        if (sessionId == Guid.Empty)
            return storedSessionId;

        if (storedSessionId != sessionId.ToString())
            return null;

        var dataKey  = DataKey(storedSessionId);
        var payload  = (string?)db.StringGet(dataKey);

        if (payload is not null)
            db.KeyExpire(dataKey, SlidingExpiry);

        return payload ?? string.Empty;
    }

    /// <summary>
    /// Overwrites the session payload after validating email + sessionId ownership.
    /// Throws AuthenticationException if the session is not found or the ID does not match.
    /// </summary>
    public string UpdateSession(string email, Guid sessionId, string newValue)
    {
        var db              = _redis.GetDatabase();
        var emailKey        = EmailKey(email);
        var storedSessionId = (string?)db.StringGet(emailKey);

        if (storedSessionId is null || storedSessionId != sessionId.ToString())
            throw new AuthenticationException("Invalid session.");

        var dataKey = DataKey(storedSessionId);
        db.StringSet(dataKey, newValue, SlidingExpiry);

        // Refresh the email key expiry too — any activity resets the sliding window.
        db.KeyExpire(emailKey, SlidingExpiry);

        // Update LastModifiedAt in the index.
        var raw = (string?)db.HashGet(IndexKey(), email);
        if (raw is not null)
        {
            var entry    = JsonSerializer.Deserialize<SessionIndexEntry>(raw)!;
            var updated  = entry with { LastModifiedAt = DateTime.UtcNow };
            db.HashSet(IndexKey(), email, JsonSerializer.Serialize(updated));
        }

        return newValue;
    }

    /// <summary>
    /// Returns a snapshot of all sessions currently tracked in the index hash.
    /// Sessions whose keys have expired in Redis but whose index entry was not
    /// explicitly cleaned up will show a null SessionData — callers can filter
    /// these out if needed.
    /// </summary>
    public IReadOnlyCollection<SessionInfoDto> GetAllSessions()
    {
        var db      = _redis.GetDatabase();
        var entries = db.HashGetAll(IndexKey());

        var result = new List<SessionInfoDto>(entries.Length);

        foreach (var entry in entries)
        {
            var email = (string?)entry.Name;
            var raw   = (string?)entry.Value;

            if (email is null || raw is null)
                continue;

            var indexEntry = JsonSerializer.Deserialize<SessionIndexEntry>(raw);
            if (indexEntry is null)
                continue;

            var payload = (string?)db.StringGet(DataKey(indexEntry.SessionId.ToString()));

            result.Add(new SessionInfoDto
            {
                Email          = email,
                SessionId      = indexEntry.SessionId,
                CreatedAt      = indexEntry.CreatedAt,
                LastModifiedAt = indexEntry.LastModifiedAt,
                SessionData    = payload
            });
        }

        return result.AsReadOnly();
    }

    /// <summary>
    /// Looks up a single session by email.
    /// Returns null if no session exists for the given email.
    /// </summary>
    public SessionInfoDto? TryFindSessionByEmail(string email)
    {
        var db = _redis.GetDatabase();
        var raw = (string?)db.HashGet(IndexKey(), email);
        if (raw is null)
            return null;

        var indexEntry = JsonSerializer.Deserialize<SessionIndexEntry>(raw);
        if (indexEntry is null)
            return null;

        var payload = (string?)db.StringGet(DataKey(indexEntry.SessionId.ToString()));

        return new SessionInfoDto
        {
            Email          = email,
            SessionId      = indexEntry.SessionId,
            CreatedAt      = indexEntry.CreatedAt,
            LastModifiedAt = indexEntry.LastModifiedAt,
            SessionData    = payload
        };
    }

    // ── Private types ────────────────────────────────────────────────────────

    /// <summary>
    /// Serialised into the Redis Hash that acts as the session index.
    /// Using a record with positional constructor so JsonSerializer handles it without ceremony.
    /// </summary>
    private sealed record SessionIndexEntry(
        Guid     SessionId,
        DateTime CreatedAt,
        DateTime LastModifiedAt);
}
