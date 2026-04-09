using API.Dtos;
using API.Extensions;
using Business.Abstract;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class GlasController(IGlasService glasService, FileStorage fileStorage) : ControllerBase
  {
    private readonly IGlasService _glasService = glasService;
    private readonly FileStorage _fileStorage = fileStorage;

    // -------------------------------------------------------
    // GET - LIST (no id required)
    // -------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var result = await _glasService.GetGlases();

      if (result.Success)
        return Ok(result);

      return BadRequest(result);
    }

    // -------------------------------------------------------
    // GET BY ID
    // -------------------------------------------------------
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
      var result = _glasService.GetGlas(id); // sync per your service

      if (result.Success)
        return Ok(result);

      return NotFound(result);
    }

    // -------------------------------------------------------
    // CREATE
    // -------------------------------------------------------
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add([FromForm] CreateGlasDto glas)
    {
      var userId = User.GetUserId(); // Extension method

      string? imageUrl = null;
      if (glas.File != null)
      {
        imageUrl = await _fileStorage.SaveFileAsync(
            glas.File,
            ["Images", "Glasses"]
        );
      }

      var result = await _glasService.CreateGlas(new Glas
      {
        GlasId = Guid.NewGuid(),
        Title = glas.Title,
        Description = glas.Description,
        ImageUrl = imageUrl,
        SourceURL = glas.SourceURL,
        Latitude = glas.Latitude,
        Longitude = glas.Longitude,
        LocationAddress = glas.LocationAddress,
        UserId = userId,
        CreatedAt = DateTime.UtcNow,
        LastUpdatedAt = DateTime.UtcNow
      });

      if (!result.Success)
        return BadRequest(result);

      return CreatedAtAction(
          nameof(GetById),
          new { id = result.Data },
          result
      );
    }


    // -------------------------------------------------------
    // UPDATE
    // -------------------------------------------------------
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateGlasDto dto)
    {
      if (id != dto.GlasId)
        return BadRequest("ID uyumsuz");

      var userId = User.GetUserId();

      string? imageUrl = null;
      if (dto.File != null)
      {
        imageUrl = await _fileStorage.SaveFileAsync(
            dto.File,
            ["Images", "Glasses"]
        );
      }

      var glas = new Glas
      {
        GlasId = dto.GlasId,
        Title = dto.Title,
        Description = dto.Description,
        ImageUrl = imageUrl,
        LastUpdatedAt = DateTime.UtcNow
      };

      var result = await _glasService.UpdateGlas(glas, userId);

      return result.Success ? Ok(result) : BadRequest(result);
    }


    // -------------------------------------------------------
    // DELETE
    // -------------------------------------------------------
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
      var userId = User.GetUserId();

      var result = await _glasService.DeleteGlas(id, userId);

      return result.Success ? Ok(result) : BadRequest(result);
    }
  }
}
