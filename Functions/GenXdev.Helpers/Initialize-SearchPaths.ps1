<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Initialize-SearchPaths.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.280.2025
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
Initializes and configures system search paths for package management.

.DESCRIPTION
This function builds a comprehensive list of search paths by combining default
system locations, chocolatey paths, development tool paths, and custom package
paths. It then updates the system's PATH environment variable with these
consolidated paths.

.PARAMETER WorkspaceFolder
The workspace folder path to use for node modules and PowerShell paths.

.EXAMPLE
Initialize-SearchPaths -WorkspaceFolder "C:\workspace"
#>
function Initialize-SearchPaths {

    [CmdletBinding()]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseSingularNouns', '')]
    param(
        [Parameter(Position = 0)]
        [string] $WorkspaceFolder = "$PSScriptRoot\..\..\..\..\..\"
    )

    begin {

        Microsoft.PowerShell.Utility\Write-Verbose 'Initializing search paths collection'
    }


    process {

        # create a new list to store unique search paths
        $searchPaths = [System.Collections.Generic.List[string]] (@(
                # add system and development tool paths
                (GenXdev.FileSystem\Expand-Path "${env:ProgramData}\chocolatey\bin\")
                (GenXdev.FileSystem\Expand-Path "$WorkspaceFolder\node_modules\.bin"),
                (GenXdev.FileSystem\Expand-Path "$WorkspaceFolder\scripts"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\Git\cmd"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\nodejs"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\Google\Chrome\Application"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\Microsoft VS Code\bin"),
                (GenXdev.FileSystem\Expand-Path "${env:LOCALAPPDATA}Programs\Microsoft VS Code Insiders"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\dotnet")
            ) + @(
                # add paths from GenXdev packages
                $GenXdevPackages |
                    Microsoft.PowerShell.Core\ForEach-Object {
                        if (-not [string]::IsNullOrWhiteSpace($PSItem.searchpath)) {
                            # escape special characters in path
                            $path = $PSItem.searchpath.replace('`', '``').replace(
                                '"', '`"')

                            # evaluate any variables in the path
                            $path = Microsoft.PowerShell.Utility\Invoke-Expression "`"$path`""

                            # convert to full path
                            GenXdev.FileSystem\Expand-Path $path
                        }
                    }
            ))

        # process existing PATH entries
        Microsoft.PowerShell.Utility\Write-Verbose 'Processing existing PATH environment entries'
        @($env:Path.Split(';')) |
            Microsoft.PowerShell.Core\ForEach-Object {

                $path = $PSItem

                if ([String]::IsNullOrWhiteSpace($path) -eq $false) {
                    try {
                        # convert to full path
                        $fullPath = GenXdev.FileSystem\Expand-Path $path

                        # add path if not already present
                        if ($searchPaths.IndexOf($fullPath) -lt 0) {
                            $null = $searchPaths.Add($fullPath)
                        }
                    }
                    catch {
                        Microsoft.PowerShell.Utility\Write-Host "Could not parse path: $PSItem"
                    }
                }
            }

        # update system PATH with consolidated search paths
        Microsoft.PowerShell.Utility\Write-Verbose 'Updating system PATH environment variable'
        $env:Path = [string]::Join(';', $searchPaths)
    }

    end {
    }
}