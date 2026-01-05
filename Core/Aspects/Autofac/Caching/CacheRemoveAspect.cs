using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Caching;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspects.Autofac.Caching
{
    public class CacheRemoveAspect : MethodInterception
    {
        private string _pattern;
        private ICacheManager _cacheManager;

        public CacheRemoveAspect(string pattern)
        {
            _pattern = pattern;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        protected override void OnSuccess(IInvocation invocation)
        {
            // İşlem başarılı olursa (veritabanına kayıt atılırsa) ilgili desendeki tüm cache'leri siler
            // Örn: "ICarService.Get" dersen, GetAll, GetById gibi tüm okuma cache'lerini temizler
            _cacheManager.RemoveByPattern(_pattern);
        }
    }
}