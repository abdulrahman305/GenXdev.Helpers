// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Initialize-SearchPaths.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.308.2025
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Initializes and configures system search paths for package management.
    /// </para>
    ///
    /// <para type="description">
    /// This function builds a comprehensive list of search paths by combining default
    /// system locations, chocolatey paths, development tool paths, and custom package
    /// paths. It then updates the system's PATH environment variable with these
    /// consolidated paths.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -WorkspaceFolder &lt;String&gt;<br/>
    /// The workspace folder path to use for node modules and PowerShell paths.<br/>
    /// - <b>Position</b>: 0<br/>
    /// - <b>Default</b>: "Powershell profile root"<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Initialize search paths with default workspace folder</para>
    /// <para>Initializes search paths using the default workspace folder path.</para>
    /// <code>
    /// Initialize-SearchPaths
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Initialize search paths with custom workspace folder</para>
    /// <para>Initializes search paths using a specific workspace folder path.</para>
    /// <code>
    /// Initialize-SearchPaths -WorkspaceFolder "C:\workspace"
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet("Initialize", "SearchPaths")]
    [OutputType(typeof(void))]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSUseSingularNouns", "")]
    public class InitializeSearchPathsCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// The workspace folder path to use for node modules and PowerShell paths.
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 0,
            HelpMessage = "The workspace folder path to use for node modules and PowerShell paths.")]
        public string WorkspaceFolder { get; set; }

        /// <summary>
        /// Begin processing - initialization logic
        /// </summary>
        protected override void BeginProcessing()
        {
            WriteVerbose("Initializing search paths collection");
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            // Set default WorkspaceFolder if not provided
            if (string.IsNullOrEmpty(WorkspaceFolder))
            {
                var moduleBase = this.MyInvocation.MyCommand.Module.ModuleBase;
                WorkspaceFolder = Path.GetFullPath(Path.Combine(moduleBase, "..", "..", ".."));
            }

            // Create a new list to store unique search paths
            var searchPaths = new List<string>();

            // Add system and development tool paths
            var defaultPaths = new[]
            {
                ExpandPath("${env:ProgramData}\\chocolatey\\bin\\"),
                ExpandPath($"{WorkspaceFolder}\\node_modules\\.bin"),
                ExpandPath($"{WorkspaceFolder}\\scripts"),
                ExpandPath("${env:ProgramFiles}\\Git\\cmd"),
                ExpandPath("${env:ProgramFiles}\\nodejs"),
                ExpandPath("${env:ProgramFiles}\\Google\\Chrome\\Application"),
                ExpandPath("${env:ProgramFiles}\\Microsoft VS Code\\bin"),
                ExpandPath("${env:LOCALAPPDATA}Programs\\Microsoft VS Code Insiders"),
                ExpandPath("${env:ProgramFiles}\\dotnet")
            };

            foreach (var path in defaultPaths)
            {
                searchPaths.Add(path);
            }

            // Add paths from GenXdev packages
            var genXdevPackages = GetGenXdevPackages();
            foreach (var package in genXdevPackages)
            {
                var searchPathProperty = package.Properties["searchpath"];
                if (searchPathProperty != null && searchPathProperty.Value != null)
                {
                    var searchPath = searchPathProperty.Value.ToString();
                    if (!string.IsNullOrWhiteSpace(searchPath))
                    {
                        // Escape special characters in path
                        var escapedPath = searchPath.Replace("`", "``").Replace("\"", "`\"");
                        // Evaluate any variables in the path
                        var evaluatedPath = InvokeCommand.InvokeScript($"Invoke-Expression \"{escapedPath}\"").FirstOrDefault()?.ToString() ?? escapedPath;
                        // Convert to full path
                        var fullPath = ExpandPath(evaluatedPath);
                        searchPaths.Add(fullPath);
                    }
                }
            }

            // Process existing PATH entries
            WriteVerbose("Processing existing PATH environment entries");
            var existingPath = System.Environment.GetEnvironmentVariable("Path") ?? "";
            var existingPaths = existingPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var path in existingPaths)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    try
                    {
                        // Convert to full path
                        var fullPath = ExpandPath(path);
                        // Add path if not already present
                        if (!searchPaths.Contains(fullPath))
                        {
                            searchPaths.Add(fullPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteInformation(new InformationRecord($"Could not parse path: {ex.Message}", "PathParseError"));
                    }
                }
            }

            // Update system PATH with consolidated search paths
            WriteVerbose("Updating system PATH environment variable");
            var newPath = string.Join(";", searchPaths);
            System.Environment.SetEnvironmentVariable("Path", newPath);

            // Also update PowerShell session PATH variable
            var scriptBlock = ScriptBlock.Create("param($path) $Env:Path = $path");
            scriptBlock.Invoke(newPath);
        }

        /// <summary>
        /// End processing - cleanup logic
        /// </summary>
        protected override void EndProcessing()
        {
            // No cleanup needed
        }

        /// <summary>
        /// Gets the GenXdevPackages variable from PowerShell session
        /// </summary>
        private IEnumerable<PSObject> GetGenXdevPackages()
        {
            var result = InvokeCommand.InvokeScript("$GenXdevPackages");
            return result.Select(r => r as PSObject).Where(r => r != null);
        }
    }
}