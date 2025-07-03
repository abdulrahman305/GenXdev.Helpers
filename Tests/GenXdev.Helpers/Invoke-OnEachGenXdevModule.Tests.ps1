###############################################################################
Pester\Describe "Invoke-OnEachGenXdevModule.Tests" {

    Pester\It "Should pass PSScriptAnalyzer rules" {

# get the script path for analysis
        $FullModuleName = "GenXdev.Helpers"
        $scriptPath = GenXdev.FileSystem\Expand-Path "$PSScriptRoot\..\..\Functions\$FullModuleName\Invoke-OnEachGenXdevModule.ps1"

# run analyzer with explicit settings
        $analyzerResults = GenXdev.Coding\Invoke-GenXdevScriptAnalyzer `
            -Path $scriptPath

        [string] $message = ""
        $analyzerResults | Microsoft.PowerShell.Core\ForEach-Object {

            $message = $message + @"
--------------------------------------------------
Rule: $($_.RuleName)`
Description: $($_.Description)
Message: $($_.Message)
`r`n
"@
        }

        $analyzerResults.Count | Pester\Should -Be 0 -Because @"
The following PSScriptAnalyzer rules are being violated:
$message
"@;
    }
}
###############################################################################