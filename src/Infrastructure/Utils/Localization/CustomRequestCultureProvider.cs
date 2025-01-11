using Infra.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Infrastructure.Utils.Localization;

public class CustomRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        //You can read culture-info from another process and return override of it:

        var queryProcessor = httpContext.RequestServices.GetService<IQueryProcessor>();

        return Task.FromResult(new ProviderCultureResult("en-US"));
    }
}