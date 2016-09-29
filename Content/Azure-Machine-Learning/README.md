##Abstract (May 2016)

In this tutorial we will demonstrate how to apply Azure Machine Learning technology to perform analysis of a dummy system in order to detect and alert about any anomaly or unusual behavior. We will build a Machine Learning model/experiment for the system which performs data transfer between different Azure datacenters. We will also build a watchdog to monitor data transfer response time.

##RESPONSE DURATION WATCHER SAMPLE

#Prerequisite

In order to complete this sample, you will need to have an active Azure Subscription and create a Machine Learning workspace. The following article provides step by step instructions and describes how to setup up a required environment: <https://azure.microsoft.com/en-us/trial/get-started-machine-learning/>.

In this sample Azure Resource Manager templates will be used to deploy an application. Detailed information on using ARM templates are available here: <https://azure.microsoft.com/en-us/documentation/articles/resource-group-authoring-templates/>.

In order to build the solution Visual Studio 2015 is required.

#Workflow proposal
The following high level flow is used for this sample:

-   Create storage account in *Western Europe* geo region (Account A)

-   Create storage account in *Southeast Asia* geo region (Account B)

-   Execute stat job, which measures the data transfer time:

-   Generate a file S with some random data of random X Mbytes size locally

-   Copy S to A

-   Copy A to B and measure the time required to complete the operation. Store the time to collect a statistic data using week day/hour and file size as a parameter (to simplify the sample we made an assumption that load causing traffic delays depends only on week day, time and file size. In real life the mode will be more complicated, but everything described in this article will be applicable as well).

-   Execute a scheduled retrain job, which retrains machine learning model using recently collected statistic:

-   If the statistical data exists we should retrain a model with current data

-   Execute a scheduled measure job:

-   Generate a file S with some random data of random X Mbytes locally

-   Copy S to A

-   Copy A to B and measure the time.

-   Compare the measured time with predicted time based on the week/hour and file size.

-   If the time difference between actual and predicted performance is greater than preconfigured value, then system rises alert vial provided HTTP GET callback URL.

#Preparing Microsoft Azure environment

-   Deploy required services using a deployment template

-   Open Microsoft Azure Portal at <https://portal.azure.com> and sign in to your subscription

-   Click on New, enter "Template Deployment" in the search box and click on found Template Deploymentand click template deployment icon button

<img src="media/image1.png" width="601" height="177" />

-   Click Create to open a Custom deployment blade

-   Click Edit template and paste a content of *AzureResourceGroup/Templates/azuredeploy.json file.*

-   Click Save

-   Click Edit Parameters. There should be 8 parameters if the template is successfully parsed. Enter a CALLBACK\_URL you need to be called if the copy operation duration exceeds the preconfigured limit. It can be any web address supporting the HTTP GET request. This value can be left empty if you don't need notifications and can be changed later in the web app configuration. We propose to leave other values as is and click OK

-   Enter a new resource group name for ex. *MachineLearningSample* in the Resource group field.

-   Select a resource group location you prefer

-   Accept Legal terms

-   Click Create to start deployment

<img src="media/image3.png" width="247" height="581" />

-   The resource group containing a job collection with 3 disabled scheduler tasks, two storage accounts in different geo regions, app service plan and a web app is created after deployment.

<img src="media/image4.png" width="299" height="275" />

Some additional manual configuration is required:

-   Click on a web app resource and open its Application Settings

-   Copy content of *StorageAConnectionString* parameter and paste as a "*StorageAConnectionString*" parameter in *ResponseWatcher/WebApp/Web.config*

-   Copy content of *StorageBConnectionString* parameter and paste as a "*StorageBConnectionString*" parameter in *ResponseWatcher/WebApp/Web.config*

-   The Callback URL also can be changed in Application Settings as well

#Build and publish a Web App

-   Open ResponseWatcher.sln in the Visual Studio 2015

-   Open *WebApp/Web.config* and make sure *StorageAConnectionString* and *StorageBConnectionString* are configured properly according settings in your Azure Management portal. Leave settings with ML prefix empty

-   Build the solution and open a publish dialog for WebApp project and click on Microsoft Azure App Service:

<img src="media/image5.png" width="286" height="223" />

-   Sign in to your Azure Subscription and select a Web App in the MachineLearningSample resource group:

<img src="media/image6.png" width="363" height="273" />

-   Click on Settings, select a Debug configuration and click Publish

<img src="media/image7.png" width="362" height="285" />

-   The deployed web service will be opened in a browser. Please add a */api/MachineLearning/Collect* to the opened URL and press enter several times to collect initial statistics for ML experiments which will be created later. Everything is ok if you receive something like this:

<img src="media/image8.png" width="601" height="51" />

-   Open Azure Portal, then open the *MachineLearningSample* resource group, click on a storage account with name started with "*storagea*", click on Blobs, then click on container with name statistics, then click on blob named CopyDurations.csv and copy its URL. Copy this URL to use as default URL for Reader on the Machine Learning Training experiment in the next section.

<img src="media/image9.png" width="601" height="258" />

##Preparing Machine Learning environment

#Create training experiment

-   Open a Machine Learning studio at <https://studio.azureml.net> and sign in to your workspace

-   Create a new blank experiment by clicking + NEW at the bottom 

<img src="media/image10.png" width="80" height="23" />
 
<img src="media/image11.png" width="244" height="205" />

-   Rename the experiment as "*Duration*"

-   Search and add a Reader module to experiment.

-   Select a Reader module and set its properties:

-   Specify Data source type as "*Web URL via HTTP*"

-   URL with address of CopyDurations.csv blob you copied earlier

-   Set data format as "*CSV*"

-   Mark "*CSV or TSV has header row*" checkbox

<img src="media/image12.png" width="537" height="257" />

-   Click Save

-   Search and add an Execute R Script module and replace a script with:

#### **dataset &lt;- maml.mapInputPort(1)**

#### **dataset$DaySeconds &lt;- dataset$UtcUnixTimeSeconds %% 86400**

#### **maml.mapOutputPort("dataset");
**

-   Connect modules and click Run

<img src="media/image13.png" width="344" height="186" />

-   Search and add following modules to experiment:

-   Project Columns

-   Split Data

-   Linear Regression

-   Train Model

-   Score Model

-   Evaluate Model

-   Connect modules as it is on the screenshot:

<img src="media/image14.png" width="252" height="251" />

-   Select the Project Columns module and click "*Launch column selector*". Select WITH RULES, ALL COLUMNS, exclude column with name *UtcUnixTimeSecond*s and click check mark

<img src="media/image15.png" width="497" height="158" />

-   Select the Train Model module and click "*Launch column selector*". Select WITH RULES, include column with name *DurationMs* and click check mark

<img src="media/image16.png" width="528" height="105" />

-   Select the Split Data module and enter 0.9 in the "*Fraction of rows in the first output dataset*"

-   Click Save and then on Run and ensure you see green check marks on every module after the run.

#Create a predictive experiment and web service

-   Open a Duration experiment created earlier:

<img src="media/image17.png" width="601" height="185" />

-   Run an experiment

-   Click "*Set up web service*" and then on "*Predictive Web Service*"

<img src="media/image18.png" width="385" height="200" />

-   Wait for some time
<img src="media/image19.png" width="197" height="37" />

-   Make sure your predictive experiment looks like this:

<img src="media/image20.png" width="180" height="224" />

-   Copy a name of trained model (for our case it is a *Duration \[trained model\]*) and paste as a "*MLTrainedModelName*" parameter in *ResponseWatcher/WebApp/Web.config*

-   Add Project Columns module and connect it between Reader and Execute R Script modules.

<img src="media/image21.png" width="454" height="177" />

-   Select the Project Columns module and click on "*Launch column selector*". Select WITH RULES, ALL COLUMNS, exclude column named *DurationMs* and click on check mark.

<img src="media/image22.png" width="601" height="201" />

-   Click Run to execute an experiment

-   Add Project Columns module and connect it between Score Model and Web service output.

<img src="media/image23.png" width="200" height="147" />

-   Select the Project Columns module and click on "*Launch column selector*". Select WITH RULES, NO COLUMNS, include column named Scored Labels and click on check mark.

<img src="media/image24.png" width="476" height="157" />

-   Click and Run to execute an experiment. Make sure there are green check marks on every module after the run

-   Click Deploy Web Service

<img src="media/image25.png" width="287" height="87" />

-   Now is a good time to test. Click Test to initiate a web service call

<img src="media/image26.png" width="364" height="408" />

-   Enter some values and click check mark

<img src="media/image27.png" width="242" height="158" />

-   The result should look like this:
<img src="media/image28.png" width="322" height="37" />

#Make a predictive web service re-trainable

In order to make a predictive experiment re-trainable we setup a web service for training experiment and create an additional endpoint for predictive web service. The training web service produces a new trained model, and the model is used to update a predictive web service.

#Add a secondary endpoint to predictive web service

-   Select WEB SERVICES an open a predictive web service Duration \[Predictive Exp.\]
<img src="media/image29.png" width="298" height="124" />

-   Click "Manage endpoints in Azure management portal" on bottom of page

<img src="media/image30.png" width="303" height="71" />

-   In the management portal click ADD ENDPOINT:

<img src="media/image31.png" width="305" height="127" />

-   Enter endpoint name as "secondary", optionally enter a description and click check mark.

<img src="media/image32.png" width="334" height="266" />

-   Open "secondary" endpoint page and scroll down. At the right bottom corner, you can find an API KEY. Copy this key and paste as a "*MLPredictiveSecondaryApiKey*" parameter in *ResponseWatcher/WebApp/Web.config*

-   Click "*Request/Response*"

<img src="media/image33.png" width="148" height="112" />

-   Copy a Request URI and paste as "*MLPredictiveSecondaryRequestUrl*" parameter in *ResponseWatcher/WebApp/Web.config*. Do not forget to escape the ‘*&*' symbol as *&amp*;

<img src="media/image34.png" width="601" height="100" />

-   Go back on "*secondary*" endpoint page and click Update Resource

<img src="media/image35.png" width="164" height="124" />

-   Copy a Request URI and paste as a "*MLPredictiveSecondaryUpdateUrl*" parameter in *ResponseWatcher/WebApp/Web.config*

<img src="media/image36.png" width="601" height="108" />

#Extend a training experiment with web service

-   Open a training experiment duration

-   Add a Web service output module, name it as "*trained*" and connect to the Train Model module output

<img src="media/image37.png" width="482" height="161" />

-   Add a Web service output module, name it as "evaluated" and connect to the Evaluate Model module output

<img src="media/image38.png" width="479" height="158" />

-   Copy data from Enter Data Manually to file Dummy.csv and make it available online for example by sharing from OneDrive.

-   Select a Reader module, click on the <img src="media/image39.png" width="23" height="20" />above the URL property and click Set as web service parameter:

<img src="media/image40.png" width="178" height="195" />

-   Click Run and make sure there is no errors: <img src="media/image41.png" width="144" height="18" />

-   Click SETUP WEB SERVICE and then Deploy Web Service and then Yes**
**

<img src="media/image42.png" width="246" height="157" /><img src="media/image43.png" width="198" height="56" />

-   Copy API key and paste as the "*MLTrainingApiKey*" parameter in *ResponseWatcher/WebApp/Web.config*. Click BATCH EXECUTION.

<img src="media/image44.png" width="408" height="345" />

-   Copy a Request URI and paste as the "MLTrainingBatchUrl" parameter in ResponseWatcher/WebApp/Web.config*


<img src="media/image45.png" width="601" height="103" />

#Redeploying a completely configured WebApp

At the moment you should have ResponseWatcher/WebApp/Web.config with every ML-prefixed application setting configured. If not, please follow instructions provided above and make sure all steps are executed. Republish the WebApp in order to be able to execute Retrain and Compare API calls.

-   Open ResponseWatcher.sln in the Visual Studio 2015

-   Right click WebApp project, select the Publish… menu item, ensure a correct publish profile is selected and click Publish

<img src="media/image46.png" width="470" height="369" />

-   The deployed web service will be opened in the browser. Please ad*d /api/MachineLearning/Retrain* to the opened URL and press enter. If everything is ok, then you get something like this:

<img src="media/image47.png" width="322" height="33" />

-   Please use */api/MachineLearning/Compare?fileSize=17000000&alarmLevel=2.0* in the URL and click enter. If everything is ok then you will get something like this:

<img src="media/image48.png" width="474" height="27" />

#Enabling scheduler tasks

-   Open Azure Portal, open the *MachineLearningSample* resource group, click Scheduler Job Collection with a name beginning with "j*obcollection*", click Scheduler Jobs.

-   Enable a job with a name beginning with "*collectjob*". This job needs to be enabled for several days to collect enough statistics.

-   Enable a job with name started with "r*etrainjob*" in case you need regular retraining. It's ok if this job is executed just once after *collectjob* execution interruption.

-   The third job with name "*compare*" should be enabled after *collectjob* has collected enough statistics and "*retrainjob*" is executed at least once after *collectjob* execution interruption.
