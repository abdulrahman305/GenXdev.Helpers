if (-not $IsWindows) {
    throw "This module only supports Windows 10+ x64 with PowerShell 7.5+ x64"
}

$osVersion = [System.Environment]::OSVersion.Version
$major = $osVersion.Major

if ($major -ne 10) {
    throw "This module only supports Windows 10+ x64 with PowerShell 7.5+ x64"
}



. "$PSScriptRoot\Functions\GenXdev.Helpers\_EnsureTypes.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\alignScript.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\EnsureGenXdev.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\EnsureNuGetAssembly.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Get-GenXDevCmdlet.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Get-ImageMetadata.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Import-GenXdevModules.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Invoke-OnEachGenXdevModule.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Out-Serial.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\resetdefaultmonitor.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\SecondScreen.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Show-GenXDevCmdlet.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Show-Verb.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\SideBySide.ps1"
