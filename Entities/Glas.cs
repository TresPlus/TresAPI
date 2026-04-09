using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entities
{
  public class Glas
  {
    [Key]
    public Guid GlasId { get; set; }

    [Required]
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    [MaxLength(120)]
    public string? Title { get; set; }

    [Required]
    [MaxLength(280)]
    public string Description { get; set; } = null!;

    [MaxLength(2048)]
    public string? SourceURL { get; set; }

    [MaxLength(2048)]
    public string? ImageUrl { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [MaxLength(256)]
    public string? LocationAddress { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
  }
}
