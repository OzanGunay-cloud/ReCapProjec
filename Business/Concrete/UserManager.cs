using Business.Abstract;
using DataAccess.Abstract;
using Core.Entities.Concrete; //
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal; //
        }

        public async Task<List<OperationClaim>> GetClaims(User user)
        {
            // IUserDal içindeki GetClaims'i asenkron bekliyoruz
            return await _userDal.GetClaims(user);
        }

        public async Task Add(User user)
        {
            // Base repository'deki asenkron metodu çağırıyoruz
            await _userDal.AddAsync(user);
        }

        public async Task<User> GetByMail(string email)
        {
            // Tek bir kullanıcıyı asenkron getiriyoruz
            return await _userDal.GetAsync(u => u.Email == email);
        }
    }
}