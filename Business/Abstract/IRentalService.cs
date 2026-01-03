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

        // Transaction test edeceğimiz ve gerçek iş mantığını yürüteceğimiz özel metot
      Task<IDataResult<RentalInvoiceDto>> RentCarAsync(Rental rental);
    }
}