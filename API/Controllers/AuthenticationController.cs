using API.Dtos;
using API.Extensions;
using Business.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthenticationController(
    IAuthService authService,
        IUserService userService,
        IEmailSender emailSender,
        FileStorage profileUpdate,
        ILogger<AuthenticationController> logger) : ControllerBase
  {
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly FileStorage _profileUpdate = profileUpdate;
    private readonly ILogger<AuthenticationController> _logger = logger;

    // ---------------------------------------------
    // POST /api/auth/register
    // ---------------------------------------------
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterDto model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Email check
      var emailCheck = await _authService.EmailAlreadyExists(model.Email);
      if (!emailCheck.Succeeded)
        return BadRequest(new { message = "Email already exists." });

      // Username check
      var usernameCheck = await _authService.UserNameAlreadyExists(model.UserName);
      if (!usernameCheck.Succeeded)
        return BadRequest(new { message = "Username already exists." });

      // Save profile picture (optional)
      string profilePath = null;
      if (model.ProfilePicture != null)
        profilePath = await _profileUpdate.SaveProfilePicture(model.ProfilePicture);

      // Create user
      var result = await _authService.RegisterAsync(profilePath, model.Email, model.Password, model.UserName);

      if (!result.Succeeded)
        return BadRequest(result.Errors);

      var user = await _authService.GetByEmail(model.Email);

      await _userService.UploadPicture(user, profilePath);

      _logger.LogInformation("User created a new account.");

      // -----------------------------------------------------
      // Email Confirmation URL
      // -----------------------------------------------------
      var userId = await _authService.GetUserId(user);
      var code = await _authService.GenerateUserConfirmationToken(user);
      var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

      // API olduğu için callbackUrl frontende gider!
      var callbackUrl = $"{Request.Scheme}://{Request.Host}/confirm-email?userId={userId}&code={encodedCode}";

      await _emailSender.SendEmailAsync(model.Email, "Confirm your email", $@"Lütfen doğrulamak için tıklayın: {callbackUrl}");

      return Ok(new
      {
        message = "Registration successful. Please check your email to confirm your account.",
        email = model.Email
      });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var user = await _authService.GetByEmail(model.EmailOrUserName)
               ?? await _authService.GetByName(model.EmailOrUserName);

      if (user == null)
        return Unauthorized(new { message = "Invalid credentials." });

      var result = await _authService.PasswordSignInAsync(
        user.UserName,
        model.Password,
        model.RememberMe);

      if (result.RequiresTwoFactor)
      {
        var providers = await _authService.GetEnabledTwoFactorProvidersAsync(user);

        // SADECE email/sms ise kod gönder
        if (providers.Contains("email"))
        {
          var code = await _authService.GenerateTwoFactorCode(
            user,
            TokenOptions.DefaultEmailProvider);

          await _emailSender.SendEmailAsync(
            user.Email,
            "2FA Code",
            $"Kodunuz: {code}");
        }

        if (providers.Contains("sms"))
        {
          var code = await _authService.GenerateTwoFactorCode(
            user,
            TokenOptions.DefaultPhoneProvider);

        }

        return Ok(new
        {
          requires2FA = true,
          providers
        });
      }

      if (!result.Succeeded)
        return Unauthorized(new { message = "Invalid credentials." });

      var SessionId = await _userService.CreateSessionAsync(user, CreateSession.CreateUserSessionEntity(user, HttpContext));

      var token = await _authService.GenerateJwtToken(user,SessionId);

      return Ok(new
      {
        accessToken=token,
        user = new
        {
          id = user.Id,
          Email = user.Email,
          UserName = user.UserName,
          ProfilePicture = $"{Request.Scheme}://{Request.Host}{user.ProfilePicturePath}"
        }
      });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var user = await _authService.GetByEmail(model.Email);

      if (user == null || !await _authService.IsEmailConfirmedAsync(user))
        return Ok(); // enumeration yok

      var token = await _authService.GeneratePasswordResetTokenAsync(user);
      var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

      var url =
        $"{Request.Scheme}://{Request.Host}/reset-password?email={user.Email}&code={encoded}";

      await _emailSender.SendEmailAsync(
        user.Email,
        "Reset password",
        $"Link: {url}");

      return Ok(new { message = "If the email exists, instructions were sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var user = await _authService.GetByEmail(model.Email);
      if (user == null)
        return BadRequest(new { message = "Invalid request." });

      var code =
        Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));

      var result = await _authService.ResetPassword(
        user,
        code, 
        model.NewPassword);

      if (!result.Succeeded)
        return BadRequest(result.Errors);

      return Ok(new { message = "Password reset successful." });
    }

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationDto model)
    {
      var user = await _authService.GetByEmail(model.Email);
      if (user == null)
        return Ok();

      if (await _authService.IsEmailConfirmedAsync(user))
        return BadRequest(new { message = "Email already confirmed." });

      var code = await _authService.GenerateUserConfirmationToken(user);
      var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

      var url =
        $"{Request.Scheme}://{Request.Host}/confirm-email?userId={user.Id}&code={encoded}";

      await _emailSender.SendEmailAsync(user.Email, "Confirm email", url);

      return Ok(new { message = "Confirmation email sent." });
    }

    [HttpPost("login-recovery")]
    public async Task<IActionResult> LoginWithRecovery([FromBody] RecoveryLoginDto model)
    {
      var result =
        await _authService.TwoFactorRecoveryCodeSignInAsync(model.RecoveryCode);

      if (!result.Succeeded)
        return Unauthorized();

      var user = await _authService.GetTwoFactorAuthenticationUserAsync();

      var SessionId = await _userService.CreateSessionAsync(user, CreateSession.CreateUserSessionEntity(user, HttpContext));

      var token = await _authService.GenerateJwtToken(user,SessionId);

      return Ok(new { token });
    }


    [HttpPost("confirm-2fa")]
    public async Task<IActionResult> ConfirmTwoFactor([FromForm] TwoFactorDto model)
    {
      var user = await _authService.GetById(model.UserId.ToString());
      if (user == null)
        return Unauthorized(new { message = "Invalid user." });

      var isValid = await _authService.TwoFactorAuthenticatorSignInAsync(
        model.Provider,
        model.Code,
        model.RememberMe,
        model.RememberMachine);
      if (!isValid.Succeeded)
        return Unauthorized(new { message = "Invalid 2FA code." });

      var SessionId = await _userService.CreateSessionAsync(user, CreateSession.CreateUserSessionEntity(user,HttpContext));

      var token = await _authService.GenerateJwtToken(user,SessionId);

      return Ok(new
      {
        token,
        user = new
        {
          id = user.Id,
          email = user.Email,
          username = user.UserName,
          photo = user.ProfilePicturePath
        }
      });
    }
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDto model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var user = await _authService.GetById(model.UserId);
      if (user == null)
        return BadRequest(new { message = "Invalid user." });

      var code =
        Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));

      var result = await _authService.confirmEmail(user, code);

      if (!result.Succeeded)
        return BadRequest(result.Errors);

      return Ok(new { message = "Email confirmed successfully." });
    }

  }
}
