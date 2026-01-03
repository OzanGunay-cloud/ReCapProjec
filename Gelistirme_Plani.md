# Proje Analizi ve Geliştirme Planı

Bu rapor, mevcut projenin durumunu analiz eder ve eksik olan temel özellikleri, entity'leri ve mimari yapı taşlarını listeler.

## Mevcut Durum (Özet)
- **Entities**: Temel varlıklar (`Car`, `Brand` vs.) var ancak `User` ve `Customer` eksik veya boş.
- **Business Katmanı**: Temel CRUD operasyonları var. `RentalManager` ve `TransactionScopeAspect` implemente edilmiş durumda.

## Eksik Görülen ve Eklenmesi Gerekenler Listesi

### 1. Entity Katmanı (Eksik Varlıklar ve Özellikler)
Mevcut entity'lere ek olarak aşağıdaki tabloların ve özelliklerin eklenmesi gerekmektedir:

#### A. Yeni Entities
- **[NEW] `CarImage`**: Arabaların resimlerini tutmak için.
  - *Properties*: `Id`, `CarId`, `ImagePath`, `Date`.
- **[NEW] `User`** (Core veya Entities katmanında): Kullanıcı yönetimi için.
  - *Properties*: `Id`, `FirstName`, `LastName`, `Email`, `PasswordHash`, `PasswordSalt`, `Status`.
- **[NEW] `OperationClaim`**: Rol yönetimi için (Admin, User vs.).
  - *Properties*: `Id`, `Name`.
- **[NEW] `UserOperationClaim`**: Kullanıcı-Rol ilişkisi için.
  - *Properties*: `Id`, `UserId`, `OperationClaimId`.
- **[NEW] `Customer`**: Müşterileri Kullanıcılardan ayırmak veya ilişkilendirmek için.
  - *Properties*: `UserId`, `CompanyName`, `FindexScore` (Kredi notu simülasyonu için).
- **[NEW] `CreditCard`**: Ödeme simülasyonu ve kart kaydetme için.
  - *Properties*: `CardNumber`, `ExpiryDate`, `Cvv`, `CardHolderName`, `CustomerId`.

#### B. Mevcut Entity Güncellemeleri
- **`Car.cs`**:
  - `MinFindexScore`: Arabanın kiralanabilmesi için gereken minimum findeks puanı.
- **Tüm Entity'ler için (Opsiyonel ama önerilen)**:
  - `CreatedDate`, `UpdatedDate`, `IsActive` gibi audit alanları (BaseEntity üzerinden).

### 2. Core Katmanı ve Mimari (Altyapı Eksikleri)
- **Authentication & Authorization (JWT)**:
  - `ITokenHelper`, `JwtHelper` implementasyonları.
  - `AuthManager` (Register, Login işlemleri).
  - Hashing Helper (Şifrelerin hashlenmesi).
- **Aspect'ler (Cross Cutting Concerns)**:
  - **[SecuredOperation]**: Metot bazlı yetkilendirme (Örn: Sadece admin araç ekleyebilir).
  - **[CacheAspect]**: Sık kullanılan verilerin önbelleğe alınması (Tamamen eksik).
  - **[PerformanceAspect]**: Yavaş çalışan metotların tespiti.

### 3. Business Katmanı (İş Kuralları ve Mantık)
- **Resim Yükleme Kuralları**:
  - Bir arabanın en fazla 5 resmi olabilir.
  - Resim yüklenmezse default bir resim (logo) gösterilmelidir.
- **Kiralama Kuralları**:
  - Kullanıcının Findeks puanı, arabanın `MinFindexScore` değerinden düşükse kiralama yapılamaz.
  - Kullanıcının kredi kartı bilgileri doğrulanmalıdır (Fake Banka Servisi).
- **Yetkilendirme Kontrolleri**:
  - Araç ekleme/silme/güncelleme işlemlerini sadece `Admin` veya `Car.Add` yetkisi olanlar yapabilmeli.

### 4. WebAPI Katmanı
- **`AuthController`**: Login ve Register endpointleri.
- **Resim Upload Desteği**: `CarImagesController` üzerinden `IFormFile` ile dosya yükleme desteği.

## Özet Yapılacaklar Listesi (Action Items)

1.  **Veritabanı Nesnelerini Tamamla**: `Customer`, `User`, `CarImage` tablolarını ve sınıflarını oluştur.
2.  **Güvenlik Altyapısını Kur**: JWT entegrasyonunu yap ve `User` tablosunu bu yapıya göre düzenle.
3.  **İş Kurallarını Yaz**: Özellikle resim yükleme ve kiralama (Findex) kurallarını Business katmanına ekle.
4.  **API'yi Güncelle**: Yeni entityler için Controller'ları oluştur.

Bu adımları sırayla uygulayarak projenizi kurumsal mimariye uygun, tam kapsamlı bir araç kiralama sistemine dönüştürebilirsiniz.
