################################################################################
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
$hashTable = ConvertTo-HashTable -InputObject $object
#>
function ConvertTo-HashTable {

    [CmdletBinding()]
    [OutputType([hashtable[]])]
    param (
        ########################################################################
        [Parameter(
            Mandatory = $true,
            Position = 0,
            ValueFromPipeline = $true,
            HelpMessage = "The PSCustomObject to convert into a HashTable"
        )]
        [ValidateNotNull()]
        [object[]] $InputObject
        ########################################################################
    )

    begin {

        Write-Verbose "Starting PSCustomObject to HashTable conversion"
    }

    process {

        # return empty hashtable if input is null
        if ($null -eq $InputObject) {

            Write-Verbose "Input object is null, returning empty hashtable"
            return @{}
        }

        $InputObject | ForEach-Object {

            $currentObject = $_

            # create new hashtable for storing converted properties
            $resultTable = @{}

            # process each property of the current object
            foreach ($property in $currentObject.PSObject.Properties) {

                Write-Verbose "Processing property: $($property.Name)"

                # handle nested PSCustomObject properties
                if ($property.Value -is [System.Management.Automation.PSCustomObject]) {

                    $resultTable[$property.Name] = ConvertTo-HashTable `
                        -InputObject $property.Value
                }
                # handle collection properties
                elseif ($property.Value -is [System.Collections.IEnumerable] -and
                    $property.Value -isnot [string]) {

                    $resultTable[$property.Name] = @(
                        $property.Value | ForEach-Object {

                            if ($_ -is [System.Management.Automation.PSCustomObject]) {
                                ConvertTo-HashTable -InputObject $_
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

            Write-Output $resultTable
        }
    }

    end {

        Write-Verbose "Completed HashTable conversion"
    }
}
################################################################################
