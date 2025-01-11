namespace Campaign.Infrastructure.Utils.HandlerAttributes.Resellers;

public static class ResellerExtensions
{
    public static int? GetUserResellerId(this IExecutionContext executionContext)
    {
        try
        {
            var claimResellerId = int.Parse(executionContext.GetByClaimType("resellerId"));

            return claimResellerId > 0
                ? claimResellerId
                : null;
        }
        catch
        {
            return null;
        }
    }
}