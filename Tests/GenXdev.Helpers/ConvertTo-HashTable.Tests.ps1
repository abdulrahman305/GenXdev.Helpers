Describe "ConvertTo-HashTable function tests" {

    BeforeAll {

        $testObject = @(
            @{
                Name  = "Test"
                Value = 123
                Sub   = @{

                    Name  = "SubTest"
                    Value = 456
                }
            } | ConvertTo-Json -Compress | ConvertFrom-Json
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
        ) | ConvertTo-Json -Compress | ConvertFrom-Json
    }

    It "should pass PSScriptAnalyzer rules" {

        # get the script path for analysis
        $scriptPath = GenXdev.FileSystem\Expand-Path "$PSScriptRoot\..\..\Functions\GenXdev.Helpers\ConvertTo-HashTable.ps1"

        # run analyzer with explicit settings
        $analyzerResults = GenXdev.Coding\Invoke-GenXdevScriptAnalyzer `
            -Path $scriptPath

        [string] $message = ""
        $analyzerResults | ForEach-Object {

            $message = $message + @"
--------------------------------------------------
Rule: $($_.RuleName)`
Description: $($_.Description)
Message: $($_.Message)
`r`n
"@
        }

        $analyzerResults.Count | Should -Be 0 -Because @"
The following PSScriptAnalyzer rules are being violated:
$message
"@;
    }

    Context "Basic functionality" {

        It "Should convert PSCustomObject to HashTable" {

            # convert test object to hashtable
            $result = ConvertTo-HashTable -InputObject $testObject

            # verify result is hashtable
            $result | Should -BeOfType [System.Collections.Hashtable]

            # verify properties are correctly converted
            $result.Name | Should -Be "Test"
            $result.Value | Should -Be 123
        }

        It "Should convert array of PSCustomObjects to array of HashTables" {

            # convert test array to hashtable array
            $result = ConvertTo-HashTable -InputObject $testArray

            # verify result is array
            $result | Should -BeOfType [System.Collections.IEnumerable]

            # verify array items are hashtables
            $result[0] | Should -BeOfType [System.Collections.Hashtable]
            $result[1] | Should -BeOfType [System.Collections.Hashtable]

            # verify properties are correctly converted
            $result[0].Name | Should -Be "Item1"
            $result[0].Value | Should -Be 1
            $result[1].Name | Should -Be "Item2"
            $result[1].Value | Should -Be 2
        }
    }

    Context "Pipeline input" {

        It "Should accept pipeline input" {

            # test pipeline input
            $result = $testObject | ConvertTo-HashTable

            # verify result
            $result | Should -BeOfType [System.Collections.Hashtable]
            $result.Name | Should -Be "Test"
        }
    }
}

################################################################################
