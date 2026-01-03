using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.DependencyResolvers.Autofac;
// using Core.Extensions.Middleware; // Eðer bu klasör ve sýnýf yoksa hata verir, varsa kalsýn.

var builder = WebApplication.CreateBuilder(args);

// --- AUTOFAC ENTEGRASYONU ---
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new AutofacBusinessModule());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 1. HATA YAKALAMA (Eðer hata almaya devam edersen burayý geçici olarak YORUM SATIRI YAP) ---
// app.ConfigureCustomExceptionMiddleware(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- 2. EKLENMESÝ GEREKEN KRÝTÝK SATIR ---
// Bu olmazsa yüklediðin resimleri göremezsin!
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();