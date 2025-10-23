// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Test-UnattendedMode.cs
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



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Detects if PowerShell is running in unattended/automated mode
    /// </para>
    ///
    /// <para type="description">
    /// Analyzes various indicators to determine if PowerShell is running in an
    /// unattended or automated context, including pipeline analysis, environment
    /// variables, console redirection, and invocation context.
    ///
    /// When CallersInvocation is provided, it analyzes the pipeline position and
    /// count to determine if the function is being called as part of an automated
    /// pipeline or script execution.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -CallersInvocation &lt;System.Management.Automation.InvocationInfo&gt;<br/>
    /// The caller's invocation information for pipeline and automation detection.<br/>
    /// Pass $MyInvocation from the calling function to analyze pipeline context.<br/>
    /// - <b>Position</b>: 0<br/>
    /// - <b>Default</b>: None<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -Detailed &lt;SwitchParameter&gt;<br/>
    /// Return detailed analysis object instead of simple boolean<br/>
    /// - <b>Position</b>: Named<br/>
    /// - <b>Default</b>: False<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Test-UnattendedMode</para>
    /// <para>Returns a boolean indicating if running in unattended mode using standard detection.</para>
    /// <code>
    /// Test-UnattendedMode
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Test-UnattendedMode -CallersInvocation $MyInvocation</para>
    /// <para>Analyzes the caller's invocation context and returns a boolean result.</para>
    /// <code>
    /// Test-UnattendedMode -CallersInvocation $MyInvocation
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Test-UnattendedMode -CallersInvocation $MyInvocation -Detailed</para>
    /// <para>Returns detailed analysis object with all indicators and pipeline information.</para>
    /// <code>
    /// Test-UnattendedMode -CallersInvocation $MyInvocation -Detailed
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>function My-Function { $isUnattended = Test-UnattendedMode -CallersInvocation $MyInvocation; if ($isUnattended) { Write-Verbose "Running in unattended mode, skipping interactive prompts" } }</para>
    /// <para>Example usage in a function to check for unattended mode.</para>
    /// <code>
    /// function My-Function {
    ///     $isUnattended = Test-UnattendedMode -CallersInvocation $MyInvocation
    ///     if ($isUnattended) {
    ///         Write-Verbose "Running in unattended mode, skipping interactive prompts"
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Test, "UnattendedMode")]
    [OutputType(typeof(bool), typeof(PSObject))]
    public partial class TestUnattendedModeCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// The caller's invocation information for pipeline and automation detection
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 0,
            HelpMessage = "Caller's invocation info for pipeline and automation detection"
        )]
        public InvocationInfo CallersInvocation { get; set; }

        /// <summary>
        /// Return detailed analysis object instead of simple boolean
        /// </summary>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Return detailed analysis object instead of simple boolean"
        )]
        public SwitchParameter Detailed { get; set; }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {

            // Initialize list for unattended indicators
            var unattendedIndicators = new List<string>();

            // Check 1: Environment variables indicating CI/CD or automation
            var automationEnvVars = new[]
            {
                "JENKINS_URL", "GITHUB_ACTIONS", "TF_BUILD", "CI", "BUILD_ID",
                "RUNNER_OS", "SYSTEM_TEAMPROJECT", "TEAMCITY_VERSION", "TRAVIS",
                "APPVEYOR", "CIRCLECI", "GITLAB_CI", "AZURE_PIPELINES"
            };

            // Check if any automation environment variables are set
            var hasAutomationEnv = automationEnvVars.Any(envVar =>
                !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable(envVar)));

            if (hasAutomationEnv)
            {

                unattendedIndicators.Add("AutomationEnvironment");
            }

            // Check 2: Console redirection
            var hasRedirection = false;
            try
            {

                hasRedirection = Console.IsInputRedirected || Console.IsOutputRedirected;
            }
            catch
            {
                // If checking redirection fails, assume no redirection
            }

            if (hasRedirection)
            {
                unattendedIndicators.Add("ConsoleRedirection");
            }

            // Check 3: Non-interactive environment
            var isNonInteractive = false;
            try
            {

                // Check if we're running in a service or without a window station
                isNonInteractive = string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("SESSIONNAME")) &&
                                  string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("USERDOMAIN"));
            }
            catch
            {
                // If check fails, assume interactive
            }

            if (isNonInteractive)
            {
                unattendedIndicators.Add("NonInteractiveEnvironment");
            }

            // Check 4: PowerShell host indicators
            var automationHosts = new[] { "ServerRemoteHost", "Default Host", "BackgroundHost" };

            // Get host name via PowerShell
            var hostName = (string)this.InvokeScript<string>("$Host.Name");

            var isAutomationHost = automationHosts.Contains(hostName);

            if (isAutomationHost)
            {
                unattendedIndicators.Add($"AutomationHost:{hostName}");
            }

            // Check 5: No console window (for GUI apps calling PowerShell)
            var hasNoConsole = false;
            try
            {

                hasNoConsole = Console.WindowWidth == 0;
            }
            catch
            {
                hasNoConsole = true;
            }

            if (hasNoConsole)
            {
                unattendedIndicators.Add("NoConsoleWindow");
            }

            // Check 6: PowerShell execution parameters
            var hasNonInteractiveParam = false;
            try
            {

                // Check if NonInteractive was passed
                hasNonInteractiveParam = this.MyInvocation.BoundParameters.ContainsKey("NonInteractive") ||
                                        (bool)this.InvokeScript<bool>(
                                            "$PSBoundParameters.NonInteractive -or " +
                                            "(Get-Variable -Name PSBoundParameters -Scope 1 " +
                                            "-ErrorAction SilentlyContinue -ValueOnly).NonInteractive");
            }
            catch
            {
                // If check fails, assume no non-interactive parameter
            }

            if (hasNonInteractiveParam)
            {
                unattendedIndicators.Add("NonInteractiveParameter");
            }

            // Check 7: Pipeline analysis (if CallersInvocation provided)
            int? pipelinePosition = null;
            int? pipelineLength = null;
            string scriptName = null;
            string commandLine = null;

            if (this.CallersInvocation != null)
            {

                pipelinePosition = this.CallersInvocation.PipelinePosition;
                pipelineLength = this.CallersInvocation.PipelineLength;
                scriptName = this.CallersInvocation.ScriptName;
                commandLine = this.CallersInvocation.Line;

                // If we're in a multi-command pipeline (not just a single command)
                var isInPipeline = pipelineLength > 1;

                // If we're not at the end of the pipeline (suggesting automated processing)
                var isNotPipelineEnd = pipelinePosition < pipelineLength;

                // Check if called from a script file (not interactive)
                var isFromScript = !string.IsNullOrEmpty(scriptName);

                // Check command line context
                var isAutomatedCommand = !string.IsNullOrEmpty(commandLine) &&
                                        (System.Text.RegularExpressions.Regex.IsMatch(commandLine,
                                            @"^\s*(foreach|%|\||;|&)") ||
                                         System.Text.RegularExpressions.Regex.IsMatch(commandLine,
                                            @"(Get-|Set-|Invoke-|Start-|Stop-).+\|") ||
                                         System.Text.RegularExpressions.Regex.IsMatch(commandLine,
                                            @"\$\w+\s*\|\s*"));

                // Only flag as unattended if we have strong indicators
                // Being in a simple function call from console should not count as unattended
                var isInteractiveFunction = hostName == "ConsoleHost" &&
                                           pipelineLength == 1 &&
                                           string.IsNullOrEmpty(scriptName);

                if (isInPipeline && !isInteractiveFunction)
                {

                    unattendedIndicators.Add($"MultiCommandPipeline:{pipelinePosition}/{pipelineLength}");
                }

                if (isNotPipelineEnd && !isInteractiveFunction)
                {

                    unattendedIndicators.Add("NotPipelineEnd");
                }

                if (isAutomatedCommand && !isInteractiveFunction)
                {

                    unattendedIndicators.Add("AutomatedCommandPattern");
                }
            }

            // Final determination
            var isUnattended = unattendedIndicators.Count > 0;

            // Return detailed object or simple boolean
            if (this.Detailed.ToBool())
            {

                var result = new PSObject();
                result.Properties.Add(new PSNoteProperty("IsUnattended", isUnattended));
                result.Properties.Add(new PSNoteProperty("Indicators", unattendedIndicators.ToArray()));
                result.Properties.Add(new PSNoteProperty("IndicatorCount", unattendedIndicators.Count));
                result.Properties.Add(new PSNoteProperty("HostName", hostName));
                result.Properties.Add(new PSNoteProperty("PipelinePosition", pipelinePosition));
                result.Properties.Add(new PSNoteProperty("PipelineLength", pipelineLength));
                result.Properties.Add(new PSNoteProperty("ScriptName", scriptName));
                result.Properties.Add(new PSNoteProperty("CommandLine", commandLine));

                this.WriteObject(result);
            }
            else
            {

                // Return simple boolean by default
                this.WriteObject(isUnattended);
            }
        }
    }
}