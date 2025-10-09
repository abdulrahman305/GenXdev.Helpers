<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Get-DefaultWebLanguage.ps1
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