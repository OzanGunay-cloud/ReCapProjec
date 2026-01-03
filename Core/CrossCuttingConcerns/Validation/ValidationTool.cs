using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Core.CrossCuttingConcerns.Validation
{
    // static class: Bu sınıftan "new" yaparak nesne üretmene gerek yok. 
    // Doğrudan ValidationTool.Validate(...) diyerek her yerden çağırabilirsin.
    public static class ValidationTool
    {
        /// <summary>
        /// Nesneleri FluentValidation kurallarına göre doğrulamak için kullanılan merkezi araç.
        /// </summary>
        /// <param name="validator">Doğrulama kurallarının olduğu sınıf (Örn: ProductValidator)</param>
        /// <param name="entity">Doğrulanacak olan veri/nesne (Örn: Product)</param>
        public static void Validate(IValidator validator, object entity)
        {
            // 1. ADIM: Doğrulama Bağlamı (Context) oluşturma.
            // FluentValidation, doğrulanacak nesneyi bir "ValidationContext" içine koymanı ister.
            // Burada 'object' kullanarak her türlü sınıfın (User, Product, vb.) buraya girebilmesini sağlıyoruz.
            var context = new ValidationContext<object>(entity);

            // 2. ADIM: Asıl doğrulama işlemini başlatma.
            // Parametre olarak gelen 'validator' nesnesinin içindeki .Validate() metodunu tetikleriz.
            // Bu metot, yazdığın tüm kuralları (NotEmpty, MinimumLength vb.) tek tek kontrol eder.
            var result = validator.Validate(context);

            // 3. ADIM: Hata kontrolü.
            // FluentValidation'da doğrulama bittiğinde sonuç 'result' içine atılır.
            // Not: Genellikle 'if (!result.IsValid)' şeklinde kontrol edilir (Geçerli değilse).
            // Eğer result başarılı değilse, FluentValidation'ın kendi hata fırlatma mekanizmasını çalıştırırız.
            if (!result.IsValid)
            {
                // result.Errors içinde hangi alanın neden hatalı olduğu bilgisi (liste olarak) bulunur.
                // throw diyerek uygulamayı durdururuz ve bu hataları dışarı fırlatırız.
                throw new ValidationException(result.Errors);
            }
        }
    }
}