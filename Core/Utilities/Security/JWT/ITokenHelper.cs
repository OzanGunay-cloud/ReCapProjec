using Core.Entities.Concrete;
using Core.Utilities.Security.JWT; // Kendisini tanımalı
using System.Collections.Generic;

namespace Core.Utilities.Security.JWT // Bu satırı ekliyoruz!
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(User user, List<OperationClaim> operationClaims);
    }
}