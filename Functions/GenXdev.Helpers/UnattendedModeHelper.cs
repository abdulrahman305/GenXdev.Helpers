// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : UnattendedModeHelper.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.300.2025
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
    /// Helper for detecting if the current process is running in unattended/automated mode.
    /// </summary>
    public static class UnattendedModeHelper
    {
        /// <summary>
        /// Detects if the current process is running in unattended/automated mode.
        /// </summary>
        /// <param name="callersInvocation">Optional: The caller's InvocationInfo for pipeline and automation detection.</param>
        /// <returns>True if running in unattended/automated mode, otherwise false.</returns>
        public static bool IsUnattendedMode(InvocationInfo callersInvocation = null)
        {
            // Check 1: Environment variables indicating CI/CD or automation
            string[] automationEnvVars = new[]
            {
                "JENKINS_URL", "GITHUB_ACTIONS", "TF_BUILD", "CI", "BUILD_ID",
                "RUNNER_OS", "SYSTEM_TEAMPROJECT", "TEAMCITY_VERSION", "TRAVIS",
                "APPVEYOR", "CIRCLECI", "GITLAB_CI", "AZURE_PIPELINES"
            };
            bool hasAutomationEnv = automationEnvVars.Any(envVar =>
                !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable(envVar)));
            if (hasAutomationEnv)
                return true;

            // Check 2: Console redirection
            try
            {
                if (Console.IsInputRedirected || Console.IsOutputRedirected)
                    return true;
            }
            catch { /* Ignore */ }

            // Check 3: Non-interactive environment
            try
            {
                if (!System.Environment.UserInteractive)
                    return true;
            }
            catch { /* Ignore */ }

            // Check 4: PowerShell host indicators (if available)
            // Not available in pure C# context without PowerShell Host, so skip

            // Check 5: No console window
            try
            {
                if (Console.WindowWidth == 0)
                    return true;
            }
            catch
            {
                // If we can't access the console, assume no console window
                return true;
            }

            // Check 6: PowerShell execution parameters (not available in pure C# context)
            // Skipped

            // Check 7: Pipeline analysis (if callersInvocation provided)
            if (callersInvocation != null)
            {
                int pipelineInfo = callersInvocation.PipelinePosition;
                int pipelineLength = callersInvocation.PipelineLength;
                bool isInPipeline = pipelineLength > 1;
                bool isNotPipelineEnd = pipelineInfo < pipelineLength;
                bool isFromScript = !string.IsNullOrEmpty(callersInvocation.ScriptName);
                string commandLine = callersInvocation.Line ?? string.Empty;
                bool isAutomatedCommand =
                    System.Text.RegularExpressions.Regex.IsMatch(commandLine, @"^\s*(foreach|%|\||;|&)") ||
                    System.Text.RegularExpressions.Regex.IsMatch(commandLine, @"(Get-|Set-|Invoke-|Start-|Stop-).+\|") ||
                    System.Text.RegularExpressions.Regex.IsMatch(commandLine, @"\$\w+\s*\|\s*");
                bool isInteractiveFunction = pipelineLength == 1 && string.IsNullOrEmpty(callersInvocation.ScriptName);
                if (isInPipeline && !isInteractiveFunction)
                    return true;
                if (isNotPipelineEnd && !isInteractiveFunction)
                    return true;
                if (isAutomatedCommand && !isInteractiveFunction)
                    return true;
            }

            // If none of the above, assume attended/interactive
            return false;
        }
    }
}
