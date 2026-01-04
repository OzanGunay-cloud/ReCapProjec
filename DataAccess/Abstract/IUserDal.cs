using Core.DataAccess; // Add ve Get metotlarının kaynağı burasıdır!
using Core.Entities.Concrete;
using System.Collections.Generic;

namespace DataAccess.Abstract
{
    // : IEntityRepository<User> eklediğinden emin ol
    public interface IUserDal : IEntityRepository<User>
    {
        Task<List<OperationClaim>> GetClaims(User user);
    }
}