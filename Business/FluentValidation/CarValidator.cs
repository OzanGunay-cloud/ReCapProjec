using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class CarValidator : AbstractValidator<Car>
    {
        public CarValidator()
        {
            RuleFor(c => c.CarName).NotEmpty();
            RuleFor(C => C.CarName).MaximumLength(10);


            RuleFor(c=>c.DailyPrice).NotEmpty();
            RuleFor(c=>c.DailyPrice).GreaterThan(0);


            // İlişki (ID) Kuralları
            RuleFor(c => c.BrandId).NotEmpty().WithMessage("Marka seçimi zorunludur.");
            RuleFor(c => c.ColorId).NotEmpty().WithMessage("Renk seçimi zorunludur.");

            // Model Yılı Kuralları (Ekstra)
            RuleFor(c => c.ModelYear).GreaterThan(1990);



        }


    }

}