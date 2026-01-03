using System.Linq.Expressions;
using Core.DataAccess;
using Entities.Concrete;
using Entities.DTOs;

namespace DataAccess.Abstract
{
    // IEntityRepository sayesinde GetAll, Get, Add, Update, Delete metodları otomatik gelir.
    public interface IRentalDal : IEntityRepository<Rental>
    {
        // Buraya Rental tablosuna özel (Join vb.) metodlar gelirse ekleyebiliriz.


        Task<List<RentalInvoiceDto>> GetRentalDetailsAsync(Expression<Func<Rental, bool>> filter = null);
    }

}
