using Entities;

namespace API.Extensions
{
  public static class CreateSession
  {
    public static UserSession CreateUserSessionEntity(
      this Entities.AppUser user,
      Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
      var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
      var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
      var uaParser = UAParser.Parser.GetDefault();
      var clientInfo = uaParser.Parse(userAgent);
      return new Entities.UserSession
      {
        UserId = user.Id,
        Device = userAgent,
        OS = clientInfo.OS.Family,
        Browser = clientInfo.UserAgent.Family,
        DeviceType = clientInfo.Device.Family == "Other" ? "Desktop" : clientInfo.Device.Family,
        IpAddress = ipAddress,
        CreatedAt = DateTime.UtcNow,
        LastSeen = DateTime.UtcNow,
        User = user
      };
    }
  }
}
