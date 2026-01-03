using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using IResult = Core.Utilities.Results.IResult; // Task için gerekli

namespace Business.Abstract
{
    public interface ICarImageService
    {
        // Tüm dönüş tipleri Task<> içine alındı
        Task<IResult> AddAsync(IFormFile file, CarImage carImage);

        Task<IResult> DeleteAsync(CarImage carImage);

        Task<IResult> UpdateAsync(IFormFile file, CarImage carImage);

        Task<IDataResult<List<CarImage>>> GetAllAsync();

        Task<IDataResult<List<CarImage>>> GetImagesByCarIdAsync(int carId);

        Task<IDataResult<CarImage>> GetByIdAsync(int imageId);
    }
}