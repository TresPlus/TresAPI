namespace Entities
{
  public class ModelConvertedFile
  {
    public Guid Id { get; set; }
    public Guid Model3DId { get; set; }
    public Model3D Model3D { get; set; } = null!;

    public int VersionNumber { get; set; }
    public string Type { get; set; } = null!; // thumbnail, lod1, usdz, optimized_glb vb.
    public string StoragePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  }

}
