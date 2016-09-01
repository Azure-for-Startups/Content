using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers.Api
{
    [RoutePrefix("api/MachineLearning")]
    public class MachineLearningController : ApiController
    {
        private const string Separator = ",";

        [HttpGet]
        [Route("Collect")]
        public async Task<IEnumerable<CopyDuration>> Collect()
        {
            string row = "";
            var copyDurations = new List<CopyDuration>();
            foreach (var fileSize in StorageHelper.FileSizes)
            {
                var copyDuration = await StorageHelper.MeasureCopyDurationAsync(StorageHelper.StorageAccountA, StorageHelper.StorageAccountB, fileSize);
                row += string.Join(Separator,
                    copyDuration.UtcUnixTimeSeconds.ToString(),
                    copyDuration.FileSize.ToString(),
                    copyDuration.DurationMs.ToString()) + Environment.NewLine;
                copyDurations.Add(copyDuration);
            }

            var appendBlob = await StorageHelper.GetCopyDurationsBlobReference();
            if ((appendBlob.Properties.AppendBlobCommittedBlockCount ?? 0) == 0)
            {
                //Put a csv header
                row = string.Join(Separator,
                    nameof(CopyDuration.UtcUnixTimeSeconds),
                    nameof(CopyDuration.FileSize),
                    nameof(CopyDuration.DurationMs)) + Environment.NewLine + row;
            }

            await StorageHelper.AppendTextToAppendBlobAsync(row, appendBlob);
            return copyDurations;
        }

        [HttpGet]
        [Route("Retrain")]
        public async Task<IHttpActionResult> Retrain()
        {
            await MachineLearningHelper.TrainCopyDurationModelAsync((await StorageHelper.GetCopyDurationsBlobReference()).Uri);
            return Ok("Done");
        }

        [HttpGet]
        [Route("Compare")]
        public async Task<object> Compare(long fileSize, double alarmLevel)
        {
            var realCopyDuration = await StorageHelper.MeasureCopyDurationAsync(StorageHelper.StorageAccountA, StorageHelper.StorageAccountB, fileSize);
            var predictedCopyDurationMs = await MachineLearningHelper.PredictCopyDurationMsAsync(realCopyDuration.UtcUnixTimeSeconds, realCopyDuration.FileSize);
            bool exceed = predictedCopyDurationMs*alarmLevel < realCopyDuration.DurationMs;
            bool? callbackSucceed = null;
            if (exceed)
            {
                callbackSucceed = false;
                try
                {
                    var httpClient = new HttpClient();
                    var httpResponseMessage = await httpClient.GetAsync(CloudConfigurationManager.GetSetting("CallbackUrl"));
                    httpResponseMessage.EnsureSuccessStatusCode();
                    callbackSucceed = true;
                }
                catch
                {
                    //Handle error details if necessary here
                }
            }

            return new {realCopyDuration, predictedCopyDurationMs, alarmLevel, exceed, callbackSucceed};
        }

        [HttpGet]
        [Route("Download")]
        public async Task<IHttpActionResult> Download()
        {
            return Redirect((await StorageHelper.GetCopyDurationsBlobReference()).Uri);
        }
    }
}
