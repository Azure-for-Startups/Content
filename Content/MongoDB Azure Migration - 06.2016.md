**\
**NoSQL databases and Microsoft Azure

June 2016

OVERVIEW

This article provides a brief overview of NoSQL databases on a Microsoft
Azure Cloud environment. Migration of the NoSQL databases from AWS or
another cloud provider to Microsoft Azure generally involves two steps:
infrastructure migration (deployment of the DB engine) and data
migration.**\
**

DEPLOYMENT

There are two most common NoSQL solutions to be deployed in Microsoft
Azure: Microsoft DocumentDB and MongoDB.

-   DocumentDB is a true schema-free NoSQL document database service
    provided by Microsoft in Azure Cloud. Here is the starting point of
    the documentation:
    <https://azure.microsoft.com/en-us/documentation/articles/documentdb-introduction/>

-   MongoDB is a well-known open source NoSQL database.
    <https://docs.mongodb.org/manual/?_ga=1.34680609.1023724.1446806197>

The Azure Cloud Computing stack provides the following levels: Software
as a Service (SaaS), Platform as a Service (PaaS) and Infrastructure as
a Service (IaaS). The table below shows possible deployments of MongoDB
and DocumentDB.

                                MongoDB   DocumentDB
  ----------------------------- --------- ------------
  Software as a Service         +         +
  Platform As a Service         +         -
  Infrastructure as a Service   +         -

MongoDB IaaS/PaaS deployment

MongoDB can be deployed on a Virtual Machine in Azure (IaaS) running
Windows or Linux OS.

-   A step-by-step guide outlining how to install MongoDB on a Windows
    machine can be found here:
    <https://azure.microsoft.com/en-us/documentation/articles/virtual-machines-install-mongodb-windows-server/>

-   Here is a ‘how to’ guide for Linux based VMs:
    <https://docs.mongodb.com/v3.0/administration/install-on-linux/>

-   You can leverage Azure PaaS and install MongoDB on a Worker Role
    which is very close to installation to a standalone VM. The
    following ‘how to’ guide with tips and tricks can be very useful:
    <https://docs.mongodb.org/ecosystem/platforms/windows-azure/>

-   Microsoft provides complete end-to-end scenario of web application
    which uses MongoDB at the backend. It includes the step-by-step
    documentation, code snippets and full source code, which is
    publically available here:
    <https://azure.microsoft.com/en-us/documentation/articles/web-sites-dotnet-store-data-mongodb-vm/>

MongoDB SaaS deployment

MongoLab (<https://mongolab.com/azure/>) provides a MongoDB-as-a-Service
on Microsoft Azure.

DATA MIGRATION

The following data migration scenarios are available, depending on the
target NoSQL database:

-   Standard MongoDB backup and restore procedure:
    <https://docs.mongodb.org/manual/tutorial/backup-and-restore-tools/>

-   DocumentDB supports data migration from MongoDB:
    <https://azure.microsoft.com/en-us/documentation/articles/documentdb-import-data/#Overviewl>

-   MongoLab provides a set of tools for data migration:
    <http://docs.mongolab.com/migrating/>
