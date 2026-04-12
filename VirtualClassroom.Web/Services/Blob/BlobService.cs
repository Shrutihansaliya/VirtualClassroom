//using Azure.Storage.Blobs;

//namespace VirtualClassroom.Web.Services.Blob
//{




//    public class BlobService
//    {
//        private readonly string _connection;
//        private readonly string _container;

//        public BlobService(IConfiguration config)
//        {
//            _connection = config["AzureBlob:ConnectionString"];
//            _container = config["AzureBlob:ContainerName"];

//        }

//        //public async Task<string> UploadFileAsync(IFormFile file)
//        //{
//        //    var containerClient = new BlobContainerClient(_connection, _container);
//        //    await containerClient.CreateIfNotExistsAsync();

//        //    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
//        //    var blobClient = containerClient.GetBlobClient(fileName);

//        //    using var stream = file.OpenReadStream();
//        //    await blobClient.UploadAsync(stream, true);

//        //    return blobClient.Uri.ToString();
//        //}

//        public async Task<string> UploadFileAsync(IFormFile file, string filePath)
//        {
//            var containerClient = new BlobContainerClient(_connection, _container);
//            await containerClient.CreateIfNotExistsAsync();

//            var blobClient = containerClient.GetBlobClient(filePath);

//            using var stream = file.OpenReadStream();
//            await blobClient.UploadAsync(stream, true);

//            return blobClient.Uri.ToString();
//        }
//    }
//}

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

            if (string.IsNullOrEmpty(_connection))
                throw new Exception("Azure Blob ConnectionString is missing!");

            if (string.IsNullOrEmpty(_container))
                throw new Exception("Azure Blob ContainerName is missing!");
        }
        public async Task<string> UploadFileAsync(IFormFile file, string filePath)
        {
            Console.WriteLine("=== BLOB UPLOAD START ===");
            Console.WriteLine("Connection exists: " + (!string.IsNullOrEmpty(_connection)));
            Console.WriteLine("Container: " + _container);
            Console.WriteLine("FilePath: " + filePath);





        {
            if (string.IsNullOrEmpty(fileUrl))
                return;

            var containerClient = new BlobContainerClient(_connection, _container);



            return blobClient.Uri.ToString();
        }
        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            var blobContainer = new BlobContainerClient(_connection, _container);

            var uri = new Uri(imageUrl);
            var blobName = uri.AbsolutePath.Split('/').Skip(2).Aggregate((a, b) => $"{a}/{b}");

            var blobClient = blobContainer.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
        //public async Task<string> UploadFileAsync(IFormFile file, string filePath)
        //{
        //    var containerClient = new BlobContainerClient(_connection, _container);

        //    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        //    var blobClient = containerClient.GetBlobClient(filePath);

        //    var options = new BlobUploadOptions
        //    {
        //        HttpHeaders = new BlobHttpHeaders
        //        {
        //            ContentType = file.ContentType
        //        }
        //    };

        //    using var stream = file.OpenReadStream();
        //    await blobClient.UploadAsync(stream, options);

        //    return blobClient.Uri.ToString();
        //}
    }
}