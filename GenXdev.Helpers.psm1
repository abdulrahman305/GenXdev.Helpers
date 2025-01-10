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

    [CmdletBinding()]

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

    [CmdletBinding()]

    param(

        [parameter(ValueFromPipeline, Position = 0, Mandatory)]
        [string[]] $Json
    )

    process {

        [GenXdev.Helpers.Serialization]::RemoveJSONComments($json)
    }
}

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

###############################################################################

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

###############################################################################


<#
.SYNOPSIS
Sends a string to a serial port

.DESCRIPTION
Allows you to send a string to a serial communication port

.PARAMETER Portname
The port to use (for example, COM1).

.PARAMETER BaudRate
The baud rate.

.PARAMETER MaxBytesToRead
Limits the nr of bytes to read.

.PARAMETER ReadTimeout
Enables reading with a specified timeout in milliseconds.

.PARAMETER WriteTimeout
Enables writing with a specified timeout in milliseconds.

.PARAMETER Parity
One of the System.IO.Ports.SerialPort.Parity values.

.PARAMETER DataBits
The data bits value.

.PARAMETER StopBits
One of the System.IO.Ports.SerialPort.StopBits values.

.PARAMETER Text
Text to sent to serial port.

.PARAMETER AddCRLinefeeds
Add linefeeds to input text parts.

#>
function Out-Serial {
    param (
        [Parameter(Mandatory = $false,
            HelpMessage = "The port to use (for example, COM1).")]
        [string]$Portname = 'COM5',

        [Parameter(Mandatory = $false,
            HelpMessage = "The baud rate.")]
        [int]$BaudRate = 9600,

        [Parameter(Mandatory = $false,
            HelpMessage = "Limits the nr of bytes to read.")]
        [uint]$MaxBytesToRead = 0,

        [Parameter(Mandatory = $false,
            HelpMessage = "Enables reading with a specified timeout in milliseconds.")]
        [uint]$ReadTimeout,

        [Parameter(Mandatory = $false,
            HelpMessage = "Enables writing with a specified timeout in milliseconds.")]
        [uint]$WriteTimeout,

        [Parameter(Mandatory = $false,
            HelpMessage = "One of the System.IO.Ports.SerialPort.Parity values.")]
        [string]$Parity = 'None',

        [Parameter(Mandatory = $false,
            HelpMessage = "The data bits value.")]
        [int]$DataBits = 8,

        [Parameter(Mandatory = $false,
            HelpMessage = "One of the System.IO.Ports.SerialPort.StopBits values.")]
        [string]$StopBits = 'One',

        [Parameter(Mandatory, ValueFromPipeline,
            HelpMessage = "Text to sent to serial port.")]
        [object]$Text,

        [Parameter(Mandatory = $false,
            HelpMessage = "Add linefeeds to input text parts.")]
        [switch]$AddCRLinefeeds = $false
    )
    begin {
        $serialPort = New-Object System.IO.Ports.SerialPort -ArgumentList $Portname, $BaudRate, $Parity, $DataBits, $StopBits
        $serialPort.ReadTimeout = if ($ReadTimeout) { $ReadTimeout } else { 100 }
        $serialPort.WriteTimeout = if ($WriteTimeout) { $WriteTimeout } else { 100 }
        $serialPort.Open()
    }
    process {
        try {
            if ($Text -is [string]) {
                if ($AddCRLinefeeds) {
                    $serialPort.WriteLine($Text)
                }
                else {
                    $serialPort.Write($Text)
                }
            }
            if ($ReadTimeout -or $MaxBytesToRead -gt 0) {
                $serialPort.ReadTimeout = $ReadTimeout
                Start-Sleep -Milliseconds $serialPort.ReadTimeout
                $bytes = New-Object byte[] ($MaxBytesToRead -gt 0 ? $MaxBytesToRead : 1024)
                $readTotal = 0
                while ($readTotal -lt $bytes.Length) {
                    try {
                        $read = $serialPort.Read($bytes, $readTotal, $bytes.Length - $readTotal)
                        if ($read -eq 0) { break }
                        $readTotal += $read
                    }
                    catch {
                        if ($readTotal -gt 0) { Write-Output ([System.Text.Encoding]::ASCII.GetString($bytes, 0, $readTotal)) }
                        break
                    }
                }
                if ($readTotal -gt 0) { Write-Output ([System.Text.Encoding]::ASCII.GetString($bytes, 0, $readTotal)) }
            }
        }
        catch {
            Write-Verbose "Error occured: $PSItem"
        }
    }
    end {
        $serialPort.Close()
    }
}

################################################################################

function Get-ImageGeolocation {
    param (
        [Parameter(Mandatory)]
        [string]$ImagePath
    )

    if (-Not (Test-Path $ImagePath)) {
        Write-Error "The specified image path does not exist."
        return
    }

    try {
        $image = [System.Drawing.Image]::FromFile($ImagePath)
        $propertyItems = $image.PropertyItems

        $latitudeRef = $propertyItems | Where-Object { $PSItem.Id -eq 0x0001 }
        $latitude = $propertyItems | Where-Object { $PSItem.Id -eq 0x0002 }
        $longitudeRef = $propertyItems | Where-Object { $PSItem.Id -eq 0x0003 }
        $longitude = $propertyItems | Where-Object { $PSItem.Id -eq 0x0004 }

        if ($latitude -and $longitude -and $latitudeRef -and $longitudeRef) {
            $lat = [BitConverter]::ToUInt32($latitude.Value, 0) / [BitConverter]::ToUInt32($latitude.Value, 4)
            $lon = [BitConverter]::ToUInt32($longitude.Value, 0) / [BitConverter]::ToUInt32($longitude.Value, 4)

            if ($latitudeRef.Value -eq [byte][char]'S') { $lat = - $lat }
            if ($longitudeRef.Value -eq [byte][char]'W') { $lon = - $lon }

            return @{
                Latitude  = $lat
                Longitude = $lon
            }
        }
    }
    catch {
    }
}

################################################################################

function AssurePester {

    # Check if Pester is installed
    if (-not (Get-Module -Name Pester -ErrorAction SilentlyContinue)) {

        Write-Host "Pester not found. Installing Pester..."

        # Install Pester from the PowerShell Gallery
        try {
            Install-Module -Name Pester -Force -SkipPublisherCheck | Out-Null
            Import-Module -Name Pester -Force | Out-Null
            Write-Host "Pester installed successfully."
        }
        catch {

            Write-Error "Failed to install Pester. Error: $PSItem"
        }
    }
}

################################################################################

Import-Module "$PSScriptRoot\lib\GenXdev.Helpers.dll"
