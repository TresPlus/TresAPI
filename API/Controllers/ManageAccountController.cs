using API.Dtos;
using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/manage")]
  public class ManageAccountController(
    IUserService userService,
    UserManager<Entities.AppUser> userManager,
    SignInManager<Entities.AppUser> signInManager
  ) : ControllerBase
  {
    private readonly IUserService _userService = userService;
    private readonly UserManager<Entities.AppUser> _userManager = userManager;
    private readonly SignInManager<Entities.AppUser> _signInManager = signInManager;

    // =========================================================
    // ACCOUNT OVERVIEW
    // =========================================================
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      return Ok(new
      {
        id = user.Id,
        userName = user.UserName,
        email = user.Email,
        emailConfirmed = user.EmailConfirmed,
        phoneNumber = user.PhoneNumber,
        phoneNumberConfirmed = user.PhoneNumberConfirmed,
        twoFactorEnabled = user.TwoFactorEnabled,
        createdAt = user.CreatedAt,
        profilePicturePath = $"{Request.Scheme}://{Request.Host}{user.ProfilePicturePath}",
        TwoFactorProviders = await _userManager.GetValidTwoFactorProvidersAsync(user),
        RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
      });
    }

    // =========================================================
    // EMAIL CHANGE
    // =========================================================
    [HttpPost("email/change-request")]
    public async Task<IActionResult> RequestEmailChange([FromBody] ChangeEmailRequestDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.NewEmail);

      // burada mail gönder
      // token frontend'e gönderilmez

      return Ok(new { message = "Confirmation code sent." });
    }

    [HttpPost("email/confirm")]
    public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var result = await _userManager.ChangeEmailAsync(user, dto.NewEmail, dto.Code);
      if (!result.Succeeded)
        return BadRequest(result.Errors);

      return Ok(new { message = "Email updated." });
    }

    // =========================================================
    // PHONE NUMBER
    // =========================================================
    [HttpPost("phone/request")]
    public async Task<IActionResult> RequestPhoneChange([FromBody] PhoneRequestDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, dto.PhoneNumber);

      // SMS gönder

      return Ok(new { message = "Verification code sent." });
    }

    [HttpPost("phone/confirm")]
    public async Task<IActionResult> ConfirmPhoneChange([FromBody] PhoneConfirmDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var result = await _userManager.ChangePhoneNumberAsync(user, dto.PhoneNumber, dto.Code);
      if (!result.Succeeded)
        return BadRequest(result.Errors);

      return Ok(new { message = "Phone number updated." });
    }

    // =========================================================
    // 2FA - AUTHENTICATOR SETUP
    // =========================================================
    [HttpPost("2fa/authenticator/setup")]
    public async Task<IActionResult> SetupAuthenticator()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var key = await _userManager.GetAuthenticatorKeyAsync(user);
      if (string.IsNullOrEmpty(key))
      {
        await _userManager.ResetAuthenticatorKeyAsync(user);
        key = await _userManager.GetAuthenticatorKeyAsync(user);
      }

      var uri = GenerateOtpUri(user.Email!, key);

      return Ok(new
      {
        secretKey = key,
        qrUri = uri
      });
    }

    [HttpPost("2fa/authenticator/enable")]
    public async Task<IActionResult> EnableAuthenticator([FromBody] EnableAuthenticatorDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var valid = await _userManager.VerifyTwoFactorTokenAsync(
        user,
        TokenOptions.DefaultAuthenticatorProvider,
        dto.Code
      );

      if (!valid)
        return BadRequest(new { message = "Invalid code." });

      await _userManager.SetTwoFactorEnabledAsync(user, true);

      var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

      return Ok(new
      {
        message = "2FA enabled.",
        recoveryCodes
      });
    }

    // =========================================================
    // 2FA DISABLE
    // =========================================================
    [HttpPost("2fa/disable")]
    public async Task<IActionResult> DisableTwoFactor()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      await _userManager.SetTwoFactorEnabledAsync(user, false);
      await _userManager.ResetAuthenticatorKeyAsync(user);
      await _signInManager.ForgetTwoFactorClientAsync();

      return Ok(new { message = "2FA disabled." });
    }

    // =========================================================
    // RECOVERY CODES
    // =========================================================
    [HttpPost("2fa/recovery-codes")]
    public async Task<IActionResult> RegenerateRecoveryCodes()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var codes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

      return Ok(new { recoveryCodes = codes });
    }

    // =========================================================
    // HELPERS
    // =========================================================
    private string GenerateOtpUri(string email, string key)
    {
      var issuer = "YourApp";
      return $"otpauth://totp/{issuer}:{email}?secret={key}&issuer={issuer}&digits=6";
    }

    // =========================================================
    // EXTERNAL LOGINS - LIST
    // =========================================================
    [HttpGet("external-logins")]
    public async Task<IActionResult> GetExternalLogins()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var logins = await _userManager.GetLoginsAsync(user);

      return Ok(logins.Select(l => new
      {
        l.LoginProvider,
        l.ProviderDisplayName
      }));
    }

    // =========================================================
    // EXTERNAL LOGINS - ADD (REDIRECT)
    // =========================================================
    [HttpPost("external-logins/add")]
    public async Task<IActionResult> AddExternalLogin([FromBody] AddExternalLoginDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var redirectUrl = Url.Action(
        nameof(AddExternalLoginCallback),
        "ManageAccount",
        values: null,
        protocol: Request.Scheme
      );

      var properties = _signInManager.ConfigureExternalAuthenticationProperties(
        dto.Provider,
        redirectUrl,
        user.Id.ToString()
      );

      return Challenge(properties, dto.Provider);
    }

    // =========================================================
    // EXTERNAL LOGINS - CALLBACK
    // =========================================================
    [HttpGet("external-logins/callback")]
    public async Task<IActionResult> AddExternalLoginCallback()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var info = await _signInManager.GetExternalLoginInfoAsync(user.Id.ToString());
      if (info == null)
        return BadRequest(new { message = "External login info not found." });

      var result = await _userManager.AddLoginAsync(user, info);
      if (!result.Succeeded)
        return BadRequest(result.Errors);

      await _signInManager.RefreshSignInAsync(user);
      return Ok(new { message = "External login added." });
    }

    // =========================================================
    // EXTERNAL LOGINS - REMOVE
    // =========================================================
    [HttpPost("external-logins/remove")]
    public async Task<IActionResult> RemoveExternalLogin([FromBody] RemoveExternalLoginDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var result = await _userManager.RemoveLoginAsync(
        user,
        dto.LoginProvider,
        dto.ProviderKey
      );

      if (!result.Succeeded)
        return BadRequest(result.Errors);

      await _signInManager.RefreshSignInAsync(user);
      return Ok(new { message = "External login removed." });
    }

    [HttpGet("password/status")]
    public async Task<IActionResult> PasswordStatus()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      return Ok(new
      {
        hasPassword = await _userManager.HasPasswordAsync(user)
      });
    }

    [HttpPost("password/set")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      if (await _userManager.HasPasswordAsync(user))
        return BadRequest(new { message = "Password already exists." });

      var result = await _userManager.AddPasswordAsync(user, dto.NewPassword);
      if (!result.Succeeded)
        return BadRequest(result.Errors);

      await _signInManager.RefreshSignInAsync(user);
      return Ok(new { message = "Password set." });
    }

    [HttpPost("password/change")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var result = await _userManager.ChangePasswordAsync(
        user,
        dto.CurrentPassword,
        dto.NewPassword
      );

      if (!result.Succeeded)
        return BadRequest(result.Errors);

      await _signInManager.RefreshSignInAsync(user);
      return Ok(new { message = "Password updated." });
    }

    [HttpGet("external-logins/available")]
    public async Task<IActionResult> AvailableExternalLogins()
    {
      var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();

      return Ok(schemes.Select(s => new
      {
        s.Name,
        s.DisplayName
      }));
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> Sessions()
    {
      var sessions = await _userService.GetActiveSessionsAsync(User);
      return Ok(sessions);
    }

    [HttpPost("sessions/logout-others")]
    public async Task<IActionResult> LogoutOtherSessions()
    {
      var sidClaim = User.FindFirst("sid")?.Value;

      if (string.IsNullOrEmpty(sidClaim))
        return Unauthorized(new { message = "Session not found." });

      var currentSessionId = Guid.Parse(sidClaim);

      var result = await _userService.RevokeOtherSessionsAsync(
        User,
        currentSessionId
      );

      if (!result.Succeeded)
        return BadRequest(result.Errors);

      return Ok(new { message = "Other sessions revoked." });
    }

    [HttpPost("reauth")]
    public async Task<IActionResult> ReAuthenticate([FromBody] ReAuthDto dto)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      var result = await _signInManager.CheckPasswordSignInAsync(
        user,
        dto.Password,
        false
      );

      if (!result.Succeeded)
        return Unauthorized(new { message = "Re-authentication failed." });

      return Ok(new { message = "Re-authenticated." });
    }

    [HttpGet("security-log")]
    public async Task<IActionResult> SecurityLog()
    {
      var logs = await _userService.GetSecurityEventsAsync(User);
      return Ok(logs);
    }

    [HttpPost("deactivate")]
    public async Task<IActionResult> DeactivateAccount()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      user.LockoutEnabled = false;
      await _userManager.UpdateAsync(user);

      await _signInManager.SignOutAsync();
      return Ok(new { message = "Account deactivated." });
    }

    [HttpPost("delete")]
    public async Task<IActionResult> DeleteAccount()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return Unauthorized();

      await _userManager.DeleteAsync(user);
      return Ok(new { message = "Account deleted." });
    }

  }
}
