using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _colorService;

        // IColorService bağımlılığını enjekte ediyoruz
        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _colorService.GetAllAsync();
            if (result.Success)
            {
                return Ok(result); // 200 OK ve veriler
            }
            return BadRequest(result); // 400 Hata ve mesaj
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(Color color)
        {
            var result = await _colorService.AddAsync(color);
            if (result.Success)
            {
                return Ok(result); // 200 OK ve başarı mesajı
            }
            return BadRequest(result); // 400 Hata ve mesaj
        }
    }
}