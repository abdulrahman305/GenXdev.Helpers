################################################################################
<#
.SYNOPSIS
Copies parameter values from bound parameters to a new hashtable based on
another function's possible parameters.

.DESCRIPTION
This function creates a new hashtable containing only the parameter values that
match the parameters defined in the specified target function.
This can then be used to invoke the function using splatting.

.PARAMETER BoundParameters
The bound parameters from which to copy values, typically $PSBoundParameters.

.PARAMETER FunctionName
The name of the function whose parameter set will be used as a filter.

.EXAMPLE
function Test-Function {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string] $Path,
        [Parameter(Mandatory = $false)]
        [switch] $Recurse
    )

    $params = GenXdev.Helpers\Copy-IdenticalParamValues -BoundParameters $PSBoundParameters `
        -FunctionName 'Get-ChildItem'

    Get-ChildItem @params
}
#>
function Copy-IdenticalParamValues {

    [CmdletBinding()]
    [OutputType([hashtable])]
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSUseSingularNouns", "")]
    param(
        ########################################################################
        [Parameter(
            Mandatory = $true,
            Position = 0,
            HelpMessage = "Source bound parameters to copy from"
        )]
        [ValidateNotNull()]
        [object[]] $BoundParameters,
        ########################################################################
        [Parameter(
            Mandatory = $true,
            Position = 1,
            HelpMessage = "Target function name to filter parameters"
        )]
        [ValidateNotNullOrEmpty()]
        [string] $FunctionName,
        ########################################################################
        [Parameter(
            Mandatory = $false,
            Position = 2,
            HelpMessage = "Default values for parameters"
        )]
        [System.Management.Automation.PSVariable[]] $DefaultValues = @()
        ########################################################################
    )

    begin {

        # define common parameters to filter out
        $filter = @(
            "input",
            "MyInvocation",
            "null",
            "PSBoundParameters",
            "PSCmdlet",
            "PSCommandPath",
            "PSScriptRoot",
            "Verbose",
            "Debug",
            "ErrorAction",
            "ErrorVariable",
            "WarningAction",
            "WarningVariable",
            "InformationAction",
            "InformationVariable",
            "OutVariable",
            "OutBuffer",
            "PipelineVariable",
            "WhatIf",
            "Confirm",
            "OutVariable",
            "ProgressAction",
            "ErrorVariable"
        )

        # initialize results hashtable
        [hashtable] $results = @{}

        # create hashtable of default parameter values
        [hashtable] $defaults = (& {
                $defaultsHash = @{}

                $DefaultValues |
                Microsoft.PowerShell.Core\Where-Object -Property Options -EQ "None" |
                Microsoft.PowerShell.Core\ForEach-Object {
                    if ($filter.IndexOf($_.Name) -lt 0) {

                        if (-not ($_.Value -is [string] -and
                                [string]::IsNullOrWhiteSpace($_.Value))) {

                            if ($null -ne $_.Value) {

                                $defaultsHash."$($_.Name)" = $_.Value
                            }
                        }
                    }
                }

                $defaultsHash
            })

        # get function info for parameter validation
        Microsoft.PowerShell.Utility\Write-Verbose "Getting command info for function '$FunctionName'"
        $functionInfo = Microsoft.PowerShell.Core\Get-Command -Name $FunctionName -ErrorAction SilentlyContinue

        # validate function exists
        if ($null -eq $functionInfo) {

            Microsoft.PowerShell.Utility\Write-Error "Function '$FunctionName' not found"
            return
        }

        Microsoft.PowerShell.Utility\Write-Verbose "Found function with $($functionInfo.Parameters.Count) parameters"
    }

    process {

        # iterate through all parameters of the target function
        $functionInfo.Parameters.Keys | Microsoft.PowerShell.Core\ForEach-Object {

            # get parameter name
            $paramName = $_

            # check if parameter exists in bound parameters
            if ($BoundParameters.ContainsKey($paramName)) {

                Microsoft.PowerShell.Utility\Write-Verbose "Copying value for parameter '$paramName'"
                $value = $BoundParameters[0].GetEnumerator() |
                Microsoft.PowerShell.Core\Where-Object -Property Key -EQ $paramName |
                Microsoft.PowerShell.Utility\Select-Object -Property "Value"

                $results."$paramName" = $value.Value
            }
            else {

                $defaultValue = $defaults."$paramName"

                if ($null -ne $defaultValue) {

                    $results."$paramName" = $defaultValue

                    Microsoft.PowerShell.Utility\Write-Verbose ("Using default value " +
                        ($defaultValue | Microsoft.PowerShell.Utility\ConvertTo-Json -Depth 1 -WarningAction SilentlyContinue -ErrorAction SilentlyContinue))
                }
            }
        }
    }

    end {

        Microsoft.PowerShell.Utility\Write-Verbose "Returning hashtable with $($results.Count) parameters"
        $results
    }
}
################################################################################