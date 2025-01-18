namespace Application.Common.Interfaces.Services;

public interface IFileService
{
    Task<string> SaveFile(byte[] bytes, string extension, params string[] subfolders);
    Task<bool> DeleteFile(string filePath);
}
