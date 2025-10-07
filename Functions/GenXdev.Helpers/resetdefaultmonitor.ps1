#####################################################################
<#
.SYNOPSIS
Restores default secondary monitor configuration.

.DESCRIPTION
This script restores the default secondary monitor configuration for the system,
setting the secondary monitor to the original default value.
This is useful for users who want to revert to their previous multi-monitor setup after using side-by-side configurations.
See also: 'sidebyside' function to switch to side-by-side mode for new windows.

.EXAMPLE
secondscreen

Restores the default secondary monitor configuration for the system.
#>
#####################################################################
function resetdefaultmonitor {

    [CmdletBinding()]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSAvoidGlobalVars", "")]
    param()

    #####################################################################

    begin {

        Microsoft.PowerShell.Utility\Write-Verbose `
            'Setting default secondary monitor configuration'
    }

    #####################################################################

    process {

        $Global:DefaultSecondaryMonitor = $null -ne $Global:LastOriginalDefaultSecondaryMonitor ?
            $Global:OriginalDefaultSecondaryMonitor : $Global:DefaultSecondaryMonitor

        $Global:LastOriginalDefaultSecondaryMonitor = $null;

        Microsoft.PowerShell.Utility\Write-Verbose `
            "Secondary monitor set to: ${Global:DefaultSecondaryMonitor}"
    }

    #####################################################################

    end {

        Microsoft.PowerShell.Utility\Write-Verbose `
            'Secondary monitor configuration completed'
    }
}