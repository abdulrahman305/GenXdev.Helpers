################################################################################
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
# Shows all approved PowerShell verbs

.EXAMPLE
Show-Verb -Verb "Get*"
# Shows all approved verbs starting with "Get"

.EXAMPLE
showverbs "Set*", "Get*"
# Shows all approved verbs starting with "Set" or "Get" using the alias
#>
function Show-Verb {

    [CmdletBinding()]
    [Alias("showverbs")]
    param(
        ########################################################################
        [parameter(
            Position = 0,
            ValueFromPipeline,
            ValueFromPipelineByPropertyName,
            HelpMessage = "One or more verb patterns to filter (supports wildcards)",
            Mandatory = $False
        )]
        [SupportsWildcards()]
        [string[]] $Verb = @()
        ########################################################################
    )

    begin {
        Write-Verbose "Starting Show-Verb with filter patterns: $($Verb -join ', ')"
    }

    process {

        # if no specific verbs requested, get all approved verbs
        if ($Verb.Length -eq 0) {

            $verbs = Get-Verb
        }
        else {
            # filter verbs based on provided patterns
            $verbs = Get-Verb |
            ForEach-Object -ErrorAction SilentlyContinue {

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
        Sort-Object { $PSItem.Verb } |
        ForEach-Object Verb -ErrorAction SilentlyContinue) -Join ", "
    }

    end {
    }
}
################################################################################
