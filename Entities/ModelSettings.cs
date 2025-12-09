namespace Entities
{
  public class ModelSettings
  {
    public Guid Id { get; set; }
    public Guid Model3DId { get; set; }
    public Model3D Model3D { get; set; } = null!;

    // JSON saklanabilecek alanlar (DB'ye göre jsonb / nvarchar(max) seçimi yapılabilir)
    public string? CameraPositionJson { get; set; }
    public string? LightSettingsJson { get; set; }
    public string? BackgroundColor { get; set; }
    public bool AutoRotate { get; set; } = false;
  }
}
