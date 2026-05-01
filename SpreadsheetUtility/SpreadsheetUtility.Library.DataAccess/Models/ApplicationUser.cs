using Microsoft.AspNetCore.Identity;

namespace SpreadsheetUtility.Library.DataAccess.Models;

/// <summary>
/// Represents an application user with extended properties for the spreadsheet utility application.
/// Inherits from IdentityUser to leverage ASP.NET Core Identity framework.
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time of the last login.
    /// Null if the user has never logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets the full name of the user.
    /// </summary>
    public string GetFullName() => $"{FirstName} {LastName}".Trim();
}
