using API.Dtos;
using API.Extensions;
using Azure;
using Business.Abstract;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
    public async Task<IActionResult> Add([FromForm] CreateGlasDto glas)
    {
      var ImageResult = await _fileStorage.SaveFileAsync(glas.File, ["Images", "Glasses"]);
      var result = await _glasService.CreateGlas(
        new Glas
        {
          Title = glas.Title,
          Description = glas.Description,
          CreatedAt = DateTime.UtcNow,
          UserId = glas.UserId,
          SourceURL = ImageResult,
          LastUpdatedAt = DateTime.UtcNow,
        });

      if (result.Success)
        return StatusCode(201,
          new
          {
            Id = result.Data,
            Title = glas.Title,
            Description = glas.Description,
            SourceURL = ImageResult,
            CreatedAt = DateTime.Now,
            UserId = glas.UserId
          });

      return BadRequest(result);
    }

    // -------------------------------------------------------
    // UPDATE
    // -------------------------------------------------------
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateGlasDto glas)
    {
      if (id != glas.GlasId)
        return BadRequest(new { Success = false, Message = "ID alanı uyumsuz." });

      var result = await _glasService.UpdateGlas(new Glas
      {
        GlasId = glas.GlasId,
        Title = glas.Title,
        Description = glas.Description,
        LastUpdatedAt = DateTime.UtcNow,
      });

      if (result.Success)
        return Ok(result);

      return BadRequest(result);
    }

    // -------------------------------------------------------
    // DELETE
    // -------------------------------------------------------
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      var result = await _glasService.DeleteGlas(id);

      if (result.Success)
        return Ok(result);

      return BadRequest(result);
    }
  }
}
