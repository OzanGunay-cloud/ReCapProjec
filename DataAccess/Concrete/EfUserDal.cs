using Core.DataAccess;
using DataAccess.Abstract;
using Core.Entities.Concrete;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, ReCapContext>, IUserDal
    {
        public EfUserDal(ReCapContext context) : base(context)
        {
        }

        public async Task<List<OperationClaim>> GetClaims(User user)
        {
            using (var context = new ReCapContext())
            {
                var result = from operationClaim in context.OperationClaims
                             join userOperationClaim in context.UserOperationClaims
                                 on operationClaim.Id equals userOperationClaim.OperationClaimId
                             where userOperationClaim.UserId == user.Id
                             select new OperationClaim { Id = operationClaim.Id, Name = operationClaim.Name };

                // .ToList() yerine .ToListAsync() ve await kullanıyoruz
                return await result.ToListAsync();
            }
        }
    }
    }
