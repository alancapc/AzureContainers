using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using RerouteBlobs.Configurations;
using RerouteBlobs.Interfaces;
using Serilog;

namespace RerouteBlobs.Implementations
{
    public class BlobService : IBlobService
    {
        private readonly ILogger<BlobService> _logger;
        private readonly AzureConfig _azureConfig;
        private readonly IAzureStorage _azureStorage;
        public List<string> ApplicantIds = new List<string>();

        public BlobService(ILogger<BlobService> logger, IOptions<AzureConfig> azureConfig, IAzureStorage azureStorage)
        {
            _logger = logger;
            _azureConfig = azureConfig.Value;
            _azureStorage = azureStorage;
        }

        public async Task Run()
        {
            _logger.LogInformation($"Azure Storage Connection String: {_azureConfig.StorageConnectionString}");

            await MoveBlobInSameStorageAccountAsync();
        }

        public async Task MoveBlobInSameStorageAccountAsync()
        {
            BlobContinuationToken token = null;
            do
            {
                var watchAllBlobsSelection = Stopwatch.StartNew();
                BlobResultSegment resultSegment = await _azureStorage.Container.ListBlobsSegmentedAsync(token);
                watchAllBlobsSelection.Stop();
                var elapsedMs = watchAllBlobsSelection.ElapsedMilliseconds;
                _logger.LogInformation($"Gettting all blobs elapsed time (ms): {elapsedMs}");
                token = resultSegment.ContinuationToken;

                var watchCopyingBlobs = Stopwatch.StartNew();
                foreach (IListBlobItem item in resultSegment.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        
                        var applicantId = blob.Name.Substring(0, blob.Name.IndexOf("-")); // todo 
                        if (!ApplicantIds.Contains(applicantId))
                        {
                            ApplicantIds.Add(applicantId);
                            Console.WriteLine($"Creating directory: {applicantId}");
                            _logger.LogInformation($"Directory created for applicant: {applicantId}");
                        }
                        var previousDocumentLocation = _azureStorage.Container.GetBlockBlobReference($"{blob.Name}");
                        var newDocumentLocation = _azureStorage.Container.GetBlockBlobReference($"{applicantId}/{blob.Name}");
                        try
                        {
                            await newDocumentLocation.StartCopyAsync(previousDocumentLocation);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.Message);
                            throw;
                        }
                        Log.Logger.Information($"Moved {previousDocumentLocation.Name} to {newDocumentLocation.Name}");
                    }
                    else if (item.GetType() == typeof(CloudBlobDirectory))
                    {
                        CloudBlobDirectory directory = (CloudBlobDirectory)item;
                        Console.WriteLine($"Skipping existing directory: {directory.Uri}");
                    }
                    else if (item.GetType() == typeof(CloudPageBlob))
                    {
                        CloudPageBlob pageBlob = (CloudPageBlob)item;
                        Console.WriteLine($"We are not using pageBlobs: {pageBlob}");
                    }
                }
                watchCopyingBlobs.Stop();
                /* Only run this if you want to create records to be processed.
                 * await CreateNBlobsOf500KbAsync(1200);
                 */
                _logger.LogInformation($"Copying blobs elapsed time(ms): {watchCopyingBlobs.ElapsedMilliseconds}");
            } while (token != null);
        }

        public async Task CreateNBlobsOf500KbAsync(int numberOfBlobsToCreate)
        {
            for (var i = 0; i < numberOfBlobsToCreate; i++)
            {
                CloudBlockBlob blockBlob = _azureStorage.Container.GetBlockBlobReference($"{i}.jpg");
                await blockBlob.UploadFromFileAsync($"C:/Users/alan.costa/Desktop/sample.jpg");
                Log.Logger.Information(blockBlob.Name);}
        }
    }
}
