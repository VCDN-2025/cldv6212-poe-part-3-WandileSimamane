using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace OrderSystem.Services
{
    public class FileService
    {
        private readonly string _connectionString;

        // Initializes the File Service client.
        public FileService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureFileStorage");
        }

        /// <summary>
        /// Uploads a file to a specified file share.
        /// </summary>
        public async Task UploadFileAsync(IFormFile file, string shareName)
        {
            var shareClient = new ShareClient(_connectionString, shareName);
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(file.FileName);
            using var stream = file.OpenReadStream();
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadAsync(stream);
        }
    }
}