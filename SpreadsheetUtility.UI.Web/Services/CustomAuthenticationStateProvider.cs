using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SpreadsheetUtility.UI.Web.Services;

/// <summary>
/// Custom authentication state provider with persistent storage support.
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;
    private ClaimsPrincipal? _cachedPrincipal;
    private bool _isAuthenticated;

    public CustomAuthenticationStateProvider(ILogger<CustomAuthenticationStateProvider> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cachedPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        _isAuthenticated = false;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _logger.LogDebug("Getting authentication state.");
        var authState = new AuthenticationState(_cachedPrincipal ?? new ClaimsPrincipal(new ClaimsIdentity()));
        return Task.FromResult(authState);
    }

    /// <summary>
    /// Notifies the provider that a user has been authenticated and persists the state.
    /// </summary>
    public void NotifyUserAuthentication(string email, int userId, string fullName, string? authToken = null)
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

        if (!string.IsNullOrEmpty(authToken))
        {
            claims.Add(new Claim("AuthToken", authToken));
        }

        var identity = new ClaimsIdentity(claims, "Custom");
        var principal = new ClaimsPrincipal(identity);

        _cachedPrincipal = principal;
        _isAuthenticated = true;

        // Persist to localStorage (you'll need to implement this via JS interop)
        PersistAuthenticationState();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void NotifyUserLogout()
    {
        _logger.LogInformation("User logged out.");

        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        _cachedPrincipal = principal;
        _isAuthenticated = false;

        // Clear from localStorage
        ClearAuthenticationState();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Attempts to restore authentication state from persistent storage.
    /// </summary>
    public async Task InitializeAuthenticationStateAsync()
    {
        _logger.LogDebug("Initializing authentication state from storage.");
        // This will be called from App.razor to restore state on app initialization
    }

    public bool IsAuthenticated => _isAuthenticated;

    public string? GetUserEmail() => _cachedPrincipal?.FindFirst(ClaimTypes.Email)?.Value;
    public string? GetUserFullName() => _cachedPrincipal?.FindFirst(ClaimTypes.Name)?.Value;
    public int? GetUserId()
    {
        var userIdClaim = _cachedPrincipal?.FindFirst("UserId")?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private void PersistAuthenticationState()
    {
        // Implement via JS interop to store in localStorage
        _logger.LogDebug("Persisting authentication state to storage.");
    }

    private void ClearAuthenticationState()
    {
        _logger.LogDebug("Clearing authentication state from storage.");
    }
}