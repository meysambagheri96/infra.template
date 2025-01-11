namespace Campaign.Infrastructure.Utils.HandlerAttributes.CurrentUser;

public static class CurrentUserExtensions
{
    public static int? GetUserCurrentUserId(this IExecutionContext executionContext)
    {
        try
        {
            var claimCurrentUserId = int.Parse(executionContext.GetByClaimType("CurrentUserId"));

            return claimCurrentUserId > 0
                ? claimCurrentUserId
                : null;
        }
        catch
        {
            return null;
        }
    }
}