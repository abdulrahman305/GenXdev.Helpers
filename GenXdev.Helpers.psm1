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

        [parameter(ValueFromPipeline = $true, Position = 0, Mandatory)]
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
    #         $val = $_.value
    #         $key = $_.key
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

            $PSCmdlet.ThrowCommandNotFoundError($CommandName, $PSCmdlet.MyInvocation.MyCommand.Name)
        }

        # lookup the command metadata.
        $metadata = New-Object -TypeName System.Management.Automation.CommandMetadata -ArgumentList $wrappedCmd

        # create dynamic parameters, one for each parameter on the command being proxied.
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

        [Parameter(Mandatory = $true, ValueFromPipeline = $true,
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
            Write-Verbose "Error occured: $_"
        }
    }
    end {
        $serialPort.Close()
    }
}

################################################################################
################################################################################
################################################################################

# SIG # Begin signature block
# MIIbzgYJKoZIhvcNAQcCoIIbvzCCG7sCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCBFGqzxE7l6CO9C
# G3TygvteNBt5FFp2476t6/4re8fcNKCCFhswggMOMIIB9qADAgECAhBwxOfTiuon
# hU3SZf3YwpWAMA0GCSqGSIb3DQEBCwUAMB8xHTAbBgNVBAMMFEdlblhkZXYgQXV0
# aGVudGljb2RlMB4XDTI0MDUwNTIwMzEzOFoXDTM0MDUwNTE4NDEzOFowHzEdMBsG
# A1UEAwwUR2VuWGRldiBBdXRoZW50aWNvZGUwggEiMA0GCSqGSIb3DQEBAQUAA4IB
# DwAwggEKAoIBAQDAD4JXwna5uBAYw54JXXscQPSos9pMeeyV99hvQPs6IcQ/wIXs
# zQ0xdkMGlzo1Nvldyqwa6+OXMyHsZM2D6QA1WjRoTzjT432hlGJT3VrP3R9cvOfg
# sAnVLpZy+4uty2fh5o8NEk4tmULOXDPZBT6NOoRjRCyt+KwCL8yioCFWa/7pqpG0
# niyJka8rhOVQLg8sZ+n5DrSihs1o3PyN28mZLendSbL9Y06cbqadL0J6sn31sw6e
# tpLOToIj1DXQbID0ejeafONHYJ3cKBrQ0TG7aoK8dte4X+iQQuDgA/l7ATxCjC7V
# 18vKRQXzSjvBQvNuWSw6DX2b7sc7dzC9v2T1AgMBAAGjRjBEMA4GA1UdDwEB/wQE
# AwIHgDATBgNVHSUEDDAKBggrBgEFBQcDAzAdBgNVHQ4EFgQUf8ZHrsKtJB9RD6z2
# x2Txu7wQ1/4wDQYJKoZIhvcNAQELBQADggEBAK/GgNjLVhQkhbFMrJUt3nFfYa2a
# iP/+U2vapwtqeyNBreMiTYwtqkULEPotRlRCMZ+k8kwRhv1bsR82MXK1H74DKcTM
# 0gu62RxOMXz8ij0BjXW9axEWqYGAbbP0EoNyoBzqiLYqXkwCXqIFsywuDZO4QY3D
# 1c+NEKVnPnhf/gufOUrlugklExh9i4QagCSlUObYAa9yBhcoxOHzN0v6mN+I7EjM
# sVsydPsk3NshubldpNSavFUcF477l21eM5F1bFXGTJGgGq9k1/drpILe5e4oLy9w
# sxmdnqpyvbwtPe2+LZx0XSlR5vCfYFih6eV8fNcgvMmAKAcuIuKxKwJkAscwggWN
# MIIEdaADAgECAhAOmxiO+dAt5+/bUOIIQBhaMA0GCSqGSIb3DQEBDAUAMGUxCzAJ
# BgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5k
# aWdpY2VydC5jb20xJDAiBgNVBAMTG0RpZ2lDZXJ0IEFzc3VyZWQgSUQgUm9vdCBD
# QTAeFw0yMjA4MDEwMDAwMDBaFw0zMTExMDkyMzU5NTlaMGIxCzAJBgNVBAYTAlVT
# MRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5j
# b20xITAfBgNVBAMTGERpZ2lDZXJ0IFRydXN0ZWQgUm9vdCBHNDCCAiIwDQYJKoZI
# hvcNAQEBBQADggIPADCCAgoCggIBAL/mkHNo3rvkXUo8MCIwaTPswqclLskhPfKK
# 2FnC4SmnPVirdprNrnsbhA3EMB/zG6Q4FutWxpdtHauyefLKEdLkX9YFPFIPUh/G
# nhWlfr6fqVcWWVVyr2iTcMKyunWZanMylNEQRBAu34LzB4TmdDttceItDBvuINXJ
# IB1jKS3O7F5OyJP4IWGbNOsFxl7sWxq868nPzaw0QF+xembud8hIqGZXV59UWI4M
# K7dPpzDZVu7Ke13jrclPXuU15zHL2pNe3I6PgNq2kZhAkHnDeMe2scS1ahg4AxCN
# 2NQ3pC4FfYj1gj4QkXCrVYJBMtfbBHMqbpEBfCFM1LyuGwN1XXhm2ToxRJozQL8I
# 11pJpMLmqaBn3aQnvKFPObURWBf3JFxGj2T3wWmIdph2PVldQnaHiZdpekjw4KIS
# G2aadMreSx7nDmOu5tTvkpI6nj3cAORFJYm2mkQZK37AlLTSYW3rM9nF30sEAMx9
# HJXDj/chsrIRt7t/8tWMcCxBYKqxYxhElRp2Yn72gLD76GSmM9GJB+G9t+ZDpBi4
# pncB4Q+UDCEdslQpJYls5Q5SUUd0viastkF13nqsX40/ybzTQRESW+UQUOsxxcpy
# FiIJ33xMdT9j7CFfxCBRa2+xq4aLT8LWRV+dIPyhHsXAj6KxfgommfXkaS+YHS31
# 2amyHeUbAgMBAAGjggE6MIIBNjAPBgNVHRMBAf8EBTADAQH/MB0GA1UdDgQWBBTs
# 1+OC0nFdZEzfLmc/57qYrhwPTzAfBgNVHSMEGDAWgBRF66Kv9JLLgjEtUYunpyGd
# 823IDzAOBgNVHQ8BAf8EBAMCAYYweQYIKwYBBQUHAQEEbTBrMCQGCCsGAQUFBzAB
# hhhodHRwOi8vb2NzcC5kaWdpY2VydC5jb20wQwYIKwYBBQUHMAKGN2h0dHA6Ly9j
# YWNlcnRzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydEFzc3VyZWRJRFJvb3RDQS5jcnQw
# RQYDVR0fBD4wPDA6oDigNoY0aHR0cDovL2NybDMuZGlnaWNlcnQuY29tL0RpZ2lD
# ZXJ0QXNzdXJlZElEUm9vdENBLmNybDARBgNVHSAECjAIMAYGBFUdIAAwDQYJKoZI
# hvcNAQEMBQADggEBAHCgv0NcVec4X6CjdBs9thbX979XB72arKGHLOyFXqkauyL4
# hxppVCLtpIh3bb0aFPQTSnovLbc47/T/gLn4offyct4kvFIDyE7QKt76LVbP+fT3
# rDB6mouyXtTP0UNEm0Mh65ZyoUi0mcudT6cGAxN3J0TU53/oWajwvy8LpunyNDzs
# 9wPHh6jSTEAZNUZqaVSwuKFWjuyk1T3osdz9HNj0d1pcVIxv76FQPfx2CWiEn2/K
# 2yCNNWAcAgPLILCsWKAOQGPFmCLBsln1VWvPJ6tsds5vIy30fnFqI2si/xK4VC0n
# ftg62fC2h5b9W9FcrBjDTZ9ztwGpn1eqXijiuZQwggauMIIElqADAgECAhAHNje3
# JFR82Ees/ShmKl5bMA0GCSqGSIb3DQEBCwUAMGIxCzAJBgNVBAYTAlVTMRUwEwYD
# VQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xITAf
# BgNVBAMTGERpZ2lDZXJ0IFRydXN0ZWQgUm9vdCBHNDAeFw0yMjAzMjMwMDAwMDBa
# Fw0zNzAzMjIyMzU5NTlaMGMxCzAJBgNVBAYTAlVTMRcwFQYDVQQKEw5EaWdpQ2Vy
# dCwgSW5jLjE7MDkGA1UEAxMyRGlnaUNlcnQgVHJ1c3RlZCBHNCBSU0E0MDk2IFNI
# QTI1NiBUaW1lU3RhbXBpbmcgQ0EwggIiMA0GCSqGSIb3DQEBAQUAA4ICDwAwggIK
# AoICAQDGhjUGSbPBPXJJUVXHJQPE8pE3qZdRodbSg9GeTKJtoLDMg/la9hGhRBVC
# X6SI82j6ffOciQt/nR+eDzMfUBMLJnOWbfhXqAJ9/UO0hNoR8XOxs+4rgISKIhjf
# 69o9xBd/qxkrPkLcZ47qUT3w1lbU5ygt69OxtXXnHwZljZQp09nsad/ZkIdGAHvb
# REGJ3HxqV3rwN3mfXazL6IRktFLydkf3YYMZ3V+0VAshaG43IbtArF+y3kp9zvU5
# EmfvDqVjbOSmxR3NNg1c1eYbqMFkdECnwHLFuk4fsbVYTXn+149zk6wsOeKlSNbw
# sDETqVcplicu9Yemj052FVUmcJgmf6AaRyBD40NjgHt1biclkJg6OBGz9vae5jtb
# 7IHeIhTZgirHkr+g3uM+onP65x9abJTyUpURK1h0QCirc0PO30qhHGs4xSnzyqqW
# c0Jon7ZGs506o9UD4L/wojzKQtwYSH8UNM/STKvvmz3+DrhkKvp1KCRB7UK/BZxm
# SVJQ9FHzNklNiyDSLFc1eSuo80VgvCONWPfcYd6T/jnA+bIwpUzX6ZhKWD7TA4j+
# s4/TXkt2ElGTyYwMO1uKIqjBJgj5FBASA31fI7tk42PgpuE+9sJ0sj8eCXbsq11G
# deJgo1gJASgADoRU7s7pXcheMBK9Rp6103a50g5rmQzSM7TNsQIDAQABo4IBXTCC
# AVkwEgYDVR0TAQH/BAgwBgEB/wIBADAdBgNVHQ4EFgQUuhbZbU2FL3MpdpovdYxq
# II+eyG8wHwYDVR0jBBgwFoAU7NfjgtJxXWRM3y5nP+e6mK4cD08wDgYDVR0PAQH/
# BAQDAgGGMBMGA1UdJQQMMAoGCCsGAQUFBwMIMHcGCCsGAQUFBwEBBGswaTAkBggr
# BgEFBQcwAYYYaHR0cDovL29jc3AuZGlnaWNlcnQuY29tMEEGCCsGAQUFBzAChjVo
# dHRwOi8vY2FjZXJ0cy5kaWdpY2VydC5jb20vRGlnaUNlcnRUcnVzdGVkUm9vdEc0
# LmNydDBDBgNVHR8EPDA6MDigNqA0hjJodHRwOi8vY3JsMy5kaWdpY2VydC5jb20v
# RGlnaUNlcnRUcnVzdGVkUm9vdEc0LmNybDAgBgNVHSAEGTAXMAgGBmeBDAEEAjAL
# BglghkgBhv1sBwEwDQYJKoZIhvcNAQELBQADggIBAH1ZjsCTtm+YqUQiAX5m1tgh
# QuGwGC4QTRPPMFPOvxj7x1Bd4ksp+3CKDaopafxpwc8dB+k+YMjYC+VcW9dth/qE
# ICU0MWfNthKWb8RQTGIdDAiCqBa9qVbPFXONASIlzpVpP0d3+3J0FNf/q0+KLHqr
# hc1DX+1gtqpPkWaeLJ7giqzl/Yy8ZCaHbJK9nXzQcAp876i8dU+6WvepELJd6f8o
# VInw1YpxdmXazPByoyP6wCeCRK6ZJxurJB4mwbfeKuv2nrF5mYGjVoarCkXJ38SN
# oOeY+/umnXKvxMfBwWpx2cYTgAnEtp/Nh4cku0+jSbl3ZpHxcpzpSwJSpzd+k1Os
# Ox0ISQ+UzTl63f8lY5knLD0/a6fxZsNBzU+2QJshIUDQtxMkzdwdeDrknq3lNHGS
# 1yZr5Dhzq6YBT70/O3itTK37xJV77QpfMzmHQXh6OOmc4d0j/R0o08f56PGYX/sr
# 2H7yRp11LB4nLCbbbxV7HhmLNriT1ObyF5lZynDwN7+YAN8gFk8n+2BnFqFmut1V
# wDophrCYoCvtlUG3OtUVmDG0YgkPCr2B2RP+v6TR81fZvAT6gt4y3wSJ8ADNXcL5
# 0CN/AAvkdgIm2fBldkKmKYcJRyvmfxqkhQ/8mJb2VVQrH4D6wPIOK+XW+6kvRBVK
# 5xMOHds3OBqhK/bt1nz8MIIGwjCCBKqgAwIBAgIQBUSv85SdCDmmv9s/X+VhFjAN
# BgkqhkiG9w0BAQsFADBjMQswCQYDVQQGEwJVUzEXMBUGA1UEChMORGlnaUNlcnQs
# IEluYy4xOzA5BgNVBAMTMkRpZ2lDZXJ0IFRydXN0ZWQgRzQgUlNBNDA5NiBTSEEy
# NTYgVGltZVN0YW1waW5nIENBMB4XDTIzMDcxNDAwMDAwMFoXDTM0MTAxMzIzNTk1
# OVowSDELMAkGA1UEBhMCVVMxFzAVBgNVBAoTDkRpZ2lDZXJ0LCBJbmMuMSAwHgYD
# VQQDExdEaWdpQ2VydCBUaW1lc3RhbXAgMjAyMzCCAiIwDQYJKoZIhvcNAQEBBQAD
# ggIPADCCAgoCggIBAKNTRYcdg45brD5UsyPgz5/X5dLnXaEOCdwvSKOXejsqnGfc
# YhVYwamTEafNqrJq3RApih5iY2nTWJw1cb86l+uUUI8cIOrHmjsvlmbjaedp/lvD
# 1isgHMGXlLSlUIHyz8sHpjBoyoNC2vx/CSSUpIIa2mq62DvKXd4ZGIX7ReoNYWyd
# /nFexAaaPPDFLnkPG2ZS48jWPl/aQ9OE9dDH9kgtXkV1lnX+3RChG4PBuOZSlbVH
# 13gpOWvgeFmX40QrStWVzu8IF+qCZE3/I+PKhu60pCFkcOvV5aDaY7Mu6QXuqvYk
# 9R28mxyyt1/f8O52fTGZZUdVnUokL6wrl76f5P17cz4y7lI0+9S769SgLDSb495u
# ZBkHNwGRDxy1Uc2qTGaDiGhiu7xBG3gZbeTZD+BYQfvYsSzhUa+0rRUGFOpiCBPT
# aR58ZE2dD9/O0V6MqqtQFcmzyrzXxDtoRKOlO0L9c33u3Qr/eTQQfqZcClhMAD6F
# aXXHg2TWdc2PEnZWpST618RrIbroHzSYLzrqawGw9/sqhux7UjipmAmhcbJsca8+
# uG+W1eEQE/5hRwqM/vC2x9XH3mwk8L9CgsqgcT2ckpMEtGlwJw1Pt7U20clfCKRw
# o+wK8REuZODLIivK8SgTIUlRfgZm0zu++uuRONhRB8qUt+JQofM604qDy0B7AgMB
# AAGjggGLMIIBhzAOBgNVHQ8BAf8EBAMCB4AwDAYDVR0TAQH/BAIwADAWBgNVHSUB
# Af8EDDAKBggrBgEFBQcDCDAgBgNVHSAEGTAXMAgGBmeBDAEEAjALBglghkgBhv1s
# BwEwHwYDVR0jBBgwFoAUuhbZbU2FL3MpdpovdYxqII+eyG8wHQYDVR0OBBYEFKW2
# 7xPn783QZKHVVqllMaPe1eNJMFoGA1UdHwRTMFEwT6BNoEuGSWh0dHA6Ly9jcmwz
# LmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydFRydXN0ZWRHNFJTQTQwOTZTSEEyNTZUaW1l
# U3RhbXBpbmdDQS5jcmwwgZAGCCsGAQUFBwEBBIGDMIGAMCQGCCsGAQUFBzABhhho
# dHRwOi8vb2NzcC5kaWdpY2VydC5jb20wWAYIKwYBBQUHMAKGTGh0dHA6Ly9jYWNl
# cnRzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydFRydXN0ZWRHNFJTQTQwOTZTSEEyNTZU
# aW1lU3RhbXBpbmdDQS5jcnQwDQYJKoZIhvcNAQELBQADggIBAIEa1t6gqbWYF7xw
# jU+KPGic2CX/yyzkzepdIpLsjCICqbjPgKjZ5+PF7SaCinEvGN1Ott5s1+FgnCvt
# 7T1IjrhrunxdvcJhN2hJd6PrkKoS1yeF844ektrCQDifXcigLiV4JZ0qBXqEKZi2
# V3mP2yZWK7Dzp703DNiYdk9WuVLCtp04qYHnbUFcjGnRuSvExnvPnPp44pMadqJp
# ddNQ5EQSviANnqlE0PjlSXcIWiHFtM+YlRpUurm8wWkZus8W8oM3NG6wQSbd3lqX
# TzON1I13fXVFoaVYJmoDRd7ZULVQjK9WvUzF4UbFKNOt50MAcN7MmJ4ZiQPq1JE3
# 701S88lgIcRWR+3aEUuMMsOI5ljitts++V+wQtaP4xeR0arAVeOGv6wnLEHQmjNK
# qDbUuXKWfpd5OEhfysLcPTLfddY2Z1qJ+Panx+VPNTwAvb6cKmx5AdzaROY63jg7
# B145WPR8czFVoIARyxQMfq68/qTreWWqaNYiyjvrmoI1VygWy2nyMpqy0tg6uLFG
# hmu6F/3Ed2wVbK6rr3M66ElGt9V/zLY4wNjsHPW2obhDLN9OTH0eaHDAdwrUAuBc
# YLso/zjlUlrWrBciI0707NMX+1Br/wd3H3GXREHJuEbTbDJ8WC9nR2XlG3O2mflr
# LAZG70Ee8PBf4NvZrZCARK+AEEGKMYIFCTCCBQUCAQEwMzAfMR0wGwYDVQQDDBRH
# ZW5YZGV2IEF1dGhlbnRpY29kZQIQcMTn04rqJ4VN0mX92MKVgDANBglghkgBZQME
# AgEFAKCBhDAYBgorBgEEAYI3AgEMMQowCKACgAChAoAAMBkGCSqGSIb3DQEJAzEM
# BgorBgEEAYI3AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEVMC8GCSqG
# SIb3DQEJBDEiBCAJOPXoqOyxC0hx3wcswOSd8u6sWC/nuuF3RZwqLDp5PzANBgkq
# hkiG9w0BAQEFAASCAQCDmAR7Ile83XvOTndvejbHUyQ20vorjP+8hFgigYnKPGxl
# DOl/l5TGhjnAhPGbYuximCMoKJPm3VJzJBN9jWfarcb34NR9vfri8jvUVhMecSpb
# wNr4XoBcItrel/sCozj7K2f6lrCp2vSLy8MSAuzhSM6wy5ksP7/6j5DQ1cNjpqRi
# hgWr4GcyEbDVR4qnrZq+nyU9+j0QbBmVF/1Bs9Gq4Kr2Mzz/xTcYa0i6kt+a4xXe
# qVmzwTPdv2/eQxcMogmT/tlXCIb97DhnQ/X7TpmoZOFqu1pQzIi0srQtrpIAL+6d
# QZaFO4MN7+7txQBOQ6M9Tv6H9XRx2/sQqvATkgI7oYIDIDCCAxwGCSqGSIb3DQEJ
# BjGCAw0wggMJAgEBMHcwYzELMAkGA1UEBhMCVVMxFzAVBgNVBAoTDkRpZ2lDZXJ0
# LCBJbmMuMTswOQYDVQQDEzJEaWdpQ2VydCBUcnVzdGVkIEc0IFJTQTQwOTYgU0hB
# MjU2IFRpbWVTdGFtcGluZyBDQQIQBUSv85SdCDmmv9s/X+VhFjANBglghkgBZQME
# AgEFAKBpMBgGCSqGSIb3DQEJAzELBgkqhkiG9w0BBwEwHAYJKoZIhvcNAQkFMQ8X
# DTI0MDUyNjE3MDcxN1owLwYJKoZIhvcNAQkEMSIEIOxF9/GVoTRMXQXn2Ns2DFmC
# FZV2NCxlGu/jmq37HPV/MA0GCSqGSIb3DQEBAQUABIICADCiD7TLv1KFUT7qGUN+
# nQmiMSx3yUThS1hGSxIjTyZHUYaDn1LoyqG4NY7DiVGo1h46Beu1AI7qTi+UwrOQ
# SwGLZHX7AH84hPpEkCLZwyUJm8XvWC6T7wOdXsh2OTOhwwWQ1Cg+PiS+ZilLr9r5
# Wp3UvIPN8s4O/m/y7twdcZ8hQvqLeP/0STHjyRGkh9hwU+98o3i5kc768ZMrPPJm
# XqQgO0gg1PwHHUae/qV6OWoAV21aXo7ZhTlLZ9zqG4DPGKAWBCN3SqCvQ8qhbN2b
# kWhYhq4AfPmibyjqwGnHYcChWr8uuDCZHERepORTOWV7Q7i3CIEUZ3Oq1szGnZc8
# slFL1bgTtm9wgeb3Iq8SVAt6LjbiFHrM0OzWtTzWszTP2uUUahNWltunfGvhc89J
# 4M++9nWEb8xlbUE9wnMwM4ZP4+mYc+aS3JgUY7nuq7RvI/z/pExco7mkNZCFCHI2
# kjiP2ItNbIq/D8HYHlxa92erkPyvzcf1fAEddaDngmdXsh8ko6tlSmlHsyQ6ml1L
# 6OnCYu3l4R04SLYEacYS5iLVoz0zhAx9JtQbmDQEm0+ewJ+zFXq6WVqczfhocSun
# S6WxCwS8U/eTc5EpjD9lQrpxqG9bg3FbZZCllK22NjPtuKMYDCtDRrJrzDPTSMR0
# XMtRiX+YMxXZPfPP/WAGE2ft
# SIG # End signature block
