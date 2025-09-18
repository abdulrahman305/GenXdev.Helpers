// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : UnattendedModeHelper.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.276.2025
// ################################################################################
// MIT License
//
// Copyright 2021-2025 GenXdev
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
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
