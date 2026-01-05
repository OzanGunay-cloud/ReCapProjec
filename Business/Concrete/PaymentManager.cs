using Business.Abstract;
using Core.Utilities.Results;

namespace Business.Concrete
{
    public class PaymentManager : IPaymentService
    {
        public IResult Pay(decimal amount)
        {
            // Ne gelirse gelsin kabul et
            return new SuccessResult("Ödeme başarılı.");
        }
    }
}