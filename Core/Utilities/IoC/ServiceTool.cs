using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.Utilities.IoC
{
    /// <summary>
    /// Servislerin (bağımlılıkların) merkezi olarak yönetilmesini ve her yerden erişilmesini sağlayan araç sınıfı.
    /// </summary>
    public static class ServiceTool
    {
        // IServiceProvider: İçerisinde kayıtlı olan tüm servisleri (IProductService, IHttpContextAccessor vb.) 
        // barındıran ve ihtiyaç duyulduğunda bize veren "Merkezi Servis Kutusu"dur.
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// .NET'in servis koleksiyonunu alır, build eder ve ServiceProvider çekmecesine koyar.
        /// </summary>
        /// <param name="services">Uygulama başladığında (Program.cs'de) kayıt edilen servislerin listesi.</param>
        /// <returns>Aynı servis listesini geri döndürür (zincirleme kullanım için).</returns>
        public static IServiceCollection Create(IServiceCollection services)
        {
            // .BuildServiceProvider(): Şu ana kadar listeye (IServiceCollection) eklediğimiz 
            // tüm servisleri "çalışmaya hazır" hale getirir ve bir paket (ServiceProvider) oluşturur.
            ServiceProvider = services.BuildServiceProvider();

            // Bu paketi yukarıdaki static 'ServiceProvider' özelliğine atıyoruz ki 
            // projenin her yerinden bu pakete ulaşabilelim.
            return services;
             /*
             return services; satırı ise; projenin vitrininde (Program.cs gibi yerlerde) kod yazımının kesintisiz ve 
            şık bir şekilde devam etmesini sağlar.
             */
            
        }
    }
}