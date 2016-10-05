#How to Guide - Azure migration scenario
##Web applications on Azure using Node.js, PHP, and Python

November 2015

###OVERVIEW
This article outlines the most relevant resources on how to build and deploy Web applications based on non-Microsoft technologies on Microsoft Azure. The focus of the article is to serve as a starting point if you’re looking to learn how to deploy your application on Azure (rather than how to build it from scratch) and so is especially useful if you are looking to migrate applications. The following technologies are covered: Node.js, PHP, and Python.  

This article is also available in PDF format [here] (media/PDF-files/Node.js PHP and Python.pdf).

If you have any feedback on the documentation please log the feedback via an [issue] (https://github.com/XynergiesLLC/Azure-IoT-demo/issues).


###App Deployment General Info. 
A great starting point is the conceptual overview of deployment to Microsoft Azure (language agnostic) that can be found here - <https://azure.microsoft.com/en-us/documentation/articles/web-sites-deploy/>

###Node.js

-	A video tutorial is available on Channel9 and describes a high level step-by-step guide on how to build and deploy Node.js application https://channel9.msdn.com/Blogs/Windows-Azure/Create-NodejsAzure-website-deploy-from-GitHub 
-	A video tutorial for building your first Node.js app from scratch, all the way to deploying it on Azure: https://channel9.msdn.com/Blogs/raw-tech/Nodejs-on-Azure 
-	Discover how to use Web.deploy technology for deploying Node.js applications: https://github.com/Microsoft/nodejstools/wiki/Publish-to-Azure-Website-using-Web-Deploy 
-	Learn more about the Git based publishing of Node.js application along with tips and tweaks of applications to support Git publishing: https://azure.microsoft.com/en-us/documentation/articles/websites-nodejs-develop-deploy-mac/#publish-your-application 
-	After publishing a Node.js application you will need to debug and monitor the application. This article provides great information about how to debug and troubleshoot Node.js apps on Azure: http://blogs.msdn.com/b/azureossds/archive/2015/08/19/detecting-memory-leak-in-node-js-webapps-at-azure.aspx 


###PHP

-	PHP web application can be deployed using two standard ways: FTP based publishing and deployment directly from the source control (Git).   
-	The following article is the step-by-step guide about how to deploy PHP application from the Git source control: https://azure.microsoft.com/en-us/documentation/articles/web-sites-php-mysqldeploy-use-git/ 
 
-	This tutorial shows how to create a PHP-MySQL web app and how to deploy it using FTP: https://azure.microsoft.com/en-us/documentation/articles/web-sites-php-mysql-deploy-use-ftp/ 


###Python

-	This tutorial describes options for authoring and configuring a basic Web Server Gateway Interface (WSGI) compliant Python application on Azure App Service Web Apps https://azure.microsoft.com/enus/documentation/articles/web-sites-python-configure/ 
-	In this tutorial, Python Tools for Visual Studio are used to create a simple polls web app using one of the PTVS sample templates. https://azure.microsoft.com/en-us/documentation/articles/web-sitespython-ptvs-django-mysql/ 

