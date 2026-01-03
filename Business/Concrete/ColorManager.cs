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
    public class ColorManager : IColorService
    {
        private IColorDal _colordal;

        public ColorManager(IColorDal colordal)
        {
            _colordal = colordal;
        }

        public async Task<IResult> AddAsync(Color color)
        {
            await _colordal.AddAsync(color);
            return new SuccessResult("Renk başarıyla eklendi ");
                 

        }

        public async Task<IDataResult<List<Color>>> GetAllAsync()
        {
           
            return new SuccesDataResult<List<Color>>(await _colordal.GetAllAsync());


        }
    }
}
