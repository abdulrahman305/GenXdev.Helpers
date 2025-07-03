###############################################################################
###############################################################################Define the full module name for use in paths

Pester\Describe "Get-GenXDevCmdlets.Tests" {

    Pester\It "Should find certain cmdlets" {

# run the script to get the cmdlets

        GenXdev.Helpers\Get-GenXDevCmdlets gcmds | Microsoft.PowerShell.Core\ForEach-Object Name | Pester\Should -Contain "Get-GenXDevCmdlets"
        GenXdev.Helpers\Get-GenXDevCmdlets refactors | Microsoft.PowerShell.Core\ForEach-Object Name | Pester\Should -Contain "Show-RefactorReport"
    }

    Pester\It "Should pass PSScriptAnalyzer rules" {
        $FullModuleName = "GenXdev.Helpers"

# get the script path for analysis
        $scriptPath = GenXdev.FileSystem\Expand-Path "$PSScriptRoot\..\..\Functions\$FullModuleName\Get-GenXDevCmdlets.ps1"

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