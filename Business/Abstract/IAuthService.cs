using Core.Entities.Concrete;
using Core.Entities.DTOs;
using Core.Utilities.Results;
using Core.Utilities.Security.JWT;
using System.Threading.Tasks;

namespace Business.Abstract // Eksik olan namespace burası!
{
    public interface IAuthService
    {
        Task<IDataResult<User>> Register(UserForRegisterDto userForRegisterDto, string password);
        Task<IDataResult<User>> Login(UserForLoginDto userForLoginDto);
        Task<IResult> UserExists(string email);
        Task<IDataResult<AccessToken>> CreateAccessToken(User user);
    }
}