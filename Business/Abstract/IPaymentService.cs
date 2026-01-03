using Core.Utilities.Results;

public interface IPaymentService
{
    // Kart bilgilerini ve tutarı alıp ödeme başarılı mı diye bakacak
    IResult Pay(decimal amount);
}