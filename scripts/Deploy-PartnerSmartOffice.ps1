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

$adAppAccess = [Microsoft.Open.AzureAD.Model.RequiredResourceAccess]@{
    ResourceAppId = "00000002-0000-0000-c000-000000000000";
    ResourceAccess = 
    [Microsoft.Open.AzureAD.Model.ResourceAccess]@{
        Id = "311a71cc-e848-46a1-bdf8-97ff7156d8e6";
        Type = "Scope"}
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

$app = New-AzureADApplication -AvailableToOtherTenants $true -DisplayName $DisplayName -IdentifierUris "https://$($sessionInfo.TenantDomain)/$((New-Guid).ToString())" -RequiredResourceAccess $adAppAccess, $graphAppAccess
$password = New-AzureADApplicationPasswordCredential -ObjectId $app.ObjectId

if($ConfigurePreconsent) {
    $adminAgentsGroup = Get-AzureADGroup -Filter "DisplayName eq 'AdminAgents'"
    $spn = New-AzureADServicePrincipal -AppId $app.AppId -DisplayName $DisplayName

    Add-AzureADGroupMember -ObjectId $adminAgentsGroup.ObjectId -RefObjectId $spn.ObjectId
}

$AppName = Read-Host -Prompt "Specify name for the function app with no spaces"
$ResourceGroup = Read-Host -Prompt "Specify a resource group name"

New-AzureRmResourceGroup -Location southcentralus -Name $ResourceGroup
New-AzureRmResourceGroupDeployment -Name $(New-Guid).ToString() -ResourceGroupName $ResourceGroup -TemplateUri https://raw.githubusercontent.com/Microsoft/Partner-Smart-Office/master/azuredeploy.json -appName $appName