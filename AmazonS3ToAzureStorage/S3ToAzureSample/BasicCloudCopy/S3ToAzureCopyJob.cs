// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3ToAzureCopyJob.cs" company="Microsoft">
//   Copyright (c) 2016 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicCloudCopy
{
    #region Usings

    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon.S3.Model;
    using Microsoft.WindowsAzure.Storage.Blob;

    #endregion

    /// <summary>
    ///     Copy job for copying object from Amazon S3 bucket to Azure Storage container blob.
    /// </summary>
    public class S3ToAzureCopyJob: ICopyJob
    {
        #region Private Fields

        private readonly S3ToAzureCopyJobFactory _factory;
        private readonly S3Object _s3Object;
        private int _retryCount;

        #endregion

        #region Constructors

        internal S3ToAzureCopyJob(S3ToAzureCopyJobFactory factory, S3Object s3Object)
        {
            _factory = factory;
            _s3Object = s3Object;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Executes a copying operation
        /// </summary>
        /// <returns></returns>
        public async Task CopyAsync()
        {
            var blobName = GetBlobName(_factory.AzureStorageTargetOptions.TargetPath, _s3Object.Key);
            CloudBlockBlob blob = null;
            try
            {
                var cloudBlobContainer = _factory.CloudBlobClient.GetContainerReference(_factory.AzureStorageTargetOptions.TargetContainer);
                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess = (BlobContainerPublicAccessType) _factory.AzureStorageTargetOptions.AccessLevel
                        });
                }

                var response = await _factory.AmazonS3Client.GetObjectAsync(_factory.AmazonS3SourceOptions.SourceBucket, _s3Object.Key);
                var contentLength = response.Headers.ContentLength;
                using (var sourceStream = new ForwardOnlyReadStream(response.ResponseStream, contentLength))
                {
                    sourceStream.ProgressChanged += (sender, args) =>
                                                    {
                                                        OnProgressChanged(
                                                            new JobProgressEventArgs
                                                            {
                                                                Position = args.Position,
                                                                TotalLength = args.TotalLength,
                                                                Error = args.Error,
                                                                TargetPath = blobName
                                                            });
                                                    };

                    var deleteBlob = cloudBlobContainer.GetBlobReference(blobName);
                    //Blob type independent deleting of existing blob
                    await deleteBlob.DeleteIfExistsAsync();

                    blob = cloudBlobContainer.GetBlockBlobReference(blobName);
                    blob.Properties.ContentType = response.Headers.ContentType;
                    blob.Properties.ContentEncoding = response.Headers.ContentEncoding;
                    blob.Properties.ContentDisposition = response.Headers.ContentDisposition;
                    await blob.UploadFromStreamAsync(sourceStream);
                }
            }
            catch (Exception exception)
            {
                blob?.DeleteIfExists();
                OnProgressChanged(
                    new JobProgressEventArgs
                    {
                        Error = exception.Message,
                        TargetPath = blobName
                    });
            }
        }

        /// <summary>
        ///     Should be used to determine if the retry operation is possible in case previous copying job task had failed
        /// </summary>
        /// <returns></returns>
        public bool RequestRetry()
        {
            ++_retryCount;
            return _retryCount < _factory.CommonCopyOptions.RetryCount;
        }

        #endregion

        #region Protected Methods

        protected virtual void OnProgressChanged(JobProgressEventArgs args)
        {
            try
            {
                ProgressChanged?.Invoke(this, args);
            }
            catch
            {
                ProgressChanged = null;
            }
        }

        #endregion

        #region Private Static Methods

        private static string GetBlobName(string targetPath, string s3ObjectKey)
        {
            return Path.Combine(targetPath, s3ObjectKey).TrimStart('/');
        }

        #endregion

        #region ICopyJob Members

        /// <summary>
        ///     Progress event
        /// </summary>
        public event EventHandler<JobProgressEventArgs> ProgressChanged;

        #endregion
    }
}