using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IRentalService
    {
        Task<IDataResult<List<Rental>>> GetAllAsync();
        Task<IDataResult<Rental>> GetByIdAsync(int rentalId); // Hata veren metot 1
        Task<IResult> AddAsync(Rental rental);               // Hata veren metot 2
        Task<IResult> UpdateAsync(Rental rental);            // Hata veren metot 3
        Task<IResult> DeleteAsync(Rental rental);            // Hata veren metot 4
        Task<IDataResult<RentalInvoiceDto>> RentCarAsync(Rental rental);
    }
}