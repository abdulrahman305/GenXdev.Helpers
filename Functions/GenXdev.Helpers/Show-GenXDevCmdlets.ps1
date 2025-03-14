################################################################################
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

.EXAMPLE
Show-GenXDevCmdlets -CmdletName "Get" -ModuleName "Console" -ShowTable
Lists all cmdlets starting with "Get" in the Console module as a table

.EXAMPLE
cmds get -m console
Lists all cmdlets starting with "Get" in the Console module
#>
function Show-GenXDevCmdlets {

    [CmdletBinding()]
    [Alias("cmds")]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSUseSingularNouns", "")]

    param(
        ########################################################################
        [parameter(
            Mandatory = $false,
            Position = 0,
            ValueFromRemainingArguments = $false,
            HelpMessage = "Search pattern to filter cmdlets"
        )]
        [Alias("Filter", "CmdLet", "Cmd", "FunctionName", "Name")]
        [SupportsWildcards()]
        [string] $CmdletName = "*",
        ########################################################################
        [parameter(
            Mandatory = $false,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true,
            Position = 1,
            HelpMessage = "GenXdev module names to search"
        )]
        [Alias("Module", "ModuleName")]
        [SupportsWildcards()]
        [string[]] $BaseModuleName = @("GenXdev*"),
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
            HelpMessage = "Open GitHub documentation instead of console output"
        )]
        [switch] $Online,
        #######################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = "When specified displays only aliases of cmdlets"
        )]
        [Alias("aliases", "nonboring", "notlame", "handyonces")]
        [switch] $OnlyAliases,
        #######################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = "Display results in table format"
        )]
        [Alias("table", "grid")]
        [switch] $ShowTable
        #######################################################################
    )

    begin {
        Write-Verbose "Initializing Show-GenXDevCmdlets"

        # initialize results collection
        $results = [System.Collections.ArrayList]::new()

        if (-not ($CmdletName.Contains("*") -or $CmdletName.Contains("?"))) {

            $CmdletName = "*$CmdletName*"
        }
    }

    process {
        try {
            $invocationParams = GenXdev.Helpers\Copy-IdenticalParamValues `
                -FunctionName "GenXdev.Helpers\Get-GenXDevCmdlets" `
                -BoundParameters $PSBoundParameters

            $invocationParams.CmdletName = $CmdletName

            # get cmdlets using Get-GenXDevCmdlets
            $cmdlets = GenXdev.Helpers\Get-GenXDevCmdlets @invocationParams

            foreach ($cmdlet in $cmdlets) {

                # handle online documentation
                if ($Online) {
                    if (@("GenXdev.Local", "GenXdev.Scripts").IndexOf($cmdlet.BaseModule) -ge 0) {

                        Write-Verbose "Opening documentation for $($cmdlet.ModuleName)"
                        GenXdev.Webbrowser\Open-Webbrowser `
                            -Url "https://github.com/genXdev/$($cmdlet.ModuleName)/blob/main/README.md#$($cmdlet.Name)" `
                            -Monitor -1

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
                        ForEach-Object { if ($_.Trim() -notlike "*-*") { $_.Trim() } } |
                        Select-Object -First 1) -join ', '

                    if ([string]::IsNullOrWhiteSpace($cmdletData.Aliases)) {

                        $cmdletData.Aliases = ""
                        continue
                    }
                }

                # add to results collection
                $null = $results.Add([PSCustomObject]$cmdletData)
            }
        }
        catch {
            Write-Error "Error processing cmdlets: $_"
        }
    }

    end {
        if ($results.Count -eq 0) {
            Write-Verbose "No results found matching criteria"
            return
        }

        if ($ShowTable) {

            if ($OnlyAliases) {

                # display as table
                $results |
                Select-Object ModuleName, Aliases, Description |
                Format-Table -AutoSize
            }
            else {

                # display as table
                $results |
                Select-Object ModuleName, Name, Aliases, Description |
                Format-Table -AutoSize
            }
        }
        else {
            # group by module and display with formatting
            $results | Group-Object ModuleName | ForEach-Object {
                "`r`n$($_.Name):" | Write-Host -ForegroundColor Yellow

                $all = @($_.Group | ForEach-Object {

                        if ($OnlyAliases) {

                            $_.Aliases.Split(",") |
                            Select-Object -First 1 |
                            ForEach-Object {

                                if ($_.Trim() -notlike "*-*") {

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

                [string]::Join(", ", $all) | Write-Host -ForegroundColor White
            }
        }
    }
}
################################################################################