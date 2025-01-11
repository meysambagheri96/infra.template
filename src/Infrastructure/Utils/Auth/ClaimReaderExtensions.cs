using Domain;
using Extensions.Http.Mvc;

namespace Campaign.Infrastructure.Utils.Auth;

public static class ClaimReaderExtensions
{
    public static int GetCurrentUserId(this IExecutionContext executionContext)
    {
        try
        {
            if (int.TryParse(executionContext.CurrentUserId, out var userId))
                return userId;
        }
        catch
        {
            if (int.TryParse(executionContext.GetByClaimType("client_ownerId"), out var ownerId))
                return ownerId;
        }

        throw new DomainValidationException("User was not found");
    }
}