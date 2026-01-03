using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IRentalService
    {


        Task<IDataResult<List<Rental>>> GetAllAsync();

        // Transaction test edeceğimiz ve gerçek iş mantığını yürüteceğimiz özel metot
        Task<IResult> RentCarAsync(Rental rental);
    }
}