
namespace Core.Utilities.Security.JWT;
public class AccessToken
{
    public string Token { get; set; } // Şifreli anahtar metni
    public DateTime Expiration { get; set; } // Anahtarın son kullanma tarihi
    public string SessionId { get; set; } // Oturumun JWT içindeki jti değeri
}
