using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.ApiClients;

public partial class SpreadsheetUtilitiesAuthApiClient
{
    // ─── InitiateSessionAsync with CacheBackend ───

    public virtual System.Threading.Tasks.Task<string> InitiateSessionAsync(string eMail, System.Guid? guid, CacheBackend cache)
    {
        return InitiateSessionAsync(eMail, guid, cache, System.Threading.CancellationToken.None);
    }

    public virtual async System.Threading.Tasks.Task<string> InitiateSessionAsync(string eMail, System.Guid? guid, CacheBackend cache, System.Threading.CancellationToken cancellationToken)
    {
        if (eMail == null)
            throw new System.ArgumentNullException("eMail");

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
                urlBuilder_.Append("initiateSession");
                urlBuilder_.Append('?');
                urlBuilder_.Append(System.Uri.EscapeDataString("eMail")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(eMail, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                if (guid != null)
                {
                    urlBuilder_.Append(System.Uri.EscapeDataString("guid")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(guid, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                }
                urlBuilder_.Append(System.Uri.EscapeDataString("cache")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(cache, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                urlBuilder_.Length--;

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
                        return result_;
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

    // ─── GetSessionAsync with CacheBackend ───

    public virtual System.Threading.Tasks.Task<string> GetSessionAsync(string eMail, System.Guid guid, CacheBackend cache)
    {
        return GetSessionAsync(eMail, guid, cache, System.Threading.CancellationToken.None);
    }

    public virtual async System.Threading.Tasks.Task<string> GetSessionAsync(string eMail, System.Guid guid, CacheBackend cache, System.Threading.CancellationToken cancellationToken)
    {
        if (eMail == null)
            throw new System.ArgumentNullException("eMail");
        if (guid == null)
            throw new System.ArgumentNullException("guid");

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
                urlBuilder_.Append("getSession");
                urlBuilder_.Append('?');
                urlBuilder_.Append(System.Uri.EscapeDataString("eMail")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(eMail, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                urlBuilder_.Append(System.Uri.EscapeDataString("guid")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(guid, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                urlBuilder_.Append(System.Uri.EscapeDataString("cache")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(cache, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                urlBuilder_.Length--;

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
                        return result_;
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

    // ─── UpdateSessionAsync with CacheBackend ───

    public virtual System.Threading.Tasks.Task<string> UpdateSessionAsync(string eMail, System.Guid guid, string body, CacheBackend cache)
    {
        return UpdateSessionAsync(eMail, guid, body, cache, System.Threading.CancellationToken.None);
    }

    public virtual async System.Threading.Tasks.Task<string> UpdateSessionAsync(string eMail, System.Guid guid, string body, CacheBackend cache, System.Threading.CancellationToken cancellationToken)
    {
        if (eMail == null)
            throw new System.ArgumentNullException("eMail");
        if (guid == null)
            throw new System.ArgumentNullException("guid");
        if (body == null)
            throw new System.ArgumentNullException("body");

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var json_ = Newtonsoft.Json.JsonConvert.SerializeObject(body, JsonSerializerSettings);
                var content_ = new System.Net.Http.StringContent(json_);
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));

                var urlBuilder_ = new System.Text.StringBuilder();
                if (!string.IsNullOrEmpty(_baseUrl)) urlBuilder_.Append(_baseUrl);
                urlBuilder_.Append("updateSession");
                urlBuilder_.Append('?');
                urlBuilder_.Append(System.Uri.EscapeDataString("eMail")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(eMail, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                urlBuilder_.Append(System.Uri.EscapeDataString("guid")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(guid, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                urlBuilder_.Append(System.Uri.EscapeDataString("cache")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(cache, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                urlBuilder_.Length--;

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
                        return result_;
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

    // ─── ListSessionsAsync with CacheBackend ───

    public virtual System.Threading.Tasks.Task<string> ListSessionsAsync(CacheBackend cache)
    {
        return ListSessionsAsync(cache, System.Threading.CancellationToken.None);
    }

    public virtual async System.Threading.Tasks.Task<string> ListSessionsAsync(CacheBackend cache, System.Threading.CancellationToken cancellationToken)
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
                urlBuilder_.Append("listSessions");
                urlBuilder_.Append('?');
                urlBuilder_.Append(System.Uri.EscapeDataString("cache")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(cache, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                urlBuilder_.Length--;

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
                        return result_;
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
