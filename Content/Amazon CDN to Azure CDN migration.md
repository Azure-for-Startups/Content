> ![](media/image1.png){width="8.5in" height="9.132369860017498in"}

![](media/image3.jpg){width="1.8533333333333333in"
height="0.49333333333333335in"}Noga Tal *nogat@microsoft.com*

> Amazon CDN to Azure CDN Migration
>
> December 2015

OVERVIEW 
=========

> This article illustrates the basics of how to migrate existing content
> delivery from Amazon CDN to Azure CDN. The article illustrates how to
> prepare a test environment and perform migration tasks on this
> environment. The migration can be a partial or a complete one. The
> partial migration means you keep an origin for CDN in Amazon untouched
> and use the Azure CDN as content delivery only. The complete migration
> means you copy the origin data into Azure Storage and Azure CDN uses
> this storage as its origin. Let’s have a look on both solutions.

PREREQUISITES 
==============

> We need an active Amazon S3 account and Microsoft Azure subscription.
> Microsoft Windows 7-10 is required to execute PowerShell script.

CREATE A TEST ENVIRONMENT ON AMAZON AWS 
========================================

> 1\. Login to
> [https://console.aws.amazon.com.](https://console.aws.amazon.com/) To
> create a test Amazon S3 storage please click on Services and then on S3.
>
> ![](media/image4.jpg){width="4.1418755468066495in"
> height="3.749385389326334in"}
>
> 4\. Create folders and upload some files. (Below we created a folder
> named Images and uploaded a set of One Drive’s sample images).

![](media/image10.jpg){width="5.071875546806649in"
height="3.697080052493438in"}

6.  Double-check the permissions and address of some of the files in
    your folder to ensure they are publically available on the
    properties page. Everyone should be able to open the hyperlink of
    this file.

> ![](media/image14.jpg){width="4.355542432195976in"
> height="3.3927734033245844in"}

6.  To create a test Amazon CDN please click on Services and then
    on CloudFront.

![](media/image16.jpg){width="3.543264435695538in"
height="5.249023403324585in"}

10. Click in the Origin Domain Name on the Create distribution page and
    select a bucket created during the previous steps.

![](media/image22.jpg){width="2.4883333333333333in"
height="0.6783333333333333in"}

10. Leave other options untouched and click on Create Distribution.

![](media/image24.jpg){width="3.4583333333333335in" height="0.245in"}

10. Your distribution is being prepared while the status is
    ‘In Progress’.

![](media/image26.jpg){width="3.5016666666666665in"
height="0.6483333333333333in"}

10. Please wait for the Deployed status. You can now check that Amazon
    Bucket objects are available both directly and through the
    Amazon CDN.

> ![](media/image28.jpg){width="3.5806255468066492in"
> height="2.044996719160105in"}

PARTIAL MIGRATION - AZURE CDN WITH AMAZON S3 STORAGE 
=====================================================

> 1\. Login to https://manage.windowsazure.com. To create a new CDN
> endpoint please click on NEW -&gt; APP SERVICES -&gt; CDN -&gt; QUICK
> CREATE. Select the ORIGIN TYPE as Custom Origin. Type a S3 bucket URL as
> ORIGIN URL. Press CREATE.
>
> ![](media/image32.jpg){width="5.1386253280839895in"
> height="3.118817804024497in"}

3.  Your CDN address is located at CDN ENDPOINT.

> ![](media/image36.jpg){width="3.9588757655293088in"
> height="2.1216633858267717in"}

3.  Please wait some time while your CDN is preparing. You will get 404
    error trying to load the resource by Azure CDN based address at
    this time.

> ![](media/image38.jpg){width="1.9383333333333332in"
> height="0.5316666666666666in"}

3.  After 1 hour (in our case) we checked the address and found the
    image to be available. So now, we have an Azure CDN endpoint using
    Amazon S3 bucket as an origin.

![](media/image40.jpg){width="3.835in" height="0.65in"}

COMPLETE MIGRATION - COPY DATA TO AZURE STORAGE AND USE IT AS AN ORIGIN FOR AZURE CDN 
======================================================================================

1.  Login to
    [https://manage.windowsazure.com.](https://manage.windowsazure.com/)
    To create a new storage account, click on NEW -&gt; DATA SERVICES
    -&gt; STORAGE -&gt; QUICK CREATE. Assign a unique URL, select
    location and press on CREATE STORAGE ACCOUNT.

> ![](media/image42.jpg){width="4.372292213473316in"
> height="1.922567804024497in"}

1.  Click on the name of the account that was just created.

> ![](media/image44.jpg){width="4.393958880139983in"
> height="1.4614566929133859in"}

1.  Select a CONTAINERS tab and press ADD to create a new container.

> ![](media/image46.jpg){width="4.435625546806649in"
> height="1.2752055993000875in"}

1.  Type a NAME for a new container and select ACCESS as
    Public Container.

> ![](media/image48.jpg){width="1.8720144356955382in"
> height="1.3371511373578302in"}

1.  Remember the container’s URL. We will use it later on for the CDN
    endpoint step.

![](media/image50.jpg){width="4.803333333333334in"
height="1.5716666666666668in"}

1.  To copy data from Amazon S3 bucket to Azure Blob Storage I will use
    a PowerShell script. In order to use PowerShell script, your system
    should meet the requirements. You should have enough disk space on
    your user TEMP folder to copy the largest files of the stored ones
    in the S3 bucket you are going to copy. Prerequisites are AWS Tools
    for Windows PowerShell and Azure PowerShell.

2.  Download and install AWS Tools for Windows PowerShell from
    [*http://aws.amazon.com/powershell/*
    ](http://aws.amazon.com/powershell/)

3.  Download and install Azure PowerShell as described here:
    [*https://azure.microsoft.com/enus/documentation/articles/powershell-install-configure/*
    ](https://azure.microsoft.com/en-us/documentation/articles/powershell-install-configure/)

4.  Please save a content of subsequent script as a
    CopyS3ToAzure.ps1 file.

> \#Amazon S3 settings
>
> \$accessKey = "YOUR\_accessKey"
>
> \$secretKey = "YOUR\_secretKey"
>
> \$bucketName = "YOUR\_bucketName"
>
> \$keyPrefix = "/" \# Use a root folder used for your CDN
>
> \$continueFromMarker = \$null \# Use \$null to start from the first
> object. Object key to start from the subsequent object.
>
> \#Azure settings
>
> \$storageAccountName = "YOUR\_storageAccountName"
>
> \$storageAccountKey = "YOUR\_storageAccountKey"
>
> \$containerName = "YOUR\_containerName"
>
> \[System.Reflection.Assembly\]::LoadWithPartialName("System.Web") |
> Out-Null
>
> \$tempFile = \[System.IO.Path\]::GetTempFileName()
>
> \$blobContext = New-AzureStorageContext -StorageAccountName
> \$storageAccountName -StorageAccountKey \$storageAccountKey do {
>
> \$objects = Get-S3Object -BucketName \$bucketName -KeyPrefix
> \$keyPrefix -Marker \$continueFromMarker -AccessKey \$accessKey
> -SecretKey \$secretKey foreach(\$object in \$objects) { if
> (\$object.Size -ne 0) { "Copying: " + \$object.Key
>
> try {
>
> \$down = Copy-S3Object -BucketName \$bucketName -Key \$object.Key
> -LocalFile \$tempFile -AccessKey \$accessKey -SecretKey \$secretKey
>
> \$props = @{ 'ContentType' =
> \[System.Web.MimeMapping\]::GetMimeMapping(\$object.Key) }
>
> \$up = Set-AzureStorageBlobContent -Properties \$props -File
> \$tempFile -Container \$containerName -Blob \$object.Key -Context
> \$blobContext -Force
>
> }
>
> catch \[system.exception\] {
>
> write-host "\`nTerminated. Failed to copy " + \$object.Key + ". Change
> variable \`\$continueFromMarker = """ + \$continueFromMarker + """ to
> retry and continue.\`n" -foregroundcolor "magenta" throw
>
> } finally {
>
> Remove-Item -Path \$tempFile
>
> }
>
> }
>
> \$continueFromMarker = \$object.Key
>
> }
>
> } while (\$objects)

10. Please replace YOUR\_accessKey, YOUR\_secretKey, YOUR\_bucketName
    with S3 storage access keys and source bucket name. Replace
    YOUR\_storageAccountName , YOUR\_storageAccountKey,
    YOUR\_containerName with Azure Storage credentials and
    container name. Open Microsoft Azure PowerShell from the Windows
    Start Menu. Type a path to your script and press Enter.

![](media/image51.jpg){width="5.806666666666667in" height="3.875in"}

> **Please note the script is not intendent for huge amount of data and
> is not performance efficient for many small files.**

10. To create a new Azure CDN endpoint using Azure Storage blob
    container please click on NEW -&gt; APP SERVICES -&gt; CDN -&gt;
    QUICK CREATE. Select the ORIGIN TYPE as Custom Origin. We will not
    be using a Storage Accounts origin type because we are going to use
    a concrete container as a CDN root instead of using all storage
    account containers. Enter a container URL with replaced https to
    http in the ORIGIN URL.

> Press CREATE.
>
> ![](media/image52.jpg){width="5.080625546806649in"
> height="2.253538932633421in"}

10. The new endpoint is here. Please wait approx. 1 hour for this step
    to complete, though time may vary so please be patient.

> ![](media/image54.jpg){width="5.247292213473316in"
> height="1.5871511373578302in"}
>
> Once complere, we have a migrated Azure powered CDN with Azure
> storage, as origin. You can now check and see that Azure Blobs are
> available both directly and via Azure CDN.
>
> ![](media/image56.jpg){width="5.250972222222222in"
> height="2.1292344706911637in"}
