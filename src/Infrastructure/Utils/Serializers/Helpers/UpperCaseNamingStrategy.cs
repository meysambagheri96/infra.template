using Newtonsoft.Json.Serialization;

namespace Campaign.Infrastructure.Utils.Serializers.Helpers;

public class UpperCaseNamingStrategy : NamingStrategy
{
    /// <summary>
    /// Resolves the specified property name.
    /// </summary>
    /// <param name="name">The property name to resolve.</param>
    /// <returns>The resolved property name.</returns>
    protected override string ResolvePropertyName(string name)
    {
        return name.ToUpper();
    }
}