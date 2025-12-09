namespace Entities
{
  public class ModelShare
  {
    public Guid Id { get; set; }

    public Guid Model3DId { get; set; }
    public Model3D Model3D { get; set; } = null!;

    public Guid SharedWithUserId { get; set; }
    public AppUser SharedWithUser { get; set; } = null!;

    // view / edit
    public string Permission { get; set; } = "view";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  }

  public class ModelShareLink
  {
    public Guid Id { get; set; }
    public Guid Model3DId { get; set; }
    public Model3D Model3D { get; set; } = null!;

    public string Token { get; set; } = null!; // paylaşım için rastgele token
    public DateTimeOffset? ExpiresAt { get; set; }
    public string Permission { get; set; } = "view";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  }
}
