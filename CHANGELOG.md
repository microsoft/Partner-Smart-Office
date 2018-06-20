# Partner Smart Office Changelog

## 1.0.0 (2018-06-11)

The following issues were addressed with this release

- Added partitioning to select collections. Please note that this change is a **breaking** change. If you have an existing deployment you will need to delete all collections excepted for the *Environments* collection.
- Added the missing account controller to enable the access denied page and sign out capability
- Modified the processing restrictions
  - Maximum dequeue count is now configured to 3
  - Maximum number of records dequeued is now configured to 14
  - Threshold for the number of messages fetched is now configured to 7
- Only audit records from the pervious day will be imported for now.
- When inserting a collection of records, they will be inserted in chunks. This change ensures that all records will be inserted in a timely manner.
- When inserting a large collection of records, the request unit (RU) throughput for the collection will be dynamically increased from 500 to either 1000 or 2000. Once the collection of records has been inserted, the RUs for the collection will be reset to 500.

## 0.8.0 (2018-06-07)

The following issues were addressed with this release

- Added the missing properties from the Azure utilization record class.
- Addressed issue where Azure utilization records were not getting imported correct. See issue [#27](https://github.com/Microsoft/Partner-Smart-Office/issues/27) for more information
- Addressed issue where an exception stating messages cannot be larger than 65536 bytes might occurr. See issue [#28](https://github.com/Microsoft/Partner-Smart-Office/issues/28) for more information
- Service address values will now default to <https://graph.microsoft.com> and <https://api.partnercenter.microsoft.com>

## 0.6.0 (2018-06-06)

As of this release, we are introducing a code freeze. This means no new configurations or features will be added until after version 1.0 has been released. This code freeze will allow us to focus on addressing issues and simplifying the deployment. If there is a new feature you would like to see added please log a request using the [issue tracker](https://github.com/Microsoft/Partner-Smart-Office/issues), and we will prioritize it accordingly for a future release.

The following enhancements were made with this release

- Added logic to the *ProcessEnvironments* function to write a warning to the console if no environments have been created.
- Modified the authentication configuration for the portal to utilize Azure AD app roles over Azure AD directory roles. This change will make it easier for organizations that are using separate Azure AD tenants for authentication to deploy the solution.

The following issues were addressed with this release

- Deployment may fail with error an stating the key vault name is invalid. Pull request [#24](https://github.com/Microsoft/Partner-Smart-Office/pull/24) introduced the solution for this issue.

This update will require an Azure AD application role be defined and assigned to users that will be managing environments using the portal. Users who are not assigned to this role will receive an access denied error when attempting to access the portal. If you have an existing Azure AD application that you would like to use for the portal then it is recommended that you run the following PowerShell script to create the application role

```powershell
Connect-AzureAD

$adminAppRole = [Microsoft.Open.AzureAD.Model.AppRole]@{
    AllowedMemberTypes = @("User");
    Description = "Administrative users the have the ability to perform all Smart Office operations.";
    DisplayName = "Partner Smart Office Admins";
    IsEnabled = $true;
    Id = New-Guid;
    Value = "Admins";
}

# Note the following value can be found in the Azure management portal. Also, it should be a GUID with no trailing spaces.
$appId = Read-Host -Prompt "What is the application identifier for the application you would like to configure?"
$app = Get-AzureADApplication -Filter "AppId eq '$($appId)'"

Set-AzureADApplication -ObjectId $app.ObjectId -AppRoles @($adminAppRole)
```

If you need information on how to assign users to Azure AD application roles please refer to [How to assign users and groups to an application](https://docs.microsoft.com/en-us/azure/active-directory/application-access-assignment-how-to-add-assignment). Please see the [wiki](https://github.com/Microsoft/Partner-Smart-Office/wiki) for more information on to deploy this solution and create new environments using the portal.

## 0.5.0 (2018-06-05)

The following enhancements were made with this release

- Added a portal to manage the creation and settings for an environment.
- Added the ability to synchronize Azure utilization records for Azure CSP subscriptions. This ability is controlled using the ProcessAzureUsage flag configured on each instance. The default value is false for all environments.

The following bugs were fixed with this release

- A null reference exception was thrown when processing audit records that did not have a customer identifier. This has been fixed through [#19](https://github.com/Microsoft/Partner-Smart-Office/pull/19)
- A format exception was thrown when processing security information for an EA environment. This was fixed through [#20](https://github.com/Microsoft/Partner-Smart-Office/pull/20)

The following changes should be made to existing deployments so that the portal will function as excepted

- Add the _Read Directory Data_ application permission to the application you created using the [Create-AzureADApplication.ps1](https://raw.githubusercontent.com/Microsoft/Partner-Smart-Office/master/scripts/Create-AzureADApplication.ps1) script

## 0.0.3 (2018-06-01)

- Added the ability to seed audit logs for CSP environments that are new to this platform.
- Added the ability to seed Secure Score information from the past 30 days for customers that are new to this platform.
- Added the ability to track exceptions when processing a customer. If an exception is encountered when processing a customer the entire exception will be written to the ProcessException property of the customer details object.
- Added the environment identifier to the customer details. If you existing customer details without this property, you will need to delete the LastProcessed property from the environment collection to get the property added to the existing records.
- Each request to the Partner Center API now includes the MS-CorrelationId, MS-PartnerCenter-ApplicationName, and MS-RequestId header
- Fixed issue [#11](https://github.com/Microsoft/Partner-Smart-Office/issues/12)
- Fixed issue [#12](https://github.com/Microsoft/Partner-Smart-Office/issues/12)
- Fixed issue [#13](https://github.com/Microsoft/Partner-Smart-Office/issues/13)

## 0.0.2 (2018-05-29)

- Added the ability to synchronize data from multiple environments. This change makes it possible for partners with more than one Cloud Solution Provider reseller tenant to aggregate data for all customers.
- Added the ability to synchronize more than 500 resources using the Partner Center API.
- Added the ability to synchronize CSP subscriptions.
- Defined processing restrictions
  - Maximum dequeue count is now configured to 3
  - Maximum number of records dequeued is now configured to 10
  - Threshold for the number of messages fetched is now configured to 5

The following breaking changes were made with this release.

With this release, environments need to be defined in a collection named Environments. This collection can contain the configuration information for CSP and EA environments. The following is an example of what the configuration should look like for a CSP environment.

```json
{
    "AppEndpoint": {
        "ApplicationId": "INSERT-ID-FOR-THE-AZURE-AD-APP",
        "ApplicationSecretId": "NAME-OF-SECRET-FOR-THE-APP-IN-KEYVAULT",
        "ServiceAddress": "https://graph.microsoft.com",
        "TenantId": "INSERT-THE-TENANT-ID-HERE"
    },
    "EnvironmentType": "CSP",
    "FriendlyName": "INSERT FRIENDLY NAME HERE",
    "id": "INSERT-THE-TENANT-ID-HERE",
    "PartnerCenterEndpoint": {
        "ApplicationId": "INSERT-THE-PC-APP-ID-HERE",
        "ApplicationSecretId": "NAME-OF-SECRET-FOR-THE-APP-IN-KEYVAULT",
        "ServiceAddress": "https://api.partnercenter.microsoft.com",
        "TenantId": "INSERT-THE-TENANT-ID-HERE"
    }
}
```

The following is an example of what the configuration should look like for an EA environment.

```json
{
    "AppEndpoint": {
        "ApplicationId": "INSERT-ID-FOR-THE-AZURE-AD-APP",
        "ApplicationSecretId": "NAME-OF-SECRET-FOR-THE-APP-IN-KEYVAULT",
        "ServiceAddress": "https://graph.microsoft.com",
        "TenantId": "INSERT-THE-TENANT-ID-HERE"
    },
    "EnvironmentType": "EA",
    "FriendlyName": "INSERT FRIENDLY NAME HERE",
    "id": "INSERT-THE-TENANT-ID-HERE"
}
```

In a future release, there will be a portal to manage environments and review exceptions.

## 0.0.1 (2018-05-07)

- Added the ability to synchronize security alerts from the [Intelligent Security Graph](https://www.microsoft.com/en-us/security/intelligence-security-api).
- Corrected an error with handling exceptions for HTTP service operations.

The following breaking changes were made with this release.

- Adding the ability to synchronize security alerts from the Intelligent Security Graph requires additional permissions. After deploying this update perform the following to add the required permissions

    1. Locate the Azure AD application created, in the Azure management portal, that provides access to Microsoft Graph.
    2. Add the *Read your organization’s security events* application permission to the *Microsoft Graph* API.
    3. Click *Grant Permissions* found at the top of the *Required Permissions* blade.