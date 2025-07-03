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
        ###############################################################################>
function EnsureGenXdev {

    [CmdletBinding()]

    param(
         ###################################################################
         [Parameter(
            Mandatory = $false,
            HelpMessage = "Show Docker Desktop window during initialization"
        )]
        [switch]$ShowWindow
        ###################################################################
    )

    begin {

        # retrieve all ensure cmdlets from genxdev helpers module
        Microsoft.PowerShell.Utility\Write-Verbose (
            "Retrieving all Ensure* cmdlets from GenXdev.Helpers module"
        )
    }

    process {

        # get all ensure cmdlets and execute each one (excluding self to prevent infinite recursion)
        GenXdev.Helpers\Show-GenXDevCmdlets Ensure* -PassThru |
        Microsoft.PowerShell.Core\ForEach-Object name |
        Microsoft.PowerShell.Core\Where-Object { $_ -ne "EnsureGenXdev" } |
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
    }

    end {
    }
}
        ###############################################################################
