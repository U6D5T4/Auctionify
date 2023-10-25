using Microsoft.AspNetCore.StaticFiles;

namespace Auctionify.Application.Common.Extensions
{
    public static class FileExtensions
    {
        private static readonly FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();

        public static string GetContentType(this string fileName)
        {
            if (!provider.TryGetContentType(fileName, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
