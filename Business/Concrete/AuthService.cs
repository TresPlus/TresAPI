using Business.Abstract;
using DataAccess.Abstract;
using Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Business.Static;

namespace Business.Concrete
{
  public class AuthService(IUserRepository userRepository, IConfiguration conf) : IAuthService
  {
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _conf = conf;

public async Task<string> GenerateJwtToken(
  AppUser user,
  Guid sessionId)
{
  var key = new SymmetricSecurityKey(
    Convert.FromBase64String(_conf["Jwt:Key"])
  );

  var algorithm = _conf.GetJwtAlgorithm();
  var creds = new SigningCredentials(key, algorithm);

  var claims = await _userRepository.GetClaimsAsync(user);

  claims.Add(new Claim("sid", sessionId.ToString()));

  var token = new JwtSecurityToken(
    issuer: _conf["Jwt:Issuer"],
    audience: _conf["Jwt:Audience"],
    claims: claims,
    expires: DateTime.UtcNow.AddDays(7),
    signingCredentials: creds
  );

  return new JwtSecurityTokenHandler().WriteToken(token);
}


    public async Task<IdentityResult> CheckPasswordAsync(AppUser user, string Password)
    {
      bool passwordValid = await _userRepository.CheckPasswordAsync(user, Password);

      if (passwordValid)
      {
        return IdentityResult.Success;
      }
      else
      {
        var error = new IdentityError { Description = "Geçersiz parola." };
        return IdentityResult.Failed(error);
      }
    }
    public Task<string> GenerateUserConfirmationToken(AppUser user)
    {
      return _userRepository.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<AppUser> GetByEmail(string Email)
    {
      return await _userRepository.GetUserByEmail(Email);
    }

    public async Task<AppUser> GetById(string Id)
    {
      return await _userRepository.GetUserById(Id);
    }

    public async Task<AppUser> GetByName(string Name)
    {
      return await _userRepository.GetUserByName(Name);
    }

    public async Task<string> GetUserId(AppUser user)
    {
      return await _userRepository.GetUserIdAsync(user);
    }

    public IList<AuthenticationScheme> GetExternalAuthenticationScheme()
    {
      return _userRepository.GetAuthenticationSchemes();
    }

    public async Task<IdentityResult> RegisterAsync(AppUser user, ExternalLoginInfo Info)
    {
      var existingUser = await _userRepository.GetUserByName(user.UserName);
      if (existingUser != null)
      {
        var addLoginResult = await _userRepository.AddLoginAsync(existingUser, Info);
        return addLoginResult;
      }

      await _userRepository.SetUserNameAsync(user, user.UserName, CancellationToken.None);
      await _userRepository.SetEmailAsync(user, user.Email, CancellationToken.None);

      var createResult = await _userRepository.CreateAsync(user);
      if (!createResult.Succeeded)
      {
        return createResult;
      }

      var addLoginResult2 = await _userRepository.AddLoginAsync(user, Info);
      if (!addLoginResult2.Succeeded)
      {
        return addLoginResult2;
      }

      var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);

      return IdentityResult.Success;
    }

    public void SignIn(AppUser user, bool isPersistent = false)
    {
      _userRepository.Login(user, isPersistent);
    }

    public bool RequiresConfirmedAccount()
    {
      return _userRepository.RequiresConfirmedAccount();
    }

    public IUserEmailStore<AppUser> GetEmailStore()
    {
      return _userRepository.GetEmailStore();
    }

    public Task<IdentityResult> confirmEmail(AppUser user, string code)
    {
      return _userRepository.confirmEmail(user, code);
    }

    public Task RefreshSignIn(AppUser user)
    {
      return _userRepository.RefreshSignIn(user);
    }

    public Task<AuthenticationProperties> AuthenticateProps(string provider, string RedirectURL)
    {
      return Task.FromResult(_userRepository.AuthenticateProps(provider, RedirectURL));
    }

    public Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
    {
      return _userRepository.GetExternalLoginInfoAsync();
    }

    public Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo Info, bool isPersistent, bool bypassTwoFactor)
    {
      return _userRepository.ExternalLoginSignInAsync(Info, isPersistent, bypassTwoFactor);
    }

    public Task<IdentityResult> AddLoginAsync(AppUser user, ExternalLoginInfo Info)
    {
      return _userRepository.AddLoginAsync(user, Info);
    }

    public Task SetEmail(AppUser user, string Email)
    {
      return _userRepository.SetEmailAsync(user, Email, CancellationToken.None);
    }

    public Task SetName(AppUser user, string Name)
    {
      return _userRepository.SetUserNameAsync(user, Name, CancellationToken.None);
    }

    public Task<SignInResult> SignIn(string Email, string Password, bool RememberMe, bool lockoutOnFailure)
    {
      return _userRepository.SignIn(Email, Password, RememberMe, lockoutOnFailure);
    }

    public async Task<bool> IsEmailConfirmedAsync(AppUser user)
    {
      return await _userRepository.IsEmailConfirmedAsync(user);
    }

    public Task<string> GeneratePasswordResetTokenAsync(AppUser user)
    {
      return _userRepository.GeneratePasswordResetTokenAsync(user);
    }

    public Task<IdentityResult> ResetPassword(AppUser user, string Token, string NewPassword)
    {
      return _userRepository.ResetPassword(user, Token, NewPassword);
    }

    public void SignOut()
    {
      _userRepository.SignOut();
    }

    public bool IsSignedIn(ClaimsPrincipal principal)
    {
      return _userRepository.IsSignedIn(principal);
    }

    public async Task<IdentityResult> UserNameAlreadyExists(string Username)
    {
      var existingUser = await _userRepository.GetUserByName(Username);
      if (existingUser != null)
      {
        return IdentityResult.Failed(new IdentityError { Code = "Duplicate UserName", Description = "This Name is Already Existing" });
      }
      else
      {
        return IdentityResult.Success;
      }
    }

    public async Task<IdentityResult> EmailAlreadyExists(string Email)
    {
      var existingUser = await _userRepository.GetUserByEmail(Email);
      if (existingUser != null)
      {
        return IdentityResult.Failed(new IdentityError { Code = "Duplicate Email", Description = "This Email is Already Existing" });
      }
      else
      {
        return IdentityResult.Success;
      }
    }

    public async Task<IdentityResult> RegisterAsync(string ProfilePicturePath, string Email, string Password, string UserName)
    {
      var existingUser = await _userRepository.GetUserByEmail(Email);
      if (existingUser != null)
      {
        return IdentityResult.Failed(
            new IdentityError
            {
              Code = "EmailAlreadyUsed",
              Description = "This email is already in use"
            }
        );
      }

      var user = new AppUser();


      await _userRepository.SetUserNameAsync(user, UserName, CancellationToken.None);
      await _userRepository.SetEmailAsync(user, Email, CancellationToken.None);

      var result = await _userRepository.CreateAsync(user, Password);
      if (!result.Succeeded)
      {
        return result;
      }
      var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName ?? ""),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
    };
      var ClaimResult = await _userRepository.AddClaimsInDbAsync(user, claims);

      var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);

      if (!result.Succeeded || !ClaimResult.Succeeded)
      {
        return IdentityResult.Failed(
            new IdentityError
            {
              Code = result.Succeeded ? "ClaimError" : "RegisterError",
              Description = result.Succeeded
                    ? string.Join(" | ", ClaimResult.Errors.Select(e => e.Description))
                    : string.Join(" | ", result.Errors.Select(e => e.Description))
            }
        );
      }

      return result;
    }

    public Task<AppUser> GetTwoFactorAuthenticationUserAsync()
    {
      return _userRepository.GetTwoFactorAuthenticationUserAsync();
    }

    public Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
    {
      return _userRepository.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
    }

    public async Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string Provider,string authenticatorCode, bool rememberMe, bool RememberMachine)
    {
      return await _userRepository.TwoFactorAuthenticatorSignInAsync(Provider,authenticatorCode, rememberMe, RememberMachine);
    }

    public Task<string> GetUserEmail(AppUser user)
    {
      return _userRepository.GetUserEmailAsync(user);
    }

    public Task<IdentityResult> RemoveLoginAsync(AppUser user, string loginProvider, string providerKey)
    {
      return _userRepository.RemoveLoginAsync(user, loginProvider, providerKey);
    }

    public async Task<bool> IsTwoFactorEnabled(AppUser user)
    {
      return await _userRepository.IsTwoFactorEnabled(user);
    }

    public async Task<string> GenerateTwoFactorCode(AppUser user,string Provider)
    {
      return await _userRepository.GenerateTwoFactorCode(user,Provider);
    }

    public async Task<IList<string>> GetEnabledTwoFactorProvidersAsync(AppUser user)
    {
      return await _userRepository.GetEnabledTwoFactorProvidersAsync(user);
    }

    public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe)
    {
      return await _userRepository.PasswordSignInAsync(userName,password,rememberMe,true);
    }

    public async Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool rememberMe, bool rememberMachine)
    {
      return await _userRepository.TwoFactorSignInAsync(provider,code,rememberMe,rememberMachine);
    }
  }
}
