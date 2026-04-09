using Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Business.Abstract
{
  public interface IAuthService
  {
    Task<string> GenerateJwtToken(AppUser user,Guid sessionId);
    Task<string> GenerateTwoFactorCode(AppUser user, string Provider);
    Task<IList<string>> GetEnabledTwoFactorProvidersAsync(AppUser user);
    Task<bool> IsTwoFactorEnabled(AppUser user);
    Task<IdentityResult> CheckPasswordAsync(AppUser user, string Password);
    Task<IdentityResult> UserNameAlreadyExists(string Username);
    Task<IdentityResult> EmailAlreadyExists(string Email);
    Task<IdentityResult> RegisterAsync(string ProfilePicturePath,string Email,string Password, string UserName);
    Task<IdentityResult> RegisterAsync(AppUser user, ExternalLoginInfo Info);
    Task<SignInResult> SignIn(string Email, string Password, bool RememberMe, bool lockoutOnFailure);
    IList<AuthenticationScheme> GetExternalAuthenticationScheme();
    Task<AppUser> GetById(string Id);
    Task<AppUser> GetByName(string Name);
    Task<AppUser> GetByEmail(string Email);
    void SignIn(AppUser user, bool isPersistent = false);
    bool RequiresConfirmedAccount();
    Task<string> GetUserId(AppUser user);
    Task<string> GetUserEmail(AppUser user);
    Task<string> GenerateUserConfirmationToken(AppUser user);
    Task<string> GeneratePasswordResetTokenAsync(AppUser user);
    IUserEmailStore<AppUser> GetEmailStore();
    Task<IdentityResult> confirmEmail(AppUser user, string code);
    Task RefreshSignIn(AppUser user);
    Task<AuthenticationProperties> AuthenticateProps(string provider, string RedirectURL);
    Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
    Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo Info, bool isPersistent, bool bypassTwoFactor);
    Task<IdentityResult> AddLoginAsync(AppUser user, ExternalLoginInfo Info);
    Task SetEmail(AppUser user, string Email);
    Task SetName(AppUser user, string Name);
    Task<bool> IsEmailConfirmedAsync(AppUser user);
    Task<IdentityResult> ResetPassword(AppUser user, string Token, string NewPassword);
    void SignOut();
    bool IsSignedIn(ClaimsPrincipal principal);
    Task<AppUser> GetTwoFactorAuthenticationUserAsync();
    Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);
    Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string Provider, string authenticatorCode,bool rememberMe,bool RememberMachine);
    Task<IdentityResult> RemoveLoginAsync(AppUser user, string loginProvider, string providerKey);
    Task<SignInResult> PasswordSignInAsync(
  string userName,
  string password,
  bool rememberMe);
    Task<SignInResult> TwoFactorSignInAsync(
string provider,
string code,
bool rememberMe,
bool rememberMachine);

  }
}
