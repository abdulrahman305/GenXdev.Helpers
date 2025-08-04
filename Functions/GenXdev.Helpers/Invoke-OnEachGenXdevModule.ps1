function Invoke-OnEachGenXdevModule {

    [CmdletBinding()]
    [Alias('foreach-genxdev-module-do')]
    param(
        [Parameter(
            Mandatory = $true,
            Position = 0,
            HelpMessage = 'The script block to execute for each GenXdev module'
        )]
        [Alias('ScriptBlock')]
        [scriptblock] $Script,

        [Parameter(
            Mandatory = $false,
            Position = 1,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true,
            HelpMessage = 'Filter to apply to module names'
        )]
        [ValidateNotNullOrEmpty()]
        [Alias('Module', 'ModuleName')]
        [ValidatePattern('^(GenXdev|GenXde[v]\*|GenXdev(\.\w+)+)+$')]
        [string[]] $BaseModuleName = @('GenXdev*'),

        [Parameter(Mandatory = $false)]
        [switch] $NoLocal,

        [Parameter(Mandatory = $false)]
        [switch] $OnlyPublished,

        [Parameter(Mandatory = $false)]
        [switch] $FromScripts,

        [Parameter(Mandatory = $false)]
        [switch] $IncludeGenXdevMainModule
    )

    begin {
        $ScriptsPath = GenXdev.FileSystem\Expand-Path "$PSScriptRoot\..\..\..\..\..\Scripts\" -CreateDirectory
        $modulesPath = GenXdev.FileSystem\Expand-Path "$PSScriptRoot\..\..\..\..\" -CreateDirectory

        if ($FromScripts) {

            $BaseModuleName = @('GenXdev.Scripts')
        }
    }


    process {

        foreach ($ModuleName in $BaseModuleName) {

            function goNext {
                param($module)

                $licenseFilePath = "$($module.FullName)\1.230.2025\LICENSE"
                $readmeFilePath = "$($module.FullName)\1.230.2025\README.md"

                if ($module.FullName -eq $scriptsPath) {

                    $licenseFilePath = $null
                    $readmeFilePath = $null
                }

                if (($module.Name -like ($IncludeGenXdevMainModule ? 'GenXdev*' : 'GenXdev.*')) -and
                        ((-not $NoLocal) -or ($module.Name -notlike '*.Local*')) -and
                        ((-not $OnlyPublished) -or (
                                    ($module.Name -notlike '*Local*') -and
                                    ($module.Name -notlike '*Local') -and
                                    ($module.Name -notlike 'GenXdev.Scripts') -and
                        [IO.File]::Exists($licenseFilePath) -and
                        [IO.File]::Exists($readmeFilePath)
                    ))) {
                    $location = (Microsoft.PowerShell.Management\Get-Location).Path
                    try {
                        if ($module.Name -like 'GenXdev.Scripts') {

                            Microsoft.PowerShell.Management\Set-Location -LiteralPath $ScriptsPath
                        }
                        else {
                            [version] $version = $null
                            Microsoft.PowerShell.Management\Set-Location -LiteralPath "$($module.FullName)"

                            $newLocation = Microsoft.PowerShell.Management\Get-ChildItem -LiteralPath '.\' -Filter "*.*.*" -dir -ErrorAction SilentlyContinue |
                                Microsoft.PowerShell.Core\Where-Object {
                                    [Version]::TryParse($_.Name, [ref]$version)
                                } |
                                Microsoft.PowerShell.Utility\Sort-Object {

                                    [Version]::new($_.Name)

                                } -Descending |
                                Microsoft.PowerShell.Utility\Select-Object -First 1 |
                                Microsoft.PowerShell.Core\ForEach-Object {

                                    $_.FullName
                                }

                            if ($null -eq $newLocation) {

                                return;
                            }

                            Microsoft.PowerShell.Management\Set-Location -LiteralPath $newLocation
                        }

                        . Microsoft.PowerShell.Core\Invoke-Command -ScriptBlock $Script -ArgumentList $module -NoNewScope
                    }
                    finally {

                        Microsoft.PowerShell.Management\Set-Location -LiteralPath $location
                    }
                }
            }

            if ($ModuleName -like 'GenXdev.Scripts') {

                goNext -module @{
                    Name     = 'GenXdev.Scripts'
                    FullName = $ScriptsPath
                }

                continue;
            }

            @(Microsoft.PowerShell.Management\Get-ChildItem -LiteralPath ("$modulesPath\") -Filter ($IncludeGenXdevMainModule ? "GenXdev*" : "GenXdev.*") -dir -Force -ErrorAction SilentlyContinue) |
                Microsoft.PowerShell.Utility\Sort-Object { $_.Name.Length } -Descending |
                Microsoft.PowerShell.Core\ForEach-Object {

                    if ($_.Name -like $ModuleName) {

                        try {
                            goNext -module $_
                        }
                        catch {

                            Microsoft.PowerShell.Utility\Write-Error $_.Exception.Message
                        }
                    }
                }
        }
    }

    end {
    }
}