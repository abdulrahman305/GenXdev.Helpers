<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Remove-JSONComments.ps1
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
Removes comments from JSON content.

.DESCRIPTION
Processes JSON content and removes both single-line and multi-line comments while
preserving the JSON structure. This is useful for cleaning up JSON files that
contain documentation comments before parsing.

.PARAMETER Json
The JSON content to process as a string array. Each element represents a line of
JSON content.

.EXAMPLE
$jsonContent = @'
{
    // This is a comment
    "name": "test", /* inline comment */
    /* multi-line
       comment */
    "value": 123
}
'@ -split "`n"
Remove-JSONComments -Json $jsonContent

.EXAMPLE
$jsonContent | Remove-JSONComments
#>
function Remove-JSONComments {

    [CmdletBinding(DefaultParameterSetName = 'Default')]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseShouldProcessForStateChangingFunctions', '')]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseSingularNouns', '')]
    [OutputType([System.String])]
    param(
        ########################################################################
        [Parameter(
            Mandatory = $true,
            Position = 0,
            ValueFromPipeline = $true,
            ParameterSetName = 'Default',
            HelpMessage = 'JSON content to process as string array'
        )]
        [string[]] $Json
    )

    begin {

        # inform user that processing is starting
        Microsoft.PowerShell.Utility\Write-Verbose 'Starting JSON comment removal process'
    }


    process {

        # remove comments from json using the helper class
        [GenXdev.Helpers.Serialization]::RemoveJSONComments($Json)
    }

    end {

        # inform user that processing is complete
        Microsoft.PowerShell.Utility\Write-Verbose 'Completed JSON comment removal process'
    }
}