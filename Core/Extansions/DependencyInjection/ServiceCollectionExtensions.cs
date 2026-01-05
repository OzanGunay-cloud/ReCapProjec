using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions.DependencyInjection
{
    /// <summary>
    /// IServiceCollection (merkezi servis listesi) yapısını genişleterek 
    /// modüllerin topluca sisteme dahil edilmesini sağlayan sınıftır.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Projedeki tüm bağımlılık çözuçülerini (modülleri) sisteme ekler ve ServiceTool'u yapılandırır.
        /// </summary>
        /// <param name="serviceCollection">.NET'in sunduğu merkezi servis koleksiyonu (sepet).</param>
        /// <param name="modules">Sisteme eklenecek olan modüllerin listesi (CoreModule, WebModule vb.).</param>
        /// <returns>Servislerin eklendiği koleksiyonu geri döner.</returns>
        public static IServiceCollection AddDependencyResolvers(this IServiceCollection serviceCollection, ICoreModule[] modules)
        {
            // 1. ADIM: Gelen tüm modülleri (paketleri) tek tek dönüyoruz.
            foreach (var module in modules)
            {
                // Her bir modülün içindeki Load metodunu tetikliyoruz.
                // Bu sayede modül içindeki servisler (örneğin IHttpContextAccessor) sepete ekleniyor.
                module.Load(serviceCollection);
                //Döngü (foreach), modül sayısına göre döner, modülün içindeki servis sayısına göre değil. tek döngüde birden fazla kayıt
            }

            // 2. ADIM: İşte en kritik nokta! 
            // Sepete (serviceCollection) tüm malzemeler eklendikten sonra,
            // bu sepeti ServiceTool.Create metoduna gönderiyoruz.
            // Böylece ServiceTool, içindeki servisleri dondurup saklayabiliyor.
            return ServiceTool.Create(serviceCollection);
        }
    }
}






