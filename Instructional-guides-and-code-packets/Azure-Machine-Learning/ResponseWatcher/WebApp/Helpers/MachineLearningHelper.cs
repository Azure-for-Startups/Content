using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.MachineLearning;
using Microsoft.Azure.MachineLearning.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApp.Helpers
{
    public static class MachineLearningHelper
    {
        public static string PredictiveSecondaryRequestUrl = CloudConfigurationManager.GetSetting("MLPredictiveSecondaryRequestUrl");
        public static string PredictiveSecondaryUpdateUrl = CloudConfigurationManager.GetSetting("MLPredictiveSecondaryUpdateUrl");
        public static string PredictiveSecondaryApiKey = CloudConfigurationManager.GetSetting("MLPredictiveSecondaryApiKey");
        public static string TrainingBatchUrl = CloudConfigurationManager.GetSetting("MLTrainingBatchUrl");
        public static string TrainingApiKey = CloudConfigurationManager.GetSetting("MLTrainingApiKey");
        public static string TrainedModelName = CloudConfigurationManager.GetSetting("MLTrainedModelName");

        public static async Task TrainCopyDurationModelAsync(Uri copyDurationBlobUri)
        {
            var runtimeClient = new RuntimeClient(TrainingBatchUrl, TrainingApiKey);
            var trainRequest = new BatchJobRequest
            {
                Outputs = new Dictionary<string, BlobReference>
                {
                    {
                        "trained",
                        BlobReference.CreateFromConnectionStringData(StorageHelper.StatisticsStorageConnectionString, "trained/trainedresults.ilearner")
                    },
                    {
                        "evaluated",
                        BlobReference.CreateFromConnectionStringData(StorageHelper.StatisticsStorageConnectionString, "evaluated/evaluatedresults.csv")
                    }
                },
                GlobalParameters = new Dictionary<string, string>
                {
                    {"URL", copyDurationBlobUri.AbsoluteUri},
                }
            };

            var batchJob = await runtimeClient.RegisterBatchJobAsync(trainRequest);
            await batchJob.StartAsync();
            var status = await batchJob.WaitForCompletionAsync();
            if (status.JobState != JobState.Finished)
            {
                throw new Exception(status.Details);
            }

            var output = status.Results["trained"];
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", PredictiveSecondaryApiKey);
                using (var updateRequest = new HttpRequestMessage(new HttpMethod("PATCH"), PredictiveSecondaryUpdateUrl))
                {

                    var resourceLocations = new
                    {
                        Resources = new[]
                        {
                            new
                            {
                                Name = TrainedModelName,
                                Location = new
                                {
                                    BaseLocation = output.Scheme + "://" + output.Authority,
                                    RelativeLocation = output.AbsolutePath,
                                    SasBlobToken = output.Query
                                }
                            }
                        }
                    };

                    string content = JsonConvert.SerializeObject(resourceLocations);
                    updateRequest.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                    var response = await client.SendAsync(updateRequest);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
            }
        }

        public static async Task<long> PredictCopyDurationMsAsync(long utcUnixTimeSeconds, long fileSize)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs =
                        new Dictionary<string, object>()
                        {
                            {
                                "input1",
                                new
                                {
                                    ColumnNames = new [] { "UtcUnixTimeSeconds", "FileSize"},
                                    Values = new [,] {{utcUnixTimeSeconds.ToString(), fileSize.ToString()}}
                                }
                            }
                        },
                    GlobalParameters = new Dictionary<string, string>()
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", PredictiveSecondaryApiKey);
                var response = await client.PostAsJsonAsync(PredictiveSecondaryRequestUrl, scoreRequest);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                var token = JObject.Parse(json).SelectToken("Results.output1.value.Values[0][0]");
                return (long) (double) token;
            }
        }
    }
}
