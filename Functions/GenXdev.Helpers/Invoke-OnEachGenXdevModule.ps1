<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Invoke-OnEachGenXdevModule.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.284.2025
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
################################################################################
<#
.SYNOPSIS
Executes a script block on each GenXdev module in the workspace.

.DESCRIPTION
This function iterates through GenXdev modules in the workspace and executes
a provided script block against each module. It can filter modules by name
pattern, exclude local modules, include only published modules, or process
scripts instead of modules. The function automatically navigates to the
correct module directory before executing the script block.

.PARAMETER Script
The script block to execute for each GenXdev module. The module object is
passed as an argument to the script block.

.PARAMETER ModuleName
Filter to apply to module names. Supports wildcards and multiple patterns.
Defaults to 'GenXdev*' to include all GenXdev modules.

.PARAMETER NoLocal
Excludes local development modules from processing.

.PARAMETER OnlyPublished
Includes only published modules that have LICENSE and README.md files.

.PARAMETER FromScripts
Process scripts directory instead of module directories.

.PARAMETER IncludeScripts
Includes the scripts directory in addition to regular modules.

.PARAMETER IncludeGenXdevMainModule
Includes the main GenXdev module in addition to sub-modules.

.EXAMPLE
Invoke-OnEachGenXdevModule -Script { Write-Host $args[0].Name }

Lists all GenXdev module names.

.EXAMPLE
foreach-genxdev-module-do {
    param($ModuleObj, $isScriptsFolder, $isSubModule, $subModuleName)
    Get-ChildItem
} -ModuleName "GenXdev.AI"

Uses alias to list contents of the GenXdev.AI module directory.
#>
function Invoke-OnEachGenXdevModule {

    [CmdletBinding()]
    [Alias('foreach-genxdev-module-do')]

    param(
        ###############################################################################
        [Parameter(
            Mandatory = $true,
            Position = 0,
            HelpMessage = ('The script block to execute for each ' +
                          'GenXdev module')
        )]
        [Alias('ScriptBlock')]
        [scriptblock] $Script,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            Position = 1,
            HelpMessage = 'Filter to apply to module names'
        )]
        [ValidateNotNullOrEmpty()]
        [Alias('Module', 'BaseModuleName', 'SubModuleName')]
        [ValidatePattern('^(GenXdev|GenXde[v]\*|GenXdev(\.[\w\*\[\]\?]*)+)+$')]
        [SupportsWildcards()]
        [string[]] $ModuleName = @('GenXdev*'),
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Excludes local development modules from processing'
        )]
        [switch] $NoLocal,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = ('Includes only published modules that have LICENSE ' +
                          'and README.md files')
        )]
        [switch] $OnlyPublished,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = 'Process scripts directory instead of module directories'
        )]
        [switch] $FromScripts,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = ('Includes the scripts directory in addition to ' +
                          'regular modules')
        )]
        [switch] $IncludeScripts,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = ('Includes the main GenXdev module in addition to ' +
                          'sub-modules')
        )]
        [switch] $IncludeGenXdevMainModule
    )

    begin {

        # resolve the scripts directory path relative to current module
        $scriptsPath = GenXdev.FileSystem\Expand-Path `
            "$PSScriptRoot\..\..\..\..\..\Scripts\" `
            -CreateDirectory

        # resolve the modules directory path relative to current module
        $modulesPath = GenXdev.FileSystem\Expand-Path `
            "$PSScriptRoot\..\..\..\..\" `
            -CreateDirectory

        # override module name filter when processing scripts only
        if ($FromScripts) {

            $ModuleName = @('GenXdev.Scripts')
        }

        # output progress information for troubleshooting
        Microsoft.PowerShell.Utility\Write-Verbose (
            "Processing modules with base names: $($ModuleName -join ', ')")

        $allGenXdevModules = @(GenXdev.FileSystem\Find-Item "$modulesPath\GenXdev*\1.284.2025\GenXdev*.psm1" -PassThru -NoRecurse | Microsoft.PowerShell.Core\ForEach-Object { [IO.Path]::GetFileNameWithoutExtension($_.FullName) }) +
            @("GenXdev.Scripts")

        $genXdevModules = $allGenXdevModules

        if ($ModuleName.Count -eq 1 -and $ModuleName[0] -eq 'GenXdev*') {

            $genXdevModules = @($allGenXdevModules | Microsoft.PowerShell.Core\ForEach-Object {

                if ((-not ($IncludeScripts -or $FromScripts)) -and ($_ -like "GenXdev.Scripts")) {

                    return;
                }

                $parts = $_.Split('.')

                if ((($parts.Length -eq 1) -and $IncludeGenXdevMainModule) -or
                    ($parts.Length -eq 2)) {

                    $_
                }
            })
        }
}

  process {
  }

end {

        # iterate through each specified module name pattern
        $uniqueModulesNames = @($ModuleName | Microsoft.PowerShell.Core\ForEach-Object {

            $moduleToFind = $_

            foreach ($module in $genXdevModules) {

                if ($module -like $moduleToFind) {

                    $module
                }
            }
        } |
        Microsoft.PowerShell.Utility\Select-Object -Unique |
        Microsoft.PowerShell.Utility\Sort-Object {

            $_.Length.ToString().PadLeft(5, '0')+"_"+$_.ToLower()
        })

        foreach ($nextModule in $uniqueModulesNames) {

            # split module name into parts for pattern matching
            $partsObj = $nextModule.Split(".")

            # output current module pattern being processed
            Microsoft.PowerShell.Utility\Write-Verbose (
                "Processing module pattern: $nextModule")

            # define internal function to process individual modules
            function processModule {

                param($ModuleObj, $parts)

                # construct expected license file path for published modules
                $licenseFilePath = (
                    "$($ModuleObj.FullName)\1.284.2025\LICENSE")

                # construct expected readme file path for published modules
                $readmeFilePath = (
                    "$($ModuleObj.FullName)\1.284.2025\README.md")

                # skip license and readme checks for scripts directory
                if ($ModuleObj.FullName -eq $scriptsPath) {

                    $licenseFilePath = $null
                    $readmeFilePath = $null
                }

                # determine if module should be processed based on all filters
                $shouldProcess = (
                    ($ModuleObj.Name -like (
                        $IncludeGenXdevMainModule ? 'GenXdev*' : 'GenXdev.*')) -and
                    ((-not $NoLocal) -or ($ModuleObj.Name -notlike '*.Local*')) -and
                    ((-not $OnlyPublished) -or (
                        ($ModuleObj.Name -notlike '*Local*') -and
                        ($ModuleObj.Name -notlike '*Local') -and
                        ($ModuleObj.Name -notlike 'GenXdev.Scripts') -and
                        [IO.File]::Exists($licenseFilePath) -and
                        [IO.File]::Exists($readmeFilePath))))

                # process module if it passes all filters
                if ($shouldProcess) {

                    # output module name being processed
                    Microsoft.PowerShell.Utility\Write-Verbose (
                        "Processing module: $($ModuleObj.Name)")

                    # save current location to restore after processing
                    $originalLocation = (
                        Microsoft.PowerShell.Management\Get-Location).Path

                    try {

                        # handle scripts directory with special logic
                        if ($ModuleObj.Name -like 'GenXdev.Scripts') {

                            # navigate directly to scripts directory
                            Microsoft.PowerShell.Management\Set-Location `
                                -LiteralPath $scriptsPath
                        }
                        else {

                            # initialize version variable for parsing
                            [version] $version = $null

                            # navigate to module root directory first
                            Microsoft.PowerShell.Management\Set-Location `
                                -LiteralPath "$($ModuleObj.FullName)"

                            # find the latest version directory for the module
                            $versionDirectory = (
                                Microsoft.PowerShell.Management\Get-ChildItem `
                                    -LiteralPath '.\' `
                                    -Filter "*.*.*" `
                                    -Directory `
                                    -ErrorAction SilentlyContinue |
                                Microsoft.PowerShell.Core\Where-Object {
                                    [Version]::TryParse($_.Name, [ref]$version)
                                } |
                                Microsoft.PowerShell.Utility\Sort-Object {
                                    [Version]::new($_.Name)
                                } -Descending |
                                Microsoft.PowerShell.Utility\Select-Object `
                                    -First 1 |
                                Microsoft.PowerShell.Core\ForEach-Object {
                                    $_.FullName
                                })

                            # skip module if no valid version directory found
                            if ($null -eq $versionDirectory) {

                                # output warning for modules without version dirs
                                Microsoft.PowerShell.Utility\Write-Verbose (
                                    "No version directory found for " +
                                    "$($ModuleObj.Name)")

                                return
                            }

                            # navigate to the specific version directory
                            Microsoft.PowerShell.Management\Set-Location `
                                -LiteralPath $versionDirectory

                            # output current working directory for debugging
                            Microsoft.PowerShell.Utility\Write-Verbose (
                                "Changed to directory: $versionDirectory")
                        }

                        Microsoft.PowerShell.Utility\Write-Progress `
                            -Activity "Processing modules" `
                            -CurrentOperation "Processing $($ModuleObj.Name)" `
                            -Status "Executing script block"

                        # execute the user script with module context parameters
                        Microsoft.PowerShell.Core\Invoke-Command `
                            -ScriptBlock $Script `
                            -ArgumentList @(
                                ($ModuleObj | Microsoft.PowerShell.Utility\ConvertTo-Json | Microsoft.PowerShell.Utility\ConvertFrom-Json),
                                ($ModuleObj.Name -eq "GenXdev.Scripts"),
                                ($parts.Length -gt 2),
                                ($parts -join '.')) `
                            -NoNewScope
                    }
                    finally {

                        # always restore original working directory
                        Microsoft.PowerShell.Management\Set-Location `
                            -LiteralPath $originalLocation
                    }
                }
                else {

                    # output information about skipped modules
                    Microsoft.PowerShell.Utility\Write-Verbose (
                        "Skipping module: $($ModuleObj.Name) (filtered out)")
                }
            }

            # handle scripts directory as a special case
            if ($nextModule -like 'GenXdev.Scripts') {

                # create mock module object for scripts directory
                processModule -parts $partsObj -ModuleObj @{
                    Name     = 'GenXdev.Scripts'
                    FullName = $scriptsPath
                }

                continue
            }

            try {

                # process the matching module
                processModule -parts $partsObj -ModuleObj @{
                    Name     = ($partsObj.Length -lt 2 ? $partsObj[0] : ($partsObj[0]+"."+$partsObj[1]))
                    FullName = "$modulesPath\$($partsObj.Length -lt 2 ? $partsObj[0] : ($partsObj[0]+"."+$partsObj[1]))"
                }
            }
            catch {

                # output any errors encountered during processing
                Microsoft.PowerShell.Utility\Write-Error `
                    $_.Exception.Message
            }
        }

        Microsoft.PowerShell.Utility\Write-Progress `
            -Activity "Processing modules" `
            -Status "Completed" `
            -Completed
    }
}
################################################################################