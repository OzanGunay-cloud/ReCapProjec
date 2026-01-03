using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Core.Utilities.Results;
using System.Linq; // Bunu mutlaka ekle!
using Business.Constants;

namespace Business.Rules
{
    public class RentalBusinessRules
    {
        private readonly IRentalDal _rentalDal;

        public RentalBusinessRules(IRentalDal rentalDal)
        {
            _rentalDal = rentalDal;
        }


        public async Task<IResult> CheckIfCarIsAvailable(int carId, DateTime rentDate, DateTime rentEndDate)
        {
            // AnyAsync yoksa GetAsync ile kontrol et
            var conflict = await _rentalDal.GetAsync(r =>
                r.CarId == carId &&
                r.ReturnDate == null && // Araba henüz dönmemişse
                (
                    // Çakışma mantığı
                    r.RentDate < rentEndDate &&
                    r.RentEndDate > rentDate
                )
            );

            if (conflict != null)
            {
                // Exception fırlatma, ErrorResult dön!
                return new ErrorResult("Araç istenen tarihlerde doludur.");
            }

            // Sorun yoksa SuccessResult dön 
            return new SuccessResult();   // bunun içinde exception dönemezsin business rules çünkü result yapısıyla çalışır 
        }



    }



    }


