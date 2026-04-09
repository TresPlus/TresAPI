namespace Entities
{
  public class GlasLike
  {
    public Guid GlasId { get; set; }
    public Glas Glas { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
