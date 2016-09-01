**
**Abstract (January 2016)

End-to-end demonstration of the video hosting sample system on Azure which includes: Azure Storage, Azure Media Services, Azure SQL database, and Azure Web application as a frontend.

INTRODUCTION

Azure Media Services powers consumer and enterprise streaming solutions worldwide. Combining powerful and highly scalable cloud-based encoding, encryption, and streaming components, Media Services helps customers with valuable and premium video content easily reach larger audiences on today’s most popular digital devices, such as tablets and mobile phones. Live broadcasters - of sporting events, news, concerts, town meetings, and more - and linear channel operators offering popular over-the-top programming and services are turning to Azure as their platform of choice.

Additionally, with exciting new features such as Azure Media Indexer to enhance discoverability, cross-platform players to simplify distribution, cloud DVR capabilities to move easily from live content to on-demand programming, and a large ecosystem of value-added third-party partners, Media Services is truly providing customers with video content as a best-in-class solution. Come have a look yourself, and see how Media Services can power your end-to-end media workflow.

Microsoft Azure Media Services is an extensible cloud-based platform that enables developers to build scalable media management and delivery applications. Media Services is based on REST APIs that enable you to securely upload, store, encode and package video or audio content for both on-demand and live streaming delivery to various clients (for example, TV, PC, and mobile devices).

You can build end-to-end workflows using entirely Media Services. You can also choose to use third-party components for some parts of your workflow. For example, encode using a third-party encoder. Then, upload, protect, package, and deliver using Media Services.

[Here](http://azure.microsoft.com/documentation/infographics/media-services/) you can view the Azure Media Services poster that depicts AMS workflows, from media creation through consumption.

You can view AMS learning paths here:

[AMS Live Streaming Workflow](http://azure.microsoft.com/documentation/learning-paths/media-services-streaming-live/)

[AMS on Demand Streaming Workflow](http://azure.microsoft.com/documentation/learning-paths/media-services-streaming-on-demand/)

Overview

This document contains the end to end demonstration of the sample Video Sharing Network web application based on the Azure Media Services which includes: Azure Media Services account for storage of media content, ASP.NET MVC Web Application for managing the media content and Azure SQL database for storage of the aggregated information about the media content. We do not consider implementing any specific UI graphic for this sample, because every startup has its own UX concept. Technical UX is based on standard MVC templates.

Prerequisites

To start using Azure Media Services, you should have the following:

1.  An Azure account. If you don't have an account, you can create a free trial account in just a couple of minutes.

2.  An Azure Media Services account. Use the Azure Classic Portal, .NET, or REST API to [*create Azure Media Services account*](https://azure.microsoft.com/en-us/documentation/articles/media-services-create-account/).

3.  *(Optional)* [*Setup development environment*](https://azure.microsoft.com/en-us/documentation/articles/media-services-dotnet-how-to-use/). Choose .NET or REST API for your development environment. Also, learn how to [*connect*](https://azure.microsoft.com/en-us/documentation/articles/media-services-dotnet-connect_programmatically/) programmatically.

4.  *(Recommended)* Allocate one or more scale units for applications in production environment. For more information, see [*Managing streaming endpoints*](https://azure.microsoft.com/en-us/documentation/articles/media-services-manage-origins/).

5.  For additional information, see the [*Concepts*](https://azure.microsoft.com/en-us/documentation/articles/media-services-concepts/).

To build the Web Portal we will use an ASP.NET MVC 5 Web Application. Also we will need one more Azure Storage Account for the temporary uploaded data but in this sample we will use one which will be created for the Azure Media Services as underlying storage.

<span id="_Toc440593674" class="anchor"></span>Components in details

<span id="h.wmcgoewu1hnk" class="anchor"><span id="_Toc440593675" class="anchor"></span></span>Creating the Media Service from the Azure Portal

Unfortunately, Media Services are available only from the Azure Classic Portal now. So let’s start there. Use the well-known creation UI for the Media Service account creation:

<img src="media/image1.png" width="624" height="228" />

After creating the Media Service, you will get results similar to the following image:

<img src="media/image2.png" width="540" height="476" />

<span id="h.oymnw3nlvwib" class="anchor"></span>Creating an ASP.NET MVC Web Application

We will use the standard ASP.NET 4.6.1 MVC Template with the Individual User Accounts:

<img src="media/image3.png" width="624" height="484" />

For the data access we will use the Entity Framework with the Code-First approach. So let’s start with the VideoElement entity creation with the minimum number of properties for storing required data.

<img src="media/image4.png" width="233" height="272" />

Also we will need the simple DbContext for the data management via Entity Framework:

<img src="media/image5.png" width="624" height="253" />

Please note that at this point you will have to check and adjust the connection string to the SQL Server for your DbContext. Now we are ready for creating the MVC Controller for the VideoElement.

> *Note: to keep the things simple we will not use the MVC Models and will use the Entities as is, but for the enterprise solution it’s highly recommended to use the MVC Models and some mapping from Entities to MVC Models and vice versa - e.g. *[AutoMapper](http://automapper.org/).**

We will use the Scaffolded MVC 5 Controller using the Entity Framework:

<img src="media/image6.png" width="624" height="413" />

After the configuration:

<img src="media/image7.png" width="536" height="344" />

You should get the generated Controller and Views. We don’t need all the actions and views for this sample so please check the attached sources to see the adjustments which have been made to the generated files to make them meet the requirements.

Next step is to configure the Azure Media Service and Azure Storage Account access keys. You can find them in the Azure Classic Portal in the appropriate sections:

<img src="media/image8.png" width="386" height="369" />

<img src="media/image9.png" width="378" height="397" />

And we need to place them in the certain places in the Web.config file:

<img src="media/image10.png" width="477" height="97" />

In the attached sources you will be able to find the adjusted UI, enabled MS Account authentication and the main logic. Provided are the high-level description of the logic so for the details please take a look at the sources.

<span id="_Toc440593677" class="anchor"></span>High-level logic description

Prerequisites

<span id="_Toc440260016" class="anchor"></span>We will need the following NuGet packages:

[*Windows Azure Media Services .NET SDK*](https://www.nuget.org/packages/windowsazure.mediaservices)

[*Windows Azure Media Services .NET SDK Extensions*](https://www.nuget.org/packages/windowsazure.mediaservices.extensions/)

[*Windows Azure Storage*](https://www.nuget.org/packages/WindowsAzure.Storage)

<span id="h.h02mdh40dnvw" class="anchor"><span id="_Toc440593679" class="anchor"></span></span>Uploading the media content logic

Uploaded file will be split to the N chunks (1 MB) which will be uploaded one by one. Each chunk will have 3 attempts for the successful uploading. Each chunk will be transferred to the temporary Azure Storage Blob which means that uploading can be paused and resumed if needed. After uploading the last chunk, the Blob is ready to be the source for the Media Asset. The final step is Publishing.

<span id="h.r4h61fc944vg" class="anchor"><span id="_Toc440593680" class="anchor"></span></span>Creating the Media Asset logic

We will use the *StartCopyAsync()* method for creating the final Blob from the temporary one and the *GenerateFromStorageAsync()* method for creating the Media Asset based on the final Blob. As the result we will have the Media Asset created but with no way to access it because of absence of the Access Policy.

<span id="h.knljpdcoeq9q" class="anchor"><span id="_Toc440593681" class="anchor"></span></span>Publishing logic

Formally Publishing is a creating the Locator with the Access Policy for the Media Asset. We will get the streaming URI as the result which will be used for accessing the media content. Azure Media Services supports two types of locators: OnDemandOrigin locators, used to stream media (for example, MPEG DASH, HLS, or Smooth Streaming) and Access Signature (SAS) locators, used to download media files.

Summary

In the article we described, how to get started with Azure Media Services to build your own video hosting and streaming solution, and how to build Web application to upload/stream video data.

Startups may take this sample as a starting point and customize to match business model into a specific application involving media services.
