using Business.Abstract;
using Core.Utilities.Results;

namespace Business.Concrete
{
    public class PaymentManager : IPaymentService
    {
        public IResult Pay(decimal amount)
        {
            // Şimdilik sahte bir ödeme simülasyonu yapıyoruz.
            // Buraya ileride kart numarası, CVV gibi parametreler ekleyebilirsin.

            if (amount > 100000) // Örnek bir limit kuralı
            {
                return new ErrorResult("Kredi kartı limiti yetersiz.");
            }

            return new SuccessResult("Ödeme başarıyla alındı.");
        }
    }
}