Pester\Describe 'Get-GenXDevCmdlet.Tests' {

    Pester\It 'Should find certain cmdlets' {

        # run the script to get the cmdlets

        GenXdev.Helpers\Get-GenXDevCmdlet gcmds -ExactMatch | Microsoft.PowerShell.Core\ForEach-Object Name | Pester\Should -Contain 'Get-GenXDevCmdlet'
        GenXdev.Helpers\Get-GenXDevCmdlet refactors -ExactMatch | Microsoft.PowerShell.Core\ForEach-Object Name | Pester\Should -Contain 'Get-Refactor'
    }
}