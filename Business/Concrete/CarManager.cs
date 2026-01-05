using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.Rules;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CarManager : ICarService
    {
        private readonly ICarDal _carDal;
        private readonly CarBusinessRules _rules;

        public CarManager(ICarDal carDal, CarBusinessRules rules)
        {
            _carDal = carDal;
            _rules = rules;
        }

        [SecuredOperation("admin,car.add")]
        [ValidationAspect(typeof(CarValidator))]
        [CacheRemoveAspect("Get")] // DÜZELTİLDİ: "ICarService.Get" yerine "Get" (Kapsamlı temizlik)
        public async Task<IResult> AddAsync(Car car)
        {
            IResult? result = await BusinessRules.RunAsync(_rules.CheckIfCarDescriptionExists(car.Description));
            if (result != null) return result;

            await _carDal.AddAsync(car);
            return new SuccessResult(Messages.CarAdded);
        }

        [CacheAspect]
        public async Task<IDataResult<List<Car>>> GetAllAsync()
        {
            var data = await _carDal.GetAllAsync();
            return new SuccesDataResult<List<Car>>(data, "Arabalar listelendi.");
        }

        [CacheAspect]
        public async Task<IDataResult<Car>> GetByIdAsync(int carId)
        {
            var data = await _carDal.GetAsync(c => c.CarId == carId);
            if (data == null) return new ErrorDataResult<Car>("Araba bulunamadı.");
            return new SuccesDataResult<Car>(data);
        }

        [CacheAspect]
        public async Task<IDataResult<List<Car>>> GetCarsByBrandIdAsync(int id)
        {
            var data = await _carDal.GetAllAsync(c => c.BrandId == id);
            return new SuccesDataResult<List<Car>>(data);
        }

        [CacheAspect]
        public async Task<IDataResult<List<Car>>> GetCarsByColorIdAsync(int id)
        {
            var data = await _carDal.GetAllAsync(c => c.ColorId == id);
            return new SuccesDataResult<List<Car>>(data);
        }

        [ValidationAspect(typeof(CarValidator))]
        [CacheRemoveAspect("Get")] // DÜZELTİLDİ: Araba güncellendiğinde (Kiralandığında/Teslimde) tüm listeleri siler
        public async Task<IResult> UpdateAsync(Car car)
        {
            await _carDal.UpdateAsync(car);
            return new SuccessResult("Araba bilgileri güncellendi.");
        }

        [CacheRemoveAspect("Get")] // DÜZELTİLDİ
        public async Task<IResult> DeleteAsync(Car car)
        {
            await _carDal.DeleteAsync(car);
            return new SuccessResult("Araba sistemden silindi.");
        }

        [CacheAspect]
        public async Task<IDataResult<List<CarDetailDto>>> GetCarDetailsAsync()
        {
            var data = await _carDal.GetCarDetailsAsync();
            return new SuccesDataResult<List<CarDetailDto>>(data, "Detaylı araba listesi getirildi.");
        }

        [CacheAspect]
        public async Task<IDataResult<List<CarDetailDto>>> GetAvailableCarDetailsAsync(DateTime rentDate, DateTime rentEndDate)
        {
            var result = await _carDal.GetAvailableCarDetailsAsync(rentDate, rentEndDate);
            return new SuccesDataResult<List<CarDetailDto>>(result, "Müsait araçlar başarıyla listelendi.");
        }

        [TransactionScopeAspect]
        [CacheRemoveAspect("Get")] // DÜZELTİLDİ
        public IResult TransactionalOperation(Car car)
        {
            _carDal.UpdateAsync(car);
            _carDal.AddAsync(car);
            return new SuccessResult(Messages.CarAdded);
        }
    }
}