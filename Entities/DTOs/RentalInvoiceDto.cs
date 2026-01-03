using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public  class RentalInvoiceDto
    {
        public string CarName { get; set; }
        public string BrandName {  get; set; }




        // Müşteri Bilgileri (Customer/User tablolarından gelecek)
        public string CustomerFullName { get; set; }

        // Tarih Bilgileri
        public DateTime RentDate { get; set; }
        public DateTime RentEndDate { get; set; }

        // Fiyatlandırma
        public decimal DailyPrice { get; set; }
        public int TotalDays { get; set; }
        public decimal TotalPrice { get; set; }
    }
}



