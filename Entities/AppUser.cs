using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entities
{
  public class AppUser : IdentityUser<Guid>
  {
    public DateOnly? DeletionRequestedAt { get; set; }
    public DateOnly? CreatedAt { get; set; }
    public string? ProfilePicturePath { get; set; }
    public string? Bio { get; set; }
    public bool OnlineState { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<ProjectCollaborator> ProjectCollaborations { get; set; } = new List<ProjectCollaborator>();
    public ICollection<ModelShare> SharedModels { get; set; } = new List<ModelShare>();

    // Sosyal ilişkiler
    public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
    public ICollection<ModelLike> Likes { get; set; } = new List<ModelLike>();
    public ICollection<ModelComment> Comments { get; set; } = new List<ModelComment>();

    [Timestamp]
    public byte[]? RowVersion { get; set; }

  }
}
