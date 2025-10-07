<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Get-GenXDevCmdlet.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.296.2025
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
Retrieves and lists all GenXdev cmdlets and their details.

.DESCRIPTION
Searches through installed GenXdev modules and script files to find cmdlets,
their aliases, and descriptions. Can filter by name pattern and module name.
Supports filtering by cmdlet definitions and provides flexible search options
across both local and published module paths.

.PARAMETER CmdletName
Search pattern to filter cmdlets. Supports wildcards (*) and exact matching.
When ExactMatch is false, automatically wraps simple strings with wildcards.

.PARAMETER DefinitionMatches
Regular expression to match cmdlet definitions. Used to filter cmdlets based
on their function content or implementation details.

.PARAMETER ModuleName
One or more GenXdev module names to search. Can omit GenXdev prefix. Supports
wildcards and validates module name patterns for GenXdev modules.

.PARAMETER NoLocal
Skip searching in local module paths. When specified, only searches in
published or system module locations.

.PARAMETER OnlyPublished
Limit search to published module paths only. Excludes local development
modules and focuses on released versions.

.PARAMETER FromScripts
Search in script files instead of module files. Changes the search target
from PowerShell modules to standalone script files.

.PARAMETER IncludeScripts
Includes the scripts directory in addition to regular modules. Expands the
search scope to cover both modules and scripts simultaneously.

.PARAMETER OnlyReturnModuleNames
Only return unique module names instead of full cmdlet details. Provides a
summary view of available modules rather than detailed cmdlet information.

.PARAMETER ExactMatch
Perform exact matching instead of wildcard matching. When specified, disables
automatic wildcard wrapping for simple search patterns.

.EXAMPLE
Get-GenXDevCmdlet -CmdletName "Get-*" -ModuleName "Console" -NoLocal

.EXAMPLE
gcmds Get-*

.EXAMPLE
Get-GenXDevCmdlet -OnlyReturnModuleNames
#>
function Get-GenXDevCmdlet {

    [CmdletBinding()]
    [OutputType([GenXdev.Helpers.GenXdevCmdletInfo], [string])]
    [Alias('gcmds')]

    param(
        ###############################################################################
        [Parameter(
            Position = 0,
            Mandatory = $false,
            ValueFromPipelineByPropertyName = $true,
            HelpMessage = 'Search pattern to filter cmdlets'
        )]
        [ValidateNotNullOrEmpty()]
        [Alias('Filter', 'CmdLet', 'Cmd', 'FunctionName', 'Name')]
        [SupportsWildcards()]
        [string] $CmdletName,
        ###############################################################################
        [Parameter(
            Position = 1,
            Mandatory = $false,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true,
            HelpMessage = 'Regular expression to match cmdlet definitions'
        )]
        [ValidateNotNullOrEmpty()]
        [string] $DefinitionMatches,
        ###############################################################################
        [Parameter(
            Position = 2,
            Mandatory = $false,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true,
            HelpMessage = 'GenXdev module names to search'
        )]
        [ValidateNotNullOrEmpty()]
        [Alias('Module', 'BaseModuleName', 'SubModuleName')]
        [ValidatePattern('^(GenXdev|GenXde[v]\*|GenXdev(\.[\w\*\[\]\?]*)+)+$')]
        [SupportsWildcards()]
        [string[]] $ModuleName,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Skip searching in local module paths'
        )]
        [switch] $NoLocal,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Only search in published module paths'
        )]
        [switch] $OnlyPublished,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Search in script files instead of modules'
        )]
        [switch] $FromScripts,
        ###############################################################################
        [Parameter(
            ParameterSetName = "ModuleName",
            Mandatory = $false,
            HelpMessage = ('Includes the scripts directory in addition to ' +
                'regular modules')
        )]
        [switch] $IncludeScripts,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Only return unique module names'
        )]
        [switch] $OnlyReturnModuleNames,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Perform exact matching instead of wildcard matching'
        )]
        [switch] $ExactMatch
    )

    begin {

        # get ps root path
        $psRootPath = GenXdev.FileSystem\Expand-Path "$PSScriptRoot\..\..\..\..\..\"

        # store current location to restore later and prevent side effects
        $originalLocation = Microsoft.PowerShell.Management\Get-Location

        # initialize collection to store found cmdlets
        $cmdletCollection = [System.Collections.Generic.List[GenXdev.Helpers.GenXdevCmdletInfo]]::new()

        # output verbose information about search parameters
        Microsoft.PowerShell.Utility\Write-Verbose (
            "Starting cmdlet search with CmdletName: ${CmdletName}, " +
            "ModuleName: ${ModuleName}, ExactMatch: ${ExactMatch}"
        )
    }

    process {

        # copy identical parameter values to pass to the module iteration function
        $params = GenXdev.Helpers\Copy-IdenticalParamValues `
            -BoundParameters $PSBoundParameters `
            -FunctionName 'GenXdev.Helpers\Invoke-OnEachGenXdevModule' `
            -DefaultValues (Microsoft.PowerShell.Utility\Get-Variable `
                -Scope Local `
                -ErrorAction SilentlyContinue
        )

        # output verbose information about parameter copying
        Microsoft.PowerShell.Utility\Write-Verbose (
            "Copied parameters for module iteration: " +
            "$($params.Keys -join ', ')"
        )

        # iterate through each genxdev module to find cmdlets
        GenXdev.Helpers\Invoke-OnEachGenXdevModule @params {

            param(
                $Module,
                $isScriptsFolder,
                $isSubModule,
                $subModuleName
            )

            if ($Module.Name -eq "GenXdev.FileSystem" -and (
                [string]::IsNullOrWhiteSpace($CmdletName) -or
                ($CmdletName -like "*Find-Item") -or
                ($CmdletName -eq "l")
                )
            ) {
@"
{
        `"Name`": `"Find-Item`",
        `"ModuleName`": `"GenXdev.FileSystem`",
        `"BaseModule`": `"GenXdev.FileSystem`",
        `"LineNo`": 0,
        `"Description`": `"Fast multi-threaded file and directory search with optional textcontent pattern matching`",
        `"Aliases`": `"l`",
        `"ScriptFilePath`": `"$($psRootPath.Replace("\","\\"))\\Modules\\GenXdev.FileSystem\\1.296.2025\\Functions\\GenXdev.FileSystem\\Find-Item.Cmdlet.cs`",
        `"ScriptTestFilePath`": `"$($psRootPath.Replace("\","\\"))\\Modules\\GenXdev.FileSystem\\1.296.2025\\Tests\\GenXdev.FileSystem\\Find-Item.Tests.ps1`"
}
"@ | Microsoft.PowerShell.Utility\ConvertFrom-Json | GenXdev.Helpers\ConvertTo-HashTable
            }

            # determine which files to search based on folder type
            @(
                if ($isScriptsFolder) {

                    # search all powershell scripts recursively in scripts folder
                    Microsoft.PowerShell.Management\Get-ChildItem .\*.ps1 -Recurse

                }
                elseif ($isSubModule) {

                    # get all subdirectories under Functions to process each submodule
                    Microsoft.PowerShell.Management\Get-ChildItem .\Functions\* -Directory -ErrorAction SilentlyContinue |
                        Microsoft.PowerShell.Core\ForEach-Object {
                            $subModuleDir = $_

                            # temporarily change to submodule directory
                            Microsoft.PowerShell.Management\Push-Location -LiteralPath $subModuleDir.FullName

                            # get all powershell files in this submodule directory
                            Microsoft.PowerShell.Management\Get-ChildItem .\*.ps1 -ErrorAction SilentlyContinue |
                                Microsoft.PowerShell.Core\ForEach-Object {
                                    # add a property to track the actual submodule name
                                    $_ | Microsoft.PowerShell.Utility\Add-Member -NotePropertyName 'ActualSubModuleName' -NotePropertyValue $subModuleDir.Name -Force
                                    $_
                                }

                                # restore previous location
                                Microsoft.PowerShell.Management\Pop-Location
                    }
                }
                else {

                    # search all function files recursively, excluding private ones
                    Microsoft.PowerShell.Management\Get-ChildItem .\Functions\*.ps1 -Recurse -ErrorAction SilentlyContinue |
                        Microsoft.PowerShell.Core\Where-Object {

                            # exclude files starting with underscore (private functions)
                            $_.Name -notlike "_*"
                        }
                }
            ) |
            Microsoft.PowerShell.Core\ForEach-Object {

                try {
                    ###############################################################################
                    <#
                    .SYNOPSIS
                    Helper function to extract function descriptions from sourcecode.

                    .DESCRIPTION
                    Parses PowerShell script content to extract the SYNOPSIS section from
                    comment-based help. Handles both module functions and script files with
                    different regex patterns based on file location.

                    .PARAMETER FileName
                    The path to the script file to analyze for function descriptions.

                    .PARAMETER FunctionContent
                    The content of the function to parse for extracting the synopsis.
                    #>
                    function Get-FunctionDescription {

                        [CmdletBinding()]
                        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseOutputTypeCorrectly', 'Get-FunctionDescription')]
                        [OutputType([string])]

                        param(
                            [Parameter(
                                Position = 0,
                                Mandatory = $true
                            )]
                            [string]$FileName,

                            [Parameter(
                                Position = 1,
                                Mandatory = $true
                            )]
                            [AllowEmptyString()]
                            [AllowNull()]
                            [string]$FunctionContent
                        )

                        # ensure function content is never null for string operations
                        $FunctionContent = ($null -eq $FunctionContent) ? '' : $FunctionContent

                        # check if file is in scripts folder to determine parsing strategy
                        $fromScripts = $FileName.StartsWith((GenXdev.FileSystem\Expand-Path `
                                    -FilePath "$PSScriptRoot\..\..\..\..\..\Scripts\"))

                        try {

                            # build regex pattern based on file location and structure
                            $pattern = $fromScripts ?
                            "\.SYNOPSIS\s*`r`n([^\r\n]*[\r\n]*)\r\n\r\n([^#][^>])*#>" :
                            ("\.SYNOPSIS\s*`r`n([^\r\n]*[\r\n]*)\r\n\r\n([^#][^>])*#>\s*" +
                            "function\s+$([IO.Path]::GetFileNameWithoutExtension($FileName))")

                            # extract synopsis using regex matching
                            $match = [regex]::Match(
                                $FunctionContent,
                                $pattern,
                                ([System.Text.RegularExpressions.RegexOptions]::IgnoreCase -bor
                                [System.Text.RegularExpressions.RegexOptions]::IgnorePatternWhitespace)
                            )

                            # return the first capture group if match was successful
                            if ($match.Success) {
                                return $match.Groups[1].Value.Trim()
                            }
                        }
                        catch {

                            # log verbose message on parsing failure
                            Microsoft.PowerShell.Utility\Write-Verbose (
                                "Failed to get description: $($_.Exception.Message)"
                            )
                        }

                        # return empty string if no description found
                        return ''
                    }

                    ###############################################################################
                    <#
                    .SYNOPSIS
                    Helper function to extract function aliases from sourcecode.

                    .DESCRIPTION
                    Parses PowerShell function content to locate and extract alias
                    definitions. Searches for [Alias()] attributes within function
                    declarations and returns them as a clean array of strings.

                    .PARAMETER FileName
                    The path to the script file containing the function to analyze.

                    .PARAMETER FunctionContent
                    The content of the function to parse for alias extraction.
                    #>
                    function Get-FunctionAliases {

                        [CmdletBinding()]
                        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseOutputTypeCorrectly', 'Get-FunctionAliases')]
                        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseSingularNouns', 'Get-FunctionAliases')]
                        [OutputType([string])]

                        param(
                            [Parameter(
                                Position = 0,
                                Mandatory = $true
                            )]
                            [string]$FileName,

                            [Parameter(
                                Position = 1,
                                Mandatory = $true
                            )]
                            [AllowEmptyString()]
                            [AllowNull()]
                            [string]$FunctionContent
                        )

                        # ensure function content is never null for string operations
                        $FunctionContent = $null -eq $FunctionContent ? '' : $FunctionContent

                        try {

                            # convert content to lowercase for case-insensitive searching
                            $content = $FunctionContent.ToLowerInvariant();

                            # find the end of comment-based help section
                            [int] $i = $content.IndexOf('#>');

                            # locate the function declaration after help section
                            $i = $content.IndexOf(
                                "function $([IO.Path]::GetFileNameWithoutExtension($FileName).ToLowerInvariant())", [Math]::Max(0, $i))

                            # fallback to cmdletbinding if function declaration not found
                            if ($i -lt 0) {
                                $i = $content.IndexOf('[cmdletbinding(', [Math]::Max(0, $i))
                                if ($i -lt 0) {
                                    $i = 0
                                }
                            }

                            # find the parameter section start
                            $i2 = $content.IndexOf('param(', [Math]::Max(0, $i))
                            if ($i2 -lt 0) {

                                # fallback to first parameter attribute
                                $i2 = $content.ToLowerInvariant().IndexOf('[parameter', [Math]::Max(0, $i))
                            }

                            # look for alias attribute between function and parameters
                            $i = $content.IndexOf('[alias(', [Math]::Max(0, $i))

                            # extract aliases if found in correct position
                            if ($i -ge 0 -and $i2 -gt $i) {

                                # extract the alias definition content
                                $aliases = $content.Substring($i + 7)

                                # get content until closing parenthesis
                                $aliases = $aliases.Substring(0, $aliases.IndexOf(')')).Replace(
                                    "'", "`"")

                                # clean up brackets and quotes from alias string
                                $aliases = $aliases -replace "[\)\[\]\`"]", ''

                                # split aliases and trim whitespace
                                return (@($aliases -split ',') |
                                        Microsoft.PowerShell.Core\ForEach-Object { $_.Trim() })
                                }
                        }
                        catch {

                            # log verbose message on parsing failure
                            Microsoft.PowerShell.Utility\Write-Verbose (
                                "Failed to get aliases: $($_.Exception.Message)"
                            )
                        }

                        # return empty string if no aliases found
                        return [string]::Empty
                    }

                    ###############################################################################
                    <#
                    .SYNOPSIS
                    Helper function to find the starting line number of a function.

                    .DESCRIPTION
                    Analyzes function content to determine the approximate starting line
                    number by searching for key markers like comment-based help closure,
                    cmdletbinding attributes, or function blocks.

                    .PARAMETER Content
                    The content to search for the function start position markers.
                    #>
                    function Get-FunctionStartLine {

                        [CmdletBinding()]
                        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseOutputTypeCorrectly', 'Get-FunctionStartLine')]
                        [OutputType([int])]

                        param(
                            [Parameter(
                                Position = 0,
                                Mandatory = $true
                            )]
                            [AllowEmptyString()]
                            [AllowNull()]
                            [string]$Content
                        )

                        # ensure content is never null for string operations
                        $Content = ($null -eq $Content) ? '' : $Content

                        # search for function start markers in order of preference
                        $lineNo = $Content.IndexOf('#>')
                        if ($lineNo -lt 0) { $lineNo = $Content.IndexOf('[CmdletBinding') }
                        if ($lineNo -lt 0) { $lineNo = $Content.IndexOf('params') }
                        if ($lineNo -lt 0) { $lineNo = $Content.IndexOf('begin {') }
                        if ($lineNo -lt 0) {
                            $lineNo = $Content.IndexOf('process {')
                        }
                        if ($lineNo -lt 0) { $lineNo = 0 }
                        else {
                            # calculate line number by counting newlines before marker
                            $lineNo = $Content.Substring(0, $lineNo).Split("`n").Count + 1
                        }

                        return $lineNo
                    }

                    # extract the cmdlet name from the current file being processed
                    $cName = [IO.Path]::GetFileNameWithoutExtension($_.Name)

                    # get command help information for the cmdlet
                    $help = @(@($isScriptsFolder ?
                            (Microsoft.PowerShell.Core\Get-Command -Name $_.FullName -ErrorAction SilentlyContinue) :
                            (Microsoft.PowerShell.Core\Get-Command -Name "$($Module.Name)\$($cName)" -ErrorAction SilentlyContinue)) |
                            Microsoft.PowerShell.Utility\Select-Object -first 1 -ErrorAction SilentlyContinue)

                    # determine description from help or extract from source code
                    $description = (
                        $help -and (-not [String]::IsNullOrWhiteSpace($help.ResolvedCommand.Description)) ?
                        $help.ResolvedCommand.Description :
                        (
                            Get-FunctionDescription -FileName $_.FullName `
                                -FunctionContent (
                                [IO.File]::ReadAllText($_.FullName )
                            )
                        )
                    )

                    # collect all aliases for this cmdlet from different sources
                    $aliases = @(
                        @(
                            Microsoft.PowerShell.Utility\Get-Alias -Definition $_.FullName -ErrorAction SilentlyContinue
                        ) + @(
                            Microsoft.PowerShell.Utility\Get-Alias -Definition "$($cName)" -ErrorAction SilentlyContinue
                        )
                    ) |
                        Microsoft.PowerShell.Core\Where-Object {

                            # filter aliases to only those from the current module
                            $_.Source -eq $Module.Name
                        } |
                        Microsoft.PowerShell.Core\ForEach-Object "Name";

                    # determine if cmdlet matches search criteria
                    $found = (-not $CmdletName) -or ($CmdletName.Length -eq 0)

                    # check if cmdlet name matches any provided search patterns
                    if (-not $found) {

                        foreach ($c in $CmdletName) {

                            $n = $c

                            # automatically add wildcards for non-exact matches
                            if ((-not $ExactMatch) -and ($n.IndexOfAny('*?[]'.ToCharArray()) -lt 0)) {

                                $n = "*$n*"
                            }

                            # check for module-qualified name match
                            if ($n.Contains('\')) {

                                if ("$($Module.Name)\$($cName)" -Like $n) {
                                    $found = $true
                                    break;
                                }
                            }
                            else {
                                # check for simple cmdlet name match
                                if ("$($cName)" -Like $n) {
                                    $found = $true
                                    break;
                                }
                            }
                        }
                    }

                    # check alias matches if cmdlet name didn't match
                    if (-not $found) {

                        foreach ($a in $aliases) {

                            if ($a -Like $CmdletName) {
                                $found = $true
                                break;
                            }
                        }
                    }

                    # apply definition matching filter if specified
                    if ($found -and $DefinitionMatches) {

                        $found = ($help ? $help.ResolvedCommand.Definition : [IO.File]::ReadAllText($_.FullName)) -match $DefinitionMatches
                    }

                    # skip this cmdlet if it doesn't match search criteria
                    if (-not $found) { return }

                    $moduleN = (
                        $isScriptsFolder ?
                        "GenXdev.Scripts" :
                        [IO.Path]::GetFileName([IO.Path]::GetDirectoryName($_.FullName))
                    )

                    # create cmdlet information object and add to collection
                    $cmdletInfo = [GenXdev.Helpers.GenXdevCmdletInfo]@{
                        Name               = $cName
                        ModuleName         = $moduleN
                        BaseModule         = $($Module.Name)
                        LineNo             = 0
                        Description        = $description
                        Aliases            = $aliases -join ', '
                        ScriptFilePath     = $_.FullName
                        ScriptTestFilePath = (
                            $isScriptsFolder ?
                            (GenXdev.FileSystem\Expand-Path "$([IO.Path]::GetDirectoryName($_.FullName))\$($cName).Tests.ps1") :
                            (GenXdev.FileSystem\Expand-Path "$([IO.Path]::GetDirectoryName($_.FullName))\..\..\Tests\$moduleN\$($cName).Tests.ps1")
                        )
                    }

                    # add to collection
                    $null = $cmdletCollection.Add($cmdletInfo)
                }
                catch {

                    # log warning for any file processing errors
                    Microsoft.PowerShell.Utility\Write-Warning (
                        "Error processing file '$($_.FullName)': $($_.Exception.Message)"
                    )
                }
            }
        }
    }

    end {

        # restore the original location to prevent side effects
        Microsoft.PowerShell.Management\Set-Location -LiteralPath $originalLocation

        # output verbose completion message
        Microsoft.PowerShell.Utility\Write-Verbose (
            "Completed cmdlet search and restored original location"
        )

        # return results based on OnlyReturnModuleNames parameter
        if ($OnlyReturnModuleNames) {

            # return unique BaseModule names
            $cmdletCollection |
                Microsoft.PowerShell.Core\ForEach-Object { $_.BaseModule } |
                Microsoft.PowerShell.Utility\Select-Object -Unique |
                Microsoft.PowerShell.Utility\Sort-Object {
                    "$($_.BaseModule.Length.ToString("00_"))$($_.BaseModule)"
                } -Descending

        }
        else {

            # return full cmdlet information objects
            $cmdletCollection
        }
    }
}
###############################################################################