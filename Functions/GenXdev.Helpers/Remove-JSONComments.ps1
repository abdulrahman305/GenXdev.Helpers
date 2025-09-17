<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Remove-JSONComments.ps1
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