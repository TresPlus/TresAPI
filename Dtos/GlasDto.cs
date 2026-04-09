  using System.ComponentModel.DataAnnotations;

  namespace Dtos
  {
    public class CreateGlasDto
    {
      public string? Title { get; set; }

      [Required, MaxLength(280)]
      public string Description { get; set; } = null!;

      public IFormFile? File { get; set; }

      public string? SourceURL { get; set; }

      public double? Latitude { get; set; }
      public double? Longitude { get; set; }
      public string? LocationAddress { get; set; }
    }

    public class UpdateGlasDto
    {
      [Required]
      public Guid GlasId { get; set; }

      [MaxLength(120)]
      public string? Title { get; set; }

      [MaxLength(500)]
      public string? Description { get; set; }

      public IFormFile? File { get; set; }
    }
  }
