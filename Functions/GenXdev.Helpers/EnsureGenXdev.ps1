################################################################################
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

    param(
    )

    begin {

        # retrieve all ensure cmdlets from genxdev helpers module
        Microsoft.PowerShell.Utility\Write-Verbose (
            "Retrieving all Ensure* cmdlets from GenXdev.Helpers module"
        )
    }

    process {

        # get all ensure cmdlets and execute each one
        GenXdev.Helpers\Show-GenXDevCmdlets Ensure* -PassThru |
        Microsoft.PowerShell.Core\ForEach-Object name |
        Microsoft.PowerShell.Core\ForEach-Object {

            try {

                # execute the current ensure cmdlet
                Microsoft.PowerShell.Utility\Write-Verbose (
                    "Executing cmdlet: $_"
                )

                Microsoft.PowerShell.Utility\Invoke-Expression -Command $_

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
################################################################################
