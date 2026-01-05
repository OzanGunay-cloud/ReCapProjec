using Business.Constants;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Core.Extensions;

namespace Business.BusinessAspects.Autofac
{
    // Bu sınıf, üzerine eklendiği metodun çalışmasından önce (OnBefore) yetki kontrolü yapar.
    public class SecuredOperation : MethodInterception
    {
        private string[] _roles;
        private IHttpContextAccessor _httpContextAccessor;

        public SecuredOperation(string roles)
        {
            // "admin,editor" gibi metin olarak gelen rolleri virgülle ayırıp diziye çevirir
            _roles = roles.Split(',');

            // Core katmanında yazdığımız ServiceTool sayesinde servislerimize (DI) ulaşıyoruz
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            // Kullanıcının token içindeki rollerini çekiyoruz (Core.Extensions'daki ClaimRoles kullanılıyor)
            var roleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();

            foreach (var role in _roles)
            {
                // Eğer kullanıcının rolleri arasında istenen yetki varsa metodun çalışmasına izin ver
                if (roleClaims.Contains(role))
                {
                    return;
                }
            }
            // Eğer hiçbir rol eşleşmezse hata fırlat ve işlemi durdur
            throw new Exception("Yetkiniz yok");
        }
    }
}