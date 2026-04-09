namespace Entities
{
  public class SecurityEvent
  {
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Event { get; set; }
    public string Ip { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}
