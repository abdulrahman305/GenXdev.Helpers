Describe "ConvertTo-HashTable function tests" {

    BeforeAll {
        # import required module
        Import-Module GenXdev.Helpers -Force

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
