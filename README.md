

Bu branch, veritabanı üzerindeki yükü azaltmak ve kiralama mantığını gerçek hayat senaryolarına (tarih çakışması, araç müsaitliği vb.) uygun hale getirmek için kritik bir adımdır.


🚀 Eklenen Özellikler
1. Caching (Önbellekleme) Altyapısı
Sık kullanılan verilerin her defasında veritabanından çekilmesi yerine bellekte (RAM) tutulmasını sağlayan yapı kuruldu.

CacheAspect: Belirlenen metodların (örneğin CarManager.GetAll) sonuçları, ilk çağrıda belleğe alınır. Sonraki çağrılarda veritabanına gidilmeden bellekten hızlıca yanıt dönülür.

CacheRemoveAspect: Veri manipülasyonu (Ekleme/Silme/Güncelleme) yapıldığında, ilgili cache verisinin (örneğin "tüm arabalar listesi") otomatik olarak temizlenmesini sağlar.

Microsoft Memory Cache: .NET Core'un yerleşik IMemoryCache arayüzü kullanılarak implementasyon yapıldı.

2. Gelişmiş Kiralama Kuralları (Business Rules)
Araç kiralama sürecinin hatasız işlemesi için yeni iş kuralları RentalManager içerisine eklendi:

Araç Müsaitlik Kontrolü: Kiralanmak istenen aracın, talep edilen tarihler arasında başka bir müşteride olup olmadığı kontrol edilir. Çakışma varsa kiralama reddedilir.

Teslim Tarihi Kontrolü: Teslim edilmemiş araçların tekrar kiralanması engellenir.

(Opsiyonel) Findex Puanı Kontrolü: Müşterinin findex puanının, aracı kiralamak için yeterli olup olmadığına dair altyapı hazırlandı.

🛠️ Teknik Detaylar ve Mimari
Core Katmanı: Caching mekanizması, projenin herhangi bir yerinde kullanılabilecek şekilde CrossCuttingConcerns ve Aspects klasörleri altında modüler olarak geliştirildi.

Interceptors (Arayıcılar): Metod çağrımlarını araya girerek yakalayan Autofac Interceptor yapısı kullanıldı.

Dependency Injection: CoreModule içerisinde MemoryCache servisi sisteme enjekte edildi.
