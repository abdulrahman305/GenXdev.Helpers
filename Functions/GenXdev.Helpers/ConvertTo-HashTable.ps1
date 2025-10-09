<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : ConvertTo-HashTable.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.300.2025
################################################################################
Copyright (c)  René Vaessen / GenXdev

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
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