################################################################################
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

    [CmdletBinding(DefaultParameterSetName = "Default")]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSUseSingularNouns", "")]
    param(
        [Parameter(Position = 0)]
        [string] $WorkspaceFolder = "$PSScriptRoot\..\..\..\..\..\"
    )

    begin {

        Write-Verbose "Initializing search paths collection"
    }

    process {

        # create a new list to store unique search paths
        $searchPaths = [System.Collections.Generic.List[string]] (@(
                # add system and development tool paths
                (GenXdev.FileSystem\Expand-Path "${env:ProgramData}\chocolatey\bin\")
                (GenXdev.FileSystem\Expand-Path "$WorkspaceFolder\node_modules\.bin"),
                (GenXdev.FileSystem\Expand-Path "$WorkspaceFolder\powershell"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\Git\cmd"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\nodejs"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\Google\Chrome\Application"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\Microsoft VS Code\bin"),
                (GenXdev.FileSystem\Expand-Path "${env:ProgramFiles}\dotnet")
            ) + @(
                # add paths from GenXdev packages
                $GenXdevPackages |
                ForEach-Object {
                    if (-not [string]::IsNullOrWhiteSpace($PSItem.searchpath)) {
                        # escape special characters in path
                        $path = $PSItem.searchpath.replace('`', '``').replace(
                            '"', '`"')

                        # evaluate any variables in the path
                        $path = Invoke-Expression "`"$path`""

                        # convert to full path
                        GenXdev.FileSystem\Expand-Path $path
                    }
                }
            ))

        # process existing PATH entries
        Write-Verbose "Processing existing PATH environment entries"
        @($env:Path.Split(';')) |
        ForEach-Object -Process {
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
                    Write-Host "Could not parse path: $PSItem"
                }
            }
        }

        # update system PATH with consolidated search paths
        Write-Verbose "Updating system PATH environment variable"
        $env:Path = [string]::Join(";", $searchPaths)
    }

    end {
    }
}
################################################################################
