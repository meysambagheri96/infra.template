using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Campaign.Handlers.Common
{
    public class ClaimHelper
    {
        private readonly ClaimsPrincipal _user;
        public ClaimHelper(IHttpContextAccessor httpContextAccessor)
        {
            _user = httpContextAccessor.HttpContext?.User;
        }

        public int CurrentUserId
        {
            get
            {
                var userId = _user?.FindFirst(ClaimTypes.Sid);
                return userId is null ? 0 : int.Parse(userId.Value);
            }
        }
    }
}
