using Business.Abstract;
using Core.Entities.Concrete;
using Core.Entities.DTOs;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.JWT;
using System.Collections.Generic;
using System.Threading.Tasks; // Task kullanımı için şart

namespace Business.Concrete // Eksik olan namespace eklendi
{
    public class AuthManager : IAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        public async Task<IDataResult<User>> Register(UserForRegisterDto userForRegisterDto, string password)
        {
            byte[] passwordHash, passwordSalt;
            // image_0ed88c.png'deki HashingHelper ile şifreyi mühürlüyoruz
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            var user = new User
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            };

            // UserManager asenkron olduğu için await ekledik
            await _userService.Add(user);
            return new SuccesDataResult<User>(user, "Kayıt oldu");
        }

        public async Task<IDataResult<User>> Login(UserForLoginDto userForLoginDto)
        {
            // GetByMail asenkron olduğu için await ile gerçek User nesnesini çıkarıyoruz
            var userToCheck = await _userService.GetByMail(userForLoginDto.Email);

            if (userToCheck == null)
            {
                return new ErrorDataResult<User>("Kullanıcı bulunamadı");
            }

            if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>("Şifre hatası");
            }

            return new SuccesDataResult<User>(userToCheck, "Giriş başarılı");
        }

        public async Task<IResult> UserExists(string email)
        {
            // Burayı da asenkron yapıya uygun şekilde await ile güncelledik
            var user = await _userService.GetByMail(email);
            if (user != null)
            {
                return new ErrorResult("Kullanıcı zaten mevcut");
            }
            return new SuccessResult();
        }

        public async Task<IDataResult<AccessToken>> CreateAccessToken(User user)
        {
            // GetClaims artık asenkron, mutlaka await edilmeli
            var claims = await _userService.GetClaims(user);
            var accessToken = _tokenHelper.CreateToken(user, claims);
            return new SuccesDataResult<AccessToken>(accessToken, "Token oluşturuldu");
        }
    }
}