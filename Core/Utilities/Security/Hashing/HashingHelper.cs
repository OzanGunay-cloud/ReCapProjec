
namespace Core.Utilities.Security.Hashing;

public class HashingHelper
{
    
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        // HMACSHA512 sınıfını bir 'using' bloğu içinde açıyoruz.
        // Bu sayede şifreleme işlemi biter bitmez sistem kaynakları (RAM) otomatik olarak temizlenir.
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            // O anki işlem için üretilen rastgele anahtarı salt (tuz) olarak değişkene atıyoruz.
            passwordSalt = hmac.Key;

            // Şifreyi UTF8 formatında byte dizisine çevirip, oluşturulan salt ile hash'liyoruz.
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    
    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        // Veritabanından gelen passwordSalt'ı kullanarak algoritmayı başlatıyoruz.
        // Bu sayede algoritma, kullanıcının girdiği şifreyi aynı "tuz" ile işleyebilir.
        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
        {
            // Kullanıcının login ekranında girdiği şifreyi tekrar hash'liyoruz.
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            // Hesaplanan hash ile veritabanındaki hash'i byte byte karşılaştırıyoruz.
            for (int i = 0; i < computedHash.Length; i++)
            {
                // Eğer herhangi bir byte farklıysa, şifre yanlıştır.
                if (computedHash[i] != passwordHash[i]) return false;
            }
        }

        // Tüm byte'lar başarıyla eşleştiyse şifre doğrudur.
        return true;
    }
}