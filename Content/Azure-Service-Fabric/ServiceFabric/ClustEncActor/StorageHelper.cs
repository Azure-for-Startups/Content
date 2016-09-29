using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ClustEncActor
{
    public class StorageHelper
    {
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly string _containerName;

        public StorageHelper(string storageAccountName, string storageKey, string containerName)
        {
            _containerName = containerName;
            _cloudStorageAccount = new CloudStorageAccount(new StorageCredentials(storageAccountName, storageKey), true);
        }

        public Task<CloudBlobContainer> GetCloudBlobContainerAsync()
        {
            return GetCloudBlobContainerAsync(_cloudStorageAccount, _containerName);
        }

        public static async Task<CloudBlobContainer> GetCloudBlobContainerAsync(CloudStorageAccount storageAccount,
            string containerName)
        {
            if (storageAccount == null) throw new ArgumentNullException(nameof(storageAccount));
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("Argument is null or whitespace", nameof(containerName));

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            container.SetPermissions(new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});
            return container;
        }

        public async Task<CloudBlockBlob> GetBlobReferenceAsync(string targetPath)
        {
            return (await GetCloudBlobContainerAsync()).GetBlockBlobReference(targetPath);
        }

        public async Task UploadBlobAsync(string sourcePath, string targetPath)
        {
            var blockBlob = await GetBlobReferenceAsync(targetPath);
            using (var fileStream = File.OpenRead(sourcePath))
            {
                await blockBlob.DeleteIfExistsAsync();
                await blockBlob.UploadFromStreamAsync(fileStream);
            }
        }

        public async Task CreateEmptyBlobAsync(string targetPath)
        {
            var blockBlob = await GetBlobReferenceAsync(targetPath);
            await blockBlob.DeleteIfExistsAsync();
            await blockBlob.UploadTextAsync("");
        }

        public async Task DeleteAsync(string targetPath)
        {
            await (await GetBlobReferenceAsync(targetPath)).DeleteIfExistsAsync();
        }

        public async Task<CloudBlockBlob[]> BlobsAsync(string directory)
        {
            var container = await GetCloudBlobContainerAsync();
            var directoryReference = container.GetDirectoryReference(directory);
            return directoryReference.ListBlobs().OfType<CloudBlockBlob>().ToArray();
        }
    }
}