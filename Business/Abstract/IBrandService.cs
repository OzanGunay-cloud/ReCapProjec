using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IBrandService
    {
        Task<IDataResult<List<Brand>>> GetAllAsync();
        Task<IDataResult<Brand>> GetByIdAsync(int brandId);
        Task<IResult> AddAsync(Brand brand);
        Task<IResult> UpdateAsync(Brand brand);
        Task<IResult> DeleteAsync(Brand brand);
    }
}