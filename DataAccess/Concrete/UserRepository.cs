using DataAccess.Abstract;
using Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DataAccess.Concrete
{
  public class UserRepository : IUserRepository
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly IUserEmailStore<AppUser> _emailStore;
    private readonly SignInManager<AppUser> _signInManager;

    public UserRepository(UserManager<AppUser> userManager, IUserStore<AppUser> userStore
        , SignInManager<AppUser> SignInManager)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
      if (!_userManager.SupportsUserEmail)
      {
        throw new NotSupportedException("UserManager does not support email.");
      }
      _emailStore = (IUserEmailStore<AppUser>)_userStore;
      _signInManager = SignInManager;
    }

    public async Task<IList<Claim>> GetClaimsAsync(AppUser user)
    {
      return await _userManager.GetClaimsAsync(user);
    }

    public bool SupportsUserEmail => _userManager.SupportsUserEmail;
    public async Task<bool> CheckPasswordAsync(AppUser user, string Password)
    {
      return await _userManager.CheckPasswordAsync(user, Password);
    }

    public Task<IdentityResult> CreateAsync(AppUser user, string password)
    {
      return _userManager.CreateAsync(user, password);
    }
    public Task<IdentityResult> CreateAsync(AppUser user)
    {
      return _userManager.CreateAsync(user);
    }

    public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
    {
      return _userStore.SetUserNameAsync(user, userName, cancellationToken);
    }

    public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
    {
      return _emailStore.SetEmailAsync(user, email, cancellationToken);
    }

    public Task<string> GenerateEmailConfirmationTokenAsync(AppUser user)
    {
      return _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public Task<string> GetUserIdAsync(AppUser user)
    {
      return _userManager.GetUserIdAsync(user);
    }

    public IList<AuthenticationScheme> GetAuthenticationSchemes()
    {
      return _signInManager.GetExternalAuthenticationSchemesAsync().Result.ToList();
    }

    public async Task<AppUser> GetUserById(string Id)
    {
      return await _userManager.FindByIdAsync(Id);
    }

    public async Task<AppUser> GetUserByEmail(string Email)
    {
      return await _userManager.FindByEmailAsync(Email);
    }

    public async Task<AppUser> GetUserByName(string Name)
    {
      return await _userManager.FindByNameAsync(Name);
    }

    public void Login(AppUser user, bool isPersistent = false)
    {
      _signInManager.SignInAsync(user, isPersistent);
      new ClaimsIdentity().AddClaim(new Claim("picture", user.ProfilePicturePath));
    }

    public bool RequiresConfirmedAccount()
    {
      return _signInManager.Options.SignIn.RequireConfirmedAccount;
    }

    public IUserEmailStore<AppUser> GetEmailStore()
    {
      if (!_userManager.SupportsUserEmail)
      {
        throw new NotSupportedException("The default UI requires a user store with email support.");
      }
      return (IUserEmailStore<AppUser>)_userStore;
    }

    public async Task<IdentityResult> confirmEmail(AppUser user, string code)
    {
      return await _userManager.ConfirmEmailAsync(user, code);
    }

    public async Task RefreshSignIn(AppUser user)
    {
      await _signInManager.RefreshSignInAsync(user);
    }

    public AuthenticationProperties AuthenticateProps(string provider, string RedirectURL)
    {
      return _signInManager.ConfigureExternalAuthenticationProperties(provider, RedirectURL);
    }

    public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
    {
      return await _signInManager.GetExternalLoginInfoAsync();
    }

    public Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo Info, bool isPersistent, bool bypassTwoFactor)
    {
      return _signInManager.ExternalLoginSignInAsync(Info.LoginProvider, Info.ProviderKey, isPersistent, bypassTwoFactor);
    }

    public Task SignIn(AppUser user, bool isPersistent, ExternalLoginInfo Info)
    {
      return _signInManager.SignInAsync(user, isPersistent, Info.LoginProvider);
    }

    public Task<IdentityResult> AddLoginAsync(AppUser user, ExternalLoginInfo Info)
    {
      return _userManager.AddLoginAsync(user, Info);
    }

    public async Task<SignInResult> SignIn(string Email, string Password, bool RememberMe, bool lockoutOnFailure)
    {
      return await _signInManager.PasswordSignInAsync(Email, Password, RememberMe, lockoutOnFailure);
    }

    public async Task<bool> IsEmailConfirmedAsync(AppUser user)
    {
      return await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(AppUser user)
    {
      return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ResetPassword(AppUser user, string Token, string NewPassword)
    {
      return await _userManager.ResetPasswordAsync(user, Token, NewPassword);
    }

    public void SignOut()
    {
      _signInManager.SignOutAsync();
    }

    public bool IsSignedIn(ClaimsPrincipal principal)
    {
      return _signInManager.IsSignedIn(principal);
    }

    public Task<AppUser> GetTwoFactorAuthenticationUserAsync()
    {
      return _signInManager.GetTwoFactorAuthenticationUserAsync();
    }

    public Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
    {
      return _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
    }

    public async Task<SignInResult> TwoFactorAuthenticatorSignInAsync(
      string Provider,
      string authenticatorCode,
      bool rememberMe,
      bool RememberMachine)
    {
      if (string.IsNullOrWhiteSpace(Provider))
        return SignInResult.Failed;

      Provider = Provider.ToLowerInvariant() switch
      {
        "email" => "Email",
        "sms" or "phone" => "Phone",
        "totp" or "authenticator" or "authenticatorkey" => "Authenticator",  // ✅ Hepsi küçük harf
        _ => ""
      };

      if (string.IsNullOrWhiteSpace(Provider)||Provider == null)
      {
        return SignInResult.Failed;
      }

      return await _signInManager.TwoFactorSignInAsync(Provider, authenticatorCode, rememberMe, RememberMachine);
    }

    public async Task<string> GetUserEmailAsync(AppUser user)
    {
      return await _userManager.GetEmailAsync(user);
    }

    public async Task<IdentityResult> RemoveLoginAsync(AppUser user, string loginProvider, string providerKey)
    {
      return await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
    }

    public async Task<IdentityResult> AddClaimsInDbAsync(AppUser user, IEnumerable<Claim> claims)
    {
      return await _userManager.AddClaimsAsync(user, claims);
    }

    public async Task<IdentityResult> AddClaimsInDbAsync(AppUser user, Claim claim)
    {
      return await _userManager.AddClaimAsync(user, claim);
    }

    public async Task<bool> IsTwoFactorEnabled(AppUser user)
    {
      return await _userManager.GetTwoFactorEnabledAsync(user);
    }

    public async Task<string> GenerateTwoFactorCode(AppUser user, string Provider)
    {
      return await _userManager.GenerateTwoFactorTokenAsync(user, Provider);
    }

    public async Task<bool> VerifyTwoFactorTokenAsync(AppUser user, string Provider, string Token)
    {
      return await _userManager.VerifyTwoFactorTokenAsync(user, Provider, Token);
    }

    public async Task<IList<string>> GetEnabledTwoFactorProvidersAsync(AppUser user)
    {
      var providers = new List<string>();

      if (user.TwoFactorEnabled)
      {
        if (user.EmailConfirmed)
          providers.Add("email");

        if (user.PhoneNumberConfirmed)
          providers.Add("sms");

        if (await _userManager.GetAuthenticatorKeyAsync(user) != null)
          providers.Add("totp");
      }

      return providers;
    }

    public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockoutOnFailure)
    {
      return await _signInManager.PasswordSignInAsync(userName,password, rememberMe,true);
    }

    public async Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool rememberMe, bool rememberMachine)
    {
      return await _signInManager.TwoFactorSignInAsync(provider,code,rememberMe,rememberMachine);
    }
  }
}
