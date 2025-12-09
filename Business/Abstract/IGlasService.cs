using Entities;
using Microsoft.EntityFrameworkCore;
using ResultLayer.Abstract;

namespace Business.Abstract
{
  public interface IGlasService
  {
    Task<IDataResult<IList<Glas>>> GetGlases();
    IDataResult<IList<Glas>> GetGlases(Guid UserId);
    IDataResult<Glas> GetGlas(Guid Id);
    Task<IDataResult<Guid>> CreateGlas(Glas glas);
    Task<IResult> UpdateGlas(Glas glas);
    Task<IResult> DeleteGlas(Guid Id);
  }
}
