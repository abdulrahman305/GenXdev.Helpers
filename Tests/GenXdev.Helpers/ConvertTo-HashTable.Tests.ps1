Pester\Describe "ConvertTo-HashTable function tests" {

    Pester\BeforeAll {

        $testObject = @(
            @{
                Name  = "Test"
                Value = 123
                Sub   = @{

                    Name  = "SubTest"
                    Value = 456
                }
            } | Microsoft.PowerShell.Utility\ConvertTo-Json -Compress | Microsoft.PowerShell.Utility\ConvertFrom-Json
        )

        $testArray = @(
            @{

                Name  = "Item1"
                Value = 1
            },
            @{

                Name  = "Item2"
                Value = 2
            }
        ) | Microsoft.PowerShell.Utility\ConvertTo-Json -Compress | Microsoft.PowerShell.Utility\ConvertFrom-Json
    }

    Pester\It "Should pass PSScriptAnalyzer rules" {

        # get the script path for analysis
        $scriptPath = GenXdev.FileSystem\Expand-Path "$PSScriptRoot\..\..\Functions\GenXdev.Helpers\ConvertTo-HashTable.ps1"

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

    Pester\Context "Basic functionality" {

        Pester\It "Should convert PSCustomObject to HashTable" {

            # convert test object to hashtable
            $result = GenXdev.Helpers\ConvertTo-HashTable -InputObject $testObject

            # verify result is hashtable
            $result | Pester\Should -BeOfType [System.Collections.Hashtable]

            # verify properties are correctly converted
            $result.Name | Pester\Should -Be "Test"
            $result.Value | Pester\Should -Be 123
        }

        Pester\It "Should convert array of PSCustomObjects to array of HashTables" {

            # convert test array to hashtable array
            $result = GenXdev.Helpers\ConvertTo-HashTable -InputObject $testArray

            # verify result is array
            $result | Pester\Should -BeOfType [System.Collections.IEnumerable]

            # verify array items are hashtables
            $result[0] | Pester\Should -BeOfType [System.Collections.Hashtable]
            $result[1] | Pester\Should -BeOfType [System.Collections.Hashtable]

            # verify properties are correctly converted
            $result[0].Name | Pester\Should -Be "Item1"
            $result[0].Value | Pester\Should -Be 1
            $result[1].Name | Pester\Should -Be "Item2"
            $result[1].Value | Pester\Should -Be 2
        }
    }

    Pester\Context "Pipeline input" {

        Pester\It "Should accept pipeline input" {

            # test pipeline input
            $result = $testObject | GenXdev.Helpers\ConvertTo-HashTable

            # verify result
            $result | Pester\Should -BeOfType [System.Collections.Hashtable]
            $result.Name | Pester\Should -Be "Test"
        }
    }
}

################################################################################