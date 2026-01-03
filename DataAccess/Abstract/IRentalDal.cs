using Core.DataAccess;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    // IEntityRepository sayesinde GetAll, Get, Add, Update, Delete metodları otomatik gelir.
    public interface IRentalDal : IEntityRepository<Rental>
    {
        // Buraya Rental tablosuna özel (Join vb.) metodlar gelirse ekleyebiliriz.
    }
}