using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ResultLayer.Abstract;
using System.Reflection;
using System.Security.Claims;

namespace Business.Abstract
{
  public interface IUserService
  {
    Task<IDataResult<AppUser>> FindById(Guid Id);
    Task<IDataResult<AppUser>> LoadUserFindByName(string UserName);
    Task<IdentityResult> ChangeEmailAsync(AppUser user, string newEmail, string token);
    Task<IdentityResult> ChangeUserName(AppUser user, string Name);
    Task<IdentityResult> ChangeEmail(AppUser user, string Email);
    Task<IdentityResult> SetPhoneNumber(AppUser user, string phoneNumber);
    Task<IdentityResult> ChangePassword(AppUser user, string OldPassword, string NewPassword);
    Task<AppUser> GetUserByClaims(ClaimsPrincipal User);
    Task<bool> HasPassword(AppUser user);
    Task<IdentityResult> DeleteUser(AppUser user);
    Task<IdentityResult> DeletionUserRequest(AppUser user);
    Task<IList<AppUser>> GetAllUsers();
    Task<string> getPhoneNumber(AppUser user);
    Task<IdentityResult> UploadPicture(AppUser user, string ProfilePicturePath);
    Task<IdentityResult> RefreshSignInAsync(AppUser user);
    Task<string> GetAuthenticatorKey(AppUser User);
    Task<bool> GetTwoFactorEnabledAsync(AppUser User);
    Task<bool> IsTwoFactorClientRememberedAsync(AppUser User);
    Task<int> CountRecoveryCodesAsync(AppUser User);
    void ForgetTwoFactorClientAsync();
    Task<bool> VerifyTwoFactorTokenAsync(AppUser user,string AuthenticatorProvider,string Token);
    Task<IdentityResult> SetTwoFactorEnabledAsync(AppUser user,bool Enable);
    Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(AppUser user,int Number);
    Task<string> GetAuthenticatorKeyAsync(AppUser user);
    Task<IdentityResult> ResetAuthenticcatorKeyAsync(AppUser User);
    string GetTokenOptions();
    IEnumerable<PropertyInfo> GetPersonalDataProps();
    Task<IList<UserLoginInfo>> GetUserLogins(AppUser user);
    Task<string> GenerateChangeEmailTokenAsync(AppUser user,string newEmail);
    Task<string> GenerateEmailConfirmationTokenAsync(AppUser user);
    Task<IdentityResult> AddPassword(AppUser user,string Password);
    Task<IReadOnlyList<UserSession>> GetActiveSessionsAsync(ClaimsPrincipal user);
    Task<IdentityResult> RevokeOtherSessionsAsync(
      ClaimsPrincipal user,
      Guid currentSessionId
    );

    // SECURITY LOG
    Task<IReadOnlyList<SecurityEvent>> GetSecurityEventsAsync(ClaimsPrincipal user);

    // HELPERS
    string GetUserIdAsync(ClaimsPrincipal user);

    Task<Guid> CreateSessionAsync(
  AppUser user,
  UserSession session);

  }
}
