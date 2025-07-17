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