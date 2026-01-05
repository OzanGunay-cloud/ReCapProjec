using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rentalService.GetAllAsync();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _rentalService.GetByIdAsync(id);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(Rental rental)
        {
            var result = await _rentalService.AddAsync(rental);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        // ARABAYI TESLİM ALMAK İÇİN BU ENDPOINT'İ KULLANACAKSIN
        [HttpPost("update")]
        public async Task<IActionResult> Update(Rental rental)
        {
            // Bu metot tetiklendiğinde ve returnDate dolu olduğunda araba True olur.
            var result = await _rentalService.UpdateAsync(rental);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Rental rental)
        {
            var result = await _rentalService.DeleteAsync(rental);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("rentcarasync")]
        public async Task<IActionResult> RentCarAsync(Rental rental)
        {
            // Arabayı kiralarken IsAvailable = false yapan ana metot
            var result = await _rentalService.RentCarAsync(rental);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}