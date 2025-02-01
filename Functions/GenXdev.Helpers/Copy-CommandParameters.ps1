###############################################################################

<#
.SYNOPSIS
    Proxy function dynamic parameter block for the Set-WindowPosition cmdlet
.DESCRIPTION
    The dynamic parameter block of a proxy function. This block can be used to copy a proxy function target's parameters .
#>
function Copy-CommandParameters {

    [System.Diagnostics.DebuggerStepThrough()]

    param(
        [parameter(Mandatory, Position = 0)]
        [string] $CommandName,

        [parameter(Mandatory = $false, Position = 1)]
        [string[]] $ParametersToSkip = @()
    )
    # $base = Get-Command $CommandName
    # $common = [System.Management.Automation.Internal.CommonParameters].GetProperties().name
    # $dict = [System.Management.Automation.RuntimeDefinedParameterDictionary]::new()
    # if ($base -and $base.Parameters) {

    #     $base.Parameters.GetEnumerator().foreach{
    #         $val = $PSItem.value
    #         $key = $PSItem.key
    #         if ($key -notin $common -and $key -notin $ParametersToSkip) {
    #             $param = [System.Management.Automation.RuntimeDefinedParameter]::new(
    #                 $key, $val.parameterType, $val.attributes)
    #             $dict.add($key, $param)
    #         }
    #     }
    # }
    # return $dict
    try {

        # the type of the command being proxied. Valid values include 'Cmdlet' or 'Function'.
        [System.Management.Automation.CommandTypes] $CommandType = [System.Management.Automation.CommandTypes]::Function;

        # look up the command being proxied.
        $wrappedCmd = $ExecutionContext.InvokeCommand.GetCommand($CommandName, $CommandType)

        # if the command was not found, throw an appropriate command not found exception.
        if (-not $wrappedCmd) {

            # the type of the command being proxied. Valid values include 'Cmdlet' or 'Function'.
            $CommandType = [System.Management.Automation.CommandTypes]::Cmdlet;
            $wrappedCmd = $ExecutionContext.InvokeCommand.GetCommand($CommandName, $CommandType)

            if (-not $wrappedCmd) {

                # look up the command being proxied.
                $wrappedCmd = $ExecutionContext.InvokeCommand.GetCommand($CommandName, $CommandType)

                $PSCmdlet.ThrowTerminatingError((New-Object System.Management.Automation.ErrorRecord((New-Object System.Exception "Command not found"), 'CommandNotFound', [System.Management.Automation.ErrorCategory]::ObjectNotFound, $CommandName)))
            }
        }

        # lookup the command metadata.
        $metadata = New-Object -TypeName System.Management.Automation.CommandMetadata -ArgumentList $wrappedCmd

        # create dynamic parameters, one for each parameter on the command being proxied.
        $dynamicDictionary = New-Object -TypeName System.Management.Automation.RuntimeDefinedParameterDictionary
        foreach ($key in $metadata.Parameters.Keys) {

            if ($ParametersToSkip -contains $key) { continue; }

            $parameter = $metadata.Parameters[$key]

            if ($dynamicDictionary.ContainsKey($parameter.Name)) { continue; }

            $dynamicParameter = New-Object -TypeName System.Management.Automation.RuntimeDefinedParameter -ArgumentList @(

                $parameter.Name, $parameter.ParameterType, $parameter.Attributes
            )

            $dynamicDictionary.Add($parameter.Name, $dynamicParameter)
        }

        $a = $wrappedCmd.ScriptBlock;

        if (($null -ne $a) -and (-not [string]::IsNullOrWhiteSpace($a.ToString()))) {

            $a = $a.ToString();
            $i = "$a".ToLowerInvariant().indexOf("dynamicparam");

            if ($i -ge 0) {

                $j = $a.indexOf("{", $i);
                $k = $a.indexOf("}", $j);

                if (($j -ge 0) -and ($k -ge 0)) {

                    $script = $a.substring($j + 1, $k - $j - 1);

                    $s = "Copy-CommandParameters -CommandName `"";
                    $i = $script.indexOf($s);
                    if ($i -ge 0) {

                        $i += $s.Length;
                        $script = $script.substring($i);
                        $j = $script.indexOf("`"");
                        $name = $script.substring(0, $j);
                        $script = $script.substring($j);
                        $ParametersToSkip = @()
                        $s = "-ParametersToSkip "
                        $i = $script.indexOf($s);

                        if ($i -ge 0) {

                            $i += $s.Length;
                            $script = $script.substring($i);
                            $j = $script.indexOf("`r`n");
                            if ($j -lt 0) { $j = $script.indexOf("`n"); }
                            if ($j -lt 0) { $j = $script.indexOf("`r"); }

                            $skips = $script;

                            if ($j -lt 0) {

                                $skips = $skips.substring(0, $j);
                            }

                            $ParametersToSkip = Invoke-Expression "@($skips)";
                        }

                        Copy-CommandParameters -CommandName $name -ParametersToSkip $ParametersToSkip | ForEach-Object {

                            $lib = $PSItem
                            $lib.Keys | ForEach-Object {

                                $p = $lib[$PSItem]

                                if (-not $dynamicDictionary.ContainsKey($p.Name)) {

                                    $dynamicDictionary.Add($p.Name, $p)
                                }
                            }
                        }
                    }
                }
            }
        }

        $dynamicDictionary
    }
    catch {
        $PSCmdlet.ThrowTerminatingError($PSItem)
    }
}
