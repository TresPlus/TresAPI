using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Business.Static
{
  public static class ConfigurationDefaultSettings
  {
    public static string GetJwtAlgorithm(this IConfiguration config)
    {
      var algo = config["Jwt:Algorithm"]?.ToUpper();

      return algo switch
      {
        "HS256" => SecurityAlgorithms.HmacSha256,
        "HS384" => SecurityAlgorithms.HmacSha384,
        "HS512" => SecurityAlgorithms.HmacSha512,
        _ => SecurityAlgorithms.HmacSha256
      };
    }
  }
}
