using System.Text.Json.Serialization;

namespace Entities
{
  public class Glas
  {
    public Guid GlasId { get; set; }
    public AppUser User { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string SourceURL { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
  }
}
