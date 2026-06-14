namespace SpreadsheetUtility.Infrastructure.ApiClients;

/// <summary>
/// Partial class extension adding the missing ListSessionsAsync method
/// that calls Auth.Api's /listSessions endpoint.
/// </summary>
public partial class SpreadsheetUtilitiesAuthApiClient
{
    /// <returns>OK — returns JSON array of SessionInfoDto</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<string> ListSessionsAsync()
    {
        return ListSessionsAsync(System.Threading.CancellationToken.None);
    }

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>OK — returns JSON array of SessionInfoDto</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<string> ListSessionsAsync(System.Threading.CancellationToken cancellationToken)
    {
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));

                var urlBuilder_ = new System.Text.StringBuilder();
                if (!string.IsNullOrEmpty(_baseUrl)) urlBuilder_.Append(_baseUrl);
                // Operation Path: "listSessions"
                urlBuilder_.Append("listSessions");

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var responseData_ = response_.Content == null ? null : await ReadAsStringAsync(response_.Content, cancellationToken).ConfigureAwait(false);
                        var result_ = (string)System.Convert.ChangeType(responseData_, typeof(string));
                        return result_!;
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await ReadAsStringAsync(response_.Content, cancellationToken).ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }
}
