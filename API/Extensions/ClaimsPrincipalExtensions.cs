using System.Security.Claims;

namespace API.Extensions
{
  public static class ClaimsPrincipalExtensions
  {
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
      var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                        ?? user.FindFirst("sub");

      if (userIdClaim == null)
        throw new UnauthorizedAccessException("UserId claim bulunamadı.");

      return Guid.Parse(userIdClaim.Value);
    }
  }
}
