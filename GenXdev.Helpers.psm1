<#
.SYNOPSIS
Converts an object to a JSON-formatted string - at full depth

.DESCRIPTION
The `ConvertTo-JsonEx` cmdlet converts any .NET object to a string in JavaScript Object Notation (JSON) format - at full depth

.PARAMETER object
Object to serialize

.PARAMETER Compress
Omits white space and indented formatting in the output string.

.EXAMPLE
 The `ConvertTo-JsonEx` cmdlet is implemented using Newtonsoft Json.NET (https://www.newtonsoft.com/json).

    -------------------------- Example 1 --------------------------

    (Get-UICulture).Calendar | ConvertTo-JsonEx

    {
      "MinSupportedDateTime": "0001-01-01T00:00:00",
      "MaxSupportedDateTime": "9999-12-31T23:59:59.9999999",
      "AlgorithmType": 1,
      "CalendarType": 1,
      "Eras": [
        1
      ],
      "TwoDigitYearMax": 2029,
      "IsReadOnly": true
    }
#>
function ConvertTo-JsonEx {

    param(

        [parameter(Position = 0, Mandatory)]
        [object] $object,

        [parameter(Mandatory = $false)]
        [switch] $Compress
    )

    process {

        [GenXdev.Helpers.Serialization]::ToJson($object, ($Compress -eq $true));
    }
}

<#
.SYNOPSIS
Removes any comment lines from a json file and return the result

.DESCRIPTION
Removes any comment lines from a json file and return the result

.PARAMETER Json
The json to filter for comments
#>
function Remove-JSONComments {

    param(

        [parameter(ValueFromPipeline = $true, Position = 0, Mandatory)]
        [string[]] $Json
    )

    process {

        [GenXdev.Helpers.Serialization]::RemoveJSONComments($json)
    }
}

##############################################################################################################
<#
.SYNOPSIS
    Proxy function dynamic parameter block for the Set-WindowPosition cmdlet
.DESCRIPTION
    The dynamic parameter block of a proxy function. This block can be used to copy a proxy function target's parameters, regardless of changes from version to version.
#>
function Copy-CommandParameters {

    [System.Diagnostics.DebuggerStepThrough()]

    param(
        [parameter(Mandatory, Position = 0)]
        [string] $CommandName,

        [parameter(Mandatory = $false, Position = 1)]
        [string[]] $ParametersToSkip = @()
    )
    # $base = Get-Command $CommandName
    # $common = [System.Management.Automation.Internal.CommonParameters].GetProperties().name
    # $dict = [System.Management.Automation.RuntimeDefinedParameterDictionary]::new()
    # if ($base -and $base.Parameters) {

    #     $base.Parameters.GetEnumerator().foreach{
    #         $val = $_.value
    #         $key = $_.key
    #         if ($key -notin $common -and $key -notin $ParametersToSkip) {
    #             $param = [System.Management.Automation.RuntimeDefinedParameter]::new(
    #                 $key, $val.parameterType, $val.attributes)
    #             $dict.add($key, $param)
    #         }
    #     }
    # }
    # return $dict
    try {

        # the type of the command being proxied. Valid values include 'Cmdlet' or 'Function'.
        [System.Management.Automation.CommandTypes] $CommandType = [System.Management.Automation.CommandTypes]::Function;

        # look up the command being proxied.
        $wrappedCmd = $ExecutionContext.InvokeCommand.GetCommand($CommandName, $CommandType)

        # if the command was not found, throw an appropriate command not found exception.
        if (-not $wrappedCmd) {

            $PSCmdlet.ThrowCommandNotFoundError($CommandName, $PSCmdlet.MyInvocation.MyCommand.Name)
        }

        # lookup the command metadata.
        $metadata = New-Object -TypeName System.Management.Automation.CommandMetadata -ArgumentList $wrappedCmd

        # create dynamic parameters, one for each parameter on the command being proxied.
        $dynamicDictionary = New-Object -TypeName System.Management.Automation.RuntimeDefinedParameterDictionary
        foreach ($key in $metadata.Parameters.Keys) {

            if ($ParametersToSkip -contains $key) { continue; }

            $parameter = $metadata.Parameters[$key]
            $dynamicParameter = New-Object -TypeName System.Management.Automation.RuntimeDefinedParameter -ArgumentList @(
                $parameter.Name, $parameter.ParameterType, $parameter.Attributes
            )
            $dynamicDictionary.Add($parameter.Name, $dynamicParameter)
        }
        $dynamicDictionary
    }
    catch {
        $PSCmdlet.ThrowTerminatingError($_)
    }
}

################################################################################

<#
.SYNOPSIS
Returns a string (with altered indentation) of a provided scriptblock string

.DESCRIPTION
Changes the indentation of a scriptblock string while respecting the original code-block identations

.PARAMETER script
The scriptblock string

.PARAMETER spaces
The minimum number of spaces for each line
#>
function alignScript([string] $script, [int] $spaces = 0) {

    $lines = @($script.Replace("`r`n", "`n").Replace("`t", "    ").Split("`n"));

    [int] $NrOfSpacesToTrim = [int]::MaxValue;

    $lines | ForEach-Object -ErrorAction SilentlyContinue {

        $c = 0;
        $s = $PSItem
        while (($s.Length -gt 0) -and ($s.substring(0, 1) -eq " ")) {

            $c++;
            if ($s.Length -gt 1) {

                $s = $s.substring(1);
            }
            else {

                $s = "";
            }
        }

        $NrOfSpacesToTrim = [Math]::Min($NrOfSpacesToTrim, $c);
    }

    if ($NrOfSpacesToTrim -eq [int]::MaxValue) {

        $NrOfSpacesToTrim = 0;
    }

    @($lines | ForEach-Object -ErrorAction SilentlyContinue {

            [int] $c = $NrOfSpacesToTrim;
            $a = $PSItem

            while (($a.Length -gt 0) -and ($a.substring(0, 1) -eq " ") -and ($c -gt 0)) {

                $c--;

                if ($a.Length -gt 1) {

                    $a = $a.substring(1);
                }
                else {

                    $a = "";
                }
            }

            for ($i = 0; $i -lt $spaces; $i++) {

                $a = " $a"
            }

            $a
        }) -Join "`r`n"
}

################################################################################
