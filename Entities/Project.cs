namespace Entities
{
  public class Project
  {
    public Guid Id { get; set; }

    public Guid? OwnerId { get; set; }
    public AppUser Owner { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Model3D> Models { get; set; } = new List<Model3D>();
    public ICollection<ProjectCollaborator> Collaborators { get; set; } = new List<ProjectCollaborator>();
  }

  public class ProjectCollaborator
  {
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid? UserId { get; set; }
    public AppUser User { get; set; } = null!;

    // owner / editor / viewer
    public string Role { get; set; } = "viewer";
  }
}
