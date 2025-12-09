namespace Entities
{
  public class ModelVersion
  {
    public Guid Id { get; set; }
    public Guid Model3DId { get; set; }
    public Model3D Model3D { get; set; } = null!;

    public int VersionNumber { get; set; }
    public string StoragePath { get; set; } = null!;
    public string Format { get; set; } = null!;
    public long FileSize { get; set; }
    public string? ChecksumSha256 { get; set; }

    public bool IsCurrent { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  }
}
