# Partner Smart Office

![Build status](https://usocp.visualstudio.com/_apis/public/build/definitions/24f32206-cfd5-40e6-940d-0b99368492b0/11/badge)

Partner Smart Office is an opensource project that demonstrates how a partner can aggregate security information into a single repository for all customers. Secure Score is a numerical summary of a given customer’s security posture within Office 365 based on system configurations, user behavior, and other security-related measurements. It represents the extent to which the customer has adopted security controls available in Office 365, which can help offset the risk of being breached. No online service is completely immune from security breaches; Secure Score should not be interpreted as a guarantee against security breach in any manner.

## Architecture

This solution utilizes an Azure Function App to request and store Secure Score information into an instance of Azure Cosmos DB. That database can be leveraged to construct detailed reports and provide insights into your customer's security posture with respect to Office 365. After deploying this project you will have an Azure Function with the following functions defined

| Function Name      | Trigger                 | Description |
| ------------------ | ----------------------- | ----------- |
| ImportDataControls | Timer (once a day)      | Imports the Secure Score control list entries. These entries define the various actions that can and should be taken, to improve a customer's security posture. |
| PullEnviornments   | Timer (once a day)      | Pulls environments from the Enviornments collection, found in the instance of Azure Cosmos DB, and writes each environment to the appropriate storage queue. |
| ProcessCustomer    | Customers Storage Queue | Imports the security information for the defined customer. Currently this function will import [Office 365 Secure Score](https://support.office.com/article/introducing-the-office-365-secure-score-c9e7160f-2c34-4bd0-a548-5ddcc862eaef) information and details from the [Microsoft Graph Security API](https://www.microsoft.com/security/intelligence-security-api)               |
| ProcessPartner   | Partners Storage Queue  | Adds or updates audit records and customers that are returned from the the [Partner Center API](https://docs.microsoft.com/en-us/partner-center/develop/scenarios) to the appropriate collection in the instance of Azure Cosmos DB. Customers returned from the API are also written to the Customers storage queue, so the respective security information will be imported. |

## Deployment

Perform the following tasks to deploy this project. If you are not a part of the CSP program then you can skip steps 3 - 6.

1. Create and configure the Azure AD application required to access Microsoft Graph, by running the [Create-AzureADApplication.ps1](https://raw.githubusercontent.com/Microsoft/Partner-Smart-Office/master/scripts/Create-AzureADApplication.ps1) script.
2. Document the __ApplicationId__ and __AplicationSecret__ values that the script outputs. These values will be used when deploying the ARM template.
3. Login into the [Partner Center](https://partnercenter.microsoft.com) portal using credentials that have _AdminAgents_ and _Global Admin_ privileges
4. Click _Dashboard_ -> _Account Settings_ -> _App Management_
5. Click on the _Register existing_ app if you want to use an existing Azure AD application, or click _Add new web app_ to create a new one

    ![Partner Center App](docs/media/appmgmt01.png)

6. Document the _App ID_ and _Account ID_ values. Also, if necessary create a key and document that value.

    ![Partner Center App](docs/media/appmgmt02.png)

7. Click the __Deploy to Azure__ button below to deploy this project to Azure.

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2FPartner-Smart-Office%2Fmaster%2Fazuredeploy.json)
[![Visualize](http://armviz.io/visualizebutton.png)](http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2FPartner-Smart-Office%2Fmaster%2Fazuredeploy.json)
