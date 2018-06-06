<#
 * MIT License
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this
 * software and associated documentation files (the "Software"), to deal in the Software
 * without restriction, including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
 * to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 #>

<#
    .SYNOPSIS
        This script will create the require Azure AD application. 
    .EXAMPLE
        .\Create-AzureADApplication.ps1 -ConfigurePreconsent $true -DisplayName "Partner Smart Office" 
        
        .\Create-AzureADApplication.ps1 -ConfigurePreconsent $true -DisplayName "Partner Smart Office" -TenantId eb210c1e-b697-4c06-b4e3-8b104c226b9a

        .\Create-AzureADApplication.ps1 -ConfigurePreconsent $true -DisplayName "Partner Smart Office" -TenantId tenant01.onmicrosoft.com
    .PARAMETER ConfigurePreconsent
        Flag indicating whether or not the Azure AD application should be configured for preconsent.
    .PARAMETER DisplayName
        Display name for the Azure AD application that will be created.
    .PARAMETER TenantId
        [OPTIONAL] The domain or tenant identifier for the Azure AD tenant that should be utilized to create the various resources. 
#>

Param
(
    [Parameter(Mandatory = $true)]
    [bool]$ConfigurePreconsent, 
    [Parameter(Mandatory = $true)]
    [string]$DisplayName,
    [Parameter(Mandatory = $false)]
    [string]$TenantId
)

$ErrorActionPreference = "Stop"

# Check if the Azure AD PowerShell module has already been loaded. 
if ( ! ( Get-Module AzureAD ) ) {
    # Check if the Azure AD PowerShell module is installed.
    if ( Get-Module -ListAvailable -Name AzureAD ) {
        # The Azure AD PowerShell module is not load and it is installed. This module 
        # must be loaded for other operations performed by this script.
        Write-Host -ForegroundColor Green "Loading the Azure AD PowerShell module..."
        Import-Module AzureAD
    } else {
        Install-Module AzureAD
    }
}

$credential = Get-Credential -Message "Please specify credentials that have Global Admin privileges..."

try {
    Write-Host -ForegroundColor Green "When prompted please enter the appropriate credentials..."
   
    if([string]::IsNullOrEmpty($TenantId)) {
        Connect-AzureAD -Credential $credential

        $TenantId = $(Get-AzureADTenantDetail).ObjectId
    } else {
        Connect-AzureAD -Credential $credential -TenantId $TenantId
    }
} catch [Microsoft.Azure.Common.Authentication.AadAuthenticationCanceledException] {
    # The authentication attempt was canceled by the end-user. Execution of the script should be halted.
    Write-Host -ForegroundColor Yellow "The authentication attempt was canceled. Execution of the script will be halted..."
    Exit 
} catch {
    # An unexpected error has occurred. The end-user should be notified so that the appropriate action can be taken. 
    Write-Error "An unexpected error has occurred. Please review the following error message and try again." `
        "$($Error[0].Exception)"
}

$adAppAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
    ResourceAppId = "00000002-0000-0000-c000-000000000000";
    ResourceAccess = 
    [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
        Id = "311a71cc-e848-46a1-bdf8-97ff7156d8e6";
        Type = "Scope"}
}

$adminAppRole = [Microsoft.Open.AzureAD.Model.AppRole]@{
    AllowedMemberTypes = @("User");
    Description = "Administrative users the have the ability to perform all Smart Office operations."; 
    DisplayName = "Partner Smart Office Admins";
    IsEnabled = $true; 
    Id = New-Guid; 
    Value = "Admins";
}

$graphAppAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
    ResourceAppId = "00000003-0000-0000-c000-000000000000";
    ResourceAccess = 
        [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
            Id = "230c1aed-a721-4c5d-9cb4-a90514e508ef";
            Type = "Role"},
        [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
            Id = "bf394140-e372-4bf9-a898-299cfc7564e5";
            Type = "Role"}, 
        [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
            Id = "7ab1d382-f21e-4acd-a863-ba3e13f7da61";
            Type = "Role"}
}

$SessionInfo = Get-AzureADCurrentSessionInfo
$AppAddress = Read-Host -Prompt "What is the address for the portal (e.g. https://smartoffice.azurewebsites.net)"

Write-Host -ForegroundColor Green "Creating the Azure AD application and related resources..."

$app = New-AzureADApplication -AvailableToOtherTenants $true -DisplayName $DisplayName -IdentifierUris "https://$($SessionInfo.TenantDomain)/$((New-Guid).ToString())" -RequiredResourceAccess $adAppAccess, $graphAppAccess -AppRoles @($adminAppRole) -ReplyUrls @("$($AppAddress)/signin-oidc")
$password = New-AzureADApplicationPasswordCredential -ObjectId $app.ObjectId
$spn = New-AzureADServicePrincipal -AppId $app.AppId -DisplayName $DisplayName

if($ConfigurePreconsent) {
    $adminAgentsGroup = Get-AzureADGroup -Filter "DisplayName eq 'AdminAgents'"
    Add-AzureADGroupMember -ObjectId $adminAgentsGroup.ObjectId -RefObjectId $spn.ObjectId
}

Write-Host "ApplicationId       = $($app.AppId)"
Write-Host "ApplicationSecret   = $($password.Value)"