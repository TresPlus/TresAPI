namespace Entities
{
  public class UserSession
  {
    public Guid Id { get; set; }              // Session ID
    public Guid UserId { get; set; }

    public string Device { get; set; } = null!;        // Raw UA
    public string OS { get; set; } = null!;             // Windows / Android / iOS
    public string Browser { get; set; } = null!;        // Chrome / Safari
    public string DeviceType { get; set; } = null!;     // Desktop / Mobile / Tablet

    public string IpAddress { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeen { get; set; }

    public AppUser User { get; set; } = null!;
  }
}
