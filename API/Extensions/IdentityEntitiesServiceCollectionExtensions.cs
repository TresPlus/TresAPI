using DataAccess.Interactive;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class IdentityStoreExtensions
{
  public static IServiceCollection AddIdentityEntitiesStore(
    this IServiceCollection services,
    IConfiguration config)
  {
    var connectionString =
      config.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

    // DbContext
    services.AddDbContext<DataContext>(options =>
      options.UseSqlServer(connectionString));

    services.AddDatabaseDeveloperPageExceptionFilter();

    // Identity
    services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
    {
      options.SignIn.RequireConfirmedAccount = true;
      options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

    // Token provider name (multi-env safe)
    services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
      options.Name =
        config["Environment:name"] ?? "Default";
    });

    return services;
  }
}
