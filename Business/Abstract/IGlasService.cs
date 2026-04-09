using Entities;
using Microsoft.EntityFrameworkCore;
using ResultLayer.Abstract;

namespace Business.Abstract
{
  public interface IGlasService
  {
    Task<IDataResult<IList<Glas>>> GetGlases();
    IDataResult<Glas> GetGlas(Guid id);

    Task<IDataResult<Guid>> CreateGlas(Glas glas);
    Task<IResult> UpdateGlas(Glas glas, Guid userId);
    Task<IResult> DeleteGlas(Guid id, Guid userId);
  }
}
