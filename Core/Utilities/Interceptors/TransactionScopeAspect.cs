using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using System.Transactions;
using System.Threading.Tasks; // Bunu eklemeyi unutma

namespace Core.Aspects.Autofac.Transaction
{
    public class TransactionScopeAspect : MethodInterception
    {
        public override void Intercept(IInvocation invocation)
        {
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    invocation.Proceed();

                    // --- KRİTİK DÜZELTME BURASI ---
                    // Eğer çalıştırılan metot asenkron (Task) ise, metodun bitmesini bekle!
                    if (invocation.ReturnValue is Task task)
                    {
                        // Task bitene kadar Scope'un kapanmasını engelliyoruz
                        task.GetAwaiter().GetResult();
                    }
                    // ------------------------------

                    transactionScope.Complete();
                }
                catch (System.Exception)
                {
                    transactionScope.Dispose();
                    throw;
                }
            }
        }
    }
}