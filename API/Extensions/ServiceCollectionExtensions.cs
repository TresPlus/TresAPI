using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddTresDbServices(this IServiceCollection services)
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
  }
}
