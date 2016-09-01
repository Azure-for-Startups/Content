namespace VideoSharing.Web.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Configuration;
    using Microsoft.WindowsAzure.MediaServices.Client;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Models;

    public static class AzureMediaServicesUploader
    {
        public static async Task<string> GetStreamingUrlAsync(string assetId)
        {
            var mediaContext = new CloudMediaContext(WebConfigurationManager.AppSettings["MediaAccountName"], WebConfigurationManager.AppSettings["MediaAccountKey"]);

            var daysForWhichStreamingUrlIsActive = 365;
            //the FirstOrDefault method is not supported!
            var streamingAsset = mediaContext.Assets.Where(a => a.Id == assetId).FirstOrDefault();

            var accessPolicy = await mediaContext.AccessPolicies.CreateAsync(
                streamingAsset.Name,
                TimeSpan.FromDays(daysForWhichStreamingUrlIsActive),
                AccessPermissions.Read | AccessPermissions.List);

            var streamingUrl = string.Empty;
            var assetFiles = streamingAsset.AssetFiles.ToList();
            var streamingAssetFile = assetFiles.FirstOrDefault(f => f.Name.ToLower().EndsWith("m3u8-aapl.ism"));
            if (streamingAssetFile != null)
            {
                var locator = await mediaContext.Locators.CreateLocatorAsync(LocatorType.OnDemandOrigin, streamingAsset, accessPolicy);
                var hlsUri = new Uri(locator.Path + streamingAssetFile.Name + "/manifest(format=m3u8-aapl)");
                streamingUrl = hlsUri.ToString();
            }

            streamingAssetFile = assetFiles.FirstOrDefault(f => f.Name.ToLower().EndsWith(".ism"));
            if (string.IsNullOrEmpty(streamingUrl) && streamingAssetFile != null)
            {
                var locator = await mediaContext.Locators.CreateLocatorAsync(LocatorType.OnDemandOrigin, streamingAsset, accessPolicy);
                var smoothUri = new Uri(locator.Path + streamingAssetFile.Name + "/manifest");
                streamingUrl = smoothUri.ToString();
            }

            streamingAssetFile = assetFiles.FirstOrDefault(f => f.Name.ToLower().EndsWith(".mp4"));
            if (string.IsNullOrEmpty(streamingUrl) && streamingAssetFile != null)
            {
                var locator = await mediaContext.Locators.CreateLocatorAsync(LocatorType.Sas, streamingAsset, accessPolicy);
                var mp4Uri = new UriBuilder(locator.Path);
                mp4Uri.Path += "/" + streamingAssetFile.Name;
                streamingUrl = mp4Uri.ToString();
            }

            return streamingUrl;
        }

        public static async Task CreateMediaAssetAsync(VideoUploadingInfo model)
        {
            var mediaAccountName = WebConfigurationManager.AppSettings["MediaAccountName"];
            var mediaAccountKey = WebConfigurationManager.AppSettings["MediaAccountKey"];
            var storageAccountName = WebConfigurationManager.AppSettings["StorageAccountName"];
            var storageAccountKey = WebConfigurationManager.AppSettings["StorageAccountKey"];
            var storageContainerName = WebConfigurationManager.AppSettings["StorageContainerName"] + "-" + ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

            var context = new CloudMediaContext(mediaAccountName, mediaAccountKey);
            var storageAccount = new CloudStorageAccount(new StorageCredentials(storageAccountName, storageAccountKey), true);
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var mediaBlobContainer = cloudBlobClient.GetContainerReference(storageContainerName);

            await mediaBlobContainer.CreateIfNotExistsAsync();

            // Create a new asset.
            var asset = await context.Assets.CreateAsync("asset_" + Guid.NewGuid(), AssetCreationOptions.None, new CancellationToken());
            var writePolicy = await context.AccessPolicies.CreateAsync("writePolicy", TimeSpan.FromMinutes(120), AccessPermissions.Write);
            var destinationLocator = await context.Locators.CreateLocatorAsync(LocatorType.Sas, asset, writePolicy);

            // Get the asset container URI and copy blobs from mediaContainer to assetContainer.
            var uploadUri = new Uri(destinationLocator.Path);
            var assetContainerName = uploadUri.Segments[1];
            var assetContainer = cloudBlobClient.GetContainerReference(assetContainerName);
            var fileName = HttpUtility.UrlDecode(Path.GetFileName(model.CloudBlockBlob.Uri.AbsoluteUri));

            var sourceCloudBlob = mediaBlobContainer.GetBlockBlobReference(fileName);
            await sourceCloudBlob.FetchAttributesAsync();

            if (sourceCloudBlob.Properties.Length > 0)
            {
                var destinationBlob = assetContainer.GetBlockBlobReference(fileName);

                await destinationBlob.DeleteIfExistsAsync();
                await destinationBlob.StartCopyAsync(sourceCloudBlob);

                while (true)
                {
                    // The StartCopyFromBlob is an async operation, 
                    // so we want to check if the copy operation is completed before proceeding. 
                    // To do that, we call FetchAttributes on the blob and check the CopyStatus. 
                    await destinationBlob.FetchAttributesAsync();
                    if (destinationBlob.CopyState.Status != CopyStatus.Pending)
                    {
                        break;
                    }
                    //It's still not completed. So wait for some time.
                    await Task.Delay(1000);
                }

                if (sourceCloudBlob.Properties.Length != destinationBlob.Properties.Length)
                {
                    model.UploadStatusMessage += "Failed to copy as Media Asset!";
                }
            }

            await destinationLocator.DeleteAsync();
            await writePolicy.DeleteAsync();

            await asset.GenerateFromStorageAsync();

            var ismAssetFiles = asset.AssetFiles
                .ToList()
                .Where(f => f.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (ismAssetFiles.Length != 1)
            {
                throw new ArgumentException("The asset should have only one, .ism file");
            }

            ismAssetFiles.First().IsPrimary = true;
            await ismAssetFiles.First().UpdateAsync();

            model.UploadStatusMessage += " Created Media Asset '" + asset.Name + "' successfully.";
            model.AssetId = asset.Id;
        }
    }
}