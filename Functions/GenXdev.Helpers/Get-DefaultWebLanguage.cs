// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Get-DefaultWebLanguage.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.304.2025
// ################################################################################
// Copyright (c)  René Vaessen / GenXdev
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ################################################################################



using System;
using System.Collections;
using System.Globalization;
using System.Management.Automation;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Gets the default web language key based on the system's current language settings.
    /// </para>
    ///
    /// <para type="description">
    /// Retrieves the current system language and culture settings and maps them to the
    /// corresponding web language dictionary key used by translation services.
    /// </para>
    ///
    /// <example>
    /// <para>Get-DefaultWebLanguage</para>
    /// <para>Returns "English" for an English system, "Dutch" for a Dutch system, etc.</para>
    /// <code>
    /// Get-DefaultWebLanguage
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DefaultWebLanguage")]
    [OutputType(typeof(string))]
    public class GetDefaultWebLanguageCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Get the current system culture info
            var systemCulture = CultureInfo.CurrentUICulture;
            WriteVerbose($"System culture: {systemCulture.Name}");

            // Get the dictionary of supported languages
            var webLanguagesResult = InvokeCommand.InvokeScript("GenXdev.Helpers\\Get-WebLanguageDictionary");
            WriteVerbose("t:" + webLanguagesResult[0].BaseObject.GetType().FullName);
            var webLanguages = webLanguagesResult[0].BaseObject as Dictionary<string, string>;

            if (webLanguages == null)
            {
                throw new InvalidOperationException("Failed to retrieve wzeb language dictionary");
            }

            // Get the reversed dictionary (language codes to names)
            var reversedDict = new Hashtable();
            foreach (var key in webLanguages.Keys)
            {
                reversedDict[webLanguages[key]] = key;
            }

            // Try to find exact match first (e.g. "pt-BR" for Brazilian Portuguese)
            if (reversedDict.ContainsKey(systemCulture.Name))
            {
                WriteObject(reversedDict[systemCulture.Name]);
                return;
            }

            // Try to match just the language part (e.g. "pt" for Portuguese)
            var languageCode = systemCulture.TwoLetterISOLanguageName;
            foreach (var key in webLanguages.Keys)
            {
                if (webLanguages[key].Equals(languageCode))
                {
                    WriteObject(key);
                    return;
                }
            }

            // Default to English if no match found
            WriteVerbose("No matching language found, defaulting to English");
            WriteObject("English");
        }
    }
}