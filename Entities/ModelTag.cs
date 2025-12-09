namespace Entities
{
  public class Tag
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public long UsageCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ModelTag> ModelTags { get; set; } = new List<ModelTag>();
  }

  public class ModelTag
  {
    public Guid Model3DId { get; set; }
    public Model3D Model3D { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
  }
}
