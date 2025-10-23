// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Remove-JSONComments.cs
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



using System.Management.Automation;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Removes comments from JSON content.
    /// </para>
    ///
    /// <para type="description">
    /// Processes JSON content and removes both single-line and multi-line comments while
    /// preserving the JSON structure. This is useful for cleaning up JSON files that
    /// contain documentation comments before parsing.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -Json &lt;String[]&gt;<br/>
    /// The JSON content to process as a string array. Each element represents a line of<br/>
    /// JSON content.<br/>
    /// - <b>Mandatory</b>: true<br/>
    /// - <b>Position</b>: 0<br/>
    /// - <b>Accepts pipeline input</b>: true<br/>
    /// - <b>Help message</b>: JSON content to process as string array<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Example 1: Process JSON content from variable</para>
    /// <para>This example shows how to remove comments from JSON content stored in a variable. The JSON string is split into an array of lines using the newline character before being passed to the cmdlet.</para>
    /// <code>
    /// $jsonContent = @'
    /// {
    ///     // This is a comment
    ///     "name": "test", /* inline comment */
    ///     /* multi-line
    ///        comment */
    ///     "value": 123
    /// }
    /// '@ -split "`n"
    /// Remove-JSONComments -Json $jsonContent
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Example 2: Process JSON content from pipeline</para>
    /// <para>This example demonstrates how to process JSON content from the pipeline, allowing for streaming or chained operations with other PowerShell commands.</para>
    /// <code>
    /// $jsonContent | Remove-JSONComments
    /// </code>
    /// </example>
    ///
    /// <remarks>
    /// This cmdlet supports both single-line comments (//) and multi-line comments (/* */).
    /// It preserves the JSON structure while removing all comment content.
    /// </remarks>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "JSONComments")]
    [OutputType(typeof(string))]
    public class RemoveJSONCommentsCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// The JSON content to process as a string array. Each element represents a line of JSON content.
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            HelpMessage = "JSON content to process as string array"
        )]
        public string[] Json { get; set; }

        /// <summary>
        /// Begin processing - initialization logic
        /// </summary>
        protected override void BeginProcessing()
        {
            WriteVerbose("Starting JSON comment removal process");
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Remove comments from json using the helper class
            WriteObject(Serialization.RemoveJSONComments(Json));
        }

        /// <summary>
        /// End processing - cleanup logic
        /// </summary>
        protected override void EndProcessing()
        {
            WriteVerbose("Completed JSON comment removal process");
        }
    }
}