namespace Application.Common.Interfaces.ImageTicket
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
    }
}

