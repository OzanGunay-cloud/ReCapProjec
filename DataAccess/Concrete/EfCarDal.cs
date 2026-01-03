using Core.DataAccess;

using DataAccess.Abstract;

using Entities.Concrete;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq; // LINQ için şart
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfCarDal : EfEntityRepositoryBase<Car, ReCapContext>, ICarDal
    {
        // Constructor (Senin yapına uygun)
        public EfCarDal(ReCapContext context) : base(context)
        {

        }

        public async Task<List<CarDetailDto>> GetCarDetailsAsync()
        {
            // Mevcut context üzerinden işlem yapıyoruz
            using (var context = new ReCapContext())
            {
                var result = from c in context.Cars
                             join b in context.Brands on c.BrandId equals b.BrandId
                             join co in context.Colors on c.ColorId equals co.ColorId
                             select new CarDetailDto
                             {
                                 CarId = c.CarId,
                                 CarName = c.Description, // Veya c.CarName
                                 BrandName = b.BrandName,
                                 ColorName = co.ColorName,
                                 DailyPrice = c.DailyPrice,
                                 Description = c.Description,
                                 ModelYear = c.ModelYear
                             };
                return await result.ToListAsync();
            }
        }

        // --- YENİ EKLENEN METOT ---
        public async Task<List<CarDetailDto>> GetAvailableCarDetailsAsync(DateTime minDate, DateTime maxDate)
        {
            using (var context = new ReCapContext())
            {
                // 1. ADIM: MEŞGUL ARABALARI BUL
                // SQL Mantığı: "Select CarId From Rentals Where ..."
                var busyCarIds = context.Rentals
                    .Where(r => r.ReturnDate == null && (r.RentDate < maxDate && r.RentEndDate > minDate))
                    .Select(r => r.CarId); // ToList demiyoruz, sorguya gömeceğiz (Performans için)

                // 2. ADIM: JOIN İLE DETAYLARI GETİR AMA MEŞGULLERİ HARİÇ TUT
                var result = from c in context.Cars
                             join b in context.Brands on c.BrandId equals b.BrandId
                             join co in context.Colors on c.ColorId equals co.ColorId

                             // İŞTE SİHİRLİ DOKUNUŞ BURADA:
                             // "Meşgul listesinde bu arabanın ID'si YOKSA getir"
                             where !busyCarIds.Contains(c.CarId)

                             select new CarDetailDto
                             {
                                 CarId = c.CarId,
                                 CarName = c.CarName,
                                 BrandName = b.BrandName,
                                 ColorName = co.ColorName,
                                 DailyPrice = c.DailyPrice,
                                 Description = c.Description,
                                 ModelYear = c.ModelYear,

                                 ImagePath = (from ci in context.CarImages
                                              where ci.CarId == c.CarId
                                              select ci.ImagePath).ToList()
                             };

                return await result.ToListAsync();
            }
        }
    }
}