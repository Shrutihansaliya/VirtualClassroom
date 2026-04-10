using Azure.Storage.Blobs;

namespace VirtualClassroom.Web.Services.Blob
{




    public class BlobService
    {
        private readonly string _connection;
        private readonly string _container;

        public BlobService(IConfiguration config)
        {
            _connection = config["AzureBlob:ConnectionString"];
            _container = config["AzureBlob:ContainerName"];
        }

        //public async Task<string> UploadFileAsync(IFormFile file)
        //{
        //    var containerClient = new BlobContainerClient(_connection, _container);
        //    await containerClient.CreateIfNotExistsAsync();

        //    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        //    var blobClient = containerClient.GetBlobClient(fileName);

        //    using var stream = file.OpenReadStream();
        //    await blobClient.UploadAsync(stream, true);

        //    return blobClient.Uri.ToString();
        //}

        public async Task<string> UploadFileAsync(IFormFile file, string filePath)
        {
            var containerClient = new BlobContainerClient(_connection, _container);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(filePath);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.ToString();
        }
    }
}