namespace Entities
{
  public class AuthProvidersConfig
  {
    public Dictionary<string, OAuthProviderConcrete> OAuthProviders { get; set; }
    public Dictionary<string, OpenIDProviderConcrete> OpenIdConnectProviders { get; set; }
  }
}
