using Business.Abstract;
using Business.Constants;
using Business.Rules;
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

        public async Task<IDataResult<List<Rental>>> GetAllAsync()
        {
            var result = await _rentalDal.GetAllAsync();
            // Not: Core katmanında sınıfın adı 'Succes' ise burayı ona göre güncelle
            return new SuccesDataResult<List<Rental>>(result, "Kiralama kayıtları listelendi.");
        }

        public async Task<IDataResult<Rental>> GetByIdAsync(int rentalId)
        {
            // DÜZELTİLDİ: r.Id yerine r.RentalId kullanıldı
            var result = await _rentalDal.GetAsync(r => r.RentalId == rentalId);
            return new SuccesDataResult<Rental>(result);
        }

        public async Task<IResult> AddAsync(Rental rental)
        {
            await _rentalDal.AddAsync(rental);
            return new SuccessResult("Kiralama kaydı eklendi.");
        }

        public async Task<IResult> UpdateAsync(Rental rental)
        {
            await _rentalDal.UpdateAsync(rental);
            return new SuccessResult("Kiralama kaydı güncellendi.");
        }

        public async Task<IResult> DeleteAsync(Rental rental)
        {
            await _rentalDal.DeleteAsync(rental);
            return new SuccessResult("Kiralama kaydı silindi.");
        }

        [TransactionScopeAspect]
        public async Task<IDataResult<RentalInvoiceDto>> RentCarAsync(Rental rental)
        {
            var ruleResult = await _rules.CheckIfCarIsAvailable(rental.CarId, rental.RentDate, rental.RentEndDate);
            if (!ruleResult.Success)
            {
                return new ErrorDataResult<RentalInvoiceDto>(ruleResult.Message);
            }

            var carResult = await _carService.GetByIdAsync(rental.CarId);
            if (!carResult.Success)
            {
                return new ErrorDataResult<RentalInvoiceDto>("Araba bilgisi bulunamadı.");
            }
            var car = carResult.Data;

            int totalDays = (rental.RentEndDate - rental.RentDate).Days;
            if (totalDays <= 0) totalDays = 1;
            decimal totalPrice = totalDays * car.DailyPrice;

            var paymentResult = _paymentService.Pay(totalPrice);
            if (!paymentResult.Success)
            {
                return new ErrorDataResult<RentalInvoiceDto>("Ödeme reddedildi. Bakiye yetersiz.");
            }

            await _rentalDal.AddAsync(rental);

            car.Description += " - KİRALANDI";
            await _carService.UpdateAsync(car);

            var invoiceResult = await _rentalDal.GetRentalDetailsAsync(r => r.RentalId == rental.RentalId);
            var invoice = invoiceResult.FirstOrDefault();

            return new SuccesDataResult<RentalInvoiceDto>(invoice, $"Ödeme alındı, kiralama işlemi tamamlandı. RentalId: {rental.RentalId}");
        }
    }
}