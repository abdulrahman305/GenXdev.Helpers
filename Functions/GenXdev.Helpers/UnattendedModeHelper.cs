// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : UnattendedModeHelper.cs
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
    /// Helper class for detecting if the current process is running in unattended/automated mode.
    /// </para>
    ///
    /// <para type="description">
    /// This static class provides methods to determine whether the current execution environment
    /// is automated or interactive. It checks various indicators such as environment variables,
    /// console redirection, user interactivity, and PowerShell pipeline context.
    /// </para>
    /// </summary>
    public static class UnattendedModeHelper
    {
        /// <summary>
        /// <para type="synopsis">
        /// Detects if the current process is running in unattended/automated mode.
        /// </para>
        ///
        /// <para type="description">
        /// This method performs multiple checks to determine if the current execution is automated:
        /// - Checks for common CI/CD environment variables
        /// - Detects console input/output redirection
        /// - Verifies user interactivity
        /// - Analyzes console window availability
        /// - Examines PowerShell pipeline and invocation context (if provided)
        ///
        /// Returns true if any automation indicator is detected, false if the environment appears interactive.
        /// </para>
        ///
        /// <para type="description">
        /// PARAMETERS
        /// </para>
        ///
        /// <para type="description">
        /// -CallersInvocation &lt;InvocationInfo&gt;<br/>
        /// Optional: The caller's InvocationInfo for pipeline and automation detection.<br/>
        /// - <b>Position</b>: 0<br/>
        /// - <b>Default</b>: null<br/>
        /// </para>
        ///
        /// <example>
        /// <para>Check if running in unattended mode</para>
        /// <para>This example demonstrates how to use the method to detect automation.</para>
        /// <code>
        /// bool isUnattended = UnattendedModeHelper.IsUnattendedMode();
        /// </code>
        /// </example>
        ///
        /// <example>
        /// <para>Check with invocation info</para>
        /// <para>Pass the current invocation info for more accurate detection in PowerShell context.</para>
        /// <code>
        /// bool isUnattended = UnattendedModeHelper.IsUnattendedMode(MyInvocation);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="callersInvocation">Optional: The caller's InvocationInfo for pipeline and automation detection.</param>
        /// <returns>True if running in unattended/automated mode, otherwise false.</returns>
        public static bool IsUnattendedMode(InvocationInfo callersInvocation = null)
        {

            // Define environment variables that indicate CI/CD or automation systems
            string[] automationEnvVars = new[]
            {
                "JENKINS_URL", "GITHUB_ACTIONS", "TF_BUILD", "CI", "BUILD_ID",
                "RUNNER_OS", "SYSTEM_TEAMPROJECT", "TEAMCITY_VERSION", "TRAVIS",
                "APPVEYOR", "CIRCLECI", "GITLAB_CI", "AZURE_PIPELINES"
            };

            // Check if any of these environment variables are set, indicating automation
            bool hasAutomationEnv = automationEnvVars.Any(envVar =>
                !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable(envVar)));

            if (hasAutomationEnv)
                return true;

            // Check for console input or output redirection, common in automated scripts
            try
            {
                if (Console.IsInputRedirected || Console.IsOutputRedirected)
                    return true;
            }
            catch { /* Ignore exceptions when console access fails */ }

            // Verify if the environment is interactive
            try
            {
                if (!System.Environment.UserInteractive)
                    return true;
            }
            catch { /* Ignore exceptions in non-interactive environments */ }

            // Check 4: PowerShell host indicators (if available)
            // Not available in pure C# context without PowerShell Host, so skip

            // Check for absence of console window, indicating headless execution
            try
            {
                if (Console.WindowWidth == 0)
                    return true;
            }
            catch
            {
                // If console access fails, assume no console window is available
                return true;
            }

            // Check 6: PowerShell execution parameters (not available in pure C# context)
            // Skipped

            // Analyze PowerShell pipeline and invocation context if provided
            if (callersInvocation != null)
            {
                int pipelineInfo = callersInvocation.PipelinePosition;
                int pipelineLength = callersInvocation.PipelineLength;

                // Determine if command is part of a pipeline
                bool isInPipeline = pipelineLength > 1;
                bool isNotPipelineEnd = pipelineInfo < pipelineLength;

                // Check if invoked from a script file
                bool isFromScript = !string.IsNullOrEmpty(callersInvocation.ScriptName);

                // Extract the command line for pattern analysis
                string commandLine = callersInvocation.Line ?? string.Empty;

                // Check for patterns indicating automated command execution
                bool isAutomatedCommand =
                    System.Text.RegularExpressions.Regex.IsMatch(commandLine, @"^\s*(foreach|%|\||;|&)") ||
                    System.Text.RegularExpressions.Regex.IsMatch(commandLine, @"(Get-|Set-|Invoke-|Start-|Stop-).+\|") ||
                    System.Text.RegularExpressions.Regex.IsMatch(commandLine, @"\$\w+\s*\|\s*");

                // Determine if this appears to be an interactive function call
                bool isInteractiveFunction = pipelineLength == 1 && string.IsNullOrEmpty(callersInvocation.ScriptName);

                // Return true if in pipeline and not an interactive function
                if (isInPipeline && !isInteractiveFunction)
                    return true;

                // Return true if not at pipeline end and not interactive
                if (isNotPipelineEnd && !isInteractiveFunction)
                    return true;

                // Return true if automated command pattern detected and not interactive
                if (isAutomatedCommand && !isInteractiveFunction)
                    return true;
            }

            // If none of the automation indicators are present, assume interactive mode
            return false;
        }
    }
}
