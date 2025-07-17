###############################################################################
<#
.SYNOPSIS
Ensures all GenXdev modules are properly loaded by invoking all Ensure*
cmdlets.

.DESCRIPTION
This function retrieves all GenXdev cmdlets that start with "Ensure" and
executes each one to guarantee that all required GenXdev modules and
dependencies are properly loaded and available for use. Any failures during
the execution are caught and displayed as informational messages.

.EXAMPLE
EnsureGenXdev

This command runs all available Ensure* cmdlets to initialize the GenXdev
environment.
#>
function EnsureGenXdev {

    [CmdletBinding()]
    # PSScriptAnalyzer rule exception: SideBySide and ShowWindow are used by Get-Variable
    [Diagnostics.CodeAnalysis.SuppressMessage('PSUseDeclaredVarsMoreThanAssignments','')]

    param(
    )

    begin {

        $SideBySide = $true;
        $ShowWindow = $true;
        # retrieve all ensure cmdlets from genxdev helpers module
        Microsoft.PowerShell.Utility\Write-Verbose (
            'Retrieving all Ensure* cmdlets from GenXdev.Helpers module'
        )
    }

    process {


        $fp = GenXdev.FileSystem\Expand-Path '~\Documents\WindowsPowerShell\Microsoft.PowerShell_profile.ps1' -CreateDirectory

        if (-not (Microsoft.PowerShell.Management\Test-Path -Path $fp)) {

            @"
            # Pass command and arguments on to pwsh.exe
            if (`$args.Count -gt 0) {
                `$command = `$args[0]
                `$arguments = `$args[1..(`$args.Count - 1)] -join ' '
                Start-Process pwsh.exe -ArgumentList "-Command", "`"$command $arguments`""
            } else {
                Start-Process pwsh.exe
            }
"@ | Microsoft.PowerShell.Utility\Out-File -FilePath $fp -Force
        }

        # get all ensure cmdlets and execute each one (excluding self to prevent infinite recursion)
        GenXdev.Helpers\Show-GenXDevCmdlets Ensure* -PassThru |
            Microsoft.PowerShell.Core\ForEach-Object name |
            Microsoft.PowerShell.Core\Where-Object { $_ -ne 'EnsureGenXdev' } |
            Microsoft.PowerShell.Core\ForEach-Object {

                try {

                    if ([string]::IsNullOrWhiteSpace($_)) { return }

                    # execute the current ensure cmdlet
                    Microsoft.PowerShell.Utility\Write-Verbose (
                        "Executing cmdlet: $_"
                    )
                    $params = GenXdev.Helpers\Copy-IdenticalParamValues `
                        -BoundParameters $PSBoundParameters `
                        -FunctionName $_ `
                        -DefaultValues (Microsoft.PowerShell.Utility\Get-Variable -Scope Local -ErrorAction SilentlyContinue)

                    $command = Microsoft.PowerShell.Core\Get-Command -Name $_
                    & $command @params
                } catch {

                    # capture and display any execution failures
                    $errorMessage = "Failed to ensure GenXdev module: $_"

                    Microsoft.PowerShell.Utility\Write-Host -Message $errorMessage -ForegroundColor Cyan
                }
            }

        $queryTypes = @(
            'SimpleIntelligence',
            'Knowledge',
            'Pictures',
            'TextTranslation',
            'Coding',
            'ToolUse'
        )

        foreach ($LLMQueryType in $queryTypes) {

            # invoke GenXdev.AI\Initialize-LMStudioModel with copied parameters
            try {
                Microsoft.PowerShell.Utility\Write-Verbose (
                    'Executing GenXdev.AI\Initialize-LMStudioModel'
                )
                $params = GenXdev.Helpers\Copy-IdenticalParamValues `
                    -BoundParameters $PSBoundParameters `
                    -FunctionName 'GenXdev.AI\Initialize-LMStudioModel' `
                    -DefaultValues (Microsoft.PowerShell.Utility\Get-Variable -Scope Local -ErrorAction SilentlyContinue)

                GenXdev.AI\Initialize-LMStudioModel @params -LLMQueryType $LLMQueryType
            } catch {
                $errorMessage = 'Failed to initialize LMStudio model: GenXdev.AI\Initialize-LMStudioModel'
                Microsoft.PowerShell.Utility\Write-Host -Message $errorMessage -ForegroundColor Cyan
            }
        }
    }
}