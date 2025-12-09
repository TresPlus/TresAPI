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

    // UPDATE
    public async Task<IResult> UpdateGlas(Glas glas)
    {
      try
      {
        var existing = await _data.Glasses
            .FirstOrDefaultAsync(g => g.GlasId == glas.GlasId);

        if (existing == null)
          return new ErrorResult("Güncellenecek Glas bulunamadı.");

        // Sadece dolu gelen alanları set et
        if (glas.Title != null)
          existing.Title = glas.Title;

        if (glas.SourceURL != null)
          existing.SourceURL = glas.SourceURL;

        if (glas.Description != null)
          existing.Description = glas.Description;

        // … diğer alanlar için aynı şekilde devam

        await _data.SaveChangesAsync();
        return new SuccessResult("Glas başarıyla güncellendi.");
      }
      catch (Exception ex)
      {
        return new ErrorResult($"Glas güncellenirken hata oluştu: {ex.Message}, {ex.InnerException?.Message}");
      }
    }


    // DELETE
    public async Task<IResult> DeleteGlas(Guid id)
    {
      try
      {
        var glas = await _data.Glasses.FirstOrDefaultAsync(g => g.GlasId == id);

        if (glas == null)
          return new ErrorResult("Silinecek Glas bulunamadı.");

        _data.Glasses.Remove(glas);
        await _data.SaveChangesAsync();

        return new SuccessResult("Glas başarıyla silindi.");
      }
      catch (Exception ex)
      {
        return new ErrorResult($"Glas silinirken hata oluştu: {ex.Message}");
      }
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

    // GET ALL
    public async Task<IDataResult<IList<Glas>>> GetGlases()
    {
      var result = await _data.Glasses
          .Include(u => u.User)
          .ToListAsync();

      return new SuccessDataResult<IList<Glas>>(result, "Tüm Glas'lar listelendi.");
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
