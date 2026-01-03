using Business.Abstract;
using Entities.Concrete;
using Entities.DTOs; // Yeni oluşturduğumuz DTO'ları kullanmak için ekle
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarImagesController : ControllerBase
    {
        ICarImageService _carImageService;

        public CarImagesController(ICarImageService carImageService)
        {
            _carImageService = carImageService;
        }

        // --- EKLEME İŞLEMİ (ARTIK DTO KULLANIYOR) ---
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromForm] CarImageAddDto carImageAddDto)
        {
            // DTO'dan gelen verileri asıl nesnemize (Entity) aktarıyoruz
            var carImage = new CarImage
            {
                CarId = carImageAddDto.CarId
            };

            // Manager'a resmi ve nesneyi gönderiyoruz
            var result = await _carImageService.AddAsync(carImageAddDto.File, carImage);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // --- GÜNCELLEME İŞLEMİ (ARTIK DTO KULLANIYOR) ---
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromForm] CarImageUpdateDto carImageUpdateDto)
        {
            var carImage = new CarImage
            {
                Id = carImageUpdateDto.Id,
                CarId = carImageUpdateDto.CarId
            };

            var result = await _carImageService.UpdateAsync(carImageUpdateDto.File, carImage);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // --- SİLME ---
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(CarImage carImage)
        {
            var result = await _carImageService.DeleteAsync(carImage);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // --- LİSTELEME ---
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _carImageService.GetAllAsync();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getimagesbycarid")]
        public async Task<IActionResult> GetImagesByCarId(int carId)
        {
            var result = await _carImageService.GetImagesByCarIdAsync(carId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}