<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : ConvertTo-JsonEx.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.274.2025
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
Converts an object to a JSON string with extended options.

.DESCRIPTION
Converts a PowerShell object to a JSON string using the GenXdev.Helpers.
Serialization library. This function provides enhanced JSON serialization
capabilities with optional compression to remove whitespace for reduced
output size. The function is designed to handle complex PowerShell objects
and provide more control over the JSON conversion process compared to the
built-in ConvertTo-Json cmdlet.

.PARAMETER Object
The PowerShell object to convert to JSON format. This can be any type of
PowerShell object including hashtables, arrays, custom objects, or primitive
types.

.PARAMETER Compress
When specified, removes all unnecessary whitespace from the output JSON string
to minimize the size. This is useful when transmitting JSON data over
networks or storing in space-constrained environments.

.EXAMPLE
$data = @{ name = "test"; value = 123 }
ConvertTo-JsonEx -Object $data

Converts a hashtable to JSON format with standard formatting.

.EXAMPLE
$data | ConvertTo-JsonEx -Compress

Converts pipeline input to compressed JSON format without whitespace.

.EXAMPLE
tojsonex $data

Uses the alias to convert an object to JSON format.
#>
function ConvertTo-JsonEx {

    [CmdletBinding()]
    [OutputType([System.String])]
    [Alias('tojsonex')]

    param(
        ########################################################################
        [parameter(
            Position = 0,
            Mandatory = $true,
            ValueFromPipeline = $true,
            HelpMessage = 'The object to convert to JSON'
        )]
        [object] $Object,
        ########################################################################
        [parameter(
            Mandatory = $false,
            HelpMessage = 'Compress the JSON output by removing whitespace'
        )]
        [switch] $Compress
        ########################################################################
    )

    begin {

        # output verbose information about starting the json conversion process
        Microsoft.PowerShell.Utility\Write-Verbose ('Starting JSON conversion ' +
            'process')
    }

    process {

        # output verbose information about the conversion settings being used
        Microsoft.PowerShell.Utility\Write-Verbose ('Converting object to ' +
            "JSON with compression: $Compress")

        # convert the object to json using the custom serializer with
        # compression setting based on the compress switch parameter
        [GenXdev.Helpers.Serialization]::ToJson($Object, ($Compress -eq $true))
    }

    end {
    }
}