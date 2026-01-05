using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Core.Extensions
{
    public static class ClaimExtensions
    {
        /// <summary>
        /// Giriş yapmış kullanıcının (ClaimsPrincipal) içindeki "Rol" bilgilerini ayıklar.
        /// </summary>
        /// <param name="claimsPrincipal">Metodun eklendiği (extend edildiği) kullanıcı nesnesi</param>
        /// <returns>Kullanıcının sahip olduğu rollerin listesini (string olarak) döner.</returns>
        public static List<string> ClaimRoles(this ClaimsPrincipal claimsPrincipal)
        {
            // 1. claimsPrincipal.Claims: Kullanıcının kimlik kartındaki TÜM bilgileri (ad, soyad, email, roller vb.) getirir.

            return claimsPrincipal.Claims
                // 2. Where(c => c.Type == ClaimTypes.Role): 
                // "Sıradaki bilgiye (c) bak; Eğer tipi (Type) bir Rol (ClaimTypes.Role) ise onu süzgecin üstünde tut."
                .Where(c => c.Type == ClaimTypes.Role)

                // 3. Select(c => c.Value): 
                // "Süzgeçte kalan rol nesnelerinin içinden sadece metin değerlerini (örneğin: 'Admin', 'Editor') çek al."
                .Select(c => c.Value)

                // 4. ToList(): 
                // "Elde ettiğin bu metin değerlerini bir liste (List<string>) haline getir."
                .ToList();
        }
    }
}