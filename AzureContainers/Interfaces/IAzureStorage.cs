namespace AzureContainers.Interfaces
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public interface IAzureStorage
    {
        CloudBlobClient BlobClient { get; }
        CloudBlobContainer Container { get; }
        CloudStorageAccount StorageAccount { get; }
    }
}