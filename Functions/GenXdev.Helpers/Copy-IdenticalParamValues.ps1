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

    $params = Copy-IdenticalParamValues -BoundParameters $PSBoundParameters `
        -FunctionName 'Get-ChildItem'

    Get-ChildItem @params
}
#>
function Copy-IdenticalParamValues {

    [CmdletBinding()]
    [OutputType([hashtable])]
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
        [hashtable] $defaults = (& {
                $asda2 = @{};
                $DefaultValues |
                Where-Object -Property Options -EQ "None" |
                ForEach-Object {
                    if ($filter.IndexOf($_.Name) -lt 0) {

                        if (-not ($_.Value -is [string] -and [string]::IsNullOrWhiteSpace($_.Value))) {

                            if ($null -ne $_.Value) {

                                $asda2."$($_.Name)" = $_.Value
                            }
                        }
                    }
                }
                $asda2
            });

        # get function info for parameter validation
        Write-Verbose "Getting command info for function '$FunctionName'"
        $functionInfo = Get-Command -Name $FunctionName -ErrorAction SilentlyContinue

        # validate function exists
        if ($null -eq $functionInfo) {

            Write-Error "Function '$FunctionName' not found"
            return
        }

        Write-Verbose "Found function with $($functionInfo.Parameters.Count) parameters"
    }

    process {

        # iterate through all parameters of the target function
        $functionInfo.Parameters.Keys | ForEach-Object {

            # get parameter name
            $paramName = $_

            # check if parameter exists in bound parameters
            if ($BoundParameters.ContainsKey($paramName)) {

                Write-Verbose "Copying value for parameter '$paramName'"
                $value = $BoundParameters[0].GetEnumerator() | Where-Object -Property Key -EQ $paramName | Select-Object -Property "Value"
                $results."$paramName" = $value.Value
            }
            else {

                $defaultValue = $defaults."$paramName"

                if ($null -ne $defaultValue) {

                    $results."$paramName" = $defaultValue

                    Write-Verbose "Using default value $(($defaultValue | ConvertTo-Json -Depth 1 -WarningAction SilentlyContinue -ErrorAction SilentlyContinue))"
                }
            }
        }
    }

    end {

        Write-Verbose "Returning hashtable with $($results.Count) parameters"
        Write-Output $results
    }
}
################################################################################
