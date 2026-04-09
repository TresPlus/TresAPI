using Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace API.Extensions
{
  public static class ExternalProvidersServiceCollectionExtensions
  {
    public static AuthenticationBuilder AddExternalProviders(
      this AuthenticationBuilder authBuilder,
      IConfiguration config)
    {
      var authProvidersConfig =
        config.GetSection("AuthProviders")
              .Get<AuthProvidersConfig>();

      // OAuth
      foreach (var provider in authProvidersConfig.OAuthProviders)
      {
        authBuilder.AddOAuth(provider.Key, options =>
        {
          options.ClientId = provider.Value.ClientId;
          options.ClientSecret = provider.Value.ClientSecret;

          options.AuthorizationEndpoint = provider.Value.AuthorizationEndpoint;
          options.TokenEndpoint = provider.Value.TokenEndpoint;
          options.UserInformationEndpoint = provider.Value.UserInformationEndpoint;
          options.CallbackPath = provider.Value.CallbackPath;

          options.SaveTokens = true;

          options.Scope.Clear();
          foreach (var scope in provider.Value.Scopes)
            options.Scope.Add(scope);

          options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
          options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
          options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        });
      }

      // OpenId
      foreach (var provider in authProvidersConfig.OpenIdConnectProviders)
      {
        authBuilder.AddOpenIdConnect(provider.Key, options =>
        {
          options.ClientId = provider.Value.ClientId;
          options.ClientSecret = provider.Value.ClientSecret;

          options.Authority = provider.Value.Authority;
          options.CallbackPath = provider.Value.CallbackPath;

          options.ResponseType = "code";
          options.SaveTokens = true;

          options.Scope.Clear();
          foreach (var scope in provider.Value.Scopes)
            options.Scope.Add(scope);

          options.TokenValidationParameters = new TokenValidationParameters
          {
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
          };
        });
      }

      return authBuilder;
    }
  }
}
