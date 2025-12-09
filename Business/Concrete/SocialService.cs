using Business.Abstract;
using DataAccess.Interactive;
using Entities;
using Microsoft.EntityFrameworkCore;
using ResultLayer.Abstract;
using ResultLayer.Concrete;

namespace Business.Concrete
{
  public class SocialService : ISocialService
  {
    private readonly DataContext _data;

    public SocialService(DataContext data)
    {
      _data = data;
    }

    // FOLLOW USER
    public async Task<IDataResult<UserFollow>> FollowUser(Guid followerId, Guid followeeId)
    {
      if (followerId == followeeId)
        return new ErrorDataResult<UserFollow>("Kendi kendinizi takip edemezsiniz.");

      var existing = await _data.UserFollows
          .FirstOrDefaultAsync(x => x.FollowerUserId == followerId && x.FollowedUserId == followeeId);

      if (existing != null)
        return new ErrorDataResult<UserFollow>(existing, "Zaten takip ediyorsunuz.");

      var follow = new UserFollow
      {
        FollowerUserId = followerId,
        FollowedUserId = followeeId,
        CreatedAt = DateTime.UtcNow
      };

      await _data.UserFollows.AddAsync(follow);
      await _data.SaveChangesAsync();

      return new SuccessDataResult<UserFollow>(follow, "Takip işlemi başarılı.");
    }

    // UNFOLLOW USER
    public async Task<IDataResult<UserFollow>> UnFollowUser(Guid followerId, Guid followeeId)
    {
      if (followerId == followeeId)
        return new ErrorDataResult<UserFollow>("Kendi takibinizi kaldıramazsınız.");

      var existing = await _data.UserFollows
          .FirstOrDefaultAsync(x => x.FollowerUserId == followerId && x.FollowedUserId == followeeId);

      if (existing == null)
        return new ErrorDataResult<UserFollow>("Zaten takip etmiyorsunuz.");

      _data.UserFollows.Remove(existing);
      await _data.SaveChangesAsync();

      return new SuccessDataResult<UserFollow>(existing, "Takip bırakıldı.");
    }

    // GET FOLLOWERS
    public async Task<IDataResult<List<AppUser>>> GetFollowers(Guid userId)
    {
      var followers = await _data.UserFollows
          .Where(uf => uf.FollowedUserId == userId)
          .Include(uf => uf.Follower)
          .Select(uf => uf.Follower)
          .ToListAsync();

      return new SuccessDataResult<List<AppUser>>(followers, "Takipçiler listelendi.");
    }

    // GET FOLLOWINGS
    public async Task<IDataResult<List<AppUser>>> GetFollowings(Guid userId)
    {
      var followings = await _data.UserFollows
          .Where(uf => uf.FollowerUserId == userId)
          .Include(uf => uf.Follower)
          .Select(uf => uf.Follower)
          .ToListAsync();

      return new SuccessDataResult<List<AppUser>>(followings, "Takip edilenler listelendi.");
    }

    // IS FOLLOWING BY ID
    public async Task<IResult> IsFollowing(Guid followerId, Guid followeeId)
    {
      var result = await _data.UserFollows
          .AnyAsync(uf => uf.FollowerUserId == followerId && uf.FollowedUserId == followeeId);

      if (result)
        return new SuccessResult("Takip ediliyor.");

      return new ErrorResult("Takip edilmiyor.");
    }

    // IS FOLLOWING BY USERNAME
    public async Task<IResult> IsFollowingByName(string followerName, string followeeName)
    {
      if (string.IsNullOrEmpty(followerName) || string.IsNullOrEmpty(followeeName))
        return new ErrorResult("Kullanıcı adları boş olamaz.");

      var follower = await _data.Users
          .Where(u => u.UserName == followerName)
          .Select(u => (Guid?)u.Id)
          .FirstOrDefaultAsync();

      var followee = await _data.Users
          .Where(u => u.UserName == followeeName)
          .Select(u => (Guid?)u.Id)
          .FirstOrDefaultAsync();

      if (!follower.HasValue || !followee.HasValue)
        return new ErrorResult("Kullanıcı bulunamadı.");

      var isFollowing = await _data.UserFollows
          .AnyAsync(uf => uf.FollowerUserId == follower.Value && uf.FollowedUserId == followee.Value);

      if (isFollowing)
        return new SuccessResult("Takip ediyor.");

      return new ErrorResult("Takip etmiyor.");
    }
  }
}
