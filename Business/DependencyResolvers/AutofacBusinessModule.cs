using Autofac;
using Autofac.Extras.DynamicProxy;
using Business.Abstract;
using Business.Concrete;
using Business.Rules;
using Castle.DynamicProxy;
using Core.Utilities.Helpers.FileHelper;
using Core.Utilities.Interceptors;
using Core.Utilities.Security.JWT;
using DataAccess.Abstract;
using DataAccess.Concrete;
using DataAccess.Concrete.EntityFramework;


namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 1. Veritabanı Bağlamı (Context) - Scoped (Her istekte yeni)
            builder.RegisterType<ReCapContext>().AsSelf().InstancePerLifetimeScope();

            // 2. Car (Araba) Modülü - Context kullandığı için Scoped olmak ZORUNDA
            builder.RegisterType<CarManager>().As<ICarService>().InstancePerLifetimeScope();
            builder.RegisterType<EfCarDal>().As<ICarDal>().InstancePerLifetimeScope();

            // 3. Brand (Marka) Modülü
            builder.RegisterType<BrandManager>().As<IBrandService>().InstancePerLifetimeScope();
            builder.RegisterType<EfBrandDal>().As<IBrandDal>().InstancePerLifetimeScope();

            // 4. Color (Renk) Modülü
            builder.RegisterType<ColorManager>().As<IColorService>().InstancePerLifetimeScope();
            builder.RegisterType<EfColorDal>().As<IColorDal>().InstancePerLifetimeScope();

            // 5. Rental (Kiralama) Modülü - HATANIN KAYNAĞI BURASIYDI (SingleInstance kalmıştı)
            // DÜZELTME: InstancePerLifetimeScope yapıldı.
            builder.RegisterType<RentalManager>().As<IRentalService>().InstancePerLifetimeScope();
            builder.RegisterType<EfRentalDal>().As<IRentalDal>().InstancePerLifetimeScope();
            builder.RegisterType<RentalBusinessRules>().AsSelf().InstancePerLifetimeScope();

            // 6. CarImage (Resim) Modülü - HATANIN DİĞER KAYNAĞI
            // DÜZELTME: InstancePerLifetimeScope yapıldı.
            builder.RegisterType<CarImageManager>().As<ICarImageService>().InstancePerLifetimeScope();
            builder.RegisterType<EfCarImageDal>().As<ICarImageDal>().InstancePerLifetimeScope();

            // 7. Kurallar
            builder.RegisterType<CarBusinessRules>().AsSelf().InstancePerLifetimeScope();

            // 8. Dosya Yöneticisi - Veritabanı kullanmadığı için Singleton kalabilir (Performanslıdır)
            builder.RegisterType<FileHelperManager>().As<IFileHelper>().SingleInstance();



            builder.RegisterType<PaymentManager>().As<IPaymentService>().SingleInstance();

            // User Servisleri
            builder.RegisterType<UserManager>().As<IUserService>().SingleInstance();
            builder.RegisterType<EfUserDal>().As<IUserDal>().SingleInstance();

            // AuthManager'ı Scoped (İstek başına bir tane) yapıyoruz
            builder.RegisterType<AuthManager>().As<IAuthService>().InstancePerLifetimeScope();

            // JwtHelper'ı Singleton (Uygulama boyu bir tane) bırakabiliriz
            builder.RegisterType<JwtHelper>().As<ITokenHelper>().SingleInstance();


            // --- AOP (Aspect Oriented Programming) ---
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions()
                {
                    Selector = new AspectInterceptorSelector()
                }).InstancePerLifetimeScope(); // Burası da Scoped olmalı
        }
    }
}