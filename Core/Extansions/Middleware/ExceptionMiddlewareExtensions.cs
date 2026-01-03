using Microsoft.AspNetCore.Builder;

namespace Core.Extensions.Middleware
{
    // Bu sınıf bir "Extension" (Genişletme) sınıfıdır.
    // AMACI: Program.cs dosyasında "app.UseMiddleware<ExceptionMiddleware>()" gibi uzun kodlar yazmak yerine,
    // kendi isimlendirdiğimiz bir metodu tek satırda çağırabilmektir.
    public static class ExceptionMiddlewareExtensions
    {
        // C# Bilgisi: Bir metot parametresinde "this" kelimesi varsa, o bir Extension Metottur.
        // Yani bu metot, IApplicationBuilder (yani Program.cs'deki 'app') nesnesine yeni bir yetenek kazandırır.
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            // Asıl işi yapan, motoru çalıştıran kod burasıdır.
            // Bizim yazdığımız "ExceptionMiddleware" sınıfını alır ve ASP.NET Core'un
            // çalışma borusuna (Pipeline) "Güvenlik Görevlisi" olarak ekler.
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}