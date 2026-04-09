using Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProfileController
    (
    IUserService userService
    ) : ControllerBase
  {
    private readonly IUserService _userService = userService;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      var result = await _userService.FindById(id);

      if (result.Success)
        return Ok(result);

      return NotFound(result.Message);
    }

    [HttpGet("{UserName}")]
    public async Task<IActionResult> GetById(string UserName)
    {
      var result = await _userService.LoadUserFindByName(UserName);

      if (result.Success)
        return Ok(result);

      return NotFound(result.Message);
    }

  }
}
