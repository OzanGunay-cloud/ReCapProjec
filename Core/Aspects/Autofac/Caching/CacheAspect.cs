using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Caching;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspects.Autofac.Caching
{
    public class CacheAspect : MethodInterception
    {
        private int _duration;
        private ICacheManager _cacheManager;

        public CacheAspect(int duration = 60)
        {
            _duration = duration;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        public override void Intercept(IInvocation invocation)
        {
            // Metodun ismini ve parametrelerini alarak benzersiz bir "Key" oluşturuyoruz
            // Örn: Business.Abstract.ICarService.GetAll()
            var methodName = string.Format($"{invocation.Method.ReflectedType.FullName}.{invocation.Method.Name}");
            var arguments = invocation.Arguments.ToList();
            var key = $"{methodName}({string.Join(",", arguments.Select(x => x?.ToString() ?? "<Null>"))})";

            if (_cacheManager.IsAdd(key))
            {
                // Eğer veri cache'de varsa, metodu hiç çalıştırmadan (veritabanına gitmeden) veriyi oradan döndürür
                invocation.ReturnValue = _cacheManager.Get(key);
                return;
            }

            // Veri cache'de yoksa metodu normal şekilde çalıştır (veritabanına git)
            invocation.Proceed();

            // Veritabanından gelen sonucu cache'e ekle ki bir sonraki sefer RAM'den gelsin
            _cacheManager.Add(key, invocation.ReturnValue, _duration);
        }
    }
}