using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using System.Linq;

namespace Business.Rules // İsim alanı değişti
{
    //bu sınıfı static yapar mıyım diye düşündün yapamazsın bu sınıfın db ye ihtiyacı var parametre alıyor Dependency injection 
    public class CarBusinessRules
    {
        private readonly ICarDal _carDal;
   /*CarManager (Servis), iş kurallarını kontrol etmek için CarBusinessRules'u kullanır.

Eğer CarBusinessRules da ICarService'i kullanmaya çalışırsa; Servis ->
        Rules -> Servis şeklinde bir döngü oluşur ve uygulama ayağa kalkarken hata verir.
         
         */

     
        public CarBusinessRules(ICarDal carDal)
        {
            _carDal = carDal;
        }

        // Kural metodumuz
        public async Task<IResult> CheckIfCarDescriptionExists(string description)
        {
            var result = await _carDal.GetAllAsync(c => c.Description == description);
            if (result.Any())
            {
                return new ErrorResult(Messages.CarDescriptionAlreadyExists); //
            }
            return new SuccessResult();
        }
    }
}