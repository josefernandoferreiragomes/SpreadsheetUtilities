using System.Text;
using Microsoft.AspNetCore.DataProtection;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.Services;

public class SessionCookieService : ISessionCookieService
{
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private const string CookieProtectionPurpose = "SpreadsheetUtility.SessionCookie";

    public SessionCookieService(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtectionProvider = dataProtectionProvider;
    }

    public string Protect(string plaintext)
    {
        try
        {
            var protector = _dataProtectionProvider.CreateProtector(CookieProtectionPurpose);
            var contentBytes = Encoding.UTF8.GetBytes(plaintext);
            var encryptedBytes = protector.Protect(contentBytes);
            var encryptedContent = Convert.ToBase64String(encryptedBytes);
            return encryptedContent;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to encrypt session content for cookie storage.", ex);
        }
    }

    public string Unprotect(string ciphertext)
    {
        try
        {
            var protector = _dataProtectionProvider.CreateProtector(CookieProtectionPurpose);
            var encryptedBytes = Convert.FromBase64String(ciphertext);
            var decryptedBytes = protector.Unprotect(encryptedBytes);
            var sessionContent = Encoding.UTF8.GetString(decryptedBytes);
            return sessionContent;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to decrypt session content from cookie.", ex);
        }
    }
}
