using Core.DataAccess.Abstract;
using Core.Entities; // IDto interface'in buradaysa
using Microsoft.AspNetCore.Http; // IFormFile için bu kütüphane ŞART
using System;

namespace Entities.DTOs
{
    // Ekleme İşlemi İçin DTO
    public class CarImageAddDto : IDto
    {
        public int CarId { get; set; }
        public IFormFile File { get; set; }
    }

    // Güncelleme İşlemi İçin DTO
    public class CarImageUpdateDto : IDto
    {
        public int Id { get; set; } // Hangi resim değişecek?
        public int CarId { get; set; } // (Opsiyonel) Hangi arabaya ait olduğu değişecekse
        public IFormFile File { get; set; } // Yeni dosya
    }
}