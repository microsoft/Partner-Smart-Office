# Partner Smart Office Changelog

## 0.0.2 (2018-05-23)

### Features

### Bug Fixes

### Breaking Changes

## 0.0.1 (2018-05-07)

### Features

* Security alerts from the [Intelligent Security Graph](https://www.microsoft.com/en-us/security/intelligence-security-api) are now synchronized

### Bug Fixes

* Corrected an error with handling exceptions for HTTP service operations.

### Breaking Changes

* Adding the ability to synchronize security alerts from the Intelligent Security Graph requires additional permissions. After deploying this update perform the following to add the required permissions

    1. Locate the Azure AD application created, in the Azure management portal, that provides access to Microsoft Graph.
    2. Add the *Read your organization’s security events* application permission to the *Microsoft Graph* API.
    3. Click *Grant Permissions* found at the top of the *Required Permissions* blade.