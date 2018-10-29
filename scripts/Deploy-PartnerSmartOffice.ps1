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
    This script will create and configure the required Azure AD application. Also, it will deploy Partmer Smart Office
    to Azure using the ARM template found in the root of repository. 
    .EXAMPLE
        .\Deploy-PartnerSmartOffice.ps1 -ConfigurePreconsent $true -DisplayName "Partner Smart Office" 
        
        .\Deploy-PartnerSmartOffice.ps1 -ConfigurePreconsent $true -DisplayName "Partner Smart Office" -AzureTenantId eb210c1e-b697-4c06-b4e3-8b104c226b9a -AzureAdTenantId b6d82fbd-2f39-42a0-a13d-96352e848dc4

        .\Deploy-PartnerSmartOffice.ps1 -ConfigurePreconsent $true -DisplayName "Partner Smart Office" -AzureTenantId tenant01.onmicrosoft.com -AzureAdTenantId tenant02.onmicrosoft.com
    .PARAMETER ConfigurePreconsent
        Flag indicating whether or not the Azure AD application should be configured for preconsent.
    .PARAMETER DisplayName
        Display name for the Azure AD application that will be created.
    .PARAMETER AzureTenantId
        [OPTIONAL] The domain or tenant identifier for the Azure AD tenant where the Azure resources should be deployed. 
    .PARAMETER AzureAdTenantId
        [OPTIONAL] The domain or tenant identifier for the Azure AD tenant where the Azure AD application should be created. 
#>

Param
(
    [Parameter(Mandatory = $true)]
    [bool]$ConfigurePreconsent, 
    [Parameter(Mandatory = $true)]
    [string]$DisplayName,
    [Parameter(Mandatory = $false)]
    [string]$AzureTenantId,
    [Parameter(Mandatory = $false)]
    [string]$AzureAdTenantId
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

# Check if the Azure PowerShell module has already been loaded. 
if ( ! ( Get-Module AzureRM ) ) {
     # Check if the Azure PowerShell module is installed.
     if ( Get-Module -ListAvailable -Name AzureRM ) {
         # The Azure PowerShell module is not load and it is installed. This module 
         # must be loaded for other operations performed by this script.
         Write-Host -ForegroundColor Green "Loading the Azure PowerShell module..."
         Import-Module AzureRM
     } else {
         Install-Module AzureRM
     }
}

try {
    Write-Host -ForegroundColor Green "When prompted please enter the appropriate credentials..."
    $credential = Get-Credential -Message "Please specify credentials that have Global Admin privileges..."

    if([string]::IsNullOrEmpty($AzureAdTenantId)) {
        Connect-AzureAD -Credential $credential
    } else {
        Connect-AzureAD -Credential $credential -TenantId $AzureAdTenantId
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

try {
    Write-Host -ForegroundColor Green "When prompted please enter the appropriate credentials for deploying reousrces to Azure..."
    $credential = Get-Credential -Message "Please specify credentials that have privileges to deploy resources..."

    if([string]::IsNullOrEmpty($AzureTenantId)) {
        Connect-AzureRmAccount -Credential $credential -TenantId $AzureTenantId
    } else {
        Connect-AzureRmAccount -Credential $credential -TenantId $AzureTenantId
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

$sessionInfo = Get-AzureADCurrentSessionInfo

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

Write-Host -ForegroundColor Green "Creating the Azure AD application and related resources..."

$app = New-AzureADApplication -AvailableToOtherTenants $true -DisplayName $DisplayName -IdentifierUris "https://$($sessionInfo.TenantDomain)/$((New-Guid).ToString())" -RequiredResourceAccess $adAppAccess, $graphAppAccess -AppRoles @($adminAppRole)
New-AzureADApplicationPasswordCredential -ObjectId $app.ObjectId
$spn = New-AzureADServicePrincipal -AppId $app.AppId -DisplayName $DisplayName

if($ConfigurePreconsent) {
    $adminAgentsGroup = Get-AzureADGroup -Filter "DisplayName eq 'AdminAgents'"

    Add-AzureADGroupMember -ObjectId $adminAgentsGroup.ObjectId -RefObjectId $spn.ObjectId
}

$AppName = Read-Host -Prompt "Specify the website name (this value cannot contain any spaces or special characters)"
$ResourceGroup = Read-Host -Prompt "Specify a resource group name"

Write-Host -ForegroundColor Green "Deploying Partner Smart Office. Please note this process can take several minutes."

New-AzureRmResourceGroup -Location southcentralus -Name $ResourceGroup
New-AzureRmResourceGroupDeployment -Name $(New-Guid).ToString() -ResourceGroupName $ResourceGroup -TemplateUri https://raw.githubusercontent.com/Microsoft/Partner-Smart-Office/master/azuredeploy.json -appName $appName -applicationId $app.AppId
