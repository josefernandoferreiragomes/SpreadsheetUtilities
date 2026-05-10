using SpreadsheetUtility.Auth.Api;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace SpreadsheetUtility.UI.Web.Services
{
    public class SessionService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string CookieProtectionPurpose = "SpreadsheetUtility.SessionCookie";

        public SessionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string InitiateSession(string email) 
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new AuthApi(http);  
                var result = authApiClient.InitiateSessionAsync(email, null);                
                return result.Result;
            }
        }

        public string GetSession(string email, Guid sessionId)
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new AuthApi(http);
                var result = authApiClient.GetSessionAsync(email, sessionId);
                return result.Result;
            }
        }

        public string UpdateSession(string email, Guid sessionId, string serializedObject)
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new AuthApi(http);
                var result = authApiClient.UpdateSessionAsync(email, sessionId, serializedObject);
                return result.Result;
            }
        }

        /// <summary>
        /// Securely stores session content in a cookie using encryption.
        /// </summary>
        public string StoreSessionContentInCookie(string sessionContent)
        {
            try
            {
                // Get the data protector for this specific purpose
                var protector = _dataProtectionProvider.CreateProtector(CookieProtectionPurpose);

                // Convert the session content to bytes
                var contentBytes = Encoding.UTF8.GetBytes(sessionContent);

                // Encrypt the content
                var encryptedBytes = protector.Protect(contentBytes);

                // Convert to Base64 for safe storage in cookie
                var encryptedContent = Convert.ToBase64String(encryptedBytes);

                return encryptedContent;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to encrypt session content for cookie storage.", ex);
            }
        }

        /// <summary>
        /// Retrieves and decrypts session content from a cookie.
        /// </summary>
        public string RetrieveSessionContentFromCookie(string encryptedContent)
        {
            try
            {
                // Get the data protector for this specific purpose
                var protector = _dataProtectionProvider.CreateProtector(CookieProtectionPurpose);

                // Convert from Base64
                var encryptedBytes = Convert.FromBase64String(encryptedContent);

                // Decrypt the content
                var decryptedBytes = protector.Unprotect(encryptedBytes);

                // Convert back to string
                var sessionContent = Encoding.UTF8.GetString(decryptedBytes);

                return sessionContent;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to decrypt session content from cookie.", ex);
            }
        }
    }
}
