using System.ComponentModel.DataAnnotations;

namespace Entities
{
  public class GlasReply
  {
    [Key]
    public Guid ReplyId { get; set; }

    public Guid GlasId { get; set; }
    public Glas Glas { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
  }
}
