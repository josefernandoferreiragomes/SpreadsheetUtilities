using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SpreadsheetUtility.UI.Web.Services;

/// <summary>
/// Custom authentication state provider for managing user authentication state across Blazor components.
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;
    private ClaimsPrincipal? _cachedPrincipal;
    private bool _isAuthenticated;

    /// <summary>
    /// Initializes a new instance of the CustomAuthenticationStateProvider.
    /// </summary>
    public CustomAuthenticationStateProvider(ILogger<CustomAuthenticationStateProvider> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cachedPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        _isAuthenticated = false;
    }

    /// <summary>
    /// Gets the current authentication state asynchronously.
    /// </summary>
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _logger.LogDebug("Getting authentication state.");
        var authState = new AuthenticationState(_cachedPrincipal ?? new ClaimsPrincipal(new ClaimsIdentity()));
        return Task.FromResult(authState);
    }

    /// <summary>
    /// Notifies the authentication state provider that a user has been authenticated.
    /// </summary>
    /// <param name="email">The authenticated user's email.</param>
    /// <param name="userId">The authenticated user's ID.</param>
    /// <param name="fullName">The authenticated user's full name.</param>
    public void NotifyUserAuthentication(string email, int userId, string fullName)
    {
        _logger.LogInformation("User authenticated: {Email}", email);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, fullName),
            new Claim("UserId", userId.ToString()),
            new Claim("Email", email),
            new Claim("FullName", fullName ?? string.Empty)
        };

        var identity = new ClaimsIdentity(claims, "Custom");
        var principal = new ClaimsPrincipal(identity);

        _cachedPrincipal = principal;
        _isAuthenticated = true;

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Notifies the authentication state provider that a user has been logged out.
    /// </summary>
    public void NotifyUserLogout()
    {
        _logger.LogInformation("User logged out.");

        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        _cachedPrincipal = principal;
        _isAuthenticated = false;

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Gets a value indicating whether a user is currently authenticated.
    /// </summary>
    public bool IsAuthenticated => _isAuthenticated;

    /// <summary>
    /// Gets the current authenticated user's email, if any.
    /// </summary>
    public string? GetUserEmail()
    {
        return _cachedPrincipal?.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets the current authenticated user's full name, if any.
    /// </summary>
    public string? GetUserFullName()
    {
        return _cachedPrincipal?.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// Gets the current authenticated user's ID, if any.
    /// </summary>
    public int? GetUserId()
    {
        var userIdClaim = _cachedPrincipal?.FindFirst("UserId")?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}
