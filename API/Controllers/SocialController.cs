using API.Extensions;
using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SocialController(ISocialService socialService) : ControllerBase
{
  // FOLLOW
  [HttpPost("follow")]
  public async Task<IActionResult> Follow(Guid followeeId)
  {
    var followerId = User.GetUserId();

    var result = await socialService.FollowUser(followerId, followeeId);
    return result.Success ? Ok(result) : BadRequest(result);
  }

  // UNFOLLOW
  [HttpPost("unfollow")]
  public async Task<IActionResult> UnFollow(Guid followeeId)
  {
    var followerId = User.GetUserId();

    var result = await socialService.UnFollowUser(followerId, followeeId);
    return result.Success ? Ok(result) : BadRequest(result);
  }

  // GET FOLLOWERS (PUBLIC)
  [AllowAnonymous]
  [HttpGet("{userId}/followers")]
  public async Task<IActionResult> GetFollowers(Guid userId)
  {
    var result = await socialService.GetFollowers(userId);
    return Ok(result);
  }

  // GET FOLLOWINGS (PUBLIC)
  [AllowAnonymous]
  [HttpGet("{userId}/followings")]
  public async Task<IActionResult> GetFollowings(Guid userId)
  {
    var result = await socialService.GetFollowings(userId);
    return Ok(result);
  }

  [HttpGet("is-following")]
  public async Task<IActionResult> IsFollowing(Guid followeeId)
  {
    var followerId = User.GetUserId();

    var isFollowing = await socialService.IsFollowing(followerId, followeeId);
    return Ok(isFollowing);
  }

  // REMOVE FOLLOWER
  [HttpPost("remove-follower")]
  public async Task<IActionResult> RemoveFollower(Guid followerId)
  {
    var ownerId = User.GetUserId();

    var result = await socialService.RemoveFollower(ownerId, followerId);
    return result.Success ? Ok(result) : BadRequest(result);
  }
}
