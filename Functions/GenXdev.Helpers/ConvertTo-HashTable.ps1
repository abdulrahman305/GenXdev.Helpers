<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : ConvertTo-HashTable.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.290.2025
################################################################################
MIT License

Copyright 2021-2025 GenXdev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
################################################################################>
###############################################################################
<#
.SYNOPSIS
Converts a PSCustomObject to a HashTable recursively.

.DESCRIPTION
This function converts a PSCustomObject and all its nested PSCustomObject
properties into HashTables. It handles arrays and other collection types by
processing each element recursively.

.PARAMETER InputObject
The PSCustomObject to convert into a HashTable. Accepts pipeline input.

.EXAMPLE
$object = [PSCustomObject]@{
    Name = "John"
    Age = 30
    Details = [PSCustomObject]@{
        City = "New York"
    }
}
$hashTable = GenXdev.Helpers\ConvertTo-HashTable -InputObject $object
#>
function ConvertTo-HashTable {

    [CmdletBinding(DefaultParameterSetName = 'Default')]
    [Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseFullyQualifiedCmdletNames', '')]
    [OutputType([hashtable], [System.Collections.IEnumerable], [System.ValueType], [string])]
    param (
        ########################################################################
        [Parameter(
            Mandatory = $true,
            Position = 0,
            ValueFromPipeline = $true,
            ParameterSetName = 'Default',
            HelpMessage = 'The PSCustomObject to convert into a HashTable'
        )]
        [ValidateNotNull()]
        [System.Object[]] $InputObject
        ########################################################################
    )

    begin {

        Microsoft.PowerShell.Utility\Write-Verbose 'Starting PSCustomObject to HashTable conversion'
    }

    process {

        function internalFunction($inputObject) {

            # return empty hashtable if input is null
            if ($null -eq $InputObject) {

                Microsoft.PowerShell.Utility\Write-Verbose 'Input object is null, returning empty hashtable'
                return @{}
            }

            $InputObject | Microsoft.PowerShell.Core\ForEach-Object {

                $currentObject = $_

                # handle simple value types directly
                if ($currentObject -is [System.ValueType] -or
                    $currentObject -is [string]) {

                    Microsoft.PowerShell.Utility\Write-Verbose "Processing simple value: $currentObject"
                    Microsoft.PowerShell.Utility\Write-Output $currentObject
                    return
                }

                # create new hashtable for storing converted properties
                $resultTable = @{}

                # process each property of the current object
                foreach ($property in $currentObject.PSObject.Properties) {

                    Microsoft.PowerShell.Utility\Write-Verbose "Processing property: $($property.Name)"

                    # handle nested PSCustomObject properties
                    if ($property.Value -is [System.Management.Automation.PSCustomObject]) {

                        $resultTable[$property.Name] = internalFunction $property.Value
                    }
                    # handle collection properties
                    elseif ($property.Value -is [System.Collections.IEnumerable] -and
                        $property.Value -isnot [string]) {

                        $resultTable[$property.Name] = @(
                            $property.Value | Microsoft.PowerShell.Core\ForEach-Object {

                                if ($_ -is [System.Management.Automation.PSCustomObject]) {
                                    internalFunction $_
                                }
                                else {
                                    $_
                                }
                            }
                        )
                    }
                    # handle simple value properties
                    else {

                        $resultTable[$property.Name] = $property.Value
                    }
                }

                $finalResultTable = @{}

                $resultTable.Keys | Microsoft.PowerShell.Utility\Sort-Object | Microsoft.PowerShell.Core\ForEach-Object {

                    $finalResultTable."$_" = $resultTable."$_"
                }

                Microsoft.PowerShell.Utility\Write-Output $finalResultTable
            }
        }

        internalFunction $InputObject
    }

    end {

        Microsoft.PowerShell.Utility\Write-Verbose 'Completed HashTable conversion'
    }
}