using APIs.ErrorHandling;
using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;

namespace APIs.Helpers
{
    public static class SaveOrDeleteEventImage
    {
        public static async Task<string> SaveImageFromUrlAsync(string ImageUrl, string FolderName = "Images")
        {
            using var HttpClient = new HttpClient();

            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var Response = await HttpClient.GetAsync(ImageUrl);
            var Bytes = await Response.Content.ReadAsByteArrayAsync();
         
            var ContentType = Response.Content.Headers.ContentType?.MediaType;
            string Extension = ContentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                _ => ".jpg"
            };

            var FileName = $"{Guid.NewGuid()}{Extension}";
            var FolderPath = Path.Combine(wwwrootPath, FolderName);

            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            var FullPath = Path.Combine(FolderPath, FileName);
            await File.WriteAllBytesAsync(FullPath, Bytes);

            return $"/{FolderName}/{FileName}";
        }
        public static Task<bool> DeleteImageAsync(string RelativePath)
        {
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var CleanedPath = RelativePath.TrimStart('/' , '\\');
            var FullPath = Path.Combine(wwwrootPath, CleanedPath);

            if (!File.Exists(FullPath))
                return Task.FromResult(false);
            try
            {
                File.Delete(FullPath);
                return Task.FromResult(true);
            }
            catch { return Task.FromResult(false); }
        }

    }
}


