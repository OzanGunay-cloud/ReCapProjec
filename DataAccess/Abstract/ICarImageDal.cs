using Core.DataAccess;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface ICarImageDal : IEntityRepository<CarImage>
    {
        // Şu an için özel bir SQL sorgusuna ihtiyacımız yok, standartlar yeterli.
    }
}