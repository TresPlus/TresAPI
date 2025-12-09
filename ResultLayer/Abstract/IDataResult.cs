namespace ResultLayer.Abstract
{
  public interface IDataResult<T> : IResult
  {
    T Data { get; }
  }
}
