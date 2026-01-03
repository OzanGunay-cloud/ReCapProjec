using Core.DataAccess;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    // Public olduğuna ve IEntityRepository<Car>'dan miras aldığına emin ol
    public interface ICarDal : IEntityRepository<Car>
    {

        Task<List<CarDetailDto>> GetCarDetailsAsync();

        // YENİ METOT: Tarih aralığına göre müsait araçları getir
        Task<List<CarDetailDto>> GetAvailableCarDetailsAsync(DateTime minDate, DateTime maxDate);
    }
}