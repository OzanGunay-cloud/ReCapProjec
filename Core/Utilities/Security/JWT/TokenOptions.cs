
namespace Core.Utilities.Security.JWT;
public class TokenOptions


{
    public string Audience { get; set; } // Hedef kitle
    public string Issuer { get; set; } // Yayınlayan (senin sistemin)
    public int AccessTokenExpiration { get; set; } // Geçerlilik süresi (dakika)
    public string SecurityKey { get; set; } // Gizli anahtar metni
}