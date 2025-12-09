using Entities;
using ResultLayer.Abstract;

namespace Business.Abstract
{
  public interface ISocialService
  {
    Task<IDataResult<List<AppUser>>> GetFollowers(Guid userId);
    Task<IDataResult<List<AppUser>>> GetFollowings(Guid userId);
    Task<IResult> IsFollowing(Guid followerId, Guid followeeId);
    Task<IResult> IsFollowingByName(string followerName, string followeeName);
    Task<IDataResult<UserFollow>> FollowUser(Guid followerId, Guid followeeId);
    Task<IDataResult<UserFollow>> UnFollowUser(Guid followerId, Guid followeeId);
  }
}
