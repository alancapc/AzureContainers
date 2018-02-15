namespace AzureContainers
{
    using Configurations;
    using Interfaces;
    using Microsoft.Extensions.Options;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class AzureStorage : IAzureStorage
    {
        private readonly AzureConfig _azureConfig;

        public CloudStorageAccount StorageAccount { get; set; }
        public CloudBlobClient BlobClient { get; set; }
        public CloudBlobContainer Container { get; set; }


        public AzureStorage(IOptions<AzureConfig> azureConfig)
        {
            _azureConfig = azureConfig.Value;
            StorageAccount = CloudStorageAccount.Parse(_azureConfig.StorageConnectionString);
            //instantiate the client
            BlobClient = StorageAccount.CreateCloudBlobClient();
            //set the container
            Container = BlobClient.GetContainerReference(_azureConfig.ImagesContainerName);

        }
    }
}