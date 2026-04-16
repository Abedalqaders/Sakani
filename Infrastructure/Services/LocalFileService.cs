using Application.Common.Interfaces.ImageTicket;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream is empty or invalid.");

            // مسار مجلد wwwroot
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // اسم فريد
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // نسخ الـ Stream من الذاكرة إلى الهارد ديسك
            using (var dbFileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(dbFileStream);
            }

            return $"/uploads/{folderName}/{uniqueFileName}";
        }
    }
}