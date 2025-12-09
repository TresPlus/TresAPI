using API.Extensions;
using DataAccess.Interactive;
using Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

//using Microsoft.OpenApi;
using System.Security.Claims;

namespace API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
      builder.Services.AddDbContext<DataContext>(options =>
          options.UseSqlServer(connectionString));
      builder.Services.AddDatabaseDeveloperPageExceptionFilter();
      builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
      {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
      })
          .AddEntityFrameworkStores<DataContext>()
          .AddDefaultTokenProviders();
      builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
      {
        options.Name = builder.Configuration.GetSection("Environment:name").Value.ToString();
      });

      builder.Services.AddTresPlusIdentityServices();

      builder.Services.AddJwtAuthentication(builder.Configuration);

      var authProvidersConfig = builder.Configuration.GetSection("AuthProviders").Get<AuthProvidersConfig>();

      foreach (var provider in authProvidersConfig.OAuthProviders)
      {
        var name = provider.Key;
        var settings = provider.Value;

        builder.Services.AddAuthentication().AddOAuth(name, options =>
        {
          options.ClientId = settings.ClientId;
          options.ClientSecret = settings.ClientSecret;

          options.AuthorizationEndpoint = settings.AuthorizationEndpoint;
          options.TokenEndpoint = settings.TokenEndpoint;
          options.UserInformationEndpoint = settings.UserInformationEndpoint;

          options.CallbackPath = settings.CallbackPath;

          options.SaveTokens = true;

          options.Scope.Clear();
          foreach (var scope in settings.Scopes)
            options.Scope.Add(scope);

          options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
          options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
          options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        });
      }

      // OpenID Connect Providers ekleme
      foreach (var provider in authProvidersConfig.OpenIdConnectProviders)
      {
        var name = provider.Key;
        var settings = provider.Value;

        builder.Services.AddAuthentication().AddOpenIdConnect(name, options =>
        {
          options.ClientId = settings.ClientId;
          options.ClientSecret = settings.ClientSecret;
          options.Authority = settings.Authority;
          options.CallbackPath = settings.CallbackPath;

          options.SaveTokens = true;

          options.ResponseType = "code";

          options.Scope.Clear();
          foreach (var scope in settings.Scopes)
            options.Scope.Add(scope);

          // Claim mapping örneği
          options.TokenValidationParameters.NameClaimType = "name";
          options.TokenValidationParameters.RoleClaimType = "role";
        });
      }

      builder.Services.AddControllers();
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Name = "Authorization",
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Description = "JWT Bearer token. Örnek kullanım: \n\nBearer {token}"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
      });



      builder.Services.AddOpenApi();

      var app = builder.Build();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
          c.RoutePrefix = string.Empty;
        });
        app.MapOpenApi();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseAuthentication();
      app.UseAuthorization();


      app.MapControllers();

      app.Run();
    }
  }
}
