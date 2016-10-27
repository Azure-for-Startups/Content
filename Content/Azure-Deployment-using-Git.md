# Using GIT and GIT Workflows to Add Azure Web Apps

##Abstract

This document provides an overview of available recommended documentation regarding publishing Web applications on Azure using GIT workflow.

##Using GIT and GIT Workflows to Add Azure Web Apps

It is very important to establish integration between production, staging environments and source control system for a smooth and automated publishing process. Many startups are using GIT and GIT based systems (like GitHub, BitBucket and many others) as a source control system in their development process and lifecycle. Below is a brief review of available documentation which can be a good starting point for startups hosting their web applications on Azure and using GIT based source control systems.

We would like to start on the how to style guide “Continuous deployment using GIT in Azure App Service” which is available here https://azure.microsoft.com/en-us/documentation/articles/web-sites-publish-source-control/. This is article describes deployment best practices using the new Azure portal. It covers in detail the following topics:
- Pushing local files to Azure (Local Git)
- Deploying files from a repository web site like BitBucket, CodePlex, Dropbox, GitHub, or Mercurial
- How to deploy a Visual Studio solution from BitBucket, CodePlex, Dropbox, GitHub, or Mercurial
- Providing quick start references for supported source controls for deployment

Azure as a platform, allows deployment for both compiled binaries as well as source files with Azure side compilation. 

There are also articles describing in detail GIT deployment set up for the most commonly used web platforms, such as Node.js PHP, Asp.Net 5. You can find these guides here:
- “Create a Node.js web app in Azure App Service”: https://azure.microsoft.com/en-us/documentation/articles/web-sites-nodejs-develop-deploy-mac/
- “Create a PHP-MySQL web app in Azure App Service and deploy using GIT” https://azure.microsoft.com/en-us/documentation/articles/web-sites-php-mysql-deploy-use-git/
- Asp.Net 5 “Publishing to an Azure Web App with Continuous Deployment” http://docs.asp.net/en/latest/publishing/azure-continuous-deployment.html

Resource explorer is a great new tool to enable continuous deployment and perform many other deployment specific tasks. How to use resource explorer to enable continuous deployment is described at the Kudu wiki page “Setting up continuous deployment using Resource Explorer”: https://github.com/projectkudu/kudu/wiki/Continuous-deployment. 

GitHub also provides a guide “Automating code deployment with GitHub and Azure” https://github.com/blog/2056-automating-code-deployment-with-github-and-azure
