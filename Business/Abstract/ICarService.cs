using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICarService
    {
        // Geriye hem asenkron (Task) hem de verili sonuç (IDataResult) dönüyoruz
        Task<IDataResult<List<Car>>> GetAllAsync();
        Task<IDataResult<List<Car>>> GetCarsByBrandIdAsync(int id);
        Task<IDataResult<List<Car>>> GetCarsByColorIdAsync(int id);
        Task<IDataResult<Car>> GetByIdAsync(int carId);

        // Geriye veri dönmeyen (Add/Update/Delete) asenkron sonuçlar
        Task<IResult> AddAsync(Car car);
        Task<IResult> UpdateAsync(Car car);
        Task<IResult> DeleteAsync(Car car);
        Task<IDataResult<List<CarDetailDto>>> GetAvailableCarDetailsAsync(DateTime rentDate, DateTime rentEndDate);
    }
}
