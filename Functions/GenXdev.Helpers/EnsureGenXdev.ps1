<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : EnsureGenXdev.ps1
Original author           : René Vaessen / GenXdev
Version                   : 2.1.2025
################################################################################
Copyright (c)  René Vaessen / GenXdev

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
################################################################################>
###############################################################################
<#
.SYNOPSIS
Ensures all GenXdev modules are properly loaded by invoking all Ensure*
cmdlets.

.DESCRIPTION
This function retrieves all GenXdev cmdlets that start with "Ensure" or
"Optimize-Ensure" (for c# cmdlets) and
executes each one to guarantee that all required GenXdev modules and
dependencies are properly loaded and available for use. Any failures during
the execution are caught and displayed as informational messages.

Optionally, it can also download and load all NuGet packages defined in the
packages.json manifest file, and initialize LMStudio models for AI functionality.

.PARAMETER Force
Forces the execution of ensure operations even if they appear to be already
completed.

.PARAMETER DownloadLMStudioModels
Downloads and initializes LMStudio models for various AI query types.

.PARAMETER DownloadAllNugetPackages
Downloads and loads all NuGet packages defined in the packages.json manifest
file.

.PARAMETER ForceConsent
Force a consent prompt even if a preference is already set for third-party
software installation, overriding any saved consent preferences.

.PARAMETER ConsentToThirdPartySoftwareInstallation
Automatically consent to third-party software installation and set a persistent
preference flag for all packages, bypassing interactive consent prompts.

.EXAMPLE
EnsureGenXdev

This command runs all available Ensure* cmdlets to initialize the GenXdev
environment.

.EXAMPLE
EnsureGenXdev -DownloadAllNugetPackages

This command runs all Ensure* cmdlets and also downloads and loads all NuGet
packages defined in the packages.json manifest file.

.EXAMPLE
EnsureGenXdev -DownloadAllNugetPackages -ConsentToThirdPartySoftwareInstallation

.EXAMPLE
EnsureGenXdev -DownloadLMStudioModels -DownloadAllNugetPackages

This command runs all Ensure* cmdlets, downloads NuGet packages, and initializes
LMStudio models for AI functionality.
#>
function EnsureGenXdev {

    [CmdletBinding()]
    # PSScriptAnalyzer rule exception: SideBySide and ShowWindow are used by Get-Variable
    [Diagnostics.CodeAnalysis.SuppressMessage('PSUseDeclaredVarsMoreThanAssignments', '')]

    param(
        ###########################################################################
        [parameter(
            Mandatory = $false,
            HelpMessage = "Forces the execution of ensure operations even if they appear to be already completed"
        )]
        [Switch] $Force,
        ###########################################################################
        [parameter(
            Mandatory = $false,
            HelpMessage = "Downloads and initializes LMStudio models for various AI query types"
        )]
        [Switch] $DownloadLMStudioModels,
        ###########################################################################
        [parameter(
            Mandatory = $false,
            HelpMessage = "Downloads and loads all NuGet packages defined in the packages.json manifest file"
        )]
        [Switch] $DownloadAllNugetPackages,

        ###########################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Force a consent prompt even if preference is set for third-party software installation.'
        )]
        [switch] $ForceConsent,

        ###########################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Automatically consent to third-party software installation and set persistent flag for all packages.'
        )]
        [switch] $ConsentToThirdPartySoftwareInstallation
        ###########################################################################
    )

    begin {

        # initialize variables for local scope
        $sideBySide = $true
        $showWindow = $true

        # retrieve all ensure cmdlets from genxdev helpers module
        Microsoft.PowerShell.Utility\Write-Verbose (
            'Retrieving all Ensure* cmdlets from GenXdev.Helpers module'
        )
    }

    process {

        # ensure vs code installation is available
        try {
            # request consent for VSCode installation using embedded consent
            $vscodeParams = GenXdev.FileSystem\Copy-IdenticalParamValues `
                -BoundParameters $PSBoundParameters `
                -FunctionName 'GenXdev.FileSystem\Confirm-InstallationConsent' `
                -DefaultValues (
                Microsoft.PowerShell.Utility\Get-Variable -Scope Local `
                    -ErrorAction SilentlyContinue
            )

            # Set specific parameters for VSCode
            $vscodeParams['ApplicationName'] = 'Visual Studio Code'
            $vscodeParams['Source'] = 'Winget'
            $vscodeParams['Description'] = 'Code editor required for GenXdev development environment'
            $vscodeParams['Publisher'] = 'Microsoft'

            $consent = GenXdev.FileSystem\Confirm-InstallationConsent @vscodeParams

            if (-not $consent) {
                Microsoft.PowerShell.Utility\Write-Warning "VSCode installation consent denied. Some GenXdev features may not be available."
            }
            else {
                GenXdev.Coding\EnsureVSCodeInstallation
            }
        }
        catch {
            Microsoft.PowerShell.Utility\Write-Warning "Failed to check VSCode installation consent: $($_.Exception.Message)"
        }

        # get all ensure cmdlets and execute each one (excluding self to prevent infinite recursion)
        (@(GenXdev.Helpers\Get-GenXDevCmdlet Ensure*) + @(GenXdev.Helpers\Get-GenXDevCmdlet Optimize-Ensure*)) |
            Microsoft.PowerShell.Core\ForEach-Object name |
            Microsoft.PowerShell.Core\Where-Object {

                # exclude self and other specific cmdlets to prevent recursion
                (@('EnsureGenXdev', 'EnsureComfyUIModel', 'EnsureGenXdev', 'Optimize-EnsureComfyUIModel').IndexOf($_) -lt 0) -and
                ($_ -ne "EnsureNuGetAssembly") -and
                ($_ -ne "Optimize-EnsureNuGetAssembly")

            } |
            Microsoft.PowerShell.Core\ForEach-Object {

                try {

                    # skip empty or whitespace entries
                    if ([string]::IsNullOrWhiteSpace($_)) { return }

                    # execute the current ensure cmdlet
                    Microsoft.PowerShell.Utility\Write-Verbose (
                        "Executing cmdlet: $_"
                    )

                    # copy identical parameter values for the target function
                    $params = GenXdev.FileSystem\Copy-IdenticalParamValues `
                        -BoundParameters $PSBoundParameters `
                        -FunctionName $_ `
                        -DefaultValues (Microsoft.PowerShell.Utility\Get-Variable -Scope Local -ErrorAction SilentlyContinue)

                    # get the command and invoke it with the copied parameters
                    $command = Microsoft.PowerShell.Core\Get-Command -Name $_
                    & $command @params
                }
                catch {

                    # capture and display any execution failures
                    $errorMessage = "Failed to ensure GenXdev module: $_"

                    Microsoft.PowerShell.Utility\Write-Host -Message $errorMessage -ForegroundColor Cyan
                }
            }

        # download all NuGet packages if requested
        if ($DownloadAllNugetPackages) {
            try {
                # request consent for NuGet packages installation using embedded consent
                $nugetParams = GenXdev.FileSystem\Copy-IdenticalParamValues `
                    -BoundParameters $PSBoundParameters `
                    -FunctionName 'GenXdev.FileSystem\Confirm-InstallationConsent' `
                    -DefaultValues (
                    Microsoft.PowerShell.Utility\Get-Variable -Scope Local `
                        -ErrorAction SilentlyContinue
                )

                # Set specific parameters for NuGet packages
                $nugetParams['ApplicationName'] = 'NuGet Packages'
                $nugetParams['Source'] = 'PowerShell Gallery'
                $nugetParams['Description'] = 'Third-party .NET assemblies and libraries for GenXdev functionality'
                $nugetParams['Publisher'] = 'Various'

                $nugetConsent = GenXdev.FileSystem\Confirm-InstallationConsent @nugetParams

                if (-not $nugetConsent) {
                    Microsoft.PowerShell.Utility\Write-Warning "NuGet packages installation consent denied. Some GenXdev features may not be available."
                    return
                }

                Microsoft.PowerShell.Utility\Write-Verbose (
                    'Processing all NuGet packages from packages.json'
                )

                # construct path to packages.json in GenXdev.PackageManagement module
                $packagesJsonPath = GenXdev.FileSystem\Expand-Path (
                    "$PSScriptRoot\..\..\packages.json"
                )

                # check if the packages configuration file exists
                if (Microsoft.PowerShell.Management\Test-Path -LiteralPath $packagesJsonPath) {
                    Microsoft.PowerShell.Utility\Write-Verbose (
                        "Loading packages from: $packagesJsonPath"
                    )

                    # load and parse the package configuration
                    $packages = Microsoft.PowerShell.Management\Get-Content -LiteralPath $packagesJsonPath -ErrorAction Stop |
                        Microsoft.PowerShell.Utility\ConvertFrom-Json -ErrorAction Stop -AsHashtable

                    # iterate through each package key and ensure it's loaded
                    foreach ($packageKey in $packages.Keys) {
                        try {
                            Microsoft.PowerShell.Utility\Write-Verbose (
                                "Ensuring NuGet package: $packageKey"
                            )

                            # ensure the specific nuget assembly is loaded using embedded consent
                            $ensureParams = GenXdev.FileSystem\Copy-IdenticalParamValues `
                                -BoundParameters $PSBoundParameters `
                                -FunctionName 'GenXdev.Helpers\EnsureNuGetAssembly' `
                                -DefaultValues (
                                Microsoft.PowerShell.Utility\Get-Variable -Scope Local `
                                    -ErrorAction SilentlyContinue
                            )

                            $ensureParams['PackageKey'] = $packageKey
                            GenXdev.Helpers\EnsureNuGetAssembly @ensureParams
                        }
                        catch {
                            # capture and display package loading failures
                            $errorMessage = (
                                "Failed to ensure NuGet package: $packageKey - $($_.Exception.Message)"
                            )
                            Microsoft.PowerShell.Utility\Write-Host -Message $errorMessage -ForegroundColor Cyan
                        }
                    }
                }
                else {
                    # warn when packages configuration is not found
                    Microsoft.PowerShell.Utility\Write-Warning (
                        "packages.json not found at: $packagesJsonPath"
                    )
                }
            }
            catch {
                # capture and display general nuget processing failures
                $errorMessage = "Failed to process NuGet packages: $($_.Exception.Message)"
                Microsoft.PowerShell.Utility\Write-Host -Message $errorMessage -ForegroundColor Cyan
            }
        }

        # define the types of ai queries that require model initialization
        $queryTypes = @(
            'SimpleIntelligence',
            'Knowledge',
            'Pictures',
            'TextTranslation',
            'Coding',
            'ToolUse'
        )

        # exit early if lmstudio models are not requested
        if (-not $DownloadLMStudioModels) { return }

        # request consent for LMStudio models installation
        try {
            # request consent for LMStudio using embedded consent
            $lmStudioParams = GenXdev.FileSystem\Copy-IdenticalParamValues `
                -BoundParameters $PSBoundParameters `
                -FunctionName 'GenXdev.FileSystem\Confirm-InstallationConsent' `
                -DefaultValues (
                Microsoft.PowerShell.Utility\Get-Variable -Scope Local `
                    -ErrorAction SilentlyContinue
            )

            # Set specific parameters for LMStudio
            $lmStudioParams['ApplicationName'] = 'LMStudio AI Models'
            $lmStudioParams['Source'] = 'LMStudio'
            $lmStudioParams['Description'] = 'Large language models for AI functionality in GenXdev'
            $lmStudioParams['Publisher'] = 'LMStudio'

            $lmStudioConsent = GenXdev.FileSystem\Confirm-InstallationConsent @lmStudioParams

            if (-not $lmStudioConsent) {
                Microsoft.PowerShell.Utility\Write-Warning "LMStudio models installation consent denied. AI features will not be available."
                return
            }
        }
        catch {
            Microsoft.PowerShell.Utility\Write-Warning "Failed to check LMStudio models installation consent: $($_.Exception.Message)"
            return
        }

        # initialize lmstudio models for each query type
        foreach ($llmQueryType in $queryTypes) {

            # invoke GenXdev.AI\Initialize-LMStudioModel with copied parameters
            try {
                Microsoft.PowerShell.Utility\Write-Verbose (
                    'Executing GenXdev.AI\Initialize-LMStudioModel'
                )

                # copy identical parameter values for the ai initialization function
                $params = GenXdev.FileSystem\Copy-IdenticalParamValues `
                    -BoundParameters $PSBoundParameters `
                    -FunctionName 'GenXdev.AI\Initialize-LMStudioModel' `
                    -DefaultValues (Microsoft.PowerShell.Utility\Get-Variable -Scope Local -ErrorAction SilentlyContinue)

                # initialize the lmstudio model for the current query type
                GenXdev.AI\Initialize-LMStudioModel @params -LLMQueryType $llmQueryType
            }
            catch {
                # capture and display ai model initialization failures
                $errorMessage = (
                    'Failed to initialize LMStudio model: GenXdev.AI\Initialize-LMStudioModel'
                )
                Microsoft.PowerShell.Utility\Write-Host -Message $errorMessage -ForegroundColor Cyan
            }
        }
    }

    end {
    }
}
###############################################################################