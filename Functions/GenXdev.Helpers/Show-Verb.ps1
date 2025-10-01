<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Show-Verb.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.290.2025
################################################################################
MIT License

Copyright 2021-2025 GenXdev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
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