using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using ClustEncActor.Interfaces;
using System.IO;

namespace ClustEncActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    internal class ClustEncActor : Actor, IClustEncActor
    {
        public async Task DeleteEncoderResultAsync(string storageAccount, string storageKey, string containerName, string blobName)
        {
            var storageHelper = new StorageHelper(storageAccount, storageKey, containerName);
            await storageHelper.DeleteAsync(blobName);
        }

        public async Task EncodeAsync(string sourceUrl, string storageAccount, string storageKey, string containerName, Guid taskId, DateTime createdAt, string userId, string extension, string parameters)
        {
            ActorEventSource.Current.ActorMessage(this, "EncodeAsync called.");

            try
            {
                extension = "." + extension.TrimStart('.');
                var storageHelper = new StorageHelper(storageAccount, storageKey, containerName);
                var encoder = new SampleFfmpegEncoder();
                var processingName = SamplePathHelper.SampleFileNameToString(taskId, createdAt, NamedState.Processing) +
                                     extension;
                var successName = SamplePathHelper.SampleFileNameToString(taskId, createdAt, NamedState.Success) +
                                  extension;
                var errorName = SamplePathHelper.SampleFileNameToString(taskId, createdAt, NamedState.Error) + extension;

                // Create an in-progress marker as an empty blob with processing name in the user's sub folder
                ActorEventSource.Current.ActorMessage(this, "Marking processing state...");
                await storageHelper.CreateEmptyBlobAsync(SamplePathHelper.RemotePath(processingName, userId));

                // Start encoding
                ActorEventSource.Current.ActorMessage(this, "Encoding...");
                var encoderResult =
                    await encoder.EncodeAsync(sourceUrl, SamplePathHelper.LocalPath(processingName), parameters);

                if (encoderResult == SampleEncoderResult.Success)
                {
                    ActorEventSource.Current.ActorMessage(this, "Encoding successful. Uploading...");
                    // Upload an encoded result with success name into the user's sub folder
                    await storageHelper.UploadBlobAsync(
                        SamplePathHelper.LocalPath(processingName),
                        SamplePathHelper.RemotePath(successName, userId));
                }
                else
                {
                    ActorEventSource.Current.ActorMessage(this, "Encoding failed. Marking error state...");
                    // Create an error marker as an empty blob with error name in the user's sub folder
                    await
                        storageHelper.CreateEmptyBlobAsync(
                            SamplePathHelper.RemotePath(errorName, userId));
                }

                // Delete an an in-progress marker from user's sub folder
                ActorEventSource.Current.ActorMessage(this, "Clearing processing state marker...");
                await storageHelper.DeleteAsync(SamplePathHelper.RemotePath(processingName, userId));

                // Cleanup
                File.Delete(SamplePathHelper.LocalPath(processingName));
            }
            catch (Exception exception)
            {
                ActorEventSource.Current.ActorMessage(this, $"Exception: {exception.Message}");
            }

            ActorEventSource.Current.ActorMessage(this, "Finished.");
        }

        public async Task<EncoderResult[]> GetEncoderResultsAsync(string storageAccount, string storageKey, string containerName, string userId)
        {
            var storageHelper = new StorageHelper(storageAccount, storageKey, containerName);
            var blobs = await storageHelper.BlobsAsync(userId);
            return
                blobs.Select(b =>
                {
                    var sampleFileName = SamplePathHelper.ParseSampleFileName(b.Name);
                    return
                        new EncoderResult
                        {
                            Details = sampleFileName,
                            Uri = sampleFileName.State != NamedState.Success ? null : b.Uri,
                            BlobName = sampleFileName.State == NamedState.Processing ? null : b.Name
                        };
                })
                    .OrderByDescending(t => t.Details.CreatedAt)
                    .ToArray();
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see http://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }
    }
}
