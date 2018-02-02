using System.Threading.Tasks;

namespace RerouteBlobs.Interfaces
{
    public interface IBlobService
    {
        Task Run();

        Task MoveBlobInSameStorageAccountAsync();

        Task CreateNBlobsOf500KbAsync(int numbeOfBlobsToCreate);
    }
}