using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Infrastructure.Utils.NET.DependencyInjection;

public interface IServiceCollectionModule
{
    public void Load(IServiceCollection serviceCollection, IWebHostEnvironment env);
}
