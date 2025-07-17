###############################################################################
<#
.SYNOPSIS
Displays GenXdev PowerShell modules with their cmdlets and aliases.

.DESCRIPTION
Lists all installed GenXdev PowerShell modules and their associated cmdlets and
aliases. Uses Get-GenXDevCmdlets to retrieve cmdlet information and optionally
their script positions. Provides filtering and various display options.

.PARAMETER Filter
Pattern to filter cmdlets by name. Wildcards (*) are supported and automatically
added if not present. Example: "Get" becomes "*Get*"

.PARAMETER ModuleName
Array of module names to filter on. The "GenXdev." prefix is optional.
Supports wildcards (*). Filters modules based on these names.

.PARAMETER Online
When specified, opens the GitHub documentation page instead of console output.

.PARAMETER OnlyAliases
When specified displays only aliases of cmdlets who have them.

.PARAMETER ShowTable
When specified, displays results in a table format with Name and Description.

.PARAMETER OnlyReturnModuleNames
Only return unique module names instead of displaying cmdlet details.

.EXAMPLE
Show-GenXDevCmdlets -CmdletName "Get" -ModuleName "Console" -ShowTable
Lists all cmdlets starting with "Get" in the Console module as a table

.EXAMPLE
cmds get -m console
Lists all cmdlets starting with "Get" in the Console module

.EXAMPLE
Show-GenXDevCmdlets -OnlyReturnModuleNames
Returns only unique module names
#>
function Show-GenXDevCmdlets {

    [CmdletBinding()]
    [Alias('cmds')]
    [OutputType([System.Collections.ArrayList], [void])]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseSingularNouns', '')]

    param(
        ########################################################################
        [parameter(
            Mandatory = $false,
            Position = 0,
            ValueFromRemainingArguments = $false,
            HelpMessage = 'Search pattern to filter cmdlets'
        )]
        [Alias('Filter', 'CmdLet', 'Cmd', 'FunctionName', 'Name')]
        [SupportsWildcards()]
        [string] $CmdletName = '*',
        ########################################################################
        [parameter(
            Mandatory = $false,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true,
            Position = 1,
            HelpMessage = 'GenXdev module names to search'
        )]
        [ValidateNotNullOrEmpty()]
        [Alias('Module', 'ModuleName')]
        [ValidatePattern('^(GenXdev|GenXde[v]\*|GenXdev(\.\w+)+)+$')]
        [string[]] $BaseModuleName = @('GenXdev*'),
        ########################################################################
        [Parameter(Mandatory = $false)]
        [switch] $NoLocal,
        ########################################################################

        [Parameter(Mandatory = $false)]
        [switch] $OnlyPublished,
        ########################################################################

        [Parameter(Mandatory = $false)]
        [switch] $FromScripts,
        #######################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Open GitHub documentation instead of console output'
        )]
        [switch] $Online,
        #######################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'When specified displays only aliases of cmdlets'
        )]
        [Alias('aliases', 'nonboring', 'notlame', 'handyonces')]
        [switch] $OnlyAliases,
        #######################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Display results in table format'
        )]
        [Alias('table', 'grid')]
        [switch] $ShowTable,
        #######################################################################
        [switch] $PassThru,
        #######################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Only return unique module names'
        )]
        [switch] $OnlyReturnModuleNames
    )

    begin {
        Microsoft.PowerShell.Utility\Write-Verbose 'Initializing Show-GenXDevCmdlets'

        # initialize results collection
        $results = [System.Collections.ArrayList]::new()

        if (-not ($CmdletName.Contains('*') -or $CmdletName.Contains('?'))) {

            $CmdletName = "*$CmdletName*"
        }
    }


    process {
        try {
            # copy identical parameters between functions for passing to Get-GenXDevCmdlets
            $invocationParams = GenXdev.Helpers\Copy-IdenticalParamValues `
                -FunctionName 'GenXdev.Helpers\Get-GenXDevCmdlets' `
                -BoundParameters $PSBoundParameters `
                -DefaultValues (Microsoft.PowerShell.Utility\Get-Variable `
                    -Scope Local `
                    -ErrorAction SilentlyContinue)

            # if only module names are requested, return them directly
            if ($OnlyReturnModuleNames) {
                Microsoft.PowerShell.Utility\Write-Verbose 'Returning unique module names directly'
                return GenXdev.Helpers\Get-GenXDevCmdlets @invocationParams -OnlyReturnModuleNames
            }

            # get cmdlets using Get-GenXDevCmdlets
            $cmdlets = GenXdev.Helpers\Get-GenXDevCmdlets @invocationParams

            foreach ($cmdlet in $cmdlets) {

                # handle online documentation
                if ($Online) {
                    if (@('GenXdev.Local', 'GenXdev.Scripts').IndexOf($cmdlet.BaseModule) -ge 0) {
                        Microsoft.PowerShell.Utility\Write-Verbose "Opening documentation for $($cmdlet.ModuleName)"
                        Microsoft.PowerShell.Management\Start-Process `
                            "https://github.com/genXdev/$($cmdlet.ModuleName)/blob/main/README.md#$($cmdlet.Name)" `

                        return
                    }
                    continue
                }

                # prepare cmdlet data
                $cmdletData = @{
                    Name               = $cmdlet.Name
                    Aliases            = $cmdlet.Aliases
                    ModuleName         = $cmdlet.ModuleName
                    BaseModule         = $cmdlet.BaseModule
                    ScriptFilePath     = $cmdlet.ScriptFilePath
                    ScriptTestFilePath = $cmdlet.ScriptTestFilePath
                    LineNumber         = $cmdlet.LineNo
                    Description        = $cmdlet.Description
                }

                # filter aliases if OnlyAliases specified
                if ($OnlyAliases) {

                    $cmdletData.Aliases = @($cmdletData.Aliases.Split(',') |
                            Microsoft.PowerShell.Core\ForEach-Object { if ($_.Trim() -notlike '*-*') { $_.Trim() } } |
                            Microsoft.PowerShell.Utility\Select-Object -First 1) -join ', '

                    if ([string]::IsNullOrWhiteSpace($cmdletData.Aliases)) {

                        $cmdletData.Aliases = ''
                        continue
                    }
                }

                # add to results collection
                $null = $results.Add([PSCustomObject]$cmdletData)
            }
        }
        catch {
            Microsoft.PowerShell.Utility\Write-Error "Error processing cmdlets: $_"
        }
    }

    end {
        if ($PassThru) {

            Microsoft.PowerShell.Utility\Write-Verbose 'Returning results as output'
            return $results
        }
        if ($results.Count -eq 0) {
            Microsoft.PowerShell.Utility\Write-Verbose 'No results found matching criteria'
            return
        }

        if ($ShowTable) {

            if ($OnlyAliases) {

                # display as table
                $results |
                    Microsoft.PowerShell.Utility\Select-Object ModuleName, Aliases, Description |
                    Microsoft.PowerShell.Utility\Format-Table -AutoSize
            }
            else {

                # display as table
                $results |
                    Microsoft.PowerShell.Utility\Select-Object ModuleName, Name, Aliases, Description |
                    Microsoft.PowerShell.Utility\Format-Table -AutoSize
            }
        }
        else {
            # group by module and display with formatting
            $results | Microsoft.PowerShell.Utility\Group-Object ModuleName | Microsoft.PowerShell.Core\ForEach-Object {
                "`r`n$($_.Name):" | Microsoft.PowerShell.Utility\Write-Host -ForegroundColor Yellow

                $all = @($_.Group | Microsoft.PowerShell.Core\ForEach-Object {

                        if ($OnlyAliases) {

                            $_.Aliases.Split(',') |
                                Microsoft.PowerShell.Utility\Select-Object -First 1 |
                                Microsoft.PowerShell.Core\ForEach-Object {

                                    if ($_.Trim() -notlike '*-*') {

                                        $_.Trim()
                                    }
                                }
                            }
                            elseif (-not [string]::IsNullOrWhiteSpace($_.Aliases)) {

                                "$($_.Name) --> $($_.Aliases)"
                            }
                            else {

                                $_.Name
                            }
                        })

                    [string]::Join(', ', $all) | Microsoft.PowerShell.Utility\Write-Host -ForegroundColor White
                }
            }
        }
    }