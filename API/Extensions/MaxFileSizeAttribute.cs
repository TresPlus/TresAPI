using System.ComponentModel.DataAnnotations;

namespace API.Extensions
{
  public class MaxFileSizeAttribute : ValidationAttribute
  {
    private readonly int _maxFileSize;

    public MaxFileSizeAttribute(int maxFileSize)
    {
      _maxFileSize = maxFileSize;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value is IFormFile file)
      {
        if (file.Length > _maxFileSize)
        {
          return new ValidationResult($"Dosya boyutu en fazla {_maxFileSize / 1024 / 1024} MB olabilir");
        }
      }

      return ValidationResult.Success;
    }
    }
}
