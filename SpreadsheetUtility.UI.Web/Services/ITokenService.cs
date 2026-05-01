using Microsoft.JSInterop;

namespace SpreadsheetUtility.UI.Web.Services;

public interface ITokenService
{
    Task SaveTokenAsync(string token);
    Task<string?> GetTokenAsync();
    Task ClearTokenAsync();
}

public class TokenService : ITokenService
{
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "auth_token";

    public TokenService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
    }

    public async Task ClearTokenAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }
}