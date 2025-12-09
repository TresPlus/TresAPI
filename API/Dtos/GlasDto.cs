using Entities;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
  public class CreateGlasDto
  {
    [Required]
    public Guid UserId { get; set; }

    [MaxLength(120)]
    public string? Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public IFormFile? File { get; set; }
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
