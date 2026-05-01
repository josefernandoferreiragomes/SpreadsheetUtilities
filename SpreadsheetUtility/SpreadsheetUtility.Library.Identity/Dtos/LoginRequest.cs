namespace SpreadsheetUtility.Library.Identity.Dtos;

/// <summary>
/// Data transfer object for user login requests.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for the user account.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to remember the user login.
    /// </summary>
    public bool RememberMe { get; set; }
}
