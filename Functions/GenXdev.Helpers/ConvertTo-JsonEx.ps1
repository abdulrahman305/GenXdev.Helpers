<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : ConvertTo-JsonEx.ps1
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