using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
  public class ModelLike
  {
    public Guid Model3DId { get; set; }
    public Model3D Model3D { get; set; } = null!;
    public required Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  }

  public class ModelComment
  {
    public Guid Id { get; set; }
    public Guid Model3DId { get; set; }
    public Model3D Model3D { get; set; } = null!;
    public required Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EditedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
  }

  public class UserFollow
  {
    [Key]
    public Guid UserFollowId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid FollowerUserId { get; set; }

    [Required]
    public Guid FollowedUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("FollowerUserId")]
    public virtual AppUser Follower { get; set; }

    [ForeignKey("FollowedUserId")]
    public virtual AppUser Followed { get; set; }
  }

  public class UserPreviewDto
  {
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string? VisibleName { get; set; }
    public string? ProfilePicturePath { get; set; }
  }

  public class ModelReport
  {
    public Guid Id { get; set; }
    public Guid Model3DId { get; set; }
    public Model3D Model3D { get; set; } = null!;
    public required Guid ReporterId { get; set; }
    public AppUser Reporter { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string? Details { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsResolved { get; set; } = false;
  }
}
