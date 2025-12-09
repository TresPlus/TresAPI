using Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DataAccess.Abstract
{
  public interface IUserRepository
  {
    Task<IList<Claim>> GetClaimsAsync(AppUser user);
    Task<bool> CheckPasswordAsync(AppUser user, string Password);
    Task<IdentityResult> CreateAsync(AppUser user, string password);
    Task<IdentityResult> CreateAsync(AppUser user);
    Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken);
    Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken);
    Task<string> GenerateEmailConfirmationTokenAsync(AppUser user);
    Task<string> GeneratePasswordResetTokenAsync(AppUser user);
    Task<string> GetUserIdAsync(AppUser user);
    Task<string> GetUserEmailAsync(AppUser user);
    Task<AppUser> GetUserById(string Id);
    Task<AppUser> GetUserByEmail(string Email);
    Task<AppUser> GetUserByName(string Name);
    bool SupportsUserEmail { get; }
    IUserEmailStore<AppUser> GetEmailStore();
    IList<AuthenticationScheme> GetAuthenticationSchemes();
    void Login(AppUser user, bool isPersistent = false);
    bool RequiresConfirmedAccount();
    Task<IdentityResult> confirmEmail(AppUser user, string code);
    Task RefreshSignIn(AppUser user);
    AuthenticationProperties AuthenticateProps(string provider, string RedirectURL);
    Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
    Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo Info, bool isPersistent, bool bypassTwoFactor);
    Task SignIn(AppUser user, bool isPersistent, ExternalLoginInfo Info);
    Task<SignInResult> SignIn(string Email, string Password, bool RememberMe, bool lockoutOnFailure);
    Task<IdentityResult> AddLoginAsync(AppUser user, ExternalLoginInfo Info);
    Task<bool> IsEmailConfirmedAsync(AppUser user);
    Task<IdentityResult> ResetPassword(AppUser user, string Token, string NewPassword);
    void SignOut();
    bool IsSignedIn(ClaimsPrincipal principal);
    Task<AppUser> GetTwoFactorAuthenticationUserAsync();
    Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);
    Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string authenticatorCode, bool rememberMe, bool RememberMachine);
    Task<IdentityResult> RemoveLoginAsync(AppUser user, string loginProvider, string providerKey);
  }
}
