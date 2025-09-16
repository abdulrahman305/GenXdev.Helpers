<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Get-DefaultWebLanguage.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.268.2025
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
Gets the default web language key based on the system's current language settings.

.DESCRIPTION
Retrieves the current system language and culture settings and maps them to the
corresponding web language dictionary key used by translation services.

.EXAMPLE
Get-DefaultWebLanguage
Returns "English" for an English system, "Dutch" for a Dutch system, etc.
#>
function Get-DefaultWebLanguage {

    [CmdletBinding()]
    [OutputType([System.String])]
    param()

    begin {
        # get the current system culture info
        $systemCulture = [System.Globalization.CultureInfo]::CurrentUICulture
        Microsoft.PowerShell.Utility\Write-Verbose "System culture: $($systemCulture.Name)"
    }


    process {
        # get the dictionary of supported languages
        $webLanguages = GenXdev.Helpers\Get-WebLanguageDictionary

        # get the reversed dictionary (language codes to names)
        $reversedDict = @{}
        foreach ($key in $webLanguages.Keys) {
            $reversedDict[$webLanguages[$key]] = $key
        }

        # try to find exact match first (e.g. "pt-BR" for Brazilian Portuguese)
        if ($reversedDict.ContainsKey($systemCulture.Name)) {
            return $reversedDict[$systemCulture.Name]
        }

        # try to match just the language part (e.g. "pt" for Portuguese)
        $languageCode = $systemCulture.TwoLetterISOLanguageName
        foreach ($entry in $webLanguages.GetEnumerator()) {
            if ($entry.Value -eq $languageCode) {
                return $entry.Key
            }
        }

        # default to English if no match found
        Microsoft.PowerShell.Utility\Write-Verbose 'No matching language found, defaulting to English'
        return 'English'
    }
}