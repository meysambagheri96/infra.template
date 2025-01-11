using System.Reflection;
using Refit;

namespace Campaign.Infrastructure.Utils.Refit.Formatters;

public sealed class RefitLowerCaseBooleanUrlParameterFormatter : DefaultUrlParameterFormatter
{
    public override string Format(object parameterValue, ICustomAttributeProvider attributeProvider, Type type)
    {
        if (parameterValue is bool)
        {
            return parameterValue.ToString()!.ToLower();
        }

        return base.Format(parameterValue, attributeProvider, type);
    }
}