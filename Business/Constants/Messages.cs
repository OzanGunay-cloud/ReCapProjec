using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public static  class Messages
    {
        public static string CarAdded = "Araba başarıyla eklendi.";
      
        public static string CarNameAlreadyExists = "Bu isimde zaten bir araba var.";
        // Diğer mesajlarını buraya ekleyebilirsin...
        public static string CarDescriptionAlreadyExists = "Bu açıklamada zaten bir araba var.";
        public static string CarAlreadyRented = "Bu araba şu an kiralık durumdadır, tekrar kiralanamaz.";

        public static string RentalInvalid = "Bu araba henüz teslim edilmediği için tekrar kiralanamaz.";
    }
}

