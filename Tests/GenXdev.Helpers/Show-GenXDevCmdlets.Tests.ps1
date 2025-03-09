################################################################################
Describe "Show-GenXDevCmdlets" {

    BeforeAll {
        # get the script path for analysis
        $Script:scriptPath = GenXdev.FileSystem\Expand-Path `
            "$PSScriptRoot\..\..\Functions\GenXdev.Helpers\Show-GenXDevCmdlets.ps1"
    }

    It "should pass PSScriptAnalyzer rules" {

        # run analyzer with explicit settings
        $analyzerResults = GenXdev.Coding\Invoke-GenXdevScriptAnalyzer `
            -Path $Script:scriptPath `
            -ErrorAction SilentlyContinue

        # are there any errors?
        if ($null -ne $analyzerResults -and ($analyzerResults.Length -gt 0)) {

            $analyzerResults | ForEach-Object {

                # suppress the PSUseSingularNouns rule for this test
                if ($_.RuleName -ne "PSUseSingularNouns") {
                    $_.RuleName | Should -Be $_.Message
                }
            }
        }
    }
}
################################################################################
