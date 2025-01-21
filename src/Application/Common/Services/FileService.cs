using Application.Common.Interfaces.Services;

namespace Application.Common.Services;

public class FileService : IFileService
{
    public async Task<bool> DeleteFile(string filePath)
    {
        var path = Path.Combine(ConfigureApplication.UploadsDir, filePath);
        if (File.Exists(path))
        {
            try
            {
                await Task.Run(() => File.Delete(path));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        return false;
    }

    public async Task<string> SaveFile(byte[] bytes, string extension, string folderTo = "")
    {
        var path = Path.Combine(ConfigureApplication.UploadsDir, folderTo);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string fileName = $"{Guid.NewGuid()}{extension}";
        string fullPath = Path.Combine(path, fileName);

        await using var stream = File.Create(fullPath);
        await stream.WriteAsync(bytes);

        return Path.Combine(folderTo, fileName);
    }
}
