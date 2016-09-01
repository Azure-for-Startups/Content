// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3ToAzureCopyJobFactory.cs" company="Microsoft">
//   Copyright (c) 2016 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicCloudCopy
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;

    #endregion

    /// <summary>
    ///     The factory creates a copy jobs for recursive copying of objects from Amazon S3 bucket to Azure Storage container
    ///     blobs.
    /// </summary>
    public class S3ToAzureCopyJobFactory: ICopyJobFactory
    {
        #region Private Fields

        private ListObjectsRequest _listObjectsRequest;
        private List<S3Object> _s3Objects = new List<S3Object>();

        #endregion

        #region Constructors

        public S3ToAzureCopyJobFactory(
            IAmazonCredentials amazonCredentials,
            IAzureStorageCredentials azureStorageCredentials,
            IAmazonS3SourceOptions amazonS3SourceOptions,
            IAzureStorageTargetOptions azureStorageTargetOptions,
            ICommonCopyOptions commonCopyOptions)
        {
            if (amazonCredentials == null)
            {
                throw new ArgumentNullException(nameof(amazonCredentials));
            }

            if (azureStorageCredentials == null)
            {
                throw new ArgumentNullException(nameof(azureStorageCredentials));
            }

            if (amazonS3SourceOptions == null)
            {
                throw new ArgumentNullException(nameof(amazonS3SourceOptions));
            }

            if (azureStorageTargetOptions == null)
            {
                throw new ArgumentNullException(nameof(azureStorageTargetOptions));
            }

            if (commonCopyOptions == null)
            {
                throw new ArgumentNullException(nameof(commonCopyOptions));
            }

            AmazonS3SourceOptions = amazonS3SourceOptions;
            _listObjectsRequest = new ListObjectsRequest
            {
                BucketName = AmazonS3SourceOptions.SourceBucket,
                Prefix = AmazonS3SourceOptions.SourcePath.TrimStart('/')
            };

            AzureStorageTargetOptions = azureStorageTargetOptions;
            if (!Enum.IsDefined(
                typeof(BlobContainerPublicAccessType),
                AzureStorageTargetOptions.AccessLevel))
            {
                throw new ArgumentException("Unknown level of public access");
            }

            var regionEndpoint = RegionEndpoint.GetBySystemName(amazonCredentials.AmazonRegion);
            if (regionEndpoint.DisplayName.Contains("Unknown"))
            {
                throw new ArgumentException("Unknown Amazon region");
            }

            AmazonS3Client = new AmazonS3Client(
                amazonCredentials.AmazonAccessKeyId,
                amazonCredentials.AmazonSecretAccessKey,
                regionEndpoint);
            var storageAccount = new CloudStorageAccount(
                new StorageCredentials(
                    azureStorageCredentials.AzureStorageAccountName,
                    azureStorageCredentials.AzureAccessKey),
                true);
            CloudBlobClient = storageAccount.CreateCloudBlobClient();
            CommonCopyOptions = commonCopyOptions;
        }

        #endregion

        #region Internal Properties

        internal AmazonS3Client AmazonS3Client { get; set; }
        internal IAmazonS3SourceOptions AmazonS3SourceOptions { get; set; }
        internal IAzureStorageTargetOptions AzureStorageTargetOptions { get; set; }
        internal CloudBlobClient CloudBlobClient { get; set; }
        internal ICommonCopyOptions CommonCopyOptions { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a subsequent copy job
        /// </summary>
        /// <returns>Returns null in case no new job can be created</returns>
        public async Task<ICopyJob> CreateNextCopyJobAsync()
        {
            await UpdateSourceListAsync();
            if (!_s3Objects.Any())
            {
                return null;
            }

            var s3Object = _s3Objects[0];
            _s3Objects.RemoveAt(0);
            return new S3ToAzureCopyJob(this, s3Object);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Updates a source object list
        /// </summary>
        private async Task UpdateSourceListAsync()
        {
            if (!_s3Objects.Any() && _listObjectsRequest != null)
            {
                var response = await AmazonS3Client.ListObjectsAsync(_listObjectsRequest);
                // Ignoring folder objects
                _s3Objects = response.S3Objects.Where(o => !o.Key.EndsWith("/")).ToList();
                if (response.IsTruncated)
                {
                    _listObjectsRequest.Marker = response.NextMarker;
                }
                else
                {
                    _listObjectsRequest = null;
                }
            }
        }

        #endregion
    }
}