using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class BrandManager : IBrandService
    {

        private readonly IBrandDal _brandDal;

        public BrandManager(IBrandDal brandDal)
        {
            // BURASI BOŞ KALMIŞ OLABİLİR!
            _brandDal = brandDal;

        }
        public async Task<IResult> AddAsync(Brand brand)
        {
          await _brandDal.AddAsync(brand);
          
            return new SuccessResult("marka başarıyla eklendi ");

        }

        public async Task<IResult> DeleteAsync(Brand brand)
        {
           await _brandDal.DeleteAsync(brand);
            return new SuccessResult("Marka başarıyla silindi ");



        }

        public async Task<IDataResult<List<Brand>>> GetAllAsync()
        {
            return new SuccesDataResult<List<Brand>>(await _brandDal.GetAllAsync());
        }


        

        public async Task<IDataResult<Brand>> GetByIdAsync(int brandId)
        {
            return new SuccesDataResult<Brand>(await _brandDal.GetAsync(b=>b.BrandId==brandId));
            
        }

        public async Task<IResult> UpdateAsync(Brand brand)
        {
          await _brandDal.UpdateAsync(brand);
            return new SuccessResult("Marka başarıyla güncellendi");

        }
    }
}
