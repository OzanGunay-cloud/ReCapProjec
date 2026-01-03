using Microsoft.AspNetCore.Http; // IFormFile için gerekli

namespace Core.Utilities.Helpers.FileHelper
{
    public interface IFileHelper
    {
        // Dosya yükler, geriye dosyanın kaydedildiği yolu döner (string)
        string Upload(IFormFile file, string root);

        // Dosyayı siler
        void Delete(string filePath);

        // Eski dosyayı siler, yenisini yükler (Güncelleme)
        string Update(IFormFile file, string filePath, string root);
    }
}