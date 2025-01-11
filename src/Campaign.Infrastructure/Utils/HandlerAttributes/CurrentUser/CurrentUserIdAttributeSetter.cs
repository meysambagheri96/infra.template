using System.Reflection;

namespace Campaign.Infrastructure.Utils.HandlerAttributes.CurrentUser;

public static class CurrentUserIdAttributeSetter
{
    public static void Set(object input, ILifetimeScope scope)
    {
        Guard.NotNull(input, nameof(input));

        var type = input.GetType();

        var currentUserProperty = type.GetProperties()
            .SingleOrDefault(p => p.GetCustomAttribute<CurrentUserIdAttribute>() != null);

        if (currentUserProperty == null)
            return;

        var setMethod = currentUserProperty.GetSetMethod(true);

        if (setMethod != null)
            throw new InvalidOperationException("resllerId must be settable");

        int? currentUserId = scope.Resolve<IExecutionContext>().GetUserCurrentUserId();
        if (currentUserId != null)
            setMethod.Invoke(input, new object[] { currentUserId });
    }
}
