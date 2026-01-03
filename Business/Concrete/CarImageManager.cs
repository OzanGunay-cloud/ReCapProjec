using Business.Abstract;
using Business.Constants;
using Core.Utilities.Helpers.FileHelper;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Task kütüphanesi
using System.Linq; // Count için gerekli
using IResult = Core.Utilities.Results.IResult;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        IFileHelper _fileHelper;
        ICarImageDal _carImageDal;

        public CarImageManager(IFileHelper fileHelper, ICarImageDal carImageDal)
        {
            _fileHelper = fileHelper;
            _carImageDal = carImageDal;
        }

        public async Task<IResult> AddAsync(IFormFile file, CarImage carImage)
        {
            // 1. İş Kuralı: Limiti kontrol et (Await ile bekliyoruz)
            IResult result = await CheckIfCarImageLimitExceededAsync(carImage.CarId);

            // Hata varsa (Success False ise) direkt hatayı dön
            if (!result.Success)
            {
                return result;
            }

            // 2. Resmi fiziksel olarak kaydet
            string imagePath = _fileHelper.Upload(file, PathConstants.ImagesPath);

            // 3. Veritabanı nesnesini hazırla
            carImage.ImagePath = imagePath;
            carImage.Date = DateTime.Now;

            // 4. Veritabanına ASENKRON kaydet
            await _carImageDal.AddAsync(carImage);

            return new SuccessResult("Resim başarıyla yüklendi");
        }

        public async Task<IResult> DeleteAsync(CarImage carImage)
        {
            // Fiziksel silme (IO işlemleri genelde hızlıdır, sync kalabilir veya wrapper yazılabilir) diskten silme yapar 
            _fileHelper.Delete(PathConstants.ImagesPath + carImage.ImagePath);

            // DB'den silme  yapar  resmin kimin olduğu adının ne olduğu nerede durduğu bilgisi 
            await _carImageDal.DeleteAsync(carImage);

            return new SuccessResult();
        }

        public async Task<IResult> UpdateAsync(IFormFile file, CarImage carImage)
        {
            carImage.ImagePath = _fileHelper.Update(file, PathConstants.ImagesPath + carImage.ImagePath, PathConstants.ImagesPath);

            await _carImageDal.UpdateAsync(carImage);

            return new SuccessResult();
        }

        public async Task<IDataResult<List<CarImage>>> GetAllAsync()
        {
            var data = await _carImageDal.GetAllAsync();
            return new SuccesDataResult<List<CarImage>>(data);
        }

        public async Task<IDataResult<List<CarImage>>> GetImagesByCarIdAsync(int carId)
        {
            var result = await _carImageDal.GetAllAsync(c => c.CarId == carId);

            // Resim yoksa Default resmi dön
            if (result.Count == 0)
            {
                return new SuccesDataResult<List<CarImage>>(GetDefaultImage(carId).Data);
            }

            return new SuccesDataResult<List<CarImage>>(result);
        }

        public async Task<IDataResult<CarImage>> GetByIdAsync(int imageId)
        {
            var data = await _carImageDal.GetAsync(c => c.Id == imageId);
            return new SuccesDataResult<CarImage>(data);
        }

        // --- İŞ KURALLARI (ASENKRON) ---

        private async Task<IResult> CheckIfCarImageLimitExceededAsync(int carId)
        {
            // GetAllAsync kullanarak veritabanındaki sayıyı asenkron çekiyoruz
            var result = await _carImageDal.GetAllAsync(c => c.CarId == carId);

            if (result.Count >= 5)
            {
                return new ErrorResult("Bir arabanın en fazla 5 resmi olabilir!");
            }
            return new SuccessResult();
        }

        private IDataResult<List<CarImage>> GetDefaultImage(int carId)
        {
            List<CarImage> carImage = new List<CarImage>();
            // Default resmin yolu. wwwroot/Uploads/Images/logo.jpg gibi bir dosyan olmalı.
            carImage.Add(new CarImage { CarId = carId, Date = DateTime.Now, ImagePath = "logo.jpg" });
            return new SuccesDataResult<List<CarImage>>(carImage);
        }
    }
}