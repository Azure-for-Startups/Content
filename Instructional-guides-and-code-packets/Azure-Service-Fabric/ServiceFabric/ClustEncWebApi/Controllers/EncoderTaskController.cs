using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClustEncActor.Interfaces;
using System.Web.Http;
using System.Fabric;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System.Fabric.Description;
using System.Net;

namespace ClustEncWebApi.Controllers
{
    public class EncoderTaskController : ApiController
    {
        private static readonly Uri ServiceUri = new Uri("fabric:/ClustEncApplication/ClustEncActorService");
        private readonly IClustEncActor _proxy = ActorProxy.Create<IClustEncActor>(ActorId.CreateRandom(), ServiceUri);
        private ConfigurationSection BlobStorageConfig => FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config").Settings.Sections["BlobStorageConfig"];
        private string StorageAccountName => BlobStorageConfig.Parameters["StorageAccountName"].Value;
        private string StorageAccountKey => BlobStorageConfig.Parameters["StorageAccountKey"].Value;
        private string StorageContainerName => BlobStorageConfig.Parameters["StorageContainerName"].Value;

        // GET api/encodertask
        public async Task<EncoderResult[]> Get(string userId)
        {
            return await _proxy.GetEncoderResultsAsync(StorageAccountName, StorageAccountKey, StorageContainerName, userId);
        }

        // POST api/encodertask
        public object Post([FromBody] EncoderTask task)
        {
            if (string.IsNullOrWhiteSpace(task?.UserId) || !Uri.IsWellFormedUriString(task?.SourceUrl, UriKind.Absolute))
                { throw new HttpResponseException(HttpStatusCode.BadRequest); }
            var taskId = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(task.Parameters))
            {
                // Default ffmpeg parameters for this sample. Skip 10 seconds, take 10 seconds, resize to 640x360 and encode as x264/mp3.
                task.Parameters = "-ss 30 -t 10 -b:v 1500k -vcodec libx264 -acodec mp3 -b:a 128k -s 640x360";
            }
            if (string.IsNullOrWhiteSpace(task.Extension))
            {
                // Default extension
                task.Extension = "mp4";
            }
            _proxy.EncodeAsync(task.SourceUrl, StorageAccountName, StorageAccountKey, StorageContainerName, taskId,
                DateTime.Now, task.UserId, task.Extension, task.Parameters);
            return new { TaskId = taskId.ToString("N"), Details = task };
        }

        // DELETE api/encodertask?name=123.mp4
        public Task Delete(string name)
        {
            return _proxy.DeleteEncoderResultAsync(StorageAccountName, StorageAccountKey, StorageContainerName, name);
        }
    }

    public class EncoderTask
    {
        public string Parameters;
        public string Extension;
        public string SourceUrl;
        public string UserId;
    }
}
