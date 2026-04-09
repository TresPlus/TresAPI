using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
  public class RegisterDto
  {
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, MinLength(3)]
    public string UserName { get; set; }

    [Required, MinLength(6)]
    public string Password { get; set; }

    public IFormFile? ProfilePicture { get; set; }
  }
  public class LoginDto
  {
    [Required]
    public string EmailOrUserName { get; set; }

    [Required]
    public string Password { get; set; }

    public bool RememberMe { get; set; } = false;
  }
  public class TwoFactorDto
  {
    public Guid UserId { get; set; }
    [Required]
    public string Provider { get; set; }

    [Required]
    public string Code { get; set; }

    public bool RememberMe { get; set; } = false;
    public bool RememberMachine { get; set; } = false;
  }

  public class ForgotPasswordDto
  {
    [Required, EmailAddress]
    public string Email { get; set; }
  }
  public class ResetPasswordDto
  {
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }

    [Required, MinLength(6)]
    public string NewPassword { get; set; }
  }
  public class ResendConfirmationDto
  {
    [Required, EmailAddress]
    public string Email { get; set; }
  }
  public class RecoveryLoginDto
  {
    [Required]
    public string RecoveryCode { get; set; }
  }
  public class ConfirmEmailDto
  {
    [Required]
    public string UserId { get; set; }

    [Required]
    public string Code { get; set; }
  }
}
