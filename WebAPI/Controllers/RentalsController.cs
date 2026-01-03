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

        [HttpPost("rentcarasync")]
        public async Task<IActionResult> RentCarAsync(Rental rental)
        {
            // Business katmanındaki RentCarAsync metodunu çağırıyoruz
            var result = await _rentalService.RentCarAsync(rental);

            if (result.Success)
            {
                // İşlem başarılıysa 200 OK ve fatura DTO'sunu dön
                return Ok(result);
            }

            // İşlem başarısızsa (Müsait değil veya Ödeme reddedildi) 400 BadRequest dön
            return BadRequest(result);
        }
    }
}