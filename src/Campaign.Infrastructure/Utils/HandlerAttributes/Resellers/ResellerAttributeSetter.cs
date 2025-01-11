using System.Reflection;

namespace Campaign.Infrastructure.Utils.HandlerAttributes.Resellers;

public static class ResellerAttributeSetter
{
    public static void Set(object input, ILifetimeScope scope)
    {
        Guard.NotNull(input, nameof(input));

        var type = input.GetType();

        var resellerProperty = type.GetProperties(BindingFlags.Public)
            .SingleOrDefault(p => p.GetCustomAttribute<ResellerIdAttribute>() != null);

        if (resellerProperty == null)
            return;

        var setMethod = resellerProperty.GetSetMethod(true);

        if (setMethod != null)
            throw new InvalidOperationException("resllerId must be settable");

        int? resellerId = scope.Resolve<IExecutionContext>().GetUserResellerId();
        if (resellerId != null)
            setMethod.Invoke(input, new object[] { resellerId });
    }
}
