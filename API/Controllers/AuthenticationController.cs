using API.Dtos;
using API.Extensions;
using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthenticationController(IAuthService authService,
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

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Email ile mi username ile mi girdi?
      var user = await _authService.GetByEmail(model.EmailOrUserName);
      if (user == null)
        user = await _authService.GetByName(model.EmailOrUserName);

      if (user == null)
        return Unauthorized(new { message = "Invalid credentials." });

      // Password doğrula
      var passwordCheck = await _authService.CheckPasswordAsync(user, model.Password);
      if (!passwordCheck.Succeeded)
        return Unauthorized(new { message = "Invalid credentials." });

      // Email confirmed değilse
      var emailConfirmed = await _authService.IsEmailConfirmedAsync(user);
      if (!emailConfirmed)
        return Unauthorized(new { message = "Please confirm your email first." });

      // JWT üret
      var token = await _authService.GenerateJwtToken(user);

      _logger.LogInformation("User logged in successfully.");

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

  }
}
