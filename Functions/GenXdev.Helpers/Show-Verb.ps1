<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Show-Verb.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.304.2025
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
Shows a short alphabetical list of all PowerShell verbs.

.DESCRIPTION
Displays PowerShell approved verbs in a comma-separated list. If specific verbs
are provided as input, only matching verbs will be shown. Supports wildcards.

.PARAMETER Verb
One or more verb patterns to filter the output. Supports wildcards.
If omitted, all approved verbs are shown.

.EXAMPLE
Show-Verb
Shows all approved PowerShell verbs

.EXAMPLE
Show-Verb -Verb "Get*"
Shows all approved verbs starting with "Get"

.EXAMPLE
showverbs "Set*", "Get*"
Shows all approved verbs starting with "Set" or "Get" using the alias
#>
function Show-Verb {

    [CmdletBinding()]
    [Alias('showverbs')]
    param(
        ########################################################################
        [parameter(
            Position = 0,
            ValueFromPipeline,
            ValueFromPipelineByPropertyName,
            HelpMessage = 'One or more verb patterns to filter (supports wildcards)',
            Mandatory = $False
        )]
        [SupportsWildcards()]
        [string[]] $Verb = @()
        ########################################################################
    )

    # dont remove this line [dontrefactor]

    begin {
        Microsoft.PowerShell.Utility\Write-Verbose "Starting Show-Verb with filter patterns: $($Verb -join ', ')"
    }


    process {

        # if no specific verbs requested, get all approved verbs
        if ($Verb.Length -eq 0) {

            $verbs = Microsoft.PowerShell.Utility\Get-Verb
        }
        else {
            # filter verbs based on provided patterns
            $verbs = Microsoft.PowerShell.Utility\Get-Verb |
                Microsoft.PowerShell.Core\ForEach-Object -ErrorAction SilentlyContinue {

                    $existingVerb = $PSItem

                    foreach ($verb in $Verb) {

                        if ($existingVerb.Verb -like $verb) {

                            $existingVerb
                        }
                    }
                }
        }

        # sort verbs alphabetically and return as comma-separated string
        ($verbs |
            Microsoft.PowerShell.Utility\Sort-Object { $PSItem.Verb } |
            Microsoft.PowerShell.Core\ForEach-Object Verb -ErrorAction SilentlyContinue) -Join ', '
    }

    end {
    }
}