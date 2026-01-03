using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IColorService
    {
        Task<IDataResult<List<Color>>> GetAllAsync();
        Task<IResult> AddAsync(Color color);
        // İhtiyaca göre GetById, Update, Delete eklenebilir
    }
}