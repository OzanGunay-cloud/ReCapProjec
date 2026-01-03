using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Core.Extensions.Middleware
{
    // Bu sınıf, uygulamadaki TÜM hataların toplandığı merkezi istasyondur (Global Error Handler).
    // Projenin herhangi bir yerinde hata (Exception) fırlatıldığında, sistem çökmez; buraya düşer.
    public class ExceptionMiddleware
    {
        // _next: Sıradaki işlemin ne olduğunu tutan temsilci (Delegate).
        // Middleware zincirinde "Benden sonra işi kime devredeceğim?" sorusunun cevabı burada tutulur.
        private readonly RequestDelegate _next;


        //program ayağa kalkarken  startup aşamasında app.Build() çalışır. Sıra app.UseMiddleware<ExceptionMiddleware>() satırına gelir.

       //framework burada newleme işlemini bellekte yapar oluşan nesne bellekte saklanır kapanana kadar hizmet verir ctor bu şekilde calısıyo
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Bu metot, her gelen HTTP isteğinde (Request) otomatik olarak çalışır.
        // Tünelin giriş kapısı burasıdır.
        public async Task InvokeAsync(HttpContext httpContext)

            //InvokeAsync (Middleware): Bu, WEB İSTEĞİNİN (HTTP) bekçisidir.Kapsama Alanı: Tüm uygulama.

        {
            try
            {
                // Kural: "Yoluna devam et."
                // Hata yoksa istek bir sonraki adıma (Örn: Manager'a, Veritabanına) geçer.
                await _next(httpContext);
            }
            catch (Exception e)
            {
                // Eğer "try" bloğunun içinde bir yerlerde kod patlarsa (hata verirse),
                // akış kesilir, catch bloğu hatayı havada yakalar ve analiz etmesi için aşağıdaki metoda gönderir.
                await HandleExceptionAsync(httpContext, e);
            }
        }

        // Yakalanan hatayı analiz edip, kullanıcıya düzgün bir JSON cevabı hazırladığımız yer.
        private Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            // Cevabın formatının JSON olacağını söylüyoruz (Tarayıcı veya Postman bunu anlasın diye).
            httpContext.Response.ContentType = "application/json";

            // Varsayılan olarak hatayı "500 - Sunucu Hatası" kabul ediyoruz.
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // --- ÖZEL DURUM KONTROLÜ: VALIDATION ---
            // "Yakalanan hata, bizim ValidationAspect'ten fırlatılan bir doğrulama hatası mı?" diye bakıyoruz.
            if (e is ValidationException validationException)
            {
                // Eğer öyleyse durum kodunu "400 - Hatalı İstek" (Bad Request) yapıyoruz.
                // Çünkü bu sunucunun değil, kullanıcının hatasıdır.
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                // Kullanıcıya özel formatta (ValidationErrorDetails) cevap dönüyoruz.
                // İçine hangi alanların neden hatalı olduğunu (Errors listesi) koyuyoruz.
                return httpContext.Response.WriteAsync(new ValidationErrorDetails
                {
                    StatusCode = 400,
                    Message = "Doğrulama Hatası",
                    Errors = validationException.Errors // Örn: CarName boş olamaz vb.
                }.ToString());
            }

            // --- GENEL SİSTEM HATASI ---
            // Eğer hata validation değilse (Örn: Veritabanı bağlantısı koptu, null hatası vb.)
            // Standart hata formatımızı (ErrorDetails) kullanıyoruz.
            return httpContext.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = e.Message // Hatanın mesajını yazıyoruz.
            }.ToString());
        }
    }
}