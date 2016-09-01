// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="Microsoft">
//   Copyright (c) 2016 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace S3ToAzure
{
    #region Usings

    using BasicCloudCopy;
    using CommandLine;

    #endregion

    /// <summary>
    ///     Command line options definition class
    /// </summary>
    internal class Options: IAmazonCredentials,
        IAmazonS3SourceOptions,
        IAzureStorageCredentials,
        IAzureStorageTargetOptions,
        ICommonCopyOptions
    {
        #region Public Properties

        [Option('i',
            "keyid",
            Required = true,
            HelpText = "Amazon access key id")]
        public string AmazonAccessKeyId { get; set; }

        [Option('s',
            "secret",
            Required = true,
            HelpText = "Amazon secret access key")]
        public string AmazonSecretAccessKey { get; set; }

        [Option('r',
            "region",
            Required = true,
            HelpText = "Amazon region like us-west-1, eu-central-1 and etc.")]
        public string AmazonRegion { get; set; }

        [Option('b',
            "bucket",
            Required = true,
            HelpText = "Source bucket name")]
        public string SourceBucket { get; set; }

        [Option('p',
            "prefix",
            Default = "/",
            HelpText = "Source path prefix. Recursive for matched folders")]
        public string SourcePath { get; set; }

        [Option('a',
            "account",
            Required = true,
            HelpText = "Azure Storage account name")]
        public string AzureStorageAccountName { get; set; }

        [Option('k',
            "key",
            Required = true,
            HelpText = "Azure Storage account access Key")]
        public string AzureAccessKey { get; set; }

        [Option('c',
            "container",
            Required = true,
            HelpText = "Target container name")]
        public string TargetContainer { get; set; }

        [Option('f',
            "folder",
            Default = "/",
            HelpText = "Target folder")]
        public string TargetPath { get; set; }

        [Option('l',
            "level",
            Default = 0,
            HelpText =
                "Level of public access that is allowed on the target container. 0 - Off, 1 - Container, 2 - Blob. Used in case the container does not exist"
            )]
        public int AccessLevel { get; set; }

        [Option('R',
            "retries",
            Default = 1,
            HelpText = "Maximum retry count")]
        public int RetryCount { get; set; }

        [Option('T',
            "threads",
            Default = 4,
            HelpText = "Copying threads amount")]
        public int ThreadCount { get; set; }

        #endregion
    }
}