using Business.Abstract;
using DataAccess.Interactive;
using Entities;
using Microsoft.EntityFrameworkCore;
using ResultLayer.Abstract;
using ResultLayer.Concrete;

namespace Business.Concrete
{
  public class GlasService : IGlasService
  {
    private readonly DataContext _data;

    public GlasService(DataContext dataContext)
    {
      _data = dataContext;
    }

    // CREATE
    public async Task<IDataResult<Guid>> CreateGlas(Glas glas)
    {
      try
      {
       var result = await _data.Glasses.AddAsync(glas);
        await _data.SaveChangesAsync();

        return new SuccessDataResult<Guid>(glas.GlasId,"Glas başarıyla oluşturuldu.");
      }
      catch (Exception ex)
      {
        return new ErrorDataResult<Guid>(glas.GlasId, $"Glas oluşturulurken hata oluştu: {ex.Message}");
      }
    }

    public async Task<IResult> UpdateGlas(Glas input, Guid userId)
    {
      var glas = await _data.Glasses
          .FirstOrDefaultAsync(g => g.GlasId == input.GlasId && g.UserId == userId);

      if (glas == null)
        return new ErrorResult("Glas bulunamadı.");

      if (input.Title != null)
        glas.Title = input.Title;

      if (input.Description != null)
        glas.Description = input.Description;

      if (input.ImageUrl != null)
        glas.ImageUrl = input.ImageUrl;

      glas.LastUpdatedAt = DateTime.UtcNow;

      await _data.SaveChangesAsync();
      return new SuccessResult("Glas güncellendi.");
    }
    public async Task<IResult> DeleteGlas(Guid id, Guid userId)
    {
      var glas = await _data.Glasses
          .FirstOrDefaultAsync(g => g.GlasId == id && g.UserId == userId);

      if (glas == null)
        return new ErrorResult("Glas bulunamadı.");

      glas.IsDeleted = true;
      glas.LastUpdatedAt = DateTime.UtcNow;

      await _data.SaveChangesAsync();
      return new SuccessResult("Glas silindi.");
    }


    // GET BY ID
    public IDataResult<Glas> GetGlas(Guid id)
    {
      var glas = _data.Glasses
          .Include(x => x.User)
          .FirstOrDefault(g => g.GlasId == id);

      if (glas == null)
        return new ErrorDataResult<Glas>("Glas bulunamadı.");

      return new SuccessDataResult<Glas>(glas, "Glas listelendi.");
    }

    public async Task<IDataResult<IList<Glas>>> GetGlases()
    {
      var result = await _data.Glasses
          .Include(x => x.User)
          .OrderByDescending(x => x.CreatedAt)
          .ToListAsync();

      return new SuccessDataResult<IList<Glas>>(result);
    }

    // GET BY USER
    public IDataResult<IList<Glas>> GetGlases(Guid userId)
    {
      var result = _data.Glasses
          .Where(g => g.UserId == userId)
          .Include(u => u.User)
          .ToList();

      return new SuccessDataResult<IList<Glas>>(result, "Kullanıcıya ait Glas'lar listelendi.");
    }
  }
}
