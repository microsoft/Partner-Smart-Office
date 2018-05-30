# Partner Smart Office Changelog

## 0.0.2 (2018-05-29)

* Added the ability to synchronize data from multiple environments. This change makes it possible for partners with more than one Cloud Solution Provider reseller tenant to aggregate data for all customers.
* Added the ability to synchronize more than 500 resources using the Partner Center API.
* Added the ability to synchronize CSP subscriptions.
* Defined processing restrictions
  * Maximum dequeue count is now configured to 3
  * Maximum number of records dequeued is now configured to 10
  * Threshold for the number of messages fetched is now configured to 5

The following breaking changes were made with this release.

With this release environments need to be defined in a collection named Environments. This collection can contain the configuration information for CSP and EA environments. The following is an example of what the configuration should look like for a CSP environment.

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

In a future release there will be a portal to manage environments and review exceptions.

## 0.0.1 (2018-05-07)

* Added the ability to synchronize security alerts from the [Intelligent Security Graph](https://www.microsoft.com/en-us/security/intelligence-security-api).
* Corrected an error with handling exceptions for HTTP service operations.

The following breaking changes were made with this release.

* Adding the ability to synchronize security alerts from the Intelligent Security Graph requires additional permissions. After deploying this update perform the following to add the required permissions

    1. Locate the Azure AD application created, in the Azure management portal, that provides access to Microsoft Graph.
    2. Add the *Read your organization’s security events* application permission to the *Microsoft Graph* API.
    3. Click *Grant Permissions* found at the top of the *Required Permissions* blade.