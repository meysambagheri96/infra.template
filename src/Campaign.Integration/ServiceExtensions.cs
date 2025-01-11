using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Campaign.Integration;

public static class ServiceExtensions
{
    public static void AddCampaignClient(this IServiceCollection services, string baseUrl)
    {
        Guard.NotNullOrEmpty(baseUrl, nameof(baseUrl));

        services.AddRefitClient<ICampaignClient>(new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer()
        })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));
    }
}
