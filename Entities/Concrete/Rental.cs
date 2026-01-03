using Core.Entities;
using Core.Entities.Abstract;
using System;

namespace Entities.Concrete
{
    public class Rental : IEntity
    {
        public int RentalId { get; set; }
        public int CarId { get; set; }
        public int CustomerId { get; set; }

        // Kiralamanın BAŞLAYACAĞI tarih (Rezervasyon başlangıcı)
        public DateTime RentDate { get; set; }

        // EKLENMESİ GEREKEN ALAN: Kiralamanın BİTECEĞİ tahmini tarih
        // Müşteri "3 günlüğüne kiralayacağım" dediğinde burası dolacak.
        public DateTime RentEndDate { get; set; }

        // Araç gerçekten firmaya teslim edildiğinde dolacak tarih.
        // Araç teslim edilmediği veya rezervasyon aşamasında olduğu sürece NULL kalır.
        public DateTime? ReturnDate { get; set; }
    }
}