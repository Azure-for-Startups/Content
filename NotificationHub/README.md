**
**Abstract (May 2016)

This article illustrates how to use the Azure Notification Hub to deliver push notification messages to mobile applications on iOS, Android and Windows Phone platforms.

INTRODUCTION

Azure Notification Hub provides an easy-to-use infrastructure that enables sending mobile push notifications from any backend (in the cloud or on-premises) to any mobile platform.

With Notification Hub you can easily send cross-platform, personalized push notifications, abstracting the details of the different platform notification systems (PNSs). With a single API call, you can target individual users or entire audience segments containing millions of users, across all their devices.

The detailed overview of Azure Notification Hub is available here: <https://msdn.microsoft.com/en-us/library/azure/jj891130.aspx>.

The idea of this sample is to give an Azure Web App visitor the ability to send a push notification to any registered mobile device, having installed mobile application from this sample.

Web Application displays a list of registered device. Users select a device from the list, then types in a free text message and sends it.

To cover more audiences this sample includes two Web Applications which are equivalent from functional prospective, but developed in two different technologies:

1.  Node.JS

2.  Latest Microsoft ASP.NET 5.0 + DNX.Core 5.0 (will be renamed to ASP.NET Core 1.0 and <span id="OLE_LINK13" class="anchor"><span id="OLE_LINK14" class="anchor"><span id="OLE_LINK15" class="anchor"></span></span></span>.NET Core 1.0 soon).

Node.JS is very popular in the startup world and <span id="OLE_LINK6" class="anchor"></span>.NET Core 1.0 is becoming the new wave of technology evolution from Microsoft. The Azure Notification Hub client library for <span id="OLE_LINK5" class="anchor"></span>DNX.Core 5.0 (.NET Core 1.0) has not been provided by Microsoft yet. This makes this sample extremely valuable, because it demonstrates how to use native REST API and this approach can be applied for ANY technology stack.

Mobile application allows users to describe themselves in simple string format and receive push notification messages sent to a device from Azure Notification Hub. This sample contains simple native applications for three major platforms: Windows Mobile, iOS and Android.

Prerequisites

-   In order to complete this sample, you need to have an active Azure Subscription.

-   Azure Resource Manager Templates will be used to deploy an application. Detailed information on using ARM templates are available here: <https://azure.microsoft.com/en-us/documentation/articles/resource-group-authoring-templates/>.

<!-- -->

-   In order to build a Windows Phone and .Net Web App Visual Studio 2015 is required. Android Studio (<http://developer.android.com/develop/index.html>) is required to build Android Mobile application and Xcode (<https://developer.apple.com/support/xcode>.) is required to build iOS application.

<!-- -->

-   ASP.NET 5 and DNX execution environment are required to build ASP.NET 5 Web App. If you do not have it already installed, visit this guide: <http://docs.asp.net/en/latest/getting-started/installing-on-windows.html>

Use case scenario

We are considering two actors (Subscriber and Publisher) and three major use cases for push notifications. *Note: screenshots are done using Windows Phone, but Android and iOS versions are also available and you can find screens below in the section describing iOS and Android implementation details):*

-   *Subscriber*: Subscribes for notifications

<!-- -->

-   *Subscriber* downloads and installs the application built from the sample. On the first application start subscriber enters a tag to identify the device (such as John Smith). This tag is just a free text string which will be used by the *Publisher* to identify the user’s device.

<img src="media/image1.png" width="297" height="239" />

-   *Publisher*: Sends notification to selected user (device)

    -   *Publishe**r*** visits Azure Web App page and gets the list of registered devices (list of tags provided by *subscribers*):

<img src="media/image2.png" width="208" height="141" />

-   User selects one of the devices which is target for notification, enters message and presses a send button. Selected device will display the following push notification message:

    <img src="media/image3.png" width="214" height="165" />

<!-- -->

-   *Subscriber*: Receives notification and reads message

    -   *Subscriber* receives push notification message in standard format for their specific mobile platform and can read the text message delivered.

        <img src="media/image4.png" width="172" height="112" />

COMPONENTS AND IMPLEMENTATION DETAILS

<span id="_Toc446449749" class="anchor"></span>Notification hub configuration
Notification hub configuration consists of common part and platform dependent configuration. The platform dependent settings guide is available at the official quick start guides for every type of client platform. An official Microsoft Azure Notification Hub documentation and set of Get Started Guides is located here <https://azure.microsoft.com/en-us/documentation/services/notification-hubs/>. In addition, you can find a corresponding link to guide in every client application description later in the document. Here is a common part of these official guides:

-   Log on to the [Azure Portal](https://portal.azure.com/), and then click +NEW at the top of the screen.

-   Click on Web + Mobile, then Notification Hub, and then Quick Create.

<img src="media/image5.png" width="174" height="203" />

-   Make sure you specify a unique name in the Notification Hub field. Select your desired Region, Subscription and Resource Group (if you have one already).

-   If you already have a service bus namespace that you want to create the hub in, select it through the Select Existing option in the Namespace field. Otherwise, you can use the default name that will be created based on the hub name as long as the namespace name is available. Once ready, click Create.

<img src="media/image6.png" width="110" height="262" />

-   Once the namespace and notification hub is created, you will be taken to the respective portal page.

<img src="media/image7.png" width="429" height="282" />

-   Click on Settings and then Access Policies - take note of the two connection strings that are made available to you, as you will need them to handle push notifications later.

<img src="media/image8.png" width="517" height="214" />

User registration implementation

The system needs to have a list of registered devices; at the same time, we would like to keep this sample as simple as possible to demonstrate push notification technology. To do this, we will not introduce any database backend which will be common in more real life applications. We will place the user information into a tag which represents a registered device in the Azure Notification Hub. User information is a JSON object containing required information, some salt data to make a user with the same information have a unique tag. For this sample, user information will look like this: {"name":"*John Smith*","s":"54b9d84a32"} (assuming *Subscriber* entered John Smith during device registration).

The Notification Hub’s notification registration tag can contain only alphanumeric symbols, so we will encode JSON as a base64 string increasing a salt field size to be an 8 or more to avoid an ending ‘=’ symbols in the base64 string.

<span id="OLE_LINK8" class="anchor"><span id="OLE_LINK9" class="anchor"><span id="OLE_LINK10" class="anchor"><span id="OLE_LINK11" class="anchor"><span id="OLE_LINK12" class="anchor"></span></span></span></span></span>ASP.NET 5.0 Web Application

<span id="h.2282yjp90ufw" class="anchor"><span id="_Toc444384195" class="anchor"></span></span>Notes

<span id="h.w6v6wxwg81b5" class="anchor"><span id="_Toc444384196" class="anchor"></span></span>Please note that <span id="OLE_LINK7" class="anchor"></span>ASP.NET 5.0 (new name will be ASP.NET Core 1.0) is in the release candidate RC1 state at the moment the sample is being created. In addition, there is no official notification hub client working with DNX<span id="OLE_LINK16" class="anchor"><span id="OLE_LINK17" class="anchor"></span></span>,Core 5.0 (new name will be .NET Core 1.0). So a simple REST API client NotificationHubApiClient is implemented in this sample. It does have a minimum required notification hub client functionality, but limited error handling and does not covers all possible cases. Because of this it is strongly recommended to use an official client library in the production as soon as it will be available.

Please install ASP.NET 5 as well as DNX.Core 5.0 execution environment if you do not already have it installed. Refer to this guide: <http://docs.asp.net/en/latest/getting-started/installing-on-windows.html>. Please also download and install the latest Azure SDK from <https://go.microsoft.com/fwlink/?linkid=518003&clcid=0x409>

Creating a site

-   Create a new ASP.NET web application:

<img src="media/image9.png" width="432" height="298" />

-   Select an ASP.NET 5 web application template and turn off authentication and keep Host in the Cloud selected (please note that Visual Studio is still using the unchanged name for the technology – ASP.NET 5.0, so please don’t be confused).

<img src="media/image10.png" width="482" height="374" />

-   Setup an Azure deployment as a Web App:

<img src="media/image11.png" width="458" height="355" />

-   Add System.Net.Http and Newtonsoft.Json Nuget packages.

-   Add a new project folder Models for models and create a new model class UserInfo:

> using System;
>
> using Newtonsoft.Json;
>
> namespace PushMeNow.AspNet5.Models
>
> {
>
> public class UserInfo
>
> {
>
> \[JsonProperty("name")\]
>
> public string Name { get; set; }
>
> }
>
> <span id="h.ab6ewwshkj8l" class="anchor"></span>}<span id="h.dtjreq43wrx6" class="anchor"></span>

-   Add a model class Registration:

> using System;
>
> using System.Text;
>
> using Newtonsoft.Json;
>
> namespace PushMeNow.AspNet5.Models
>
> {
>
> enum RegistrationType
>
> {
>
> Windows,
>
> Apple,
>
> Gcm,
>
> }
>
> public class Registration
>
> {
>
> public Registration()
>
> { }
>
> public Registration(Registration other)
>
> {
>
> RegistrationType = other.RegistrationType;
>
> Tags = other.Tags;
>
> }
>
> \[JsonProperty("registration\_type")\]
>
> public string RegistrationType { get; set; }
>
> \[JsonProperty("tags")\]
>
> public string Tags { get; set; }
>
> public string Name
>
> {
>
> get
>
> {
>
> try
>
> {
>
> return
>
> JsonConvert.DeserializeObject&lt;UserInfo&gt;(Encoding.UTF8.GetString(Convert.FromBase64String(Tags)))
>
> .Name;
>
> }
>
> catch
>
> {
>
> return "Unknown";
>
> }
>
> }
>
> }
>
> }
>
> }
>
> <span id="h.wfkwhrsebo4p" class="anchor"></span>

-   Add a model class Message:

> using System;
>
> using System.Collections.Generic;
>
> using System.ComponentModel.DataAnnotations;
>
> using System.Linq;
>
> using System.Threading.Tasks;
>
> using Newtonsoft.Json;
>
> namespace PushMeNow.AspNet5.Models
>
> {
>
> public class Message : Registration
>
> {
>
> public Message()
>
> {
>
> }
>
> public Message(Registration registration)
>
> : base(registration)
>
> {
>
> }
>
> \[JsonProperty("text")\]
>
> \[Required\]
>
> public string Text { get; set; }
>
> }
>
> }

### 

-   Add a new controller PushController in the folder Controllers and modify it like this:

> <span id="h.pgnno7wzr2rx" class="anchor"></span>using System.Collections.Generic;
>
> <span id="h.rysdw42uvxx6" class="anchor"></span>using System.Threading.Tasks;
>
> <span id="h.k5myasxn65a1" class="anchor"></span>using Microsoft.AspNet.Mvc;
>
> <span id="h.xsw05m8zsu6l" class="anchor"></span>using PushMeNow.AspNet5.Models;
>
> <span id="h.hwk32lg5rdl1" class="anchor"></span>
>
> <span id="h.l7bef9am8xwa" class="anchor"></span>// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
>
> <span id="h.ip3sgx13qtr8" class="anchor"></span>
>
> <span id="h.ontdev7x4qf1" class="anchor"></span>namespace PushMeNow.AspNet5.Controllers
>
> <span id="h.y6hd74xobila" class="anchor"></span>{
>
> <span id="h.afm8aze1z342" class="anchor"></span> public class PushController : Controller
>
> <span id="h.fv9s66ukmkty" class="anchor"></span> {
>
> <span id="h.ws05ozzetjai" class="anchor"></span> public async Task&lt;IActionResult&gt; Index()
>
> <span id="h.3fsyq2cxg4r" class="anchor"></span> {
>
> <span id="h.b65mhar63447" class="anchor"></span> var registrations = await GetRegistrationsAsync();
>
> <span id="h.ay9vwahk84i1" class="anchor"></span> return View(registrations);
>
> <span id="h.f4fln59n3wbi" class="anchor"></span> }
>
> <span id="h.jdr3ftl6wcxu" class="anchor"></span>
>
> <span id="h.arqjx1bb40me" class="anchor"></span> \[HttpGet\]
>
> <span id="h.vxlogc8yhq9m" class="anchor"></span> public IActionResult Send(\[Bind("Tags,RegistrationType")\] Registration registration)
>
> <span id="h.hzs1zvvbzsd3" class="anchor"></span> {
>
> <span id="h.iy5nnlnvn17s" class="anchor"></span> return View(new Message(registration));
>
> <span id="h.gxrh4wnw6b0z" class="anchor"></span> }
>
> <span id="h.oo8nny7i7stt" class="anchor"></span>
>
> <span id="h.wdc9qu7dnilo" class="anchor"></span> \[HttpPost\]
>
> <span id="h.1v8pxm8p5859" class="anchor"></span> \[ValidateAntiForgeryToken\]
>
> <span id="h.et0dfh6thg39" class="anchor"></span> public async Task&lt;IActionResult&gt; Send(\[Bind("Tags,Text,RegistrationType")\] Message message)
>
> <span id="h.2voc0654gp90" class="anchor"></span> {
>
> <span id="h.fko5f2d51rnq" class="anchor"></span> if (ModelState.IsValid)
>
> <span id="h.kx8nx97l437x" class="anchor"></span> {
>
> <span id="h.olmxi2hjwxpa" class="anchor"></span> NotificationHubApiClient notificationHubApiClient = GetClient();
>
> <span id="h.thi3utrxxf81" class="anchor"></span> await notificationHubApiClient.SendNotificationAsync(message);
>
> <span id="h.oan7p7cu5447" class="anchor"></span> return RedirectToAction("Index");
>
> <span id="h.ujgbh2xv5le8" class="anchor"></span> }
>
> <span id="h.wvegmsvpgxo4" class="anchor"></span>
>
> <span id="h.7htiutbzvhds" class="anchor"></span> return View(message);
>
> <span id="h.c0zvwio12281" class="anchor"></span> }
>
> <span id="h.mpe5dm61gy6" class="anchor"></span>
>
> <span id="h.47rz42gbactr" class="anchor"></span> private async Task&lt;IEnumerable&lt;Registration&gt;&gt; GetRegistrationsAsync()
>
> <span id="h.fjl7ve487twt" class="anchor"></span> {
>
> <span id="h.bgju10cvqel1" class="anchor"></span> var notificationHubApiClient = GetClient();
>
> <span id="h.nbfjngsed6v4" class="anchor"></span> var registrationTags = await notificationHubApiClient.GetRegistrationsAsync();
>
> <span id="h.i6iq3txnla7i" class="anchor"></span> return registrationTags;
>
> <span id="h.yv4hw3fvo4we" class="anchor"></span> }
>
> <span id="h.uyjrdgsx30w6" class="anchor"></span>
>
> <span id="h.nzepoow21xcm" class="anchor"></span> private static NotificationHubApiClient GetClient()
>
> <span id="h.awa214d2aewy" class="anchor"></span> {
>
> <span id="h.1nq23o876wd0" class="anchor"></span> string hubName = "&lt;HUB-NAME&gt;";
>
> <span id="h.4cex9sulbu9o" class="anchor"></span> string fullConnectionString = "&lt;HUB-FULL-CONNECTION\_STRING&gt;";
>
> <span id="h.mwak4pazkogs" class="anchor"></span> return new NotificationHubApiClient(hubName, fullConnectionString);
>
> <span id="h.kp8wmbhuhlbx" class="anchor"></span> }
>
> <span id="h.k2s9wipqsj9u" class="anchor"></span> }
>
> <span id="h.ng7cqu3ddwrh" class="anchor"></span>}

<span id="h.yln8kzrqfhbc" class="anchor"></span>

-   Add a new Views sub-folder Push and add a pair of views for registration selection and push message sending.

    -   Index.cshtml like this:

> @using System.Threading.Tasks
>
> @using Newtonsoft.Json
>
> @using PushMeNow.AspNet5.Models
>
> @model IEnumerable&lt;Registration&gt;
>
> @{
>
> ViewData\["Title"\] = "Push Page";
>
> }
>
> &lt;div class="row"&gt;
>
> &lt;div class="col-md-12"&gt;
>
> &lt;h2&gt;Registrations&lt;/h2&gt;
>
> &lt;ul&gt;
>
> @foreach (var registration in Model)
>
> {
>
> &lt;li&gt;&lt;a asp-controller="Push" asp-action="Send" asp-route-tags="@registration.Tags" asp-route-registrationType="@registration.RegistrationType"&gt;@registration.Name&lt;/a&gt;&lt;/li&gt;
>
> }
>
> &lt;/ul&gt;
>
> &lt;/div&gt;
>
> <span id="h.yvf7ojmrstes" class="anchor"></span>&lt;/div&gt;

-   Send.cshtml like this:

> <span id="h.z86kyql4peye" class="anchor"></span>@using System.Threading.Tasks
>
> <span id="h.cftt9ctol969" class="anchor"></span>@using PushMeNow.AspNet5.Models
>
> <span id="h.tqcpt8ki5u4s" class="anchor"></span>@model Message
>
> <span id="h.8osz4fhvhir5" class="anchor"></span>
>
> <span id="h.efc02oat369c" class="anchor"></span>@{
>
> <span id="h.bwmy397im7p2" class="anchor"></span> ViewData\["Title"\] = "Send message to " + Model.Name;
>
> <span id="h.tkl9kub7p9f6" class="anchor"></span>}
>
> <span id="h.32lhmmqccm13" class="anchor"></span>
>
> <span id="h.kwf2bxm5v06u" class="anchor"></span>&lt;form asp-action="Send"&gt;
>
> <span id="h.aolkag2eotl2" class="anchor"></span> &lt;div class="form-horizontal"&gt;
>
> <span id="h.xb4d7fojgjyb" class="anchor"></span> &lt;h4&gt;@ViewData\["Title"\]&lt;/h4&gt;
>
> <span id="h.dhj050klcq8p" class="anchor"></span> &lt;hr /&gt;
>
> <span id="h.d51hxgdnpti6" class="anchor"></span> &lt;div asp-validation-summary="ValidationSummary.ModelOnly" class="text-danger" /&gt;
>
> <span id="h.egydlh67o9xr" class="anchor"></span> &lt;input type="hidden" asp-for="Tags" /&gt;
>
> <span id="h.rv4s2aiu86b0" class="anchor"></span> &lt;input type="hidden" asp-for="RegistrationType" /&gt;
>
> <span id="h.p5hpetgxu3j2" class="anchor"></span> &lt;div class="form-group"&gt;
>
> <span id="h.mala559rrolz" class="anchor"></span> &lt;div class="col-md-12"&gt;
>
> <span id="h.jemorjypijve" class="anchor"></span> &lt;input asp-for="Text" class="form-control" /&gt;
>
> <span id="h.gree65x0p2rf" class="anchor"></span> &lt;span asp-validation-for="Text" class="text-danger" /&gt;
>
> <span id="h.vj0szinrvbyd" class="anchor"></span> &lt;/div&gt;
>
> <span id="h.9a668cxb6v2q" class="anchor"></span> &lt;/div&gt;
>
> <span id="h.210kel3w1e4" class="anchor"></span> &lt;div class="form-group"&gt;
>
> <span id="h.p68yaw5lw2d5" class="anchor"></span> &lt;div class="col-md-12"&gt;
>
> <span id="h.qaj1z66je8l7" class="anchor"></span> &lt;input type="submit" value="Send" class="btn btn-default" /&gt;
>
> <span id="h.5pb68iifw81y" class="anchor"></span> &lt;/div&gt;
>
> <span id="h.49dxmq5qdy56" class="anchor"></span> &lt;/div&gt;
>
> <span id="h.cz7gq340gx7f" class="anchor"></span> &lt;/div&gt;
>
> <span id="h.gwxi0ax0hbr1" class="anchor"></span>&lt;/form&gt;
>
> <span id="h.zev4h8t8d9ss" class="anchor"></span>
>
> <span id="h.u5wh1ctw75ru" class="anchor"></span>&lt;div&gt;
>
> <span id="h.btts2ik9vp4j" class="anchor"></span> &lt;a asp-action="Index"&gt;Back to List&lt;/a&gt;
>
> <span id="h.nuqcabxmxv38" class="anchor"></span>&lt;/div&gt;

### 

-   Edit a Shared/\_Layout.cshtml

    -   Add a line to nav bar in the to

> <span id="h.f11hk6f0gscr" class="anchor"></span>&lt;li&gt;&lt;a asp-controller="Push" asp-action="Index"&gt;Push&lt;/a&gt;&lt;/li&gt;

-   Add script references in order to enable validation

> &lt;script src="~/lib/jquery-validation/dist/jquery.validate.js"&gt;&lt;/script&gt;
>
> <span id="h.mejv1041zk24" class="anchor"></span>&lt;script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js"&gt;&lt;/script&gt;

-   Add following lines for required additional dependencies in the section "dependencies" of project.json:

> "System.Net.Http": "4.0.1-beta-23516",
>
> "Newtonsoft.Json": "8.0.2"

-   Replace &lt;HUB-NAME&gt; and &lt;HUB-FULL-CONNECTION\_STRING&gt; with your settings.

-   Add a NotificationHub client. An official client is not ready at the moment so a class NotificationHubApiClient is used as a workaround. See sample sources to get more details about this class.

-   Build and run a web application

<img src="media/image12.png" width="344" height="258" />

Note: Complete sources of the project are in the PushMeNow.AspNet5 sub-folder of sample sources.

<span id="h.mcc0t4r0ie03" class="anchor"><span id="_Toc444384197" class="anchor"></span></span>Node.JS Web App

Create a Node.JS app and connect to the Notification Hub
--------------------------------------------------------

-   First step is a creation Node.JS application. Express.JS framework v4.13.1 is used for this sample.

    Note: The detailed step by step guide is available here: [*http://expressjs.com/en/starter/generator.html*](http://expressjs.com/en/starter/generator.html).

-   Next step is installation Azure npm package. It is done using the following command:

> <span id="h.kwnmv3v9eh0l" class="anchor"></span>npm install azure --save

-   Add the following string to the top of the app.js file of the application:

> <span id="h.8qnmn8iqj0q9" class="anchor"></span>var azure = require('azure');

-   In order to connect to the Notification Hub, the NotificationHubService object is used. It is created in app.js file:

> <span id="h.3hs8pdy0dk2w" class="anchor"></span>var notificationHubService = azure.createNotificationHubService('hubname','connectionstring');

### Create pages to display the list of registrations and send notification to different platforms

-   Create a push.js file in the routes folder and write the following code:

> var express = require('express');
>
> var router = express.Router();
>
> <span id="h.acy8iztkzzd0" class="anchor"></span>module.exports = router;

-   Add the following strings to the app.js file to configure routes and set up handlers:

> var push = require('./routes/push');
>
> <span id="h.eoyp2uvaes5" class="anchor"></span>app.use('/push', push);

-   Add the methods for displaying registration list in push.js:

> router.get('/', function(req, res, next) {
>
> // getting the registrations list
>
> notificationHubService.listRegistrations(null, function (error, response) {
>
> if (!error) {
>
> var regisrations = response.map(function (item) {
>
> return {
>
> name: getNameByRegistrationTags(item.Tags),
>
> tags: item.Tags,
>
> // identify registration type
>
> registrationType: item.\_.ContentRootElement.replace('RegistrationDescription', '')
>
> };
>
> });
>
> res.render('push', { title: 'Registrations', regisrations: regisrations });
>
> } else {
>
> throw new Error(error);
>
> }
>
> });
>
> <span id="h.3yusupq9k26s" class="anchor"></span>});

-   Add function to get devices by Notification Hub’s registration tags:

> function getNameByRegistrationTags(tags) {
>
> try {
>
> return JSON.parse(new Buffer(tags, 'base64').toString('utf8')).name;
>
> }
>
> catch (e) {}
>
> return tags;
>
> <span id="h.vrcho137vc5p" class="anchor"></span>}

-   Add view to display the registration list. All views use the Jade engine. Create a push.jade file in views folder and add the following code to it:

> extends layout
>
> block content
>
> h2 \#{title}
>
> div
>
> div
>
> - each registration in regisrations
>
> li
>
> <span id="h.9x8ggw6rtzpx" class="anchor"></span> a(href='/push/send?tags=\#{registration.tags}&registrationType=\#{registration.registrationType}') \#{registration.name}

-   Open http://localhost:&lt;port&gt;/push and check that you already can see the list of registrations.

-   Add the new route to send notification to a user in the push.js:

> // GET view for sending notification
>
> router.get('/send/', function(req, res, next) {
>
> var tags = req.query.tags;
>
> var name = getNameByRegistrationTags(tags);
>
> res.render('send', { title: 'Send message', name: name, tags: tags, registrationType: req.query.registrationType, message: '' });
>
> <span id="h.bysmveqfv7t4" class="anchor"></span>});

-   Create view sand.jade for displaying message control with the following code that extends layout

> block content
>
> h4 Send message to \#{name}
>
> form(role='form'
>
> method='post')
>
> input(type='hidden'
>
> name='tags'
>
> value='\#{tags}')
>
> input(type='hidden'
>
> name='registrationType'
>
> value='\#{registrationType}')
>
> input(class='form-control'
>
> type='text'
>
> name='text'
>
> id='text'
>
> value='\#{message}')
>
> input(type='submit'
>
> value='Send'
>
> style='margin: 15px 0 0 0')
>
> div
>
> <span id="h.3jgpjqhrmmff" class="anchor"></span> a(href='/push') Back to list:

-   Add handler for sending message request and place it in the push.js file:

> // POST send notification to use
>
> router.post('/send/', function(req, res, next) {
>
> var message = req.body.text;
>
> var tags = req.query.tags;
>
> var name = getNameByRegistrationTags(tags);
>
> var registrationType = req.query.registrationType;
>
> var payload;
>
> // callback method after finished sending notification
>
> var onSendingCallback = function(error) {
>
> if (!error) {
>
> res.redirect('/push');
>
> } else {
>
> res.render('send', { title: 'Send message', name: name, tags: tags, registrationType: req.query.registrationType, message: message });
>
> }
>
> };
>
> switch (registrationType) {
>
> // sending notification to the Windows platform
>
> case 'Windows':
>
> payload = '&lt;toast&gt;&lt;visual&gt;&lt;binding template="ToastText01"&gt;&lt;text id="1"&gt;' + message + '&lt;/text&gt;&lt;/binding&gt;&lt;/visual&gt;&lt;/toast&gt;';
>
> notificationHubService.wns.send(tags, payload, 'wns/toast', onSendingCallback);
>
> break;
>
> // sending notification to the Android platform
>
> case 'Gcm':
>
> payload = {
>
> data: {
>
> msg: message
>
> }
>
> };
>
> notificationHubService.gcm.send(tags, payload, onSendingCallback);
>
> break;
>
> // sending notification to the Apple platform
>
> case 'Apple':
>
> payload = {
>
> alert: message
>
> };
>
> notificationHubService.apns.send(null, payload, onSendingCallback);
>
> break;
>
> default:
>
> throw new Error('Unknowwn registration type');
>
> }
>
> <span id="h.4hhwpbombog4" class="anchor"></span>});

<span id="_Toc444384200" class="anchor"></span>Creating Azure Web App and configuring the continuous deployment for Node.JS Web App

Please take the following steps below:

-   Sign in Azure portal <https://portal.azure.com/>

-   Click on the NEW button then click on the Web + Mobile and then click on the Web App.

-   Fill necessary fields and click on Create.

-   Select this web app and click on All Settings and then click on the Continues Deployment and configure settings by Local Git Repository

<img src="media/image13.png" width="446" height="220" />

-   Click on the “Setup Connection” and create deployment account

    <img src="media/image14.png" width="500" height="237" />

### Publish Node.JS application to Azure using Git

-   Use the command line change directory to the project directory and enter the following command to initialize the Git repository:

> <span id="h.i5tu7d5qm1b7" class="anchor"></span>git init

-   Add files to the repository:

> git add .
>
> <span id="h.3s2oei2i0hl6" class="anchor"></span>git commit -m "initial commit"

-   To get URL for the remote repository, open all settings and click on the Properties and you can find “Git URL”

<img src="media/image15.png" width="299" height="299" />

-   Add a Git remote for pushing updates to the web app that you created previously, by using the following command:

> <span id="h.7xl08rpstqwz" class="anchor"></span>git remote add azure \[URL for remote repository\]

-   Push your changes to Azure by following command:

> <span id="h.5q2kuu4j5eqp" class="anchor"></span>git push azure master

<span id="h.cjbbl05c75w" class="anchor"><span id="_Toc444384202" class="anchor"></span></span>Windows Phone Client

There is a great starter guide available here: [*https://azure.microsoft.com/en-us/documentation/articles/notification-hubs-windows-store-dotnet-get-started/*](https://azure.microsoft.com/en-us/documentation/articles/notification-hubs-windows-store-dotnet-get-started/). We will follow this guide in the sample.

-   Create a new Windows Phone project. For Visual Studio 2015 use a template shown on the image below.

<img src="media/image16.png" width="368" height="254" />

-   Register your application in the Store and tune notification hub settings as it is described in the article.

-   Add *WindowsAzure.Messaging.Managed* and *Newtonsoft.Json* Nuget packages.

-   Create a new folder Models and put a new class *UserInfo* like this:

> using Newtonsoft.Json;
>
> namespace PushMeNow.Client.WindowsPhone.Models
>
> {
>
> public class UserInfo
>
> {
>
> private string \_name;
>
> private string \_salt;
>
> \[JsonProperty("name")\]
>
> public string Name
>
> {
>
> get { return \_name; }
>
> set
>
> {
>
> \_name = value;
>
> int saltLength = 7;
>
> do
>
> {
>
> ++saltLength;
>
> \_salt = Guid.NewGuid().ToString("N").Substring(0, saltLength);
>
> } while (AsTag().Contains("="));
>
> }
>
> }
>
> \[JsonProperty("s")\]
>
> public string Salt
>
> {
>
> get { return \_salt; }
>
> }
>
> public string AsTag()
>
> {
>
> return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
>
> }
>
> }
>
> <span id="h.pcixu8ntk1oh" class="anchor"></span>}

Note. User information is stored into the tag content as a json containing required information. Some salt data is added to make a tag with the same user information unique. The notification registration tag should contain alphanumeric symbols only. So the json is encoded as a base64 string increasing a salt field size to be 8 or more to avoid an ending ‘=’ symbols in the base64 string.

-   Open a MainPage.xml and add a stackpanel instead of grid. Put a minimum of required UI elements like these:

> <span id="h.gkemb6ay0kc2" class="anchor"></span>&lt;StackPanel Margin="20"&gt;
>
> <span id="h.9go81xm49n7o" class="anchor"></span> &lt;TextBlock HorizontalAlignment="Left"
>
> <span id="h.hwbmr15b8nps" class="anchor"></span> TextWrapping="Wrap"
>
> <span id="h.16hyf2b78a3e" class="anchor"></span> Text="Register your device for Push Me Now"
>
> <span id="h.cpb928k7n3br" class="anchor"></span> VerticalAlignment="Top"
>
> <span id="h.j0kyqtv1w79q" class="anchor"></span> Style="{StaticResource HeaderTextBlockStyle}" /&gt;
>
> <span id="h.d7luxrk9zooi" class="anchor"></span> &lt;TextBlock Margin="0,50,0,0"
>
> <span id="h.n1y1z6jbt64" class="anchor"></span> HorizontalAlignment="Left"
>
> <span id="h.zg83yriytt" class="anchor"></span> TextWrapping="Wrap"
>
> <span id="h.c43c03bk86ob" class="anchor"></span> Text="User Name"
>
> <span id="h.y5mpsbxanjty" class="anchor"></span> VerticalAlignment="Top"
>
> <span id="h.5gihc0frmoct" class="anchor"></span> Style="{StaticResource BaseTextBlockStyle}" /&gt;
>
> <span id="h.br4hx7gghii6" class="anchor"></span> &lt;TextBox x:Name="textBoxName"
>
> <span id="h.c9h2rx9do2o" class="anchor"></span> HorizontalAlignment="Stretch"
>
> <span id="h.8ne0rfjnd62h" class="anchor"></span> TextWrapping="Wrap"
>
> <span id="h.z6lyidetjlc4" class="anchor"></span> Text="" VerticalAlignment="Top" /&gt;
>
> <span id="h.dlwz3mgvwcvk" class="anchor"></span> &lt;Button Margin="50"
>
> <span id="h.blie7hawwa7p" class="anchor"></span> x:Name="buttonRegister"
>
> <span id="h.bvt0enh7n2kr" class="anchor"></span> Content="Register"
>
> <span id="h.mgjo5095id03" class="anchor"></span> HorizontalAlignment="Center"
>
> <span id="h.54fbmqz4ttf9" class="anchor"></span> VerticalAlignment="Bottom"
>
> <span id="h.cvyn8gcrod0a" class="anchor"></span> Click="buttonRegister\_Click" /&gt;
>
> <span id="h.638f3dujwlhi" class="anchor"></span>&lt;/StackPanel&gt;

-   <span id="h.dltoqd6jwqpd" class="anchor"></span>Open a MainPage.xaml.cs and a set of usings:

> <span id="h.oreftmrqyaep" class="anchor"></span>using Microsoft.WindowsAzure.Messaging;
>
> using PushMeNow.Client.WindowsPhone.Models;
>
> using Windows.Networking.PushNotifications;
>
> <span id="h.c6kajsbw0i3" class="anchor"></span>using Windows.UI.Popups;

-   Create an event handler for a registration button:

> <span id="h.stcodg31dw87" class="anchor"></span>private async void buttonRegister\_Click(object sender, RoutedEventArgs e)
>
> <span id="h.2sphllagg986" class="anchor"></span>{
>
> <span id="h.k4yxbsjhee0z" class="anchor"></span> string message;
>
> <span id="h.jsgdp7xf9tgb" class="anchor"></span> try
>
> <span id="h.5qvucioxx9xt" class="anchor"></span> {
>
> <span id="h.ebxate8tvpsk" class="anchor"></span> IsEnabled = false;
>
> <span id="h.krd318bu241f" class="anchor"></span> var userInfo = new UserInfo { Name = textBoxName.Text.Trim() };
>
> <span id="h.pzdz022priyj" class="anchor"></span> if (string.IsNullOrWhiteSpace(userInfo.Name))
>
> <span id="h.sen57rn36v9s" class="anchor"></span> {
>
> <span id="h.z23w6iwt08vl" class="anchor"></span> throw new Exception("Please enter your name");
>
> <span id="h.7x6cbzvs3ff6" class="anchor"></span> }
>
> <span id="h.ouolob24llaj" class="anchor"></span>
>
> <span id="h.jwpbp8u7pk0k" class="anchor"></span> string hubName = "&lt;HUB-NAME&gt;";
>
> <span id="h.z3fy8vqry0sv" class="anchor"></span> string listenConnectionString = "&lt;HUB-LISTEN-CONNECTION\_STRING&gt;";
>
> <span id="h.yib6xjqf5ppr" class="anchor"></span>
>
> <span id="h.tu5xnpc1pfe1" class="anchor"></span> var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
>
> <span id="h.c9ct8uk2r08l" class="anchor"></span> var hub = new NotificationHub(hubName, listenConnectionString);
>
> <span id="h.16viu5m1nry5" class="anchor"></span> string tag = userInfo.AsTag();
>
> <span id="h.1a3u3576nvkn" class="anchor"></span> var result = await hub.RegisterNativeAsync(channel.Uri, new\[\] { tag });
>
> <span id="h.lqxlydww4ain" class="anchor"></span>
>
> <span id="h.dgupuq2xauaz" class="anchor"></span> if (result.RegistrationId == null)
>
> <span id="h.dz0g3b5nfbok" class="anchor"></span> {
>
> <span id="h.wzcdxdhfa9u2" class="anchor"></span> throw new Exception("No registration id");
>
> <span id="h.sdgtza35vvu2" class="anchor"></span> }
>
> <span id="h.ti6p0hei53k0" class="anchor"></span>
>
> <span id="h.d7mv4oy7foya" class="anchor"></span> message = "Registration successful: " + result.RegistrationId;
>
> <span id="h.86ysagxnbekk" class="anchor"></span> }
>
> <span id="h.g4ndzv3pi6yh" class="anchor"></span> catch (Exception exception)
>
> <span id="h.skwlrpygx8ch" class="anchor"></span> {
>
> <span id="h.ooe81qdzbyat" class="anchor"></span> message = "Registration failed: " + exception.Message;
>
> <span id="h.6sz0znxj98ig" class="anchor"></span> }
>
> <span id="h.fza27udewm2m" class="anchor"></span> finally
>
> <span id="h.fqu3h3alp7fy" class="anchor"></span> {
>
> <span id="h.k980zlimowwy" class="anchor"></span> IsEnabled = true;
>
> <span id="h.84m79osrsmbo" class="anchor"></span> }
>
> <span id="h.92rzqonwfxcp" class="anchor"></span>
>
> <span id="h.9m0bbouvauug" class="anchor"></span> var dialog = new MessageDialog(message);
>
> <span id="h.f10y3u83alx0" class="anchor"></span> dialog.Commands.Add(new UICommand("OK"));
>
> <span id="h.lstws3oghj6a" class="anchor"></span> await dialog.ShowAsync();
>
> <span id="h.r77olj7fg3dp" class="anchor"></span>}<span id="h.fnfboe7mg15l" class="anchor"></span>

-   Replace &lt;HUB-NAME&gt; and &lt;HUB-LISTEN-CONNECTION\_STRING&gt; with settings for your notification hub.

-   Double check Toast capable is enabled in the appmanifest.

-   Build and run an application on the Device or the Windows Phone Emulator.

<img src="media/image17.png" width="402" height="350" />

-   Now you can close your application. Open a Web Application and send a push message to just the registered user.

-   You will receive a push sent.

<img src="media/image4.png" width="145" height="265" />

<span id="_Toc444384203" class="anchor"></span>Android Client

-   Official Notification Hub documentation for Android platform is available here: <https://azure.microsoft.com/en-us/documentation/articles/notification-hubs-android-get-started/>

-   Class *UserInfo* handles device registration data and very similar to the one described in the Windows Phone section. Device registration is performed in a body of AsyncTask:

> ***private void** registerPushNotification() {*
>
> ***private final** String SENDER\_ID = **"&lt;Your project number&gt;"**;*
>
> ***private final** String HubName = **"&lt;Your hub name&gt;"**;*
>
> ***private final** String HubListenConnectionString = **"&lt;Your default listenconnection sctring&gt;"**;*
>
> ***final** UserInfo userInfo = **new** UserInfo();*
>
> *userInfo.setName(\_nameEditText.getText().toString());*
>
> ***new** AsyncTask() {*
>
> *@Override*
>
> ***protected** Object doInBackground(Object... params) {*
>
> ***try** {*
>
> *GoogleCloudMessaging gcm = GoogleCloudMessaging.getInstance(MainActivity.**this**);*
>
> *String regid = gcm.register(SENDER\_ID);*
>
> *NotificationHub hub = **new** NotificationHub(HubName, HubListenConnectionString, MainActivity.**this**);*
>
> *String registrationId = hub.register(regid, userInfo.getAsTag()).getRegistrationId();*
>
> *ToastNotify(**"Registered Successfully - RegId : "** + registrationId);*
>
> *} **catch** (Exception e) {*
>
> *ToastNotify(**"Registration Exception Message - "** + e.getMessage());*
>
> ***return** e;*
>
> *}*
>
> ***return null**;*
>
> *}*
>
> *}.execute(**null**, **null**, **null**);*
>
> *}*

<img src="media/image18.png" width="387" height="316" />

<span id="h.iilzflep121l" class="anchor"><span id="_Toc444384204" class="anchor"></span></span>iOS Client

Official Notification Hub documentation for Android platform is available here: <https://azure.microsoft.com/en-us/documentation/articles/notification-hubs-ios-get-started/>

Class *UserInfo* handles device registration data and is very similar to the one described in the Windows Phone section. Device registration is performed in class *AppDelegate*, because deviceToken is available only in the method *application:didRegisterForRemoteNotificationsWithDeviceToken:* of class *AppDelegate*

-   Register to receive remote notifications via Apple Push Notification service.

> -(void) registerPushNotificationForUser:(UserInfo\*) userInfo {
>
> self.userInfo = userInfo;
>
> UIUserNotificationSettings\* notificationSettings = \[UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert | UIUserNotificationTypeBadge | UIUserNotificationTypeSound categories:nil\];
>
> \[\[UIApplication sharedApplication\] registerUserNotificationSettings:notificationSettings\];
>
> \[\[UIApplication sharedApplication\] registerForRemoteNotifications\];
>
> }

-   This code connects to the notification hub. Then it sends the device token to the notification hub to register for notifications:

> - (void)application:(UIApplication \*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData \*) deviceToken {
>
> static NSString\* hubName = @"&lt;Your hub name&gt;";
>
> static NSString\* listenConnectionString = @""&lt;Your default listenconnection sctring&gt;";
>
> SBNotificationHub\* hub = \[\[SBNotificationHub alloc\] initWithConnectionString:listenConnectionString
>
> notificationHubPath:hubName\];
>
> NSSet\* tags = nil;
>
> if (self.userInfo) {
>
> tags = \[NSSet setWithObjects:self.userInfo.asTag, nil\];
>
> }
>
> \[hub registerNativeWithDeviceToken:deviceToken tags:tags completion:^(NSError\* error) {
>
> if (error != nil) {
>
> NSLog(@"Error registering for notifications: %@", error);
>
> }
>
> else {
>
> \[self MessageBox:@"Registration Status" message:@"Registered"\];
>
> }
>
> }\];
>
> }

<img src="media/image19.png" width="436" height="298" />

<img src="media/image20.jpeg" width="209" height="313" /><img src="media/image21.jpeg" width="205" height="308" />

SUMMARY

In the article we described how to get started with Azure Notification Hub to build your own push notification solution using Node.JS or .NET Core 1.0 for Android, iOS and Windows phone mobile applications

A startup can use this sample as a starting point and customize it into a specific application to match their business model and technical needs.
