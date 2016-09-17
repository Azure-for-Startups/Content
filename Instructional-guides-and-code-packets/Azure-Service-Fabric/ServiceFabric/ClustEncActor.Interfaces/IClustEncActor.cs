using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ClustEncActor.Interfaces
{
    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IClustEncActor : IActor
    {
        /// <summary>
        ///     Executes an encoding process
        /// </summary>
        /// <param name="sourceUrl">URI from the source file will be downloaded</param>
        /// <param name="storageAccount">Storage account name to store a results</param>
        /// <param name="storageKey">Storage account access key</param>
        /// <param name="containerName">Blob container name</param>
        /// <param name="taskId">Unique id of encoding task</param>
        /// <param name="createdAt">Task creation datetime</param>
        /// <param name="userId">User identifier</param>
        /// <param name="extension">Encoded file extension</param>
        /// <param name="parameters">Encoder command line parameters</param>
        /// <returns></returns>
        Task EncodeAsync(string sourceUrl, string storageAccount, string storageKey, string containerName, Guid taskId,
            DateTime createdAt, string userId, string extension, string parameters);

        /// <summary>
        ///     Returns encoding results information
        /// </summary>
        /// <param name="storageAccount">Storage account name to store a results</param>
        /// <param name="storageKey">Storage account access key</param>
        /// <param name="containerName">Blob container name</param>
        /// <param name="userId">User identifier</param>
        /// <returns></returns>
        Task<EncoderResult[]> GetEncoderResultsAsync(string storageAccount, string storageKey, string containerName,
            string userId);

        /// <summary>
        ///     Deletes encoding result by it's name
        /// </summary>
        /// <param name="storageAccount">Storage account name to store a results</param>
        /// <param name="storageKey">Storage account access key</param>
        /// <param name="containerName">Blob container name</param>
        /// <param name="blobName">Name of result file</param>
        /// <returns></returns>
        Task DeleteEncoderResultAsync(string storageAccount, string storageKey, string containerName, string blobName);
    }

    public class EncoderResult
    {
        public string BlobName;
        public SampleFileName Details;
        public Uri Uri;
    }

    public enum NamedState
    {
        Unknown,
        Processing,
        Success,
        Error
    }

    public class SampleFileName
    {
        public DateTime CreatedAt;

        [JsonConverter(typeof(StringEnumConverter))]
        public NamedState State = NamedState.Unknown;

        public Guid TaskId = Guid.NewGuid();
    }
}