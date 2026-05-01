using Microsoft.AspNetCore.Identity;
using SpreadsheetUtility.Library.DataAccess.Models;
using SpreadsheetUtility.Library.Identity.Dtos;

namespace SpreadsheetUtility.Library.Identity.Services;

/// <summary>
/// Interface for authentication service providing user registration and login functionality.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Registers a new user asynchronously.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <returns>A task representing the asynchronous operation that returns the authentication response.</returns>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user and returns an authentication response asynchronously.
    /// </summary>
    /// <param name="request">The login request containing email and password.</param>
    /// <returns>A task representing the asynchronous operation that returns the authentication response.</returns>
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Gets user information by email asynchronously.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>A task representing the asynchronous operation that returns the user, or null if not found.</returns>
    Task<ApplicationUser?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Validates user credentials asynchronously.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>A task representing the asynchronous operation that returns true if credentials are valid, otherwise false.</returns>
    Task<bool> ValidateCredentialsAsync(string email, string password);

    /// <summary>
    /// Updates the last login time for a user asynchronously.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateLastLoginAsync(int userId);
}
