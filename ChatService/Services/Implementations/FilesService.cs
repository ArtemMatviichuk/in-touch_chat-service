using ChatService.Common.Dtos.General;
using ChatService.Services.Interfaces;
using Microsoft.AspNetCore.StaticFiles;

namespace ChatService.Services.Implementations
{
    public class FilesService : IFilesService
    {
        public async Task<string> SaveFile(string path, IFormFile file, string? fileName = null)
        {
            var fileNameWithTimestamp =
                $"{fileName ?? Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMddHHmmssffff}{Path.GetExtension(file.FileName)}";

            using (var fileStream = new FileStream(Path.Combine(path, fileNameWithTimestamp), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileNameWithTimestamp;
        }

        public async Task<FileDto?> GetFile(string path, string fileName)
        {
            var filePath = Directory.GetFiles(path, fileName, SearchOption.AllDirectories).FirstOrDefault();
            if (filePath == null)
            {
                return null;
            }

            var bytes = await File.ReadAllBytesAsync(filePath);

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return new FileDto
            {
                FileName = fileName,
                Bytes = bytes,
                ContentType = contentType
            };
        }
    }
}
