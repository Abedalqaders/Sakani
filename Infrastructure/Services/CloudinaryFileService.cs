using Application.Common.Interfaces.ImageTicket;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Services
{
    public class CloudinaryFileService : IFileService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryFileService(IConfiguration config)
        {
        
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }
        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
                Folder = folderName,
                DisplayName = fileName
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary Error: {uploadResult.Error.Message}");
            }

         
            return uploadResult.SecureUrl.ToString();

        }
    }
}
