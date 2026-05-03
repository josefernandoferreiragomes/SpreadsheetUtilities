using SpreadsheetUtility.Auth.Api;

namespace SpreadsheetUtility.UI.Web.Services
{
    public class SessionService
    {

        public SessionService() { }

        public string InitiateSession(string email) 
        {
            using (var http = new HttpClient())
            {
                var authApiClient = new AuthApi(http);  
                var result = authApiClient.InitiateSessionAsync(email, null);                
                return result.Result;
            }
        }

        public string GetSessionToken(string email, Guid sessionId)
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

    }
}
