using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Results
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        // Senaryo 1: Hem Data hem Mesaj veriyorsun (Nadiren kullanılır, belki hatalı data dönmek istersen)
        public ErrorDataResult(T data, string message) : base(data, false, message)
        {
        }

        // Senaryo 2: Sadece Data veriyorsun (Mesajsız hata)
        public ErrorDataResult(T data) : base(data, false)
        {
        }

        // Senaryo 3: Sadece Mesaj veriyorsun (En çok kullandığın bu olacak)
        // Data'yı "default" (yani null) gönderiyoruz.
        public ErrorDataResult(string message) : base(default, false, message)
        {
        }

        // Senaryo 4: Hiçbir şey vermiyorsun (Ne data ne mesaj)
        public ErrorDataResult() : base(default, false)
        {
        }
    }
}