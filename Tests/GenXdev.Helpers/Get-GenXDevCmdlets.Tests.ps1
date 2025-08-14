Pester\Describe 'Get-GenXDevCmdlets.Tests' {

    Pester\It 'Should find certain cmdlets' {

        # run the script to get the cmdlets

        GenXdev.Helpers\Get-GenXDevCmdlets gcmds -ExactMatch | Microsoft.PowerShell.Core\ForEach-Object Name | Pester\Should -Contain 'Get-GenXDevCmdlets'
        GenXdev.Helpers\Get-GenXDevCmdlets refactors -ExactMatch | Microsoft.PowerShell.Core\ForEach-Object Name | Pester\Should -Contain 'Get-Refactor'
    }
}