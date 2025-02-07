using System.Security.Claims;

namespace ToDo_Backend_FrameworksDrivers_API.Services
{
    public static class GetUserIdService
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("Id");
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim.Value);
        }
    }
}
