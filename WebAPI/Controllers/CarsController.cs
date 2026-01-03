using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        // Business katmanına bağımlıyız (Manager'ı değil, Interface'i çağırıyoruz)
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            // Asenkron servisi çağırıyoruz
            var result = await _carService.GetAllAsync();

            if (result.Success)
            {
                return Ok(result); // 200 OK + Data
            }
            return BadRequest(result); // 400 Bad Request + Hata Mesajı
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(Car car)
        {
            var result = await _carService.AddAsync(car);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getavailablecardetails")]
        public async Task<IActionResult> GetAvailableCarDetails(DateTime rentDate, DateTime rentEndDate)
        {
            // HATA BURADAYDI: Parantez içinde tarihleri göndermeli ve 'await' kullanmalısın
            var result = await _carService.GetAvailableCarDetailsAsync(rentDate, rentEndDate);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }





    }
}