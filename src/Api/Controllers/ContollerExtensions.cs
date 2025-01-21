using Application.Common.Interfaces.Services;

namespace Api.Controllers;

public static class ContollerExtensions
{
    public static async Task<string> Save(this IFormFile file, string[] subFolders, IFileService fileService, CancellationToken cancellation)
    {
        using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream, cancellation);
        var fileBytes = memoryStream.ToArray();

        return await fileService.SaveFile(fileBytes, Path.GetExtension(file.FileName), Path.Combine(subFolders));
    }
}
