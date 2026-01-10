Bu branch, ReCap Project sistemine güvenli erişim kontrolü, kullanıcı yönetimi ve JWT (JSON Web Token) tabanlı yetkilendirme altyapısının entegre edildiği geliştirme aşamasını içermektedir.

Eklenen Özellikler
JWT Entegrasyonu: Kullanıcıların sisteme güvenli bir şekilde giriş yapması ve sonraki isteklerinde kimliklerini kanıtlaması için JWT altyapısı kuruldu.

Claim Bazlı Yetkilendirme: Kullanıcılara belirli roller (Admin, Editor vb.) atanması ve bu rollere göre işlem yapma yetkisi verilmesi sağlandı.

Secured Operation Aspect: İş mantığı (Business) katmanındaki metodlara, sadece belirli yetkilere sahip kullanıcıların erişmesini sağlayan [SecuredOperation] aspect yapısı eklendi.

Kullanıcı ve Rol Yönetimi:

User: Kullanıcı bilgilerinin tutulduğu tablo.

OperationClaim: Sistemdeki yetki isimlerinin (Örn: car.add, admin) tutulduğu tablo.

UserOperationClaim: Kullanıcılar ile yetkilerin eşleştirildiği tablo.

Auth Service: Kayıt olma (Register), giriş yapma (Login) ve kullanıcı varlığı kontrolü gibi süreçler yönetilmeye başlandı.

 Teknik Detaylar
Core Katmanı: Güvenlik ile ilgili temel sınıflar (Security, JWT, Hashing, Encryption) bu katmana taşınarak projenin diğer modüllerinden bağımsız bir yapı oluşturuldu.

Hashing & Salting: Kullanıcı şifreleri veritabanında açık metin olarak değil, HMACSHA512 algoritması kullanılarak hashlenmiş ve tuzlanmış (salted) şekilde saklanacak şekilde güncellendi.

Autofac AOP: Yetki kontrolleri, metodun gövdesine müdahale etmeden bir "Aspect" olarak metodun başında çalışacak şekilde tasarlandı.
