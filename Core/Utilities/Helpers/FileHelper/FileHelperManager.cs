using System;
using System.Collections.Generic;
using System.IO; // Dosya ve Klasör işlemleri (Directory, File, Path) için gerekli kütüphane
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // IFormFile için gerekli

namespace Core.Utilities.Helpers.FileHelper
{
    // Bu sınıf, projenin "Lojistik Müdürü"dür. Dosyaların fiziksel yönetimi buradan yapılır.
    public class FileHelperManager : IFileHelper
    {
        // YÜKLEME METODU: Dosyayı alır, isimlendirir ve diske kaydeder.
        public string Upload(IFormFile file, string root)
        {
            // 1. Güvenlik Kontrolü: Gelen dosya gerçekten dolu mu? (Boş dosya yüklemeyi engelle)
            if (file.Length > 0)
            {
                // 2. Klasör Kontrolü: Hedef klasör (örn: wwwroot/Uploads/Images) sunucuda var mı?
                if (!Directory.Exists(root))
                {
                    // Eğer klasör yoksa, hata vermemek için hemen oluştur.
                    Directory.CreateDirectory(root);
                }

                // 3. İsimlendirme Stratejisi
                // Kullanıcının dosyasının uzantısını al (Örn: .jpg veya .png)
                string extension = Path.GetExtension(file.FileName);

                // Dünyada eşi benzeri olmayan rastgele bir isim (GUID) üret (Örn: 550e8400-e29b...)
                string guid = Guid.NewGuid().ToString();

                // Benzersiz isim ile uzantıyı birleştir (Örn: 550e8400-e29b.jpg)
                string filePath = guid + extension;

                // 4. Fiziksel Yazma İşlemi (En Kritik Yer)
                // 'using' bloğu: İşlem bittiğinde dosya kilidini kaldırır ve belleği temizler.
                using (FileStream fileStream = File.Create(root + filePath))
                {
                    // Gelen dosyanın içeriğini (byte'larını) oluşturduğumuz boş dosyaya kopyala
                    file.CopyTo(fileStream);

                    // Tampon bellekte (buffer) kalan son verileri de temizleyip dosyaya bas (Sifonu çekmek gibi)
                    fileStream.Flush();
                }

                // 5. Veritabanına kaydedilmesi için SADECE dosyanın yeni adını geri dön
                return filePath;
            }

            // Dosya boşsa veya yoksa null dön
            return null;
        }

        // GÜNCELLEME METODU: Eski resmi siler, yerine yenisini yükler.
        public string Update(IFormFile file, string filePath, string root)
        {
            // 1. Temizlik: Eski dosya sunucuda var mı diye bak.
            // root + filePath = Tam yol (Örn: wwwroot/Uploads/Images/eski_resim.jpg)
            if (File.Exists(root + filePath))
            {
                // Varsa sil ki sunucuda çöp birikmesin.
                File.Delete(root + filePath);
            }

            // 2. Yeniden Kullanım: Silme işi bitince, yukarıdaki Upload metodunu çağırarak yenisini yükle.
            return Upload(file, root);
        }

        // SİLME METODU: Sadece fiziksel silme yapar.
        public void Delete(string filePath)
        {
            // Dosya diskte mevcut mu?
            if (File.Exists(filePath))
            {
                // Mevcutsa fiziksel olarak (kalıcı) sil.
                File.Delete(filePath);
            }
        }
    }
}