// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : GenXdevCmdletInfo.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 2.1.2025
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



using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

#nullable enable

namespace GenXdev.Helpers
{

    /// <summary>
    /// <para type="synopsis">
    /// Represents information about a GenXdev PowerShell cmdlet, including metadata such as name, module, description, and file paths.
    /// </para>
    ///
    /// <para type="description">
    /// This class encapsulates details about GenXdev cmdlets used for documentation, discovery, and tooling purposes.
    /// It provides structured access to cmdlet information parsed from PowerShell module files.
    /// </para>
    /// </summary>
    public partial class GenXdevCmdletInfo
    {

        /// <summary>
        /// <para type="description">
        /// The name of the cmdlet (e.g., Get-Example).
        /// </para>
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// <para type="description">
        /// The module name containing the cmdlet.
        /// </para>
        /// </summary>
        [JsonProperty("moduleName", NullValueHandling = NullValueHandling.Ignore)]
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>
        /// <para type="description">
        /// The base module name (e.g., GenXdev.Console).
        /// </para>
        /// </summary>
        [JsonProperty("baseModule", NullValueHandling = NullValueHandling.Ignore)]
        public string BaseModule { get; set; } = string.Empty;

        /// <summary>
        /// <para type="description">
        /// The line number where the function is defined in the source file.
        /// </para>
        /// </summary>
        [JsonProperty("lineNo", NullValueHandling = NullValueHandling.Ignore)]
        public int LineNo { get; set; }

        /// <summary>
        /// <para type="description">
        /// Description of what the cmdlet does.
        /// </para>
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// <para type="description">
        /// Comma-separated list of aliases for the cmdlet.
        /// </para>
        /// </summary>
        [JsonProperty("aliases", NullValueHandling = NullValueHandling.Ignore)]
        public string Aliases { get; set; } = string.Empty;

        /// <summary>
        /// <para type="description">
        /// Full path to the script file containing the cmdlet.
        /// </para>
        /// </summary>
        [JsonProperty("scriptFilePath", NullValueHandling = NullValueHandling.Ignore)]
        public string ScriptFilePath { get; set; } = string.Empty;

        /// <summary>
        /// <para type="description">
        /// Full path to the test file for the cmdlet.
        /// </para>
        /// </summary>
        [JsonProperty("scriptTestFilePath", NullValueHandling = NullValueHandling.Ignore)]
        public string ScriptTestFilePath { get; set; } = string.Empty;

    }

    /// <summary>
    /// <para type="synopsis">
    /// Provides static methods for deserializing GenXdevCmdletInfo objects from JSON.
    /// </para>
    ///
    /// <para type="description">
    /// This partial class extension contains utility methods for JSON deserialization.
    /// </para>
    /// </summary>
    public partial class GenXdevCmdletInfo
    {

        /// <summary>
        /// <para type="description">
        /// Deserialize JSON string to GenXdevCmdletInfo object.
        /// </para>
        ///
        /// <param name="json">JSON string representation of the cmdlet info.</param>
        ///
        /// <returns>A GenXdevCmdletInfo object deserialized from the JSON string.</returns>
        /// </summary>
        public static GenXdevCmdletInfo FromJson(string json) =>
            JsonConvert.DeserializeObject<GenXdevCmdletInfo>(json,
                GenXdev.Helpers.GenXdevCmdletInfoConverter.Settings)!;

    }

    /// <summary>
    /// <para type="synopsis">
    /// Extension methods for serializing GenXdevCmdletInfo objects to JSON.
    /// </para>
    ///
    /// <para type="description">
    /// This static class provides extension methods to convert GenXdevCmdletInfo instances and collections to JSON strings.
    /// </para>
    /// </summary>
    public static class GenXdevCmdletInfoSerialize
    {

        /// <summary>
        /// <para type="description">
        /// Serialize GenXdevCmdletInfo object to JSON string.
        /// </para>
        ///
        /// <param name="self">The GenXdevCmdletInfo object to serialize.</param>
        ///
        /// <returns>JSON string representation of the cmdlet info.</returns>
        /// </summary>
        public static string ToJson(this GenXdevCmdletInfo self) =>
            JsonConvert.SerializeObject(self,
                GenXdev.Helpers.GenXdevCmdletInfoConverter.Settings);

        /// <summary>
        /// <para type="description">
        /// Serialize collection of GenXdevCmdletInfo objects to JSON string.
        /// </para>
        ///
        /// <param name="container">Collection of GenXdevCmdletInfo objects to serialize.</param>
        ///
        /// <returns>JSON string representation of the collection.</returns>
        /// </summary>
        public static string ToJson(IEnumerable<GenXdevCmdletInfo> container) =>
            JsonConvert.SerializeObject(container,
                GenXdev.Helpers.GenXdevCmdletInfoConverter.Settings);

    }

    /// <summary>
    /// <para type="synopsis">
    /// JSON converter settings for GenXdevCmdletInfo serialization.
    /// </para>
    ///
    /// <para type="description">
    /// This internal static class defines the JsonSerializerSettings used for consistent JSON handling.
    /// </para>
    /// </summary>
    internal static class GenXdevCmdletInfoConverter
    {

        /// <summary>
        /// <para type="description">
        /// Predefined JsonSerializerSettings for GenXdevCmdletInfo serialization and deserialization.
        /// </para>
        /// </summary>
        public static readonly JsonSerializerSettings Settings =
            new JsonSerializerSettings
            {

                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,

                MissingMemberHandling = MissingMemberHandling.Ignore,

                DateParseHandling = DateParseHandling.None,

                Converters =
                {

                    new IsoDateTimeConverter
                    {

                        DateTimeStyles = DateTimeStyles.AssumeUniversal

                    }

                },

            };

    }

}
