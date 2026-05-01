namespace SpreadsheetUtility.Library.Identity.Dtos;

/// <summary>
/// Data transfer object for authentication responses.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the authentication was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the message describing the authentication result.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the authentication token (JWT).
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the expiration time of the token.
    /// </summary>
    public DateTime? TokenExpiration { get; set; }

    /// <summary>
    /// Gets or sets any errors that occurred during authentication.
    /// </summary>
    public List<string> Errors { get; set; } = [];
}
