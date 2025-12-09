using System.ComponentModel.DataAnnotations;

namespace API.Extensions
{
  public class AllowedModelExtensionsAttribute : ValidationAttribute
  {
    private readonly string[] _extensions;

    public AllowedModelExtensionsAttribute(string[] extensions)
    {
      _extensions = extensions;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value is IFormFile file)
      {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!_extensions.Contains(extension))
        {
          return new ValidationResult($"İzin verilen formatlar: {string.Join(", ", _extensions)}");
        }
      }

      return ValidationResult.Success;
    }
  }
}
