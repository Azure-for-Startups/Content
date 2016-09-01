namespace VideoSharing.Web.Models
{
    using System;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class VideoUploadingInfo
    {
        public int BlockCount { get; set; }

        public string FileName { get; set; }

        public long FileSize { get; set; }

        public CloudBlockBlob CloudBlockBlob { get; set; }

        public DateTime StartTime { get; set; }

        public bool IsUploadCompleted { get; set; }

        public string UploadStatusMessage { get; set; }

        public string AssetId { get; set; }
    }
}