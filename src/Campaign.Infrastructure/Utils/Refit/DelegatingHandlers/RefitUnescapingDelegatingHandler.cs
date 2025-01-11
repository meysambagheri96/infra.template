namespace Campaign.Infrastructure.Utils.Refit.DelegatingHandlers;

/// <summary>
/// WARNING: Add this file in startup before use it:
/// services.AddTransient<RefitUnescapingDelegatingHandler>();
/// </summary>
public class RefitUnescapingDelegatingHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var uri = request.RequestUri;
        //You could also simply unescape the whole uri.OriginalString
        //but i don´t recommend that, i.e only fix what´s broken
        var unescapedQuery = Uri.UnescapeDataString(uri.Query);

        var userInfo = string.IsNullOrWhiteSpace(uri.UserInfo) ? "" : $"{uri.UserInfo}@";
        var scheme = string.IsNullOrWhiteSpace(uri.Scheme) ? "" : $"{uri.Scheme}://";

        request.RequestUri = new Uri($"{scheme}{userInfo}{uri.Authority}{uri.AbsolutePath}{unescapedQuery}{uri.Fragment}");
        return base.SendAsync(request, cancellationToken);
    }
}