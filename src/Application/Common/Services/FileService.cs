using Application.Common.Interfaces.Services;

namespace Application.Common.Services;

public class FileService : IFileService
{
    public async Task<bool> DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                await Task.Run(() => File.Delete(filePath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        return false;
    }

    public async Task<string> SaveFile(byte[] bytes, string extension, params string[] subfolders)
    {
        var path = Path.Combine(ConfigureApplication.UploadsDir, string.Join("/", subfolders));

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string fileName = $"{Guid.NewGuid()}.{extension}";
        string fullPath = Path.Combine(path, fileName);

        await using var stream = File.Create(fullPath);
        await stream.WriteAsync(bytes);

        return fullPath;
    }
}
