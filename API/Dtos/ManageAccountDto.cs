using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
  public record ChangeEmailRequestDto
  {
    [Required, EmailAddress]
    public string NewEmail { get; init; } = null!;
  }
  public record ConfirmEmailChangeDto
  {
    [Required, EmailAddress]
    public string NewEmail { get; init; } = null!;

    [Required]
    public string Code { get; init; } = null!;
  }

  public record PhoneRequestDto
  {
    [Required]
    public string PhoneNumber { get; init; } = null!;
  }
  public record PhoneConfirmDto
  {
    [Required]
    public string PhoneNumber { get; init; } = null!;

    [Required]
    public string Code { get; init; } = null!;
  }

  public record EnableAuthenticatorDto
  {
    [Required]
    public string Code { get; init; } = null!;
  }

  public record AddExternalLoginDto
  {
    [Required]
    public string Provider { get; init; } = null!;
  }
  public record RemoveExternalLoginDto
  {
    [Required]
    public string LoginProvider { get; init; } = null!;

    [Required]
    public string ProviderKey { get; init; } = null!;
  }

  public record SetPasswordDto
  {
    [Required, MinLength(8)]
    public string NewPassword { get; init; } = null!;
  }
  public record ChangePasswordDto
  {
    [Required]
    public string CurrentPassword { get; init; } = null!;

    [Required, MinLength(8)]
    public string NewPassword { get; init; } = null!;
  }
  public record ReAuthDto
  {
    [Required]
    public string Password { get; init; } = null!;
  }
  public record SetPrimaryLoginDto
  {
    [Required]
    public string Provider { get; init; } = null!;
  }
  public record SecurityEventDto(Guid Id, DateTime CreatedAt, string UserId = null!, string Event = null!, string IP = null!);

}
