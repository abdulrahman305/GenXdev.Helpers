@('Microsoft.PowerShell.Management', 'Microsoft.PowerShell.Diagnostics', 'Microsoft.PowerShell.Utility') | Microsoft.PowerShell.Core\Import-Module
$extra = [IO.Path]::GetFullPath("$PSScriptRoot\..\..\lib")

if (-not ("$Env:Path".Contains(";$extra"))) {

    $Env:Path = "$($Env:Path);$extra"
}