namespace AzureContainers.Interfaces
{
    using System.Threading.Tasks;
    public interface IBlobService
    {
        Task Run();

        Task CountBlobsDirectoriesAndPages();
        Task SaveAllBlobNamesToFileAsync();
        Task CopyBlobInSameStorageAccountAsync();

        Task CreateNBlobsOf500KbAsync(int numbeOfBlobsToCreate);

    }
}