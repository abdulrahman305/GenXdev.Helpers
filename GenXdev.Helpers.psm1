if (-not $IsWindows) {
    throw "This module only supports Windows 10+ x64 with PowerShell 7.5+ x64"
}

$osVersion = [System.Environment]::OSVersion.Version
$major = $osVersion.Major
$build = $osVersion.Build

if ($major -ne 10) {
    throw "This module only supports Windows 10+ x64 with PowerShell 7.5+ x64"
}


. "$PSScriptRoot\Functions\GenXdev.Helpers\alignScript.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\ConvertTo-HashTable.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\ConvertTo-JsonEx.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Copy-IdenticalParamValues.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\EnsureGenXdev.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Get-DefaultWebLanguage.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Get-GenXDevCmdlets.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Get-ImageGeolocation.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Get-WebLanguageDictionary.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Import-GenXdevModules.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Initialize-SearchPaths.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Invoke-OnEachGenXdevModule.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Out-Serial.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Remove-JSONComments.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Show-GenXDevCmdlets.ps1"
. "$PSScriptRoot\Functions\GenXdev.Helpers\Show-Verb.ps1"
