using Business.Abstract;
using DataAccess.Interactive;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResultLayer.Abstract;
using ResultLayer.Concrete;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;

namespace Business.Concrete
{
  public class UserService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    DataContext context,
    IConfiguration conf) : IUserService
  {
    private UserManager<AppUser> _userManager = userManager;
    private SignInManager<AppUser> _SignInManager = signInManager;
    private DataContext _context = context;
    IConfiguration _config = conf;

    public Task<IdentityResult> ChangeEmail(AppUser user, string Email)
    {
      return _userManager.SetEmailAsync(user, Email);
    }

    public Task<IdentityResult> ChangeEmailAsync(AppUser user, string newEmail, string token)
    {
      return _userManager.ChangeEmailAsync(user, newEmail, token);
    }

    public Task<IdentityResult> ChangePassword(AppUser user, string OldPassword, string NewPassword)
    {
      return _userManager.ChangePasswordAsync(user, OldPassword, NewPassword);
    }

    public async Task<IdentityResult> SetPhoneNumber(AppUser user, string phoneNumber)
    {
      return await _userManager.SetPhoneNumberAsync(user, phoneNumber);
    }

    public Task<IdentityResult> ChangeUserName(AppUser user, string Name)
    {
      return _userManager.SetUserNameAsync(user, Name);
    }

    public async Task<bool> CheckPassword(AppUser user, string Password)
    {
      return await _userManager.CheckPasswordAsync(user, Password);
    }

    public Task<IdentityResult> DeleteUser(AppUser user)
    {
      return _userManager.DeleteAsync(user);
    }

    public Task<IdentityResult> DeletionUserRequest(AppUser user)
    {
      return _userManager.UpdateAsync(user);
    }

    public async Task<IList<AppUser>> GetAllUsers()
    {
      return await _userManager.Users.ToListAsync();
    }

    public async Task<string> getPhoneNumber(AppUser user)
    {
      return await _userManager.GetPhoneNumberAsync(user);
    }

    public async Task<AppUser> GetUserByClaims(ClaimsPrincipal User)
    {
      return await _userManager.GetUserAsync(User);
    }

    public async Task<bool> HasPassword(AppUser user)
    {
      return await _userManager.HasPasswordAsync(user);
    }

    public async Task<IdentityResult> UploadPicture(AppUser user, string ProfilePicturePath)
    {
      user.ProfilePicturePath = ProfilePicturePath;
      var result = await _userManager.UpdateAsync(user);
      return result.Succeeded
        ? IdentityResult.Success
        : IdentityResult.Failed(result.Errors.ToArray());

    }

    public async Task<IdentityResult> RefreshSignInAsync(AppUser user)
    {
      await _SignInManager.RefreshSignInAsync(user);
      return IdentityResult.Success;
    }

    public async Task<string> GetAuthenticatorKey(AppUser User)
    {
      return await _userManager.GetAuthenticatorKeyAsync(User);
    }

    public async Task<bool> GetTwoFactorEnabledAsync(AppUser User)
    {
      return await _userManager.GetTwoFactorEnabledAsync(User);
    }

    public async Task<bool> IsTwoFactorClientRememberedAsync(AppUser User)
    {
      return await _SignInManager.IsTwoFactorClientRememberedAsync(User);
    }

    public async Task<int> CountRecoveryCodesAsync(AppUser User)
    {
      return await _userManager.CountRecoveryCodesAsync(User);
    }

    public async void ForgetTwoFactorClientAsync()
    {
      await _SignInManager.ForgetTwoFactorClientAsync();
    }

    public async Task<bool> VerifyTwoFactorTokenAsync(AppUser user, string AuthenticatorProvider, string Token)
    {
      return await _userManager.VerifyTwoFactorTokenAsync(user, AuthenticatorProvider, Token);
    }

    public Task<IdentityResult> SetTwoFactorEnabledAsync(AppUser user, bool Enable)
    {
      return _userManager.SetTwoFactorEnabledAsync(user, Enable);
    }

    public async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(AppUser user, int Number)
    {
      return await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, Number);
    }

    public async Task<string> GetAuthenticatorKeyAsync(AppUser user)
    {
      return await _userManager.GetAuthenticatorKeyAsync(user);
    }

    public async Task<IdentityResult> ResetAuthenticcatorKeyAsync(AppUser User)
    {
      return await _userManager.ResetAuthenticatorKeyAsync(User);
    }

    public string GetTokenOptions()
    {
      return _userManager.Options.Tokens.AuthenticatorTokenProvider;
    }

    public IEnumerable<PropertyInfo> GetPersonalDataProps()
    {
      return typeof(AppUser).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
    }

    public async Task<IList<UserLoginInfo>> GetUserLogins(AppUser user)
    {
      return await _userManager.GetLoginsAsync(user);
    }

    public async Task<string> GenerateChangeEmailTokenAsync(AppUser user, string newEmail)
    {
      return await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(AppUser user)
    {
      return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityResult> AddPassword(AppUser user, string Password)
    {
      return await _userManager.AddPasswordAsync(user, Password);
    }

    public async Task<IDataResult<AppUser>> LoadUserFindByName(string UserName)
    {
      try
      {
        var result = await _context.Users
        .Where(X => X.UserName == UserName)
        .Include(u => u.Followers)
        .FirstOrDefaultAsync(u => u.UserName.ToLower() == UserName.ToLower());
        return new DataResult<AppUser>(result, true);

      }
      catch (Exception Ex)
      {
        return new DataResult<AppUser>(null, false, $"Liste Veritabanından Getirilmeye Çalışırken Bir Hata Oluştu{Ex.Message}");
      }
    }



    public async Task<IDataResult<AppUser>> FindById(Guid Id)
    {
      try
      {
        var result = await _userManager.FindByIdAsync(Id.ToString());
        return new DataResult<AppUser>(await _userManager.FindByIdAsync(Id.ToString()), true);

      }
      catch (Exception Ex)
      {
        return new DataResult<AppUser>(await _userManager.FindByIdAsync(Id.ToString()), false,
  $"Kullanıcı Veritabanından Getirilmeye Çalışırken Bir Hata Oluştu{Ex.Message}");

      }
    }

    public async Task<IReadOnlyList<UserSession>> GetActiveSessionsAsync(ClaimsPrincipal user)
    {
      var userId = GetUserIdAsync(user);

      return await _context.AspNetSessions
        .Where(x => x.UserId == Guid.Parse(userId))
        .OrderByDescending(x => x.LastSeen)
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<IdentityResult> RevokeOtherSessionsAsync(ClaimsPrincipal user, Guid currentSessionId)
    {
      var userId = GetUserIdAsync(user);

      var sessions = await _context.AspNetSessions
        .Where(x => x.UserId == Guid.Parse(userId) && x.Id != currentSessionId)
        .ToListAsync();

      if (!sessions.Any())
        return IdentityResult.Success;

      _context.AspNetSessions.RemoveRange(sessions);
      await _context.SaveChangesAsync();

      return IdentityResult.Success;
    }

    public async Task<IReadOnlyList<SecurityEvent>> GetSecurityEventsAsync(ClaimsPrincipal user)
    {
      var userId = GetUserIdAsync(user);

      return await _context.SecurityEvents
        .Where(x => x.UserId == userId)
        .OrderByDescending(x => x.CreatedAt)
        .AsNoTracking()
        .ToListAsync();
    }

    public string GetUserIdAsync(ClaimsPrincipal user)
    {
      var Id = _userManager.GetUserId(user)
        ?? throw new UnauthorizedAccessException();

      return Id;
    }

    public async Task<Guid> CreateSessionAsync(AppUser user, UserSession session)
    {
      _context.AspNetSessions.Add(session);
      await _context.SaveChangesAsync();

      return session.Id;
    }
  }
}
