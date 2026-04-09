using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions;

public static class JwtIdentityServiceCollectionExtensions
{
  public static AuthenticationBuilder AddJwtIdentity(
    this IServiceCollection services,
    IConfiguration config)
  {
    var signingKey = new SymmetricSecurityKey(
      Convert.FromBase64String(config["Jwt:Key"])
    );

    var authBuilder = services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    });

    authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = signingKey
      };

      options.Events = new JwtBearerEvents
      {
        OnChallenge = ctx =>
        {
          ctx.HandleResponse();
          ctx.Response.StatusCode = 401;
          ctx.Response.ContentType = "application/json";

          return ctx.Response.WriteAsync(
            "{\"success\":false,\"message\":\"Unauthorized\"}"
          );
        }
      };
    });

    return authBuilder;
  }
}
