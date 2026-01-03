using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using System.Transactions;

namespace Core.Aspects.Autofac.Transaction
{
    public class TransactionScopeAspect : MethodInterception
    {
        public override void Intercept(IInvocation invocation)
        {
            // TransactionScope, içindeki tüm işlemler bitene kadar hiçbirini veritabanına kalıcı yazmaz.
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    // Metodu çalıştır (Örn: AddAsync veya Rental işlemi)
                    invocation.Proceed();

                    // Eğer hata çıkmadıysa tüm işlemleri onayla (Commit)
                    transactionScope.Complete();
                }
                catch (System.Exception e)
                {
                    // Hata çıkarsa her şeyi geri al (Rollback)
                    transactionScope.Dispose();
                    throw; // Hatayı yukarı fırlat ki Middleware yakalasın
                }
            }
        }
    }
}