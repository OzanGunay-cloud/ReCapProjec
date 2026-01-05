using System;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Core.Utilities.Results;
using Business.Abstract; // ICarService için gerekli

namespace Business.Rules
{
    public class RentalBusinessRules
    {
        private readonly IRentalDal _rentalDal;
        private readonly ICarService _carService; // CarService'i buraya ekledik

        public RentalBusinessRules(IRentalDal rentalDal, ICarService carService)
        {
            _rentalDal = rentalDal;
            _carService = carService; // Constructor'da enjekte ettik
        }

        public async Task<IResult> CheckIfCarIsAvailable(int carId, DateTime rentDate, DateTime rentEndDate)
        {
            // 1. ADIM: İsim uyuşmazlığı giderildi (GetByIdAsync) ve await eklendi.
            var carResult = await _carService.GetByIdAsync(carId);

            if (!carResult.Success || carResult.Data == null || !carResult.Data.IsAvailable)
            {
                return new ErrorResult("Bu araç şu an kiralanabilir durumda değil (Bakımda veya Pasif)!");
            }

            // 2. ADIM: Tarih Çakışması Kontrolü
            var conflict = await _rentalDal.GetAsync(r =>
                r.CarId == carId &&
                r.ReturnDate == null &&
                (rentDate < r.RentEndDate && rentEndDate > r.RentDate)
            );

            if (conflict != null)
            {
                return new ErrorResult("Araç seçilen tarihler arasında zaten kiralanmış.");
            }

            return new SuccessResult();
        }
    }
}