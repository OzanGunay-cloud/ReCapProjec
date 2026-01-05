using Microsoft.Extensions.DependencyInjection;

namespace Core.Utilities.IoC
{
    // Proje genelinde kullanılacak tüm modüllerin uyması gereken standart
    public interface ICoreModule
    {
        void Load(IServiceCollection serviceCollection);
    }
}