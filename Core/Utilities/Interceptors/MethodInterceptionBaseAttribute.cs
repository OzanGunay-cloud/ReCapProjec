using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Core.Utilities.Interceptors
{
    // Bu sınıf, metotların üzerine koyacağımız etiketlerin (Attribute) temelidir.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    // Dosyanın içindeki IInterceptor yazan yeri şu şekilde güncelle:
    public abstract class MethodInterceptionBaseAttribute : Attribute, Castle.DynamicProxy.IInterceptor
    {
        public int Priority { get; set; }

        public virtual void Intercept(IInvocation invocation)
        {
        }
    }
}