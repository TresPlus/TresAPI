using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddTresPlusIdentityServices(this IServiceCollection services)
    {
      services.AddScoped<IUserRepository, UserRepository>();

      services.AddScoped<IUserService, UserService>();
      services.AddScoped<IAuthService, AuthService>();
      services.AddScoped<IGlasService, GlasService>();
      services.AddScoped<ISocialService, SocialService>();
      services.AddHostedService<UserDeletionBackgroundService>();
      services.AddTransient<IEmailSender, EmailSender>();
      services.AddScoped<FileStorage>();

      return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
      var key = config["Jwt:Key"];
      var issuer = config["Jwt:Issuer"];
      var audience = config["Jwt:Audience"];
      var algorithm = config["Jwt:Algorithm"]?.ToUpper() ?? "HS256";

      var signingAlgorithm = algorithm switch
      {
        "HS256" => SecurityAlgorithms.HmacSha256,
        "HS384" => SecurityAlgorithms.HmacSha384,
        "HS512" => SecurityAlgorithms.HmacSha512,
        _ => SecurityAlgorithms.HmacSha256
      };

      var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

      services
          .AddAuthentication(options =>
          {
            options.DefaultAuthenticateScheme = "JwtBearer";
            options.DefaultChallengeScheme = "JwtBearer";
          })
          .AddJwtBearer("JwtBearer", options =>
          {
            options.TokenValidationParameters = new TokenValidationParameters
            {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,

              ValidIssuer = issuer,
              ValidAudience = audience,
              IssuerSigningKey = signingKey
            };
          });

      services.AddSingleton(new SigningCredentials(signingKey, signingAlgorithm));

      return services;
    }
  }
}
