using Business.Abstract;
using Business.Constants;
using Business.Rules; // Eğer Rules kullanıyorsan
using Core.Aspects.Autofac.Transaction;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class RentalManager : IRentalService
    {
        private readonly IRentalDal _rentalDal;
        private readonly ICarService _carService; // Transaction için gerekli
        private readonly RentalBusinessRules _rules; 

        public RentalManager(IRentalDal rentalDal, ICarService carService, RentalBusinessRules rules    )
        {
            _rentalDal = rentalDal;
            _carService = carService;
            _rules = rules;
        }

        // --- STANDART CRUD İŞLEMLERİ (EKSİK OLANLAR) ---

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

        public async Task<IResult> AddAsync(Rental rental)
        {
            // İstersen burada da kurallar çalıştırabilirsin
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

        // --- TRANSACTION İÇEREN ÖZEL METOT ---

        [TransactionScopeAspect]
        // 3. METOT GÜNCELLENDİ
        public async Task<IResult> RentCarAsync(Rental rental)
        {
            // --- KURAL KONTROLÜ BAŞLIYOR ---
            // "Git bakalım, bu tarihlerde bu araba müsait mi?"
            var ruleResult = await _rules.CheckIfCarIsAvailable(rental.CarId, rental.RentDate, rental.RentEndDate);

            // Eğer kuraldan hata döndüyse (Success == false), işlemi durdur ve hatayı kullanıcıya göster.
            if (!ruleResult.Success)
            {
                return ruleResult; // "Araç istenen tarihlerde doludur" mesajı döner.
            }
            // --- KURAL KONTROLÜ BİTTİ ---


            // Eğer buraya geldiyse araç boştur, kiralamayı yapabiliriz.
            await _rentalDal.AddAsync(rental);

            // ... (Geri kalan kodların aynı kalabilir) ...

            // Arabayı bul ve açıklamasını güncelle (Senin eski kodun)
            // Not: Bu kısım opsiyoneldir, her kiralamada description güncellemek şart değil ama sen istediğin için kalsın.
            var carResult = await _carService.GetByIdAsync(rental.CarId);
            if (carResult.Success)
            {
                var car = carResult.Data;
                car.Description += " - KİRALANDI";
                await _carService.UpdateAsync(car);
            }

            return new SuccessResult("Kiralama işlemi başarıyla tamamlandı.");
        }
    }
}