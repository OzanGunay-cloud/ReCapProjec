using Core.Entities.Concrete;
using System.Collections.Generic;
using System.Threading.Tasks; // Bunu ekle

namespace Business.Abstract
{
    public interface IUserService
    {
        Task<List<OperationClaim>> GetClaims(User user);
        Task Add(User user); // void yerine Task
        Task<User> GetByMail(string email); // User yerine Task<User>
    }
}