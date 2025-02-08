################################################################################
<#
.SYNOPSIS
Converts an object to a JSON string with extended options.

.DESCRIPTION
Converts a PowerShell object to a JSON string using GenXdev.Helpers.Serialization
with optional compression.

.PARAMETER Object
The PowerShell object to convert to JSON.

.PARAMETER Compress
If specified, removes whitespace from the output JSON string.

.EXAMPLE
$data = @{ name = "test"; value = 123 }
ConvertTo-JsonEx -Object $data

.EXAMPLE
$data | ConvertTo-JsonEx -Compress
#>
function ConvertTo-JsonEx {

    [CmdletBinding()]
    [Alias("tojsonex")]

    param(
        ########################################################################
        [parameter(
            Position = 0,
            Mandatory = $true,
            ValueFromPipeline = $true,
            HelpMessage = "The object to convert to JSON"
        )]
        [object] $Object,
        ########################################################################
        [parameter(
            Mandatory = $false,
            HelpMessage = "Compress the JSON output by removing whitespace"
        )]
        [switch] $Compress
        ########################################################################
    )

    begin {
        Write-Verbose "Starting JSON conversion process"
    }

    process {
        # convert the object to json using the custom serializer
        Write-Verbose "Converting object to JSON with compression: $Compress"
        [GenXdev.Helpers.Serialization]::ToJson($Object, ($Compress -eq $true))
    }

    end {
    }
}
################################################################################
