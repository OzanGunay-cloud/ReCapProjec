using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DependencyResolvers
{
    public class CoreModule : ICoreModule
    {
        public void Load(IServiceCollection serviceCollection)
        {
            // Kullanıcı bilgilerini (Token, Claims) okuyabilmek için HttpContext'e her yerden erişim sağlıyoruz
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // İleride buraya MemoryCache, Redis veya farklı altyapı servislerini de tek satırla ekleyebileceğiz.
        }
    }
}