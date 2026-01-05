using Business.Abstract;
using Business.Constants;
using Business.Rules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Transaction;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class RentalManager : IRentalService
    {
        private readonly IRentalDal _rentalDal;
        private readonly ICarService _carService;
        private readonly RentalBusinessRules _rules;
        private readonly IPaymentService _paymentService;

        public RentalManager(IRentalDal rentalDal, ICarService carService, RentalBusinessRules rules, IPaymentService paymentservice)
        {
            _rentalDal = rentalDal;
            _carService = carService;
            _rules = rules;
            _paymentService = paymentservice;
        }

        [CacheAspect]
        public async Task<IDataResult<List<Rental>>> GetAllAsync()
        {
            var result = await _rentalDal.GetAllAsync();
            return new SuccesDataResult<List<Rental>>(result, "Kiralama kayıtları listelendi.");
        }

        public async Task<IDataResult<Rental>> GetByIdAsync(int rentalId)
        {
            var result = await _rentalDal.GetAsync(r => r.RentalId == rentalId);
            return new SuccesDataResult<Rental>(result);
        }

        [CacheRemoveAspect("Get")] // Ekleme yapınca her şeyi temizle
        public async Task<IResult> AddAsync(Rental rental)
        {
            await _rentalDal.AddAsync(rental);
            return new SuccessResult("Kiralama kaydı eklendi.");
        }

        [CacheRemoveAspect("Get")] // Güncelleme (Teslim alma) yapınca her şeyi temizle
        public async Task<IResult> UpdateAsync(Rental rental)
        {
            await _rentalDal.UpdateAsync(rental);

            if (rental.ReturnDate != null)
            {
                var carResult = await _carService.GetByIdAsync(rental.CarId);
                if (carResult.Success)
                {
                    var car = carResult.Data;
                    car.IsAvailable = true;
                    await _carService.UpdateAsync(car);
                }
            }
            return new SuccessResult("Araç teslim alındı.");
        }

        public async Task<IResult> DeleteAsync(Rental rental)
        {
            await _rentalDal.DeleteAsync(rental);
            return new SuccessResult("Kiralama kaydı silindi.");
        }

        // --- KRİTİK METOT ---
        [TransactionScopeAspect]
        [CacheRemoveAspect("Get")] // İÇİNDE 'Get' GEÇEN HER ŞEYİ SİL (Arabalar dahil)
        public async Task<IDataResult<RentalInvoiceDto>> RentCarAsync(Rental rental)
        {
            // 1. İş Kuralı
            var ruleResult = await _rules.CheckIfCarIsAvailable(rental.CarId, rental.RentDate, rental.RentEndDate);
            if (!ruleResult.Success) return new ErrorDataResult<RentalInvoiceDto>(ruleResult.Message);

            // 2. Araba Getir
            var carResult = await _carService.GetByIdAsync(rental.CarId);
            if (!carResult.Success) return new ErrorDataResult<RentalInvoiceDto>("Araba bilgisi bulunamadı.");
            var car = carResult.Data;

            // 3. Hesaplama & Ödeme
            int totalDays = (rental.RentEndDate - rental.RentDate).Days;
            if (totalDays <= 0) totalDays = 1;
            decimal totalPrice = totalDays * car.DailyPrice;

            var paymentResult = _paymentService.Pay(totalPrice);
            if (!paymentResult.Success) return new ErrorDataResult<RentalInvoiceDto>("Ödeme reddedildi.");

            // 4. Veritabanı Kayıt
            await _rentalDal.AddAsync(rental);

            // 5. Arabayı Meşgul Yap
            car.IsAvailable = false;
            await _carService.UpdateAsync(car);

            // 6. Fatura
            var invoiceResult = await _rentalDal.GetRentalDetailsAsync(r => r.RentalId == rental.RentalId);
            var invoice = invoiceResult.FirstOrDefault();

            return new SuccesDataResult<RentalInvoiceDto>(invoice, "Kiralama başarılı.");
        }
    }
}