# Azure for Startups

The purpose of this GitHub repository is to help **startups** quickly engage and get up and running on Azure services.  You'll find links to key documentation, tutorials and code packets.  If you have feedback on the content please submit an [issue] (https://github.com/Azure-for-Startups/Content/issues).
<br><br>
##Table of Contents
   <table class="table table-bordered table-striped table-hover border-0px">
   <tr>
	<td valign="top"><b>[- Getting Started on Azure] (https://github.com/Azure-for-Startups/Content/blob/master/README.md#getting-started-on-azure)<br>[- Working with Azure] (https://github.com/Azure-for-Startups/Content/blob/master/README.md#working-with-azure)<br>[- Migrating to Azure] (https://github.com/Azure-for-Startups/Content/blob/master/README.md#migrating-to-azure)</b></td>
	<td valign="top"><b>[- Infrastructure Services] (https://github.com/Azure-for-Startups/Content/blob/master/README.md#infrastructure-services-iaas)</b><br>&nbsp;&nbsp;&nbsp;Compute, Networking, Storage<br><b>[- Platform Services] (https://github.com/Azure-for-Startups/Content/blob/master/README.md#platform-services-paas)</b><br>&nbsp;&nbsp;&nbsp;Web & Mobile, IoT & Machine Learning, <br>&nbsp;&nbsp;&nbsp;Data, Intelligence & Analytics<br></td>
	<td valign="top"><b>[- Cloud Architecure] (https://github.com/Azure-for-Startups/Content/blob/master/README.md#cloud-architecture)<br>[- Additional Useful Tools and Links] (https://github.com/Azure-for-Startups/Content/blob/master/README.md#additional-useful-tools--links)<br>[- Azure Resource Manager] (https://github.com/Azure-for-Startups/Content/blob/master/README.md#azure-resource-manager-arm)</b></td>	
	</tr>
</table>

##Getting Started on Azure
- **[Get started on Azure](https://azure.microsoft.com/en-us/get-started/)** – Link to Azure.com Getting Started page.
- **[Interactive Azure Map] (https://aka.ms/azmap)** - Interactive overview of services available on Azure. Click on a service to learn about it.
- **[Azure Learning Paths] (https://azure.microsoft.com/en-us/documentation/learning-paths/)** - Get started with these learning paths for different Azure Services.
- **[Azure on Microsoft Virtual Academy] (https://mva.microsoft.com/training-topics/cloud-app-development)** - Virtual courses on cloud development.
-	**[Azure on Channel9] (https://channel9.msdn.com/Azure)** - Video tutorials about Azure services.
-	**[Startup Offers] (https://azure.microsoft.com/en-us/pricing/member-offers/bizspark-startups/)** - Get free cloud credits and offers through Microsoft’s BizSpark program.
![](https://github.com/Azure-for-Startups/Content/blob/master/Content/media/AzureMap.png)<p> </p>
<p> </p>

##Working with Azure

-	**[Azure Portal] (https://portal.azure.com/)** - The best way to get started is with the Azure portal, a web based interface for managing Azure.
-	**[Azure SDKs & Tools] (https://azure.microsoft.com/en-us/downloads/)** - SDKs for many common languages such as .NET, Java, Node.js, Python, Ruby and other tools.
-	**[Azure PowerShell] (https://msdn.microsoft.com/en-us/library/jj156055.aspx)** - Work with PowerShell cmdlets to perform Azure operations.
-	**[Azure CLI] (https://azure.microsoft.com/en-us/documentation/articles/xplat-cli-install/)** – Create/manage Azure resources using a set of open-source shell-based commands.
-	**[Azure API Reference] (https://msdn.microsoft.com/en-us/library/azure/mt420159.aspx)** - Reference for Azure REST and .NET APIs.
-	**[Azure Solutions] (https://azure.microsoft.com/en-us/solutions/?v=3)** - A listing of top Azure solutions and their brief descriptions
-	**[Azure Products and services] (https://azure.microsoft.com/en-us/services/)** – A searchable list of all Azure products and services

![](https://github.com/Azure-for-Startups/Content/blob/master/Content/media/PortalImage.png)

##Migrating to Azure 
-	**[AWS to Azure mapping] (https://azure.microsoft.com/en-us/campaigns/azure-vs-aws/mapping/)** - Map between Azure and AWS services
-	**[MongoDB to Azure Migration resources] (https://github.com/Azure-for-Startups/Content/blob/master/Content/MongoDB%20Azure%20Migration.md)**  

  <h3><i> Tutorials and Sample code</i></h3>
     <table class="table table-bordered table-striped table-hover">
	<tr>
	  <td valign="top"><b>[AWS VM to Azure VM migration] (Content/AWS VM to Azure VM Migration.md)</b></td>
	  <td valign="top"><b>[AWS RDS to Azure SQL migration] (Content/AWS RDS to Azure SQL migration.md)</b></td>
          <td valign="top"><b>[AWS S3 to Azure Blob Storage migration] (https://github.com/Azure-for-Startups/Amazon-S3-to-Azure-Storage-demo)</b></td>
          <td valign="top"><b>[AWS CDN to Azure CDN Migration] (Content/Amazon CDN to Azure CDN migration.md)</b></td>
	  <td valign="top"><b>[ASP.NET Web App migration from AWS to Azure] (Content/ASP.NET Web App migration from AWS to Azure.md)</b></td>
	 </tr>
     </table>

   ###*Tools & Services*
	- **[AWCopy] (https://github.com/cicorias/AWCopy)** - Azure service that provides parallelized copies of S3 files in Amazon Web Services to Azure blobs.
	- **[CloudBerry Cloud Migrator] (http://www.cloudberrylab.com/cloud-migrator.aspx)** - service to transfer files from one cloud storage to another (Amazon S3 & Glacier, Windows Azure Blob Storage, Rackspace Cloud Files and FTP servers).
	- **[Azure Import/Export Service] (https://azure.microsoft.com/en-us/documentation/articles/storage-import-export-service/)** - Transfer Data to Blob Storage.

#Infrastructure Services (IaaS)
Infrastructure as a service (IaaS) refers to the compute, networking and storage building blocks which allow you to build any kind of cloud solution. IaaS allows you to have maximum control on how you manage virtual machines, network configuration but requires you to invest in attaining robustness, availability and scalability in the cloud.  

##Compute 
-	**[Learning Path for Azure VMs] (https://azure.microsoft.com/en-us/documentation/learning-paths/virtual-machines/)** - Learn how to deploy and manage VMs. 
-	**[Linux VM] (https://azure.microsoft.com/en-us/documentation/articles/virtual-machines-linux-quick-create-portal/)** – Get started creating a Linux VM on the Azure Portal and utilize [proven practices] (https://azure.microsoft.com/en-us/documentation/articles/guidance-compute-single-vm-linux/) to run the Linux VM.
-	**[Windows VM] (https://azure.microsoft.com/en-us/documentation/articles/virtual-machines-windows-hero-tutorial/)** – Get started creating a Windows VM on the Azure Portal
-	**[VM Extensions] (https://azure.microsoft.com/en-us/documentation/articles/virtual-machines-windows-extensions-features/)** - Learn about extensions to virtual machines such as the Chef, Docker or custom script extensions.
-	**[VM Scale Sets Overview] (https://azure.microsoft.com/en-us/documentation/articles/virtual-machine-scale-sets-overview/)** - Learn about deploying and managing VM scale sets.
-	**[Service Fabric Overview] (https://azure.microsoft.com/en-us/documentation/services/service-fabric/)** – a distributed systems platform that makes it easy to package, deploy, and manage scalable, reliable microservices.
-	**[Choose between App Services, Service Fabric and VMs] (https://azure.microsoft.com/en-us/documentation/articles/choose-web-site-cloud-service-vm/)** - including scenarios and recommendations.
-	**[Docker Documentation] (https://docs.docker.com/)** - Starting point for documentation on Dockers including Docker for MAC, Windows, Linux, etc.


   <h3><i> Tutorials and Sample code</i></h3>
     <table class="table table-bordered table-striped table-hover">
	<tr>
	  <td valign="top"><b>[Containers on Azure] (https://github.com/Azure-for-Startups/Containers-on-Azure-demo)</b> - Perform heavy computational tasks (for example video or audio encoding, hash calculation, data encryption, etc.) and make the process scalable and cost effective using Docker containers.</td>
          <td valign="top"><b>[Deploy to Azure using the Docker VM Extension] (https://azure.microsoft.com/en-us/documentation/articles/virtual-machines-linux-dockerextension/)</b> - use Resource Manager templates to deploy the Docker VM Extension in a custom, production-ready environment that you define</td>
           <td valign="top"><b>[Ruby on Rails web app on Azure VM] (https://azure.microsoft.com/en-us/documentation/articles/virtual-machines-linux-classic-ruby-rails-web-app/)</b></td>
</tr>
     </table>

##Networking
-	**[Virtual Networks (Vnets) Overview] (https://azure.microsoft.com/en-us/documentation/articles/virtual-networks-overview/)** – Learn about Azure Virtual Networks and how to create them.
-	**[Network Security Groups] (https://azure.microsoft.com/en-us/documentation/articles/virtual-networks-nsg/)** – Learn about Network Security Groups (NSGs) and how to configure them.
-	**[Load-Balancers] (https://azure.microsoft.com/en-us/documentation/articles/load-balancer-overview/)** – Learn about Azure Load Balancer and to configure one.

##Storage
-	**[Introduction to Azure Storage] (https://azure.microsoft.com/en-us/documentation/articles/storage-introduction/)** – Learn the basics of Azure Storage including Blob, Table, Queue, and File storage.
-	**[Create a storage account] (https://azure.microsoft.com/en-us/documentation/articles/storage-create-storage-account/)** – Learn how to create a general purpose or blob storage account with links to getting started on Blob, Table, Queue, and file storage accounts.

#Platform Services (PaaS)

Platform as a Service (PaaS) resources are Azure services which are built for most cases and allow you to enjoy high availability, scalability and robustness out of the box. Instead of managing VMs directly, let Azure manage the underlying infrastructure and focus on building your applications and solutions. 

##Web & Mobile
-	**[Azure App Service overview] (https://azure.microsoft.com/en-us/documentation/articles/app-service-value-prop-what-is/)** – Learn about Web Apps, Mobile Apps, API apps.
-	**[Notification Hub overview] (https://azure.microsoft.com/en-us/documentation/services/notification-hubs/)** - An easy-to-use, multiplatform, scaled-out push infrastructure.
-	**[Azure Search] (https://azure.microsoft.com/en-us/documentation/articles/search-what-is-azure-search/)** - Ready-to-use service that you can populate with your data and then use to add search to your web or mobile apps.
-	**[Azure Mobile Services REST API Reference MSDN] (https://msdn.microsoft.com/en-US/library/azure/jj710108.aspx)** – Documentation on Mobile Services REST API and the available operations.
- **[Azure Deployment Using Git] (https://github.com/Azure-for-Startups/Content/blob/master/Content/Azure-Deployment-using-Git.md)** – Learn the basics and understand available resources to support publishing web applications on Azure using Git workflows.

 ###Media & CDN
-	**[Azure Media Services (AMS) Overview] (https://azure.microsoft.com/en-us/documentation/articles/media-services-overview/)** – Learn about Azure Media Services and how to build scalable media management and delivery apps.

  <h3><i> Tutorials and Sample code</i></h3>
     <table class="table table-bordered table-striped table-hover">
	<tr>
	  <td valign="top"><b>[Notification Hub demo] (https://github.com/Azure-for-Startups/Notification-Hub-demo)</b> - Deliver push notification messages to mobile applications on iOS, Android and Windows Phone platforms using Azure Notification Hub</td>
          <td valign="top"><b>[PHP, Node.js, and Python] (https://github.com/Azure-for-Startups/Content/blob/master/Content/Deploy%20Node.js%20PHP%20and%20Python%20Web-apps-on-Azure.md)</b> - Deploy PHP, Node.js and Python web apps on Azure and learn how to configure Azure App service</td>
	</tr>
     </table>


##Internet of Things & Machine Learning
-	**[Azure IoT suite documentation] (https://azure.microsoft.com/en-us/documentation/suites/iot-suite/)** - Starting point to learning and using the Azure IoT suite.
-	**[Machine Learning Overview] (https://azure.microsoft.com/en-us/documentation/articles/machine-learning-what-is-machine-learning/)** - Overview and tutorial on Machine Learning.
-	**[Azure Machine Learning Studio] (https://azure.microsoft.com/en-us/documentation/articles/machine-learning-what-is-ml-studio/)** - A collaborative, drag-and-drop tool you can use to build, test, and deploy predictive analytics solutions. 

  <h3><i> Tutorials and Sample code</i></h3>
     <table class="table table-bordered table-striped table-hover">
	<tr>
          <td valign="top"><b>[IoT Microsoft Imagine course content] (https://github.com/MSFTImagine/computerscience/tree/master/Complimentary Course Content/Module6)</b> – learn how to collect streaming data from IoT devices and analyze the streaming data</td>
	</tr>
     </table>


##Data 
-	**[Azure SQL Overview] (https://azure.microsoft.com/en-us/documentation/articles/sql-database-technical-overview/)** – Gain an overview on SQL and how to create a SQL DB on Azure.
-	**[Azure SQL (PaaS) vs. SQL Server on Azure VMs (IaaS)] (https://azure.microsoft.com/en-us/documentation/articles/sql-database-paas-vs-sql-server-iaas/)** - Learn what scenarios are better for an IaaS vs. PaaS SQL solution.
-	**[DocumentDB] (https://azure.microsoft.com/en-us/documentation/services/documentdb/)** – Learn about this fully managed NoSQL database service and how to build and manage DocumentDB applications.
-	**[MongoDB on Azure] (https://docs.mongodb.com/ecosystem/platforms/windows-azure/)** ¬- Learn about MongoDB on Azure and deployment recommendations.

  <h3><i> Tutorials and Sample code</i></h3>
     <table class="table table-bordered table-striped table-hover">
	<tr>
	  <td valign="top"><b>[Import data to DocumentDB with the Database Migration tool] (https://azure.microsoft.com/en-us/documentation/articles/documentdb-import-data/)</b> – Learn how to use the open source DocumentDB data migration tool to import data to Azure DocumentDB</td>
	  <td valign="top"><b>[Install MongoDB on a Windows VM] (https://azure.microsoft.com/en-us/documentation/articles/virtual-machines-windows-classic-install-mongodb/)</b> – Learn how to install MongoDB on a Windows VM in Azure.</td>
          <td valign="top"><b>[Install MongoDB on Linux] (https://docs.mongodb.com/v3.0/administration/install-on-linux/)</b> - Learn how to install MongoDB on a Linux VM in Azure.</td>
          <td valign="top"><b>[Create a web app that connects to MongoDB] (https://azure.microsoft.com/en-us/documentation/articles/web-sites-dotnet-store-data-mongodb-vm/)</b> – Learn how to create a web page in Azure that connects to MongoDB running on a VM.</td>
	  </tr>
     </table>

##Intelligence & Analytics
-	**[Stream Analytics] (http://azure.microsoft.com/en-us/services/stream-analytics/)** - Overview of Azure stream analytics, low-cost solutions to gain real-time insights from devices, sensors, infrastructure, and applications 
-	**[Additional Intelligence and Analytics services] (https://azure.microsoft.com/en-us/services/?sort=popular&filter=intelligence-analytics)** – HDInsight, Machine Learning, Data Factory, Log Analytics, Data Catalog, Power BI Embedded, Data Lake store and much more.

  <h3><i> Tutorials and Sample code</i></h3>
     <table class="table table-bordered table-striped table-hover">
	<tr>
	  <td valign="top"><b>[Data Analysis using Hadoop] (https://github.com/MSFTImagine/computerscience/tree/master/Complimentary%20Course%20Content/Module4)</b>  – Microsoft Imagine course content - learn how to use Hive for Big Data Analysis</td>
          <td valign="top"><b>[Data Science and Machine Learning] (https://github.com/MSFTImagine/computerscience/tree/master/Complimentary%20Course%20Content/Module5)</b> – Microsoft Imagine course content - learn fundamental concepts of machine learning and use Spark to predict the trend and patterns of massive data sets</td>
	  </tr>
     </table>


#Cloud Architecture
-	**[Cloud Patterns & Practices] (https://aka.ms/mspnp)** - Best practices for building cloud solutions. Including checklists and design patterns. 
-	**[Architecture Blueprints] (https://msdn.microsoft.com/architects-blueprints-msdn)** - Architectures for an array of different cloud scenarios.

#Additional Useful Tools & Links
-	**[Azure Price Calculator] (https://aka.ms/azurecalc)** - Easily calculate pricing of Azure Services. 
-	**[Azure Trust Center] (https://azure.microsoft.com/en-us/support/trust-center/)** - Learn about Azure security, compliance, privacy and transparency.
-	**[Azure Subscription Service Limits] (https://azure.microsoft.com/en-us/documentation/articles/azure-subscription-service-limits/)** - Learn about Azure subscription and service limits, quotas, and constraints.
-	**[Azure Resource Explorer] (http://resources.azure.com/)** - A very useful web application to explore the Azure REST API.
-	**[Azure Storage Explorer] (http://storageexplorer.com/)** - A client application for Linux, Mac or Windows to easily work with storage accounts on Azure.

#Azure Resource Manager (ARM)
The Azure Resource Manager is at the core of the Azure platform and is used to deploy and manage Azure services. Every resource in Azure managed under ARM can be described and managed in a consistent way. It's very helpful to understand the Azure Resource Manager and how to work with Resource Groups.
-	**[ARM Overview] (https://azure.microsoft.com/en-us/documentation/articles/resource-group-overview/)** - Get an overview of the Azure Resource Manager.
-	**[ARM vs. Classic] (https://azure.microsoft.com/en-us/documentation/articles/resource-manager-deployment-model/)** - Understand the difference between ARM and the Classic (ASM) deployment model.
-	**[Azure portal availability chart] (https://azure.microsoft.com/en-us/features/azure-portal/availability/)** – use the availability chart to determine what services are supported by ARM and the Azure portal

##ARM Templates
ARM templates are JSON descriptions of ARM deployments which can be used for "Infrastructure as Code".
-	**[Deploying ARM Templates] (https://azure.microsoft.com/en-us/documentation/articles/resource-group-template-deploy/)** – Learn how to deploy ARM templates using PowerShell, Azure CLI or REST API.
-	**[Quick Start Templates] (https://github.com/Azure/azure-quickstart-templates)** - A Github maintained, vast collection of templates for common use cases to help you get started authoring your own templates or deploying simple solutions.
-	**[Template Authoring] (https://azure.microsoft.com/en-us/documentation/articles/resource-group-authoring-templates/)** - How to author custom templates.
