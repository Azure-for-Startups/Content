using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using WebApp.Models;

namespace WebApp.Helpers
{
    public static class StorageHelper
    {
        // Retrieve storage accounts from connection strings.
        public static CloudStorageAccount StorageAccountA = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageAConnectionString"));
        public static CloudStorageAccount StorageAccountB = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageBConnectionString"));
        public static CloudStorageAccount StatisticsAccount = StorageAccountA;
        public static string StatisticsStorageConnectionString = CloudConfigurationManager.GetSetting("StorageAConnectionString");
        public static long[] FileSizes = CloudConfigurationManager.GetSetting("FileSizes").Split(',').Select(long.Parse).ToArray();
        public const string SourceContainerName = "source";
        public const string TargetContainerName = "target";
        public const string StatisticsContainerName = "statistics";
        public const string CopyDurationsBlobName = "CopyDurations.csv";
        public static long BufferSize = AlignToPageSize(128 * 1024);

        public static long AlignToPageSize(long size)
        {
            return size%512 > 0 ? (size/512 + 1)*512 : size;
        }

        public static async Task<CloudPageBlob> GenerateRandomPageBlobAsync(CloudStorageAccount storageAccount, long fileSize)
        {
            if (fileSize <= 0) throw new ArgumentOutOfRangeException(nameof(fileSize));

            var blob = (await GetCloudBlobContainerAsync(storageAccount, SourceContainerName)).GetPageBlobReference(Guid.NewGuid().ToString("N"));
            long remains = AlignToPageSize(fileSize);
            var data = new byte[BufferSize > remains ? remains : BufferSize];
            var cloudBlobStream = await blob.OpenWriteAsync(remains);
            var random = new Random();
            while (remains > 0)
            {
                random.NextBytes(data);
                int count = (int) (data.Length > remains ? remains : data.Length);
                await cloudBlobStream.WriteAsync(data, 0, count);
                remains -= count;
            }

            cloudBlobStream.Commit();
            return blob;
        }

        public static async Task<CloudBlobContainer> GetCloudBlobContainerAsync(CloudStorageAccount storageAccount, string containerName)
        {
            if (storageAccount == null) throw new ArgumentNullException(nameof(storageAccount));
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("Argument is null or whitespace", nameof(containerName));

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            return container;
        }

        public static async Task<CopyDuration> MeasureCopyDurationAsync(CloudStorageAccount sourceStorageAccount, CloudStorageAccount targetStorageAccount, long fileSize)
        {
            var sourceBlob = await GenerateRandomPageBlobAsync(sourceStorageAccount, fileSize);
            var targetBlob = (await GetCloudBlobContainerAsync(targetStorageAccount, TargetContainerName)).GetPageBlobReference(sourceBlob.Name);
            var copyDuration = new CopyDuration {FileSize = sourceBlob.Properties.Length};
            var startMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            await targetBlob.StartCopyAsync(sourceBlob);
            ICloudBlob serverBlobReference;
            do
            {
                await Task.Delay(2000);
                serverBlobReference = await targetBlob.Container.GetBlobReferenceFromServerAsync(targetBlob.Name);
            } while ((serverBlobReference?.CopyState?.Status ?? CopyStatus.Pending) == CopyStatus.Pending);

            if (serverBlobReference?.CopyState?.Status != CopyStatus.Success)
            {
                throw new Exception("Copy error");
            }

            copyDuration.DurationMs = (serverBlobReference.CopyState?.CompletionTime?.ToUnixTimeMilliseconds() ?? startMs) - startMs;
            // Cleanup of source and target blobs
            await sourceBlob.DeleteAsync();
            await targetBlob.DeleteAsync();
            return copyDuration;
        }

        public static async Task AppendTextToAppendBlobAsync(string text, CloudAppendBlob appendBlob)
        {
            // Some out of scope of the sample nones is there.
            // Here should be a handling a 50000 parts per AppendBlob limit and a 4Mb part size limit.
            if (appendBlob.Properties.AppendBlobCommittedBlockCount > 49998)
            {
                //todo: Recreate a blob here
                throw new NotImplementedException("Append blob limitation");
            }

            //todo: Handle 4mb per AppendBlob page limit

            await appendBlob.AppendTextAsync(text);
        }

        public static async Task<CloudAppendBlob> GetCopyDurationsBlobReference()
        {
            var container = await GetCloudBlobContainerAsync(StatisticsAccount, StatisticsContainerName);
            var appendBlob = container.GetAppendBlobReference(CopyDurationsBlobName);
            if (!appendBlob.Exists())
            {
                await appendBlob.CreateOrReplaceAsync();
            }

            return appendBlob;
        }
    }
}