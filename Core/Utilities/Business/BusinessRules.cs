using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities.Results;

namespace Core.Utilities.Business
{
    public static class BusinessRules
    {
        // Metodu "async Task<IResult?>" yaptık ve "params Task<IResult>[]" kabul ediyoruz.
        public static async Task<IResult?> RunAsync(params Task<IResult>[] logics)
        {
            foreach (var logic in logics)
            {
                // Her bir kuralın bitmesini bekliyoruz (await)
                var result = await logic;

                if (!result.Success)
                {
                    return result;
                }
            }
            return null;
        }
    }

}