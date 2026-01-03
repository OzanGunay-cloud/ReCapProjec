using System.Linq.Expressions;
using Core.DataAccess;
using Core.DataAccess;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfRentalDal : EfEntityRepositoryBase<Rental, ReCapContext>, IRentalDal
    {
        private readonly ReCapContext _context;

        // EfEntityRepositoryBase sayesinde asenkron metodlar (AddAsync vb.) burada hazırdır.
        public EfRentalDal(ReCapContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RentalInvoiceDto>> GetRentalDetailsAsync(Expression<Func<Rental, bool>> filter = null)
        {
            var result = from r in filter == null ? _context.Rentals : _context.Rentals.Where(filter)

                         join c in _context.Cars on r.CarId equals c.CarId into carJoin
                         from c in carJoin.DefaultIfEmpty()

                         join b in _context.Brands on c.BrandId equals b.BrandId into brandJoin
                         from b in brandJoin.DefaultIfEmpty()

                         join u in _context.Users on r.CustomerId equals u.Id into userJoin
                         from u in userJoin.DefaultIfEmpty()

                         select new RentalInvoiceDto
                         {
                             CarName = c == null ? "Araba Bulunamadı" : c.CarName,
                             BrandName = b == null ? "Marka Bulunamadı" : b.BrandName,
                             CustomerFullName = u == null ? "Müşteri Bulunamadı" : (u.FirstName + " " + u.LastName),
                             RentDate = r.RentDate,
                             RentEndDate = r.RentEndDate,
                             DailyPrice = c == null ? 0 : c.DailyPrice,
                             TotalDays = (r.RentEndDate - r.RentDate).Days <= 0 ? 1 : (r.RentEndDate - r.RentDate).Days,
                             TotalPrice = ((r.RentEndDate - r.RentDate).Days <= 0 ? 1 : (r.RentEndDate - r.RentDate).Days) * (c == null ? 0 : c.DailyPrice)
                         };

            return await result.ToListAsync();
        }
    }

}
