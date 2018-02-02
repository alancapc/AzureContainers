using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace RerouteBlobs.Interfaces
{
    public interface IAzureStorage
    {
        CloudBlobClient BlobClient { get; }
        CloudBlobContainer Container { get; }
        CloudStorageAccount StorageAccount { get; }
    }
}