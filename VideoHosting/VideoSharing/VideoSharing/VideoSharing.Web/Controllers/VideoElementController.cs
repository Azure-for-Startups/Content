namespace VideoSharing.Web.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using DataAccess.Model;
    using Extensions;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Models;
    using Services;

    [Authorize]
    public class VideoElementController : Controller
    {
        private readonly VideoSharingDataContext _context = new VideoSharingDataContext();

        // GET: VideoElement
        public async Task<ActionResult> Index()
        {
            var videoElements = await _context.VideoElements
                .OwnAndSharedItems()
                .ToListAsync();

            return View(videoElements);
        }

        // GET: VideoElement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VideoElement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VideoElement videoElement)
        {
            if (ModelState.IsValidField("Title") && ModelState.IsValidField("AssetId"))
            {
                videoElement.Id = Guid.NewGuid();
                videoElement.UserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
                videoElement.StreamingUrl = await AzureMediaServicesUploader.GetStreamingUrlAsync(videoElement.AssetId);
                _context.VideoElements.Add(videoElement);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(videoElement);
        }

        // GET: VideoElement/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var videoElement = await _context.VideoElements
                .OwnItems()
                .SingleOrDefaultAsync(e => e.Id == id.Value);

            if (videoElement == null)
            {
                return HttpNotFound();
            }
            return View(videoElement);
        }

        // POST: VideoElement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(VideoElement model)
        {
            if (ModelState.IsValid)
            {
                var entity = await _context.VideoElements
                    .OwnItems()
                    .SingleOrDefaultAsync(e => e.Id == model.Id);

                if (entity != null)
                {
                    entity.Title = model.Title;
                    entity.IsPublic = model.IsPublic;

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, "Video was not found");
            }
            return View(model);
        }

        // GET: VideoElement/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id != null)
            {
                var entity = await _context.VideoElements
                    .OwnItems()
                    .SingleOrDefaultAsync(e => e.Id == id.Value);

                if (entity != null)
                {
                    return View(entity);
                }
                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: VideoElement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var entity = await _context.VideoElements
                .OwnItems()
                .SingleOrDefaultAsync(e => e.Id == id);

            if (entity != null)
            {
                _context.VideoElements.Remove(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Delete", new {id});
        }

        [HttpPost]
        public async Task<JsonResult> SetMetadata(int blocksCount, string fileName, long fileSize)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(WebConfigurationManager.AppSettings["StorageConnectionString"]);
            var storageContainerName = WebConfigurationManager.AppSettings["StorageContainerName"] + "-" + ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

            var container = cloudStorageAccount
                .CreateCloudBlobClient()
                .GetContainerReference(storageContainerName);

            await container.CreateIfNotExistsAsync();

            var videoUploadInfo = new VideoUploadingInfo
            {
                BlockCount = blocksCount,
                FileName = fileName,
                FileSize = fileSize,
                CloudBlockBlob = container.GetBlockBlobReference(fileName),
                StartTime = DateTime.Now,
                IsUploadCompleted = false,
                UploadStatusMessage = string.Empty
            };

            Session.Add("CurrentFile", videoUploadInfo);

            return Json(true);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UploadChunk(int id)
        {
            var request = Request.Files["Slice"];
            var chunk = new byte[request.ContentLength];

            await request.InputStream.ReadAsync(chunk, 0, Convert.ToInt32(request.ContentLength));
            JsonResult returnData;
            var fileSession = "CurrentFile";

            if (Session[fileSession] != null)
            {
                var model = (VideoUploadingInfo) Session[fileSession];
                returnData = await UploadCurrentChunkAsync(model, chunk, id);

                if (returnData == null)
                {
                    if (id == model.BlockCount)
                    {
                        return await CommitAllChunksAsync(model);
                    }
                }
                else
                {
                    return returnData;
                }
            }
            else
            {
                return Json(
                    new
                    {
                        error = true,
                        isLastBlock = false,
                        message = string.Format(CultureInfo.CurrentCulture, "Failed to Upload file.", "Session Timed out")
                    });
            }
            return Json(new {error = false, isLastBlock = false, message = string.Empty});
        }

        private async Task<ActionResult> CommitAllChunksAsync(VideoUploadingInfo model)
        {
            model.IsUploadCompleted = true;
            var errorInOperation = false;
            try
            {
                var blockList = Enumerable.Range(1, model.BlockCount)
                    .ToList()
                    .ConvertAll(e => Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0:D4}", e))));

                await model.CloudBlockBlob.PutBlockListAsync(blockList);
                var duration = DateTime.Now - model.StartTime;
                float fileSizeInKb = model.FileSize/1024;
                var fileSizeMessage = fileSizeInKb > 1024
                    ? string.Concat((fileSizeInKb/1024).ToString(CultureInfo.CurrentCulture), " MB")
                    : string.Concat(fileSizeInKb.ToString(CultureInfo.CurrentCulture), " KB");

                model.UploadStatusMessage = $"File uploaded successfully. {fileSizeMessage} took {duration.TotalSeconds} seconds to upload";

                await AzureMediaServicesUploader.CreateMediaAssetAsync(model);
            }
            catch (StorageException e)
            {
                model.UploadStatusMessage = $"Failed to Upload file. Exception - {e.Message}";
                errorInOperation = true;
            }
            finally
            {
                Session.Remove("CurrentFile");
            }
            return Json(new
            {
                error = errorInOperation,
                isLastBlock = model.IsUploadCompleted,
                message = model.UploadStatusMessage,
                assetId = model.AssetId
            });
        }

        private async Task<JsonResult> UploadCurrentChunkAsync(VideoUploadingInfo model, byte[] chunk, int id)
        {
            using (var chunkStream = new MemoryStream(chunk))
            {
                var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0:D4}", id)));
                try
                {
                    await model.CloudBlockBlob.PutBlockAsync(
                        blockId,
                        chunkStream,
                        null,
                        null,
                        new BlobRequestOptions {RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(10), 3)},
                        null);

                    return null;
                }
                catch (StorageException e)
                {
                    Session.Remove("CurrentFile");
                    model.IsUploadCompleted = true;
                    model.UploadStatusMessage = "Failed to Upload file. Exception - " + e.Message;

                    return Json(new {error = true, isLastBlock = false, message = model.UploadStatusMessage});
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}