using Castle.DynamicProxy;
using System.Reflection;

namespace Core.Utilities.Interceptors
{
    public class AspectInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            // Sınıfın üzerindeki etiketleri (Aspect) oku
            var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>
                (true).ToList();

            // Metodun üzerindeki etiketleri (Aspect) oku
            var methodAttributes = type.GetMethod(method.Name)
                .GetCustomAttributes<MethodInterceptionBaseAttribute>(true);

            classAttributes.AddRange(methodAttributes);

            // Tüm aspectleri öncelik sırasına (Priority) göre diz ve geri dön
            return classAttributes.OrderBy(x => x.Priority).ToArray();
        }
    }
}