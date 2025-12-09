namespace Entities
{
  public class OpenIDProviderConcrete
  {
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Authority { get; set; }
    public string CallbackPath { get; set; }
    public List<string> Scopes { get; set; }
  }

}
