function Invoke-OnEachGenXdevModule {

    [CmdletBinding()]
    [Alias("foreach-genxdev-module-do")]
    param(
        [Parameter(
            Mandatory = $true,
            Position = 0,
            HelpMessage = "The script block to execute for each GenXdev module"
        )]
        [Alias("ScriptBlock")]
        [scriptblock] $Script,

        [Parameter(
            Mandatory = $false,
            Position = 1,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true,
            HelpMessage = "Filter to apply to module names"
        )]
        [ValidateNotNullOrEmpty()]
        [Alias("Module", "ModuleName")]
        [SupportsWildcards()]
        [string[]] $BaseModuleName = @("GenXdev*"),

        [Parameter(Mandatory = $false)]
        [switch] $NoLocal,

        [Parameter(Mandatory = $false)]
        [switch] $OnlyPublished,

        [Parameter(Mandatory = $false)]
        [switch] $FromScripts
    )

    begin {
        $ScriptsPath = GenXdev.FileSystem\Expand-Path "$PSScriptRoot\..\..\..\..\..\Scripts\" -CreateDirectory
        $modulesPath = GenXdev.FileSystem\Expand-Path "$PSScriptRoot\..\..\..\..\" -CreateDirectory

        if ($FromScripts) {

            $BaseModuleName = @("GenXdev.Scripts")
        }
    }

    process {

        foreach ($ModuleName in $BaseModuleName) {

            function go {
                param($module)

                $licenseFilePath = "$($module.FullName)\1.124.2025\LICENSE"
                $readmeFilePath = "$($module.FullName)\1.124.2025\README.md"

                if ($module.FullName -eq $scriptsPath) {

                    $licenseFilePath = $null
                    $readmeFilePath = $null
                }

                if (($module.Name -like 'GenXdev.*') -and
                        ((-not $NoLocal) -or ($module.Name -notlike '*.Local*')) -and
                        ((-not $OnlyPublished) -or (
                                    ($module.Name -notlike '*Local*') -and
                                    ($module.Name -notlike '*Local') -and
                                    ($module.Name -notlike 'GenXdev.Scripts') -and
                        [IO.File]::Exists($licenseFilePath) -and
                        [IO.File]::Exists($readmeFilePath)
                    ))) {
                    $location = (Get-Location).Path
                    try {
                        if ($module.Name -like "GenXdev.Scripts") {

                            Set-Location $ScriptsPath
                        }
                        else {

                            Set-Location "$($module.FullName)"

                            $newLocation = Get-ChildItem ".\*.*.*" -dir -ErrorAction SilentlyContinue |
                            Sort-Object -Property Name -Descending |
                            Select-Object -First 1 |
                            ForEach-Object {

                                $_.FullName
                            }

                            if ($null -eq $newLocation) {

                                return;
                            }

                            Set-Location $newLocation
                        }

                        . Invoke-Command -ScriptBlock $Script -ArgumentList $module -NoNewScope
                    }
                    finally {

                        Set-Location $location
                    }
                }
            }

            if ($ModuleName -like "GenXdev.Scripts") {

                go -module @{
                    Name     = "GenXdev.Scripts"
                    FullName = $ScriptsPath
                }

                continue;
            }

            @(Get-ChildItem "$modulesPath\GenXdev*" -dir -Force -ErrorAction SilentlyContinue) |
            ForEach-Object {

                if ($_.Name -like $ModuleName) {

                    try {
                        go -module $_
                    }
                    catch {

                        Write-Error $_.Exception.Message
                    }
                }
            }
        }
    }

    end {
    }
}