using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Core.Entities.Concrete;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.JWT // Eksik olan en kritik satır!
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }
        private TokenOptions _tokenOptions;

        public JwtHelper(IConfiguration configuration) // 1. İstek: "Bana ayarları getir"
        {
            Configuration = configuration; // 2. Atama: "Gelen ayarları sınıf içindeki değişkene aktar"

            // 3. Kullanım: "Aktardığım ayarlardan sadece TokenOptions kısmını ayıkla ve nesneye dönüştür"
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
        }

        public AccessToken CreateToken(User user, List<OperationClaim> operationClaims)
        {
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var sessionId = Guid.NewGuid().ToString("N");

            var jwtToken = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                expires: DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration),
                signingCredentials: signingCredentials,
                claims: SetClaims(user, operationClaims, sessionId)
            );

            return new AccessToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Expiration = jwtToken.ValidTo,
                SessionId = sessionId
            };
        }

        private IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims, string sessionId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, sessionId)
            };

            claims.AddRange(operationClaims.Select(oc => new Claim(ClaimTypes.Role, oc.Name))); // burada new claim diyerek nesne olusturuyoruz select ile
                                                                                               //değişken olan bu parçayıda sorgudan geçirip liste sonuna ekliyruoz
            return claims;
        }
    }
}
