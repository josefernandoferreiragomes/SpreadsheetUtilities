using Microsoft.AspNetCore.Identity;
using SpreadsheetUtility.Library.DataAccess.Models;
using SpreadsheetUtility.Library.Identity.Dtos;

namespace SpreadsheetUtility.Library.Identity.Services;

/// <summary>
/// Authentication service implementation providing user registration and login functionality.
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="userManager">The user manager for managing users.</param>
    public AuthenticationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    /// <summary>
    /// Registers a new user asynchronously.
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = new AuthResponse();

        // Validate request
        if (request is null)
        {
            response.Message = "Registration request cannot be null.";
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            response.Errors.Add("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            response.Errors.Add("Password is required.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            response.Errors.Add("Passwords do not match.");
        }

        if (response.Errors.Count > 0)
        {
            response.Message = "Registration failed due to validation errors.";
            return response;
        }

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            response.Errors.Add("An account with this email already exists.");
            response.Message = "Registration failed.";
            return response;
        }

        // Create new user
        var newUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createResult = await _userManager.CreateAsync(newUser, request.Password);

        if (!createResult.Succeeded)
        {
            response.Errors.AddRange(createResult.Errors.Select(e => e.Description));
            response.Message = "Registration failed.";
            return response;
        }

        response.IsSuccess = true;
        response.UserId = newUser.Id;
        response.Email = newUser.Email;
        response.FullName = newUser.GetFullName();
        response.Message = "User registered successfully.";

        return response;
    }

    /// <summary>
    /// Authenticates a user and returns an authentication response asynchronously.
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = new AuthResponse();

        // Validate request
        if (request is null)
        {
            response.Message = "Login request cannot be null.";
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            response.Errors.Add("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            response.Errors.Add("Password is required.");
        }

        if (response.Errors.Count > 0)
        {
            response.Message = "Login failed due to validation errors.";
            return response;
        }

        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            response.Errors.Add("Invalid email or password.");
            response.Message = "Login failed.";
            return response;
        }

        // Check if user is active
        if (!user.IsActive)
        {
            response.Errors.Add("This account is not active.");
            response.Message = "Login failed.";
            return response;
        }

        // Validate password
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            response.Errors.Add("Invalid email or password.");
            response.Message = "Login failed.";
            return response;
        }

        // Update last login
        await UpdateLastLoginAsync(user.Id);

        response.IsSuccess = true;
        response.UserId = user.Id;
        response.Email = user.Email;
        response.FullName = user.GetFullName();
        response.Message = "Login successful.";
        response.AuthToken = Guid.NewGuid().ToString(); // Placeholder for actual token generation
        return response;
    }

    /// <summary>
    /// Gets user information by email asynchronously.
    /// </summary>
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return await _userManager.FindByEmailAsync(email);
    }

    /// <summary>
    /// Validates user credentials asynchronously.
    /// </summary>
    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !user.IsActive)
        {
            return false;
        }

        return await _userManager.CheckPasswordAsync(user, password);
    }

    /// <summary>
    /// Updates the last login time for a user asynchronously.
    /// </summary>
    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is not null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
    }
}
