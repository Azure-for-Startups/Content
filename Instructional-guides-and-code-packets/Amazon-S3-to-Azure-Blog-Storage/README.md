# Amazon S3 to Azure Storage
Command line cross cloud copy tool

## Abstract (May 2016)
The following is a ‘how to’ code sample and library that can be used for recursive copy storage structure from Amazon S3 to Azure storage.  The library also can be used and integrated in any custom solution.

## Prerequisites
- Azure storage account
- Amazon S3 Account
- Visual Studio 2015

## Introduction
The S3 to Azure sample illustrates basic principles of copying from Amazon S3 bucket to Azure Storage blob container. The sample tool enables multithreaded asynchronous recursive copying of bucket folder structure and files matching the provided path prefix. A local connection is used to data transfer. The server side copying available on Azure is out of score for this sample. Also this sample is an illustration and is not intended to be a production ready application, but it can be used as is and extended for specific user scenarios 

## Source Code
The sample uses Amazon AWS SDK for .NET to work with Amazon S3 service. Microsoft _WindowsAzure.Storage_ client library is used to work with Microsoft Azure Blob Storage. A _CommandLineParser_ library is a part of the project and is used for command line parsing. 

The sample is developed using C# as a Visual Studio 2015 solution, which includes two projects:
- _BasicCloudCopy_ is a library providing sample code of the copying engine. This library can be utilized in variety of custom applications.
- _S3ToAzure_ is a command line tool built on the top of _BasicCloudCopy_ library. It provides command line parameters parsing and console output.

## BasicCloudCopy Library
The sample library _BasicCloudCopy_ provides the following concepts: 
- _CopyJob_ is an asynchronous single file copying operation.
- _CopyJobFactory_ creates copy jobs.
- _CopyJobRunner_ uses factory to construct subsequent jobs, executes jobs in parallel according to provided limit and handles job competitions.

The library provides basic interfaces:
- _ICopyJob_ is a basic interface for single thread copy job. This interface allows to start a copy operation using _CopyAsync_, check the progress using _ProgressChanged_ event and provide a way how to determine does a retry operation possible after last failure using _RequestRetry_.
- _ICopyJobFactory_ is a basic interface for the copy job factory. The factory is intended to construct copy job objects using CreateNextCopyJobAsync. This factory method creates a subsequent copy job object or returns null if no more objects available.

The library provides implementations of these interfaces for the case of local Amazon S3 to Azure copying using local memory streams.
- _S3ToAzureCopyJob_ is an implementation of _ICopyJob_ and provides copying an object from Amazon S3 bucket to Azure Storage container blob using local memory stream.
- _S3ToAzureCopyJobFactory_ is an implementation of _ICopyJobFactory_. The factory creates a copy jobs for recursive copying of objects from Amazon S3 bucket to Azure Storage container.
- _CopyJobRunner_ executes copy jobs and tracks competition, starts subsequent copy jobs according to maximum thread amount limit.

Most of Amazon/Azure related processing is performed in the _S3ToAzureCopyJob.CopyAsync_, _S3ToAzureCopyJob.UpdateSourceListAsync_ methods and _S3ToAzureCopyJobFactory_ constructor.

## S3ToAzure Application
The command line tool project is very simple: 
- Options is a class describing available options to be handled by the _CommandLineParser_ library. This class implements several interfaces used by _S3ToAzureCopyJobFactory_ and _CopyJobRunner_ as option sources. 
- Program is a main application class.

Command line arguments are parsed by _Parser.Default.ParseArguments_. If options are lexically correct and constraints are passed, then a _CopyAsyncAndReturnExitCode_ wrapper is executed. It starts and wait for asynchronous method _CopyAsync_ and handles exceptions. The _CopyAsync_ constructs the _S3ToAzureCopyJobFactory_ and _CopyJobRunner_, connects a progress handler with console output and starts the copy job runner.

The following error screen will be shown if the required command line parameters are not provided:

![alt tag](https://github.com/XynergiesLLC/test/blob/master/folder/ErrorScreen.png)

If parameters are correct the application writes a copying progress output:

![alt tag](https://github.com/XynergiesLLC/test/blob/master/folder/CopyingProgressOutput.png)
