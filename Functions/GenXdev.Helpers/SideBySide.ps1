#####################################################################
<#
.SYNOPSIS
Sets default side-by-side configuration.

.DESCRIPTION
Sets the default behavior for GenXdev window openings to be side-by-side with PowerShell.
This is useful for users with a single monitor or those who prefer side-by-side window layouts.
See also cmdlet 'secondscreen' and 'restoredefaultmonitor'

.EXAMPLE
PS> sidebyside

Sets defaults for GenXdev window openings to be side-by-side with PowerShell

#>
function sidebyside {

    [CmdletBinding()]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSAvoidGlobalVars", "")]

    param()

    begin {

        Microsoft.PowerShell.Utility\Write-Verbose `
            'Initializing default side-by-side configuration'
    }

    process {

        # establish monitor 0 as the system-wide secondary display designation
        $Global:OriginalDefaultSecondaryMonitor = $Global:LastOriginalDefaultSecondaryMonitor -ne "secondscreen" -and
            $Global:LastOriginalDefaultSecondaryMonitor -ne "sidebyside" ?
            $Global:DefaultSecondaryMonitor :
            $Global:OriginalDefaultSecondaryMonitor;

        $Global:LastOriginalDefaultSecondaryMonitor = "sidebyside"
        $Global:DefaultSecondaryMonitor = 0

        Microsoft.PowerShell.Utility\Write-Verbose `
            "Secondary monitor set to display index: ${Global:DefaultSecondaryMonitor}"
    }

    end {

        Microsoft.PowerShell.Utility\Write-Verbose `
            'Secondary monitor configuration completed successfully'
    }
}