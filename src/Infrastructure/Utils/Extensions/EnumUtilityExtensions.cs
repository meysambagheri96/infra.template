using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Campaign.Infrastructure.Utils.Extensions;

public static class EnumUtilityExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .First()?
            .GetCustomAttribute<DisplayAttribute>()?
            .Name;
    }

    public static string GetDisplayNameResource(this Enum item)
    {
        if (item == null)
            return null;

        return item.GetType()
            .GetMember(item.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()?
            .GetName();
    }
}