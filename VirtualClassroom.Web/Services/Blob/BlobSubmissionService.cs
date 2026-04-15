using Azure.Storage.Blobs;

namespace VirtualClassroom.Web.Services.Blob;

public class BlobSubmissionService
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public BlobSubmissionService(IConfiguration configuration)
    {
        //_connectionString = configuration["AzureBlobSubmissions:ConnectionString"];
        _connectionString = configuration["AzureBlobSubmissions:ConnectionString"]
            ?.Trim()
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace(" ", "");
        _containerName = configuration["AzureBlobSubmissions:ContainerName"];

        Console.WriteLine("AZURE CONNECTION => " + _connectionString);
        Console.WriteLine("START[" + _connectionString + "]END");

        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new Exception("Connection string NOT loaded from config");
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        await containerClient.CreateIfNotExistsAsync();

        string fileName = folderName + "/" + Guid.NewGuid() + Path.GetExtension(file.FileName);

        var blobClient = containerClient.GetBlobClient(fileName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }

        return blobClient.Uri.ToString();
    }
}
