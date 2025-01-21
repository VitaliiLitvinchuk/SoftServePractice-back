namespace Application.Common.Interfaces.Services;

public interface IFileService
{
    Task<string> SaveFile(byte[] bytes, string extension, string folderTo = "");
    Task<bool> DeleteFile(string filePath);
}
