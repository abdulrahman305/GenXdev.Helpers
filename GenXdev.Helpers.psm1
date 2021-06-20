<#
.SYNOPSIS
Converts an object to a JSON-formatted string - at full depth

.DESCRIPTION
The `ConvertTo-JsonEx` cmdlet converts any .NET object to a string in JavaScript Object Notation (JSON) format - at full depth

.PARAMETER object
Object to serialize

.PARAMETER Compress
Omits white space and indented formatting in the output string.

.EXAMPLE
 The `ConvertTo-JsonEx` cmdlet is implemented using Newtonsoft Json.NET (https://www.newtonsoft.com/json).

    -------------------------- Example 1 --------------------------

    (Get-UICulture).Calendar | ConvertTo-JsonEx

    {
      "MinSupportedDateTime": "0001-01-01T00:00:00",
      "MaxSupportedDateTime": "9999-12-31T23:59:59.9999999",
      "AlgorithmType": 1,
      "CalendarType": 1,
      "Eras": [
        1
      ],
      "TwoDigitYearMax": 2029,
      "IsReadOnly": true
    }
#>
function ConvertTo-JsonEx {

    param(

        [parameter(Position = 0, Mandatory)]
        [object] $object,

        [parameter(Mandatory = $false)]
        [switch] $Compress
    )
    Get-Process {

        [GenXdev.Helpers.Serialization]::ToJson($object, ($Compress -eq $true));
    }
}

<#
.SYNOPSIS
Removes any comment lines from a json file and return the result

.DESCRIPTION
Removes any comment lines from a json file and return the result

.PARAMETER Json
The json to filter for comments
#>
function Remove-JSONComments {

    param(

        [parameter(ValueFromPipeline = $true, Position = 0, Mandatory)]
        [string[]] $Json
    )

    process {

        [GenXdev.Helpers.Serialization]::RemoveJSONComments($json)
    }
}
