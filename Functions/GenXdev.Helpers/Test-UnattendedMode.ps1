<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Test-UnattendedMode.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.296.2025
################################################################################
MIT License

Copyright 2021-2025 GenXdev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
################################################################################>
###############################################################################

<#
.SYNOPSIS
Detects if PowerShell is running in unattended/automated mode

.DESCRIPTION
Analyzes various indicators to determine if PowerShell is running in an
unattended or automated context, including pipeline analysis, environment
variables, console redirection, and invocation context.

When CallersInvocation is provided, it analyzes the pipeline position and
count to determine if the function is being called as part of an automated
pipeline or script execution.

.PARAMETER CallersInvocation
The caller's invocation information for pipeline and automation detection.
Pass $MyInvocation from the calling function to analyze pipeline context.

.EXAMPLE
Test-UnattendedMode
Returns a boolean indicating if running in unattended mode using standard detection.

.EXAMPLE
Test-UnattendedMode -CallersInvocation $MyInvocation
Analyzes the caller's invocation context and returns a boolean result.

.EXAMPLE
Test-UnattendedMode -CallersInvocation $MyInvocation -Detailed
Returns detailed analysis object with all indicators and pipeline information.

.EXAMPLE
function My-Function {
    $isUnattended = Test-UnattendedMode -CallersInvocation $MyInvocation
    if ($isUnattended) {
        Write-Verbose "Running in unattended mode, skipping interactive prompts"
    }
}

.NOTES
The function combines multiple detection methods:
- Environment variables (CI/CD systems)
- Console redirection
- Interactive session detection
- PowerShell host type
- Pipeline analysis (when CallersInvocation provided)
- Console availability
#>
function Test-UnattendedMode {
    [CmdletBinding()]
    param(
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = "Caller's invocation info for pipeline and automation detection"
        )]
        [System.Management.Automation.InvocationInfo] $CallersInvocation,
        ###############################################################################
        [Parameter(
            Mandatory = $false,
            HelpMessage = "Return detailed analysis object instead of simple boolean"
        )]
        [switch] $Detailed
    )

    begin {
        $unattendedIndicators = @()
    }

    process {
        # Check 1: Environment variables indicating CI/CD or automation
        $automationEnvVars = @(
            'JENKINS_URL', 'GITHUB_ACTIONS', 'TF_BUILD', 'CI', 'BUILD_ID',
            'RUNNER_OS', 'SYSTEM_TEAMPROJECT', 'TEAMCITY_VERSION', 'TRAVIS',
            'APPVEYOR', 'CIRCLECI', 'GITLAB_CI', 'AZURE_PIPELINES'
        )

        $hasAutomationEnv = @($automationEnvVars | Microsoft.PowerShell.Core\ForEach-Object {
            [Environment]::GetEnvironmentVariable($_)
        } | Microsoft.PowerShell.Core\Where-Object { $_ -ne $null }).Count -gt 0

        if ($hasAutomationEnv) {
            $unattendedIndicators += "AutomationEnvironment"
        }

        # Check 2: Console redirection
        $hasRedirection = try {
            [Console]::IsInputRedirected -or [Console]::IsOutputRedirected
        } catch {
            $false
        }

        if ($hasRedirection) {
            $unattendedIndicators += "ConsoleRedirection"
        }

        # Check 3: Non-interactive environment
        $isNonInteractive = -not [Environment]::UserInteractive

        if ($isNonInteractive) {
            $unattendedIndicators += "NonInteractiveEnvironment"
        }

        # Check 4: PowerShell host indicators
        $automationHosts = @('ServerRemoteHost', 'Default Host', 'BackgroundHost')
        $isAutomationHost = $Host.Name -in $automationHosts

        if ($isAutomationHost) {
            $unattendedIndicators += "AutomationHost:$($Host.Name)"
        }

        # Check 5: No console window (for GUI apps calling PowerShell)
        $hasNoConsole = try {
            $null -eq [Console]::WindowWidth -or [Console]::WindowWidth -eq 0
        } catch {
            $true
        }

        if ($hasNoConsole) {
            $unattendedIndicators += "NoConsoleWindow"
        }

        # Check 6: PowerShell execution parameters
        $hasNonInteractiveParam = try {
            $PSBoundParameters.NonInteractive -or
            (Microsoft.PowerShell.Utility\Get-Variable -Name PSBoundParameters -Scope 1 -ErrorAction SilentlyContinue -ValueOnly).NonInteractive
        } catch {
            $false
        }

        if ($hasNonInteractiveParam) {
            $unattendedIndicators += "NonInteractiveParameter"
        }

        # Check 7: Pipeline analysis (if CallersInvocation provided)
        if ($CallersInvocation) {
            $pipelineInfo = $CallersInvocation.PipelinePosition
            $pipelineLength = $CallersInvocation.PipelineLength

            # If we're in a multi-command pipeline (not just a single command)
            $isInPipeline = $pipelineLength -gt 1

            # If we're not at the end of the pipeline (suggesting automated processing)
            $isNotPipelineEnd = $pipelineInfo -lt $pipelineLength

            # Check if called from a script file (not interactive)
            $isFromScript = -not [string]::IsNullOrEmpty($CallersInvocation.ScriptName)

            # Check command line context
            $commandLine = $CallersInvocation.Line
            $isAutomatedCommand = $commandLine -match '^\s*(foreach|%|\||;|&)' -or
                                 $commandLine -match '(Get-|Set-|Invoke-|Start-|Stop-).+\|' -or
                                 $commandLine -match '\$\w+\s*\|\s*'

            # Only flag as unattended if we have strong indicators
            # Being in a simple function call from console should not count as unattended
            $isInteractiveFunction = $Host.Name -eq 'ConsoleHost' -and
                                   $pipelineLength -eq 1 -and
                                   [string]::IsNullOrEmpty($CallersInvocation.ScriptName)

            if ($isInPipeline -and -not $isInteractiveFunction) {
                $unattendedIndicators += "MultiCommandPipeline:$pipelineInfo/$pipelineLength"
            }

            if ($isNotPipelineEnd -and -not $isInteractiveFunction) {
                $unattendedIndicators += "NotPipelineEnd"
            }

            if ($isFromScript) {
                # $unattendedIndicators += "ScriptFile:$([System.IO.Path]::GetFileName($CallersInvocation.ScriptName))"
            }

            if ($isAutomatedCommand -and -not $isInteractiveFunction) {
                $unattendedIndicators += "AutomatedCommandPattern"
            }
        }

        # Check 8: Current execution context (removed - too broad, flags user functions)
        # The CommandOrigin check was incorrectly flagging interactive console functions
        # as unattended mode, so this check has been removed.

        # Final determination
        $isUnattended = $unattendedIndicators.Count -gt 0

        # Return detailed object or simple boolean
        if ($Detailed) {
            return [PSCustomObject]@{
                IsUnattended = $isUnattended
                Indicators = $unattendedIndicators
                IndicatorCount = $unattendedIndicators.Count
                HostName = $Host.Name
                PipelinePosition = if ($CallersInvocation) { $CallersInvocation.PipelinePosition } else { $null }
                PipelineLength = if ($CallersInvocation) { $CallersInvocation.PipelineLength } else { $null }
                ScriptName = if ($CallersInvocation) { $CallersInvocation.ScriptName } else { $null }
                CommandLine = if ($CallersInvocation) { $CallersInvocation.Line } else { $null }
            }
        }

        # Return simple boolean by default
        return $isUnattended
    }
}