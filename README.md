
# Microsoft MSMQ Based Queue Service  

## Background

This queue service was based on connecting a contact center to the MSMQ service Microsoft provided by default, but is now being deprecated.  However, I built it with the idea of being able to implement other custom or 3rd party queue services. 

The main project in this codebase is a web service that used MSMQ as a bus service for PureConnect Contact Center software to send information to other connected applications like ServiceNow, iManage and Outlook. 

It therefore contains wrapper libraries to connect via REST to each of the applications.
This allowed service outages in other connected services to not affect the contact center.

## Installation

**Pre-requisites:**  MSMQ Service must be enabled and installed on the server.

### This service requires service handlers (MSMQHandlerService) be installed on the server hosting the MSMQ services.
1. Build the project MSMQHandlerService. Get the resulting MSMQHandlerService.exe and copy that to the server along with the MSMQHandlerService\InstallScript\install.bat file which will run the utility to install the handler as a service on the server.
2. Go the services on the server and make sure they are set to run automatically.


### The second part is the web service (QueueServiceWebApp) which the contact center was able to send REST calls to send/receive information from the various applications.
1. Build the project in QueueServiceWebApp and published the web service to a separate server than where the queue service handlers were installed.


### FYI
* I copied this from a previous project, but without the bus service, so there is SOAP project that I used originally as REST was not available for ServiceNow at the time.

* There is a logger helper project which I implemented to be able to plug in different loggers as necessary, but currently only uses Nlog.

* This project was never but into production, but was tested pretty fairly well, so this project is mainly being published in case it can be customized for any other queue service, since the MSMQ service is most likely being deprecated by Microsoft in favor of Azure.

* The wrapper libraries were used in production on another web service, but are out of date in terms of the SDK's they relied on, but may be useful if updated.

![QueueService](https://github.com/user-attachments/assets/d5448bbe-9bb8-417a-aba2-da58934abbf2)
