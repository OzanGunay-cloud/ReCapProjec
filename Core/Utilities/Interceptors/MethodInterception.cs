using Castle.DynamicProxy;

namespace Core.Utilities.Interceptors
{
    // Hangi durumlarda araya girmek istediğimizi belirlediğimiz ana sınıf.
    public abstract class MethodInterception : MethodInterceptionBaseAttribute
    {
        protected virtual void OnBefore(IInvocation invocation) { }
        protected virtual void OnAfter(IInvocation invocation) { }
        protected virtual void OnException(IInvocation invocation, Exception e) { }
        protected virtual void OnSuccess(IInvocation invocation) { }

        public override void Intercept(IInvocation invocation)
        {
            var isSuccess = true;
            OnBefore(invocation); // Metot çalışmadan önce
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                isSuccess = false;
                OnException(invocation, e); // Hata aldığında
                throw;
            }
            finally
            {
                if (isSuccess)
                {
                    OnSuccess(invocation); // Başarılı olduğunda
                }
            }
            OnAfter(invocation); // Her durumda metot bittiğinde
        }
    }
}