
namespace Application.Common.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
    }
}

