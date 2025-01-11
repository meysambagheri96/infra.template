using System.Reflection;

namespace Campaign.Infrastructure.Utils.NET.DependencyInjection;

public static class ModuleFactory
{
    public static void LoadModules(this IServiceCollection services, IWebHostEnvironment env, params Assembly[] assemblies)
    {
        var modules = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsAssignableTo(typeof(IServiceCollectionModule)));

        foreach (var moduleType in modules)
        {
            var module = (IServiceCollectionModule)Activator.CreateInstance(moduleType);
            module.Load(services, env);
        }
    }
}
