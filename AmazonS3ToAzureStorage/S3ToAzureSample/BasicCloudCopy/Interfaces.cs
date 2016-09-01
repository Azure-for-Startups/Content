// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interfaces.cs" company="Microsoft">
//   Copyright (c) 2016 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicCloudCopy
{
    public interface IAmazonCredentials
    {
        #region Public Properties

        string AmazonAccessKeyId { get; set; }
        string AmazonRegion { get; set; }
        string AmazonSecretAccessKey { get; set; }

        #endregion
    }

    public interface IAmazonS3SourceOptions
    {
        #region Public Properties

        string SourceBucket { get; set; }
        string SourcePath { get; set; }

        #endregion
    }

    public interface IAzureStorageCredentials
    {
        #region Public Properties

        string AzureAccessKey { get; set; }
        string AzureStorageAccountName { get; set; }

        #endregion
    }

    public interface IAzureStorageTargetOptions
    {
        #region Public Properties

        int AccessLevel { get; set; }
        string TargetContainer { get; set; }
        string TargetPath { get; set; }

        #endregion
    }

    public interface ICommonCopyOptions
    {
        #region Public Properties

        int RetryCount { get; set; }
        int ThreadCount { get; set; }

        #endregion
    }
}