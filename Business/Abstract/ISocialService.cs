using Entities;
using ResultLayer.Abstract;

namespace Business.Abstract
{
  public interface ISocialService
  {
    Task<IDataResult<List<UserPreviewDto>>> GetFollowers(Guid userId);
    Task<IDataResult<List<UserPreviewDto>>> GetFollowings(Guid userId);
    Task<IResult> IsFollowing(Guid followerId, Guid followeeId);
    Task<IResult> RemoveFollower(Guid ownerId, Guid followerId);
    Task<IResult> IsFollowingByName(string followerName, string followeeName);
    Task<IDataResult<UserFollow>> FollowUser(Guid followerId, Guid followeeId);
    Task<IDataResult<UserFollow>> UnFollowUser(Guid followerId, Guid followeeId);
  }
}
