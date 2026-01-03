using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interceptors;
using FluentValidation;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect : MethodInterception
    {
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            // Gönderilen tip bir IValidator (FluentValidation) değilse hata veriyoruz.
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception("Bu bir doğrulama sınıfı değil");
            }

            _validatorType = validatorType;
        }

        protected override void OnBefore(IInvocation invocation)
        {
            // Çalışma anında (Runtime) validator'ın bir örneğini oluşturuyoruz.
            var validator = (IValidator)Activator.CreateInstance(_validatorType);

            // Validator'ın çalışma tipini (Örn: Car) buluyoruz.
            var entityType = _validatorType.BaseType.GetGenericArguments()[0];

            // Metodun parametrelerini geziyoruz ve o tipe uygun olanları doğruluyoruz.
            var entities = invocation.Arguments.Where(t => t.GetType() == entityType); //bu metot bir dizi döner 

            foreach (var entity in entities)
            {
                ValidationTool.Validate(validator, entity);
            }
        }
    }
}