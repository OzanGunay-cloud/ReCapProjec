using Core.DataAccess;
using Core.DataAccess;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfRentalDal : EfEntityRepositoryBase<Rental, ReCapContext>, IRentalDal
    {
        // EfEntityRepositoryBase sayesinde asenkron metodlar (AddAsync vb.) burada hazırdır.
        public EfRentalDal(ReCapContext context) : base(context)
        {
        }
    }
}