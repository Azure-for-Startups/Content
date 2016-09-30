**\
**Web applications on Azure using Node.js, PHP, and Python

November 2015

OVERVIEW

This article provides a brief overview of how to build and deploy Web
applications based on non-Microsoft technologies on Microsoft Azure.
This is not a step-by-step how to because there is a huge amount of
information on these topics for the variety of platforms, however, it
can be considered as a starting point and collection of the live links
that have comprehensive info. The focus of the article is more about how
to deploy your application in Azure, rather than how to build it from
scratch, so it can be useful if you are looking for application
migration.

The following technologies are covered: Node.js, PHP, and Python.

General information about the deployment of the applications.

The conceptual overview of the deployment to Microsoft Azure and
considered different approaches without any language relates specific
details.
<https://azure.microsoft.com/en-us/documentation/articles/web-sites-deploy/>

Node.js

-   A video tutorial is available on ***Channel9*** and describes a high
    level step-by-step guide on how to build and deploy Node.js
    application

-   Discover how to use Web.deploy technology for deploying Node.js
    applications:
    <https://github.com/Microsoft/nodejstools/wiki/Publish-to-Azure-Website-using-Web-Deploy>

-   Learn more about the Git based publishing of Node.js application
    along with tips and tweaks of applications to support Git
    publishing:
    <https://azure.microsoft.com/en-us/documentation/articles/web-sites-nodejs-develop-deploy-mac/#publish-your-application>

-   After publishing a Node.js application you will need to debug and
    monitor the application. This article provides great information
    about how to debug and troubleshoot Node.js apps on Azure:
    <http://blogs.msdn.com/b/azureossds/archive/2015/08/19/detecting-memory-leak-in-node-js-web-apps-at-azure.aspx>

PHP

-   PHP web application can be deployed using two standard ways: FTP
    based publishing and deployment directly from the source
    control (Git).

-   The following article is the step-by-step guide about how to deploy
    PHP application from the Git source control:
    <https://azure.microsoft.com/en-us/documentation/articles/web-sites-php-mysql-deploy-use-git/>

-   This tutorial shows how to create a PHP-MySQL web app and how to
    deploy it using FTP:
    <https://azure.microsoft.com/en-us/documentation/articles/web-sites-php-mysql-deploy-use-ftp/>

Python

-   This tutorial describes options for authoring and configuring a
    basic Web Server Gateway Interface (WSGI) compliant Python
    application on Azure App Service Web Apps
    <https://azure.microsoft.com/en-us/documentation/articles/web-sites-python-configure/>

-   In this tutorial, Python Tools for Visual Studio are used to create
    a simple polls web app using one of the PTVS sample templates.
    <https://azure.microsoft.com/en-us/documentation/articles/web-sites-python-ptvs-django-mysql/>
