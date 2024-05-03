# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
  PowerShell Diagnostics Module
  This module contains a set of wrapper scripts that
  enable a user to use ETW tracing in Windows
  PowerShell.
 #>

$script:Logman="$env:windir\system32\logman.exe"
$script:wsmanlogfile = "$env:windir\system32\wsmtraces.log"
$script:wsmprovfile = "$env:windir\system32\wsmtraceproviders.txt"
$script:wsmsession = "wsmlog"
$script:pssession = "PSTrace"
$script:psprovidername="Microsoft-Windows-PowerShell"
$script:wsmprovidername = "Microsoft-Windows-WinRM"
$script:oplog = "/Operational"
$script:analyticlog="/Analytic"
$script:debuglog="/Debug"
$script:wevtutil="$env:windir\system32\wevtutil.exe"
$script:slparam = "sl"
$script:glparam = "gl"

function Start-Trace
{
    Param(
    [Parameter(Mandatory=$true,
               Position=0)]
    [string]
    $SessionName,
    [Parameter(Position=1)]
    [ValidateNotNullOrEmpty()]
    [string]
    $OutputFilePath,
    [Parameter(Position=2)]
    [ValidateNotNullOrEmpty()]
    [string]
    $ProviderFilePath,
    [Parameter()]
    [Switch]
    $ETS,
    [Parameter()]
    [ValidateSet("bin", "bincirc", "csv", "tsv", "sql")]
    $Format,
    [Parameter()]
    [int]
    $MinBuffers=0,
    [Parameter()]
    [int]
    $MaxBuffers=256,
    [Parameter()]
    [int]
    $BufferSizeInKB = 0,
    [Parameter()]
    [int]
    $MaxLogFileSizeInMB=0
    )

    Process
    {
        $executestring = " start $SessionName"

        if ($ETS)
        {
            $executestring += " -ets"
        }

        if ($null -ne $OutputFilePath)
        {
            $executestring += " -o ""$OutputFilePath"""
        }

        if ($null -ne $ProviderFilePath)
        {
            $executestring += " -pf ""$ProviderFilePath"""
        }

        if ($null -ne $Format)
        {
            $executestring += " -f $Format"
        }

        if ($MinBuffers -ne 0 -or $MaxBuffers -ne 256)
        {
            $executestring += " -nb $MinBuffers $MaxBuffers"
        }

        if ($BufferSizeInKB -ne 0)
        {
            $executestring += " -bs $BufferSizeInKB"
        }

        if ($MaxLogFileSizeInMB -ne 0)
        {
            $executestring += " -max $MaxLogFileSizeInMB"
        }

        & $script:Logman $executestring.Split(" ")
    }
}

function Stop-Trace
{
    param(
    [Parameter(Mandatory=$true,
               Position=0)]
    $SessionName,
    [Parameter()]
    [switch]
    $ETS
    )

    Process
    {
        if ($ETS)
        {
            & $script:Logman update $SessionName -ets
            & $script:Logman stop $SessionName -ets
        }
        else
        {
            & $script:Logman update $SessionName
            & $script:Logman stop $SessionName
        }
    }
}

function Enable-WSManTrace
{

    # winrm
    "{04c6e16d-b99f-4a3a-9b3e-b8325bbc781e} 0xffffffff 0xff" | Out-File $script:wsmprovfile -Encoding ascii

    # winrsmgr
    "{c0a36be8-a515-4cfa-b2b6-2676366efff7} 0xffffffff 0xff" | Out-File $script:wsmprovfile -Encoding ascii -Append

    # WinrsExe
    "{f1cab2c0-8beb-4fa2-90e1-8f17e0acdd5d} 0xffffffff 0xff" | Out-File $script:wsmprovfile -Encoding ascii -Append

    # WinrsCmd
    "{03992646-3dfe-4477-80e3-85936ace7abb} 0xffffffff 0xff" | Out-File $script:wsmprovfile -Encoding ascii -Append

    # IPMIPrv
    "{651d672b-e11f-41b7-add3-c2f6a4023672} 0xffffffff 0xff" | Out-File $script:wsmprovfile -Encoding ascii -Append

    #IpmiDrv
    "{D5C6A3E9-FA9C-434e-9653-165B4FC869E4} 0xffffffff 0xff" | Out-File $script:wsmprovfile -Encoding ascii -Append

    # WSManProvHost
    "{6e1b64d7-d3be-4651-90fb-3583af89d7f1} 0xffffffff 0xff" | Out-File $script:wsmprovfile -Encoding ascii -Append

    # Event Forwarding
    "{6FCDF39A-EF67-483D-A661-76D715C6B008} 0xffffffff 0xff" | Out-File $script:wsmprovfile -Encoding ascii -Append

    Start-Trace -SessionName $script:wsmsession -ETS -OutputFilePath $script:wsmanlogfile -Format bincirc -MinBuffers 16 -MaxBuffers 256 -BufferSizeInKB 64 -MaxLogFileSizeInMB 256 -ProviderFilePath $script:wsmprovfile
}

function Disable-WSManTrace
{
    Stop-Trace $script:wsmsession -ETS
}

function Enable-PSWSManCombinedTrace
{
    param (
        [switch] $DoNotOverwriteExistingTrace
    )

    $provfile = [io.path]::GetTempFilename()

    $traceFileName = [string][Guid]::NewGuid()
    if ($DoNotOverwriteExistingTrace) {
        $fileName = [string][guid]::newguid()
        $logfile = $PSHOME + "\\Traces\\PSTrace_$fileName.etl"
    } else {
        $logfile = $PSHOME + "\\Traces\\PSTrace.etl"
    }

    "Microsoft-Windows-PowerShell 0 5" | Out-File $provfile -Encoding ascii
    "Microsoft-Windows-WinRM 0 5" | Out-File $provfile -Encoding ascii -Append

    if (!(Test-Path $PSHOME\Traces))
    {
        New-Item -ItemType Directory -Force $PSHOME\Traces | Out-Null
    }

    if (Test-Path $logfile)
    {
        Remove-Item -Force $logfile | Out-Null
    }

    Start-Trace -SessionName $script:pssession -OutputFilePath $logfile -ProviderFilePath $provfile -ETS

    Remove-Item $provfile -Force -ea 0
}

function Disable-PSWSManCombinedTrace
{
    Stop-Trace -SessionName $script:pssession -ETS
}

function Set-LogProperties
{
    param(
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [Microsoft.PowerShell.Diagnostics.LogDetails]
        $LogDetails,
        [switch] $Force
     )

    Process
    {
        if ($LogDetails.AutoBackup -and !$LogDetails.Retention)
        {
            throw (New-Object System.InvalidOperationException)
        }

        $enabled = $LogDetails.Enabled.ToString()
        $retention = $LogDetails.Retention.ToString()
        $autobackup = $LogDetails.AutoBackup.ToString()
        $maxLogSize = $LogDetails.MaxLogSize.ToString()
        $osVersion = [Version] (Get-CimInstance Win32_OperatingSystem).Version

        if (($LogDetails.Type -eq "Analytic") -or ($LogDetails.Type -eq "Debug"))
        {
            if ($LogDetails.Enabled)
            {
                if($osVersion -lt 6.3.7600)
                {
                    & $script:wevtutil $script:slparam $LogDetails.Name -e:$Enabled
                }
                else
                {
                    & $script:wevtutil /q:$Force $script:slparam $LogDetails.Name -e:$Enabled
                }
            }
            else
            {
                if($osVersion -lt 6.3.7600)
                {
                    & $script:wevtutil $script:slparam $LogDetails.Name -e:$Enabled -rt:$Retention -ms:$MaxLogSize
                }
                else
                {
                    & $script:wevtutil /q:$Force $script:slparam $LogDetails.Name -e:$Enabled -rt:$Retention -ms:$MaxLogSize
                }
            }
        }
        else
        {
            if($osVersion -lt 6.3.7600)
            {
                & $script:wevtutil $script:slparam $LogDetails.Name -e:$Enabled -rt:$Retention -ab:$AutoBackup -ms:$MaxLogSize
            }
            else
            {
                & $script:wevtutil /q:$Force $script:slparam $LogDetails.Name -e:$Enabled -rt:$Retention -ab:$AutoBackup -ms:$MaxLogSize
            }
        }
    }
}

function ConvertTo-Bool([string]$value)
{
    if ($value -ieq "true")
    {
        return $true
    }
    else
    {
        return $false
    }
}

function Get-LogProperties
{
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true, Position=0)] $Name
    )

    Process
    {
        $details = & $script:wevtutil $script:glparam $Name
        $indexes = @(1,2,8,9,10)
        $value = @()
        foreach($index in $indexes)
        {
            $value += @(($details[$index].SubString($details[$index].IndexOf(":")+1)).Trim())
        }

        $enabled = ConvertTo-Bool $value[0]
        $retention = ConvertTo-Bool $value[2]
        $autobackup = ConvertTo-Bool $value[3]

        New-Object Microsoft.PowerShell.Diagnostics.LogDetails $Name, $enabled, $value[1], $retention, $autobackup, $value[4]
    }
}

function Enable-PSTrace
{
    param(
        [switch] $Force,
		[switch] $AnalyticOnly
     )

    $Properties = Get-LogProperties ($script:psprovidername + $script:analyticlog)

	if (!$Properties.Enabled) {
		$Properties.Enabled = $true
		if ($Force) {
			Set-LogProperties $Properties -Force
		} else {
			Set-LogProperties $Properties
		}
	}

	if (!$AnalyticOnly) {
		$Properties = Get-LogProperties ($script:psprovidername + $script:debuglog)
		if (!$Properties.Enabled) {
			$Properties.Enabled = $true
			if ($Force) {
				Set-LogProperties $Properties -Force
			} else {
				Set-LogProperties $Properties
			}
		}
	}
}

function Disable-PSTrace
{
    param(
		[switch] $AnalyticOnly
     )
    $Properties = Get-LogProperties ($script:psprovidername + $script:analyticlog)
	if ($Properties.Enabled) {
		$Properties.Enabled = $false
		Set-LogProperties $Properties
	}

	if (!$AnalyticOnly) {
		$Properties = Get-LogProperties ($script:psprovidername + $script:debuglog)
		if ($Properties.Enabled) {
			$Properties.Enabled = $false
			Set-LogProperties $Properties
		}
	}
}
Add-Type @"
using System;

namespace Microsoft.PowerShell.Diagnostics
{
    public class LogDetails
    {
        public string Name
        {
            get
            {
                return name;
            }
        }
        private string name;

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }
        private bool enabled;

        public string Type
        {
            get
            {
                return type;
            }
        }
        private string type;

        public bool Retention
        {
            get
            {
                return retention;
            }
            set
            {
                retention = value;
            }
        }
        private bool retention;

        public bool AutoBackup
        {
            get
            {
                return autoBackup;
            }
            set
            {
                autoBackup = value;
            }
        }
        private bool autoBackup;

        public int MaxLogSize
        {
            get
            {
                return maxLogSize;
            }
            set
            {
                maxLogSize = value;
            }
        }
        private int maxLogSize;

        public LogDetails(string name, bool enabled, string type, bool retention, bool autoBackup, int maxLogSize)
        {
            this.name = name;
            this.enabled = enabled;
            this.type = type;
            this.retention = retention;
            this.autoBackup = autoBackup;
            this.maxLogSize = maxLogSize;
        }
    }
}
"@

if (Get-Command logman.exe -Type Application -ErrorAction SilentlyContinue)
{
    Export-ModuleMember Disable-PSTrace, Disable-PSWSManCombinedTrace, Disable-WSManTrace, Enable-PSTrace, Enable-PSWSManCombinedTrace, Enable-WSManTrace, Get-LogProperties, Set-LogProperties, Start-Trace, Stop-Trace
}
else
{
    # Currently we only support these cmdlets as logman.exe is not available on systems like Nano and IoT
    Export-ModuleMember Disable-PSTrace, Enable-PSTrace, Get-LogProperties, Set-LogProperties
}

# SIG # Begin signature block
# MIInzQYJKoZIhvcNAQcCoIInvjCCJ7oCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCBzuFp8c8UN6ilP
# /hSmUPDtAUbo8uWTs6wUbaR/ZpM1B6CCDYUwggYDMIID66ADAgECAhMzAAADri01
# UchTj1UdAAAAAAOuMA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTEwHhcNMjMxMTE2MTkwODU5WhcNMjQxMTE0MTkwODU5WjB0MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYDVQQDExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQD0IPymNjfDEKg+YyE6SjDvJwKW1+pieqTjAY0CnOHZ1Nj5irGjNZPMlQ4HfxXG
# yAVCZcEWE4x2sZgam872R1s0+TAelOtbqFmoW4suJHAYoTHhkznNVKpscm5fZ899
# QnReZv5WtWwbD8HAFXbPPStW2JKCqPcZ54Y6wbuWV9bKtKPImqbkMcTejTgEAj82
# 6GQc6/Th66Koka8cUIvz59e/IP04DGrh9wkq2jIFvQ8EDegw1B4KyJTIs76+hmpV
# M5SwBZjRs3liOQrierkNVo11WuujB3kBf2CbPoP9MlOyyezqkMIbTRj4OHeKlamd
# WaSFhwHLJRIQpfc8sLwOSIBBAgMBAAGjggGCMIIBfjAfBgNVHSUEGDAWBgorBgEE
# AYI3TAgBBggrBgEFBQcDAzAdBgNVHQ4EFgQUhx/vdKmXhwc4WiWXbsf0I53h8T8w
# VAYDVR0RBE0wS6RJMEcxLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5kIE9wZXJh
# dGlvbnMgTGltaXRlZDEWMBQGA1UEBRMNMjMwMDEyKzUwMTgzNjAfBgNVHSMEGDAW
# gBRIbmTlUAXTgqoXNzcitW2oynUClTBUBgNVHR8ETTBLMEmgR6BFhkNodHRwOi8v
# d3d3Lm1pY3Jvc29mdC5jb20vcGtpb3BzL2NybC9NaWNDb2RTaWdQQ0EyMDExXzIw
# MTEtMDctMDguY3JsMGEGCCsGAQUFBwEBBFUwUzBRBggrBgEFBQcwAoZFaHR0cDov
# L3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9jZXJ0cy9NaWNDb2RTaWdQQ0EyMDEx
# XzIwMTEtMDctMDguY3J0MAwGA1UdEwEB/wQCMAAwDQYJKoZIhvcNAQELBQADggIB
# AGrJYDUS7s8o0yNprGXRXuAnRcHKxSjFmW4wclcUTYsQZkhnbMwthWM6cAYb/h2W
# 5GNKtlmj/y/CThe3y/o0EH2h+jwfU/9eJ0fK1ZO/2WD0xi777qU+a7l8KjMPdwjY
# 0tk9bYEGEZfYPRHy1AGPQVuZlG4i5ymJDsMrcIcqV8pxzsw/yk/O4y/nlOjHz4oV
# APU0br5t9tgD8E08GSDi3I6H57Ftod9w26h0MlQiOr10Xqhr5iPLS7SlQwj8HW37
# ybqsmjQpKhmWul6xiXSNGGm36GarHy4Q1egYlxhlUnk3ZKSr3QtWIo1GGL03hT57
# xzjL25fKiZQX/q+II8nuG5M0Qmjvl6Egltr4hZ3e3FQRzRHfLoNPq3ELpxbWdH8t
# Nuj0j/x9Crnfwbki8n57mJKI5JVWRWTSLmbTcDDLkTZlJLg9V1BIJwXGY3i2kR9i
# 5HsADL8YlW0gMWVSlKB1eiSlK6LmFi0rVH16dde+j5T/EaQtFz6qngN7d1lvO7uk
# 6rtX+MLKG4LDRsQgBTi6sIYiKntMjoYFHMPvI/OMUip5ljtLitVbkFGfagSqmbxK
# 7rJMhC8wiTzHanBg1Rrbff1niBbnFbbV4UDmYumjs1FIpFCazk6AADXxoKCo5TsO
# zSHqr9gHgGYQC2hMyX9MGLIpowYCURx3L7kUiGbOiMwaMIIHejCCBWKgAwIBAgIK
# YQ6Q0gAAAAAAAzANBgkqhkiG9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNV
# BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jv
# c29mdCBDb3Jwb3JhdGlvbjEyMDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlm
# aWNhdGUgQXV0aG9yaXR5IDIwMTEwHhcNMTEwNzA4MjA1OTA5WhcNMjYwNzA4MjEw
# OTA5WjB+MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSgwJgYD
# VQQDEx9NaWNyb3NvZnQgQ29kZSBTaWduaW5nIFBDQSAyMDExMIICIjANBgkqhkiG
# 9w0BAQEFAAOCAg8AMIICCgKCAgEAq/D6chAcLq3YbqqCEE00uvK2WCGfQhsqa+la
# UKq4BjgaBEm6f8MMHt03a8YS2AvwOMKZBrDIOdUBFDFC04kNeWSHfpRgJGyvnkmc
# 6Whe0t+bU7IKLMOv2akrrnoJr9eWWcpgGgXpZnboMlImEi/nqwhQz7NEt13YxC4D
# dato88tt8zpcoRb0RrrgOGSsbmQ1eKagYw8t00CT+OPeBw3VXHmlSSnnDb6gE3e+
# lD3v++MrWhAfTVYoonpy4BI6t0le2O3tQ5GD2Xuye4Yb2T6xjF3oiU+EGvKhL1nk
# kDstrjNYxbc+/jLTswM9sbKvkjh+0p2ALPVOVpEhNSXDOW5kf1O6nA+tGSOEy/S6
# A4aN91/w0FK/jJSHvMAhdCVfGCi2zCcoOCWYOUo2z3yxkq4cI6epZuxhH2rhKEmd
# X4jiJV3TIUs+UsS1Vz8kA/DRelsv1SPjcF0PUUZ3s/gA4bysAoJf28AVs70b1FVL
# 5zmhD+kjSbwYuER8ReTBw3J64HLnJN+/RpnF78IcV9uDjexNSTCnq47f7Fufr/zd
# sGbiwZeBe+3W7UvnSSmnEyimp31ngOaKYnhfsi+E11ecXL93KCjx7W3DKI8sj0A3
# T8HhhUSJxAlMxdSlQy90lfdu+HggWCwTXWCVmj5PM4TasIgX3p5O9JawvEagbJjS
# 4NaIjAsCAwEAAaOCAe0wggHpMBAGCSsGAQQBgjcVAQQDAgEAMB0GA1UdDgQWBBRI
# bmTlUAXTgqoXNzcitW2oynUClTAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMAQTAL
# BgNVHQ8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBRyLToCMZBD
# uRQFTuHqp8cx0SOJNDBaBgNVHR8EUzBRME+gTaBLhklodHRwOi8vY3JsLm1pY3Jv
# c29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFf
# MDNfMjIuY3JsMF4GCCsGAQUFBwEBBFIwUDBOBggrBgEFBQcwAoZCaHR0cDovL3d3
# dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFf
# MDNfMjIuY3J0MIGfBgNVHSAEgZcwgZQwgZEGCSsGAQQBgjcuAzCBgzA/BggrBgEF
# BQcCARYzaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9kb2NzL3ByaW1h
# cnljcHMuaHRtMEAGCCsGAQUFBwICMDQeMiAdAEwAZQBnAGEAbABfAHAAbwBsAGkA
# YwB5AF8AcwB0AGEAdABlAG0AZQBuAHQALiAdMA0GCSqGSIb3DQEBCwUAA4ICAQBn
# 8oalmOBUeRou09h0ZyKbC5YR4WOSmUKWfdJ5DJDBZV8uLD74w3LRbYP+vj/oCso7
# v0epo/Np22O/IjWll11lhJB9i0ZQVdgMknzSGksc8zxCi1LQsP1r4z4HLimb5j0b
# pdS1HXeUOeLpZMlEPXh6I/MTfaaQdION9MsmAkYqwooQu6SpBQyb7Wj6aC6VoCo/
# KmtYSWMfCWluWpiW5IP0wI/zRive/DvQvTXvbiWu5a8n7dDd8w6vmSiXmE0OPQvy
# CInWH8MyGOLwxS3OW560STkKxgrCxq2u5bLZ2xWIUUVYODJxJxp/sfQn+N4sOiBp
# mLJZiWhub6e3dMNABQamASooPoI/E01mC8CzTfXhj38cbxV9Rad25UAqZaPDXVJi
# hsMdYzaXht/a8/jyFqGaJ+HNpZfQ7l1jQeNbB5yHPgZ3BtEGsXUfFL5hYbXw3MYb
# BL7fQccOKO7eZS/sl/ahXJbYANahRr1Z85elCUtIEJmAH9AAKcWxm6U/RXceNcbS
# oqKfenoi+kiVH6v7RyOA9Z74v2u3S5fi63V4GuzqN5l5GEv/1rMjaHXmr/r8i+sL
# gOppO6/8MO0ETI7f33VtY5E90Z1WTk+/gFcioXgRMiF670EKsT/7qMykXcGhiJtX
# cVZOSEXAQsmbdlsKgEhr/Xmfwb1tbWrJUnMTDXpQzTGCGZ4wghmaAgEBMIGVMH4x
# CzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRt
# b25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01p
# Y3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBIDIwMTECEzMAAAOuLTVRyFOPVR0AAAAA
# A64wDQYJYIZIAWUDBAIBBQCgga4wGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQw
# HAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwLwYJKoZIhvcNAQkEMSIEIEgd
# 441WP1jGfFhxRg3sMtiUC5JDLIMXuzkPYRsZdTzgMEIGCisGAQQBgjcCAQwxNDAy
# oBSAEgBNAGkAYwByAG8AcwBvAGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5j
# b20wDQYJKoZIhvcNAQEBBQAEggEAgYT4iAc/tWDSKcBNA8n8QfsAA/QmX2YItLro
# dSPFGnGO07gKqB6M+8qnNBLslUFObd0DuL17G7ZccmQAUpYCg6nJzPQq5csrzNu4
# OpkZ/n9+CHV2K6EoMMowogdDE4oMfl6LdXiUP7ajDUybVUTL47KdN0RVPutNEmJr
# c3z0hkky4R5BIG/9xZP78U6QE7j3d0ZKRiTP1oyvU0Xbv+x4oDEOgq5VK6yeGBU0
# aezqqqv+audiD8Q6kPzZ3pvLAlO/ihUZV3c+bvnfjEM4rBVAgbEHYnsQdOKNqPyf
# i9zg1zHqSOekPwJfGz4oFE/fCbwZwEHCt7lFaYdD7kcN0YQDcqGCFygwghckBgor
# BgEEAYI3AwMBMYIXFDCCFxAGCSqGSIb3DQEHAqCCFwEwghb9AgEDMQ8wDQYJYIZI
# AWUDBAIBBQAwggFZBgsqhkiG9w0BCRABBKCCAUgEggFEMIIBQAIBAQYKKwYBBAGE
# WQoDATAxMA0GCWCGSAFlAwQCAQUABCAFzypqVtJfDuu2f0ssvHeMPAQEEkBUiqgV
# Pz26adYj0gIGZdYWB/FRGBMyMDI0MDIyMjAxMjgyNi41MDNaMASAAgH0oIHYpIHV
# MIHSMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
# UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMS0wKwYDVQQL
# EyRNaWNyb3NvZnQgSXJlbGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxJjAkBgNVBAsT
# HVRoYWxlcyBUU1MgRVNOOjg2REYtNEJCQy05MzM1MSUwIwYDVQQDExxNaWNyb3Nv
# ZnQgVGltZS1TdGFtcCBTZXJ2aWNloIIRdzCCBycwggUPoAMCAQICEzMAAAHdXVcd
# ldStqhsAAQAAAd0wDQYJKoZIhvcNAQELBQAwfDELMAkGA1UEBhMCVVMxEzARBgNV
# BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jv
# c29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3RhbXAg
# UENBIDIwMTAwHhcNMjMxMDEyMTkwNzA5WhcNMjUwMTEwMTkwNzA5WjCB0jELMAkG
# A1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQx
# HjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEtMCsGA1UECxMkTWljcm9z
# b2Z0IElyZWxhbmQgT3BlcmF0aW9ucyBMaW1pdGVkMSYwJAYDVQQLEx1UaGFsZXMg
# VFNTIEVTTjo4NkRGLTRCQkMtOTMzNTElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUt
# U3RhbXAgU2VydmljZTCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAKhO
# A5RE6i53nHURH4lnfKLp+9JvipuTtctairCxMUSrPSy5CWK2DtriQP+T52HXbN2g
# 7AktQ1pQZbTDGFzK6d03vYYNrCPuJK+PRsP2FPVDjBXy5mrLRFzIHHLaiAaobE5v
# FJuoxZ0ZWdKMCs8acjhHUmfaY+79/CR7uN+B4+xjJqwvdpU/mp0mAq3earyH+AKm
# v6lkrQN8zgrcbCgHwsqvvqT6lEFqYpi7uKn7MAYbSeLe0pMdatV5EW6NVnXMYOTR
# KuGPfyfBKdShualLo88kG7qa2mbA5l77+X06JAesMkoyYr4/9CgDFjHUpcHSODuj
# lFBKMi168zRdLerdpW0bBX9EDux2zBMMaEK8NyxawCEuAq7++7ktFAbl3hUKtuzY
# C1FUZuUl2Bq6U17S4CKsqR3itLT9qNcb2pAJ4jrIDdll5Tgoqef5gpv+YcvBM834
# bXFNwytd3ujDD24P9Dd8xfVJvumjsBQQkK5T/qy3HrQJ8ud1nHSvtFVi5Sa/ubGu
# YEpS8gF6GDWN5/KbveFkdsoTVIPo8pkWhjPs0Q7nA5+uBxQB4zljEjKz5WW7BA4w
# pmFm24fhBmRjV4Nbp+n78cgAjvDSfTlA6DYBcv2kx1JH2dIhaRnSeOXePT6hMF0I
# l598LMu0rw35ViUWcAQkUNUTxRnqGFxz5w+ZusMDAgMBAAGjggFJMIIBRTAdBgNV
# HQ4EFgQUbqL1toyPUdpFyyHSDKWj0I4lw/EwHwYDVR0jBBgwFoAUn6cVXQBeYl2D
# 9OXSZacbUzUZ6XIwXwYDVR0fBFgwVjBUoFKgUIZOaHR0cDovL3d3dy5taWNyb3Nv
# ZnQuY29tL3BraW9wcy9jcmwvTWljcm9zb2Z0JTIwVGltZS1TdGFtcCUyMFBDQSUy
# MDIwMTAoMSkuY3JsMGwGCCsGAQUFBwEBBGAwXjBcBggrBgEFBQcwAoZQaHR0cDov
# L3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9jZXJ0cy9NaWNyb3NvZnQlMjBUaW1l
# LVN0YW1wJTIwUENBJTIwMjAxMCgxKS5jcnQwDAYDVR0TAQH/BAIwADAWBgNVHSUB
# Af8EDDAKBggrBgEFBQcDCDAOBgNVHQ8BAf8EBAMCB4AwDQYJKoZIhvcNAQELBQAD
# ggIBAC5U2bINLgXIHWbMcqVuf9jkUT/K8zyLBvu5h8JrqYR2z/eaO2yo1Ooc9Shy
# vxbe9GZDu7kkUzxSyJ1IZksZZw6FDq6yZNT3PEjAEnREpRBL8S+mbXg+O4VLS0LS
# mb8XIZiLsaqZ0fDEcv3HeA+/y/qKnCQWkXghpaEMwGMQzRkhGwcGdXr1zGpQ7HTx
# vfu57xFxZX1MkKnWFENJ6urd+4teUgXj0ngIOx//l3XMK3Ht8T2+zvGJNAF+5/5q
# Bk7nr079zICbFXvxtidNN5eoXdW+9rAIkS+UGD19AZdBrtt6dZ+OdAquBiDkYQ5k
# VfUMKS31yHQOGgmFxuCOzTpWHalrqpdIllsy8KNsj5U9sONiWAd9PNlyEHHbQZDm
# i9/BNlOYyTt0YehLbDovmZUNazk79Od/A917mqCdTqrExwBGUPbMP+/vdYUqaJsp
# upBnUtjOf/76DAhVy8e/e6zR98PkplmliO2brL3Q3rD6+ZCVdrGM9Rm6hUDBBkvY
# h+YjmGdcQ5HB6WT9Rec8+qDHmbhLhX4Zdaard5/OXeLbgx2f7L4QQQj3KgqjqDOW
# InVhNE1gYtTWLHe4882d/k7Lui0K1g8EZrKD7maOrsJLKPKlegceJ9FCqY1sDUKU
# hRa0EHUW+ZkKLlohKrS7FwjdrINWkPBgbQznCjdE2m47QjTbMIIHcTCCBVmgAwIB
# AgITMwAAABXF52ueAptJmQAAAAAAFTANBgkqhkiG9w0BAQsFADCBiDELMAkGA1UE
# BhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAc
# BgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEyMDAGA1UEAxMpTWljcm9zb2Z0
# IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IDIwMTAwHhcNMjEwOTMwMTgyMjI1
# WhcNMzAwOTMwMTgzMjI1WjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGlu
# Z3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBv
# cmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDCC
# AiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAOThpkzntHIhC3miy9ckeb0O
# 1YLT/e6cBwfSqWxOdcjKNVf2AX9sSuDivbk+F2Az/1xPx2b3lVNxWuJ+Slr+uDZn
# hUYjDLWNE893MsAQGOhgfWpSg0S3po5GawcU88V29YZQ3MFEyHFcUTE3oAo4bo3t
# 1w/YJlN8OWECesSq/XJprx2rrPY2vjUmZNqYO7oaezOtgFt+jBAcnVL+tuhiJdxq
# D89d9P6OU8/W7IVWTe/dvI2k45GPsjksUZzpcGkNyjYtcI4xyDUoveO0hyTD4MmP
# frVUj9z6BVWYbWg7mka97aSueik3rMvrg0XnRm7KMtXAhjBcTyziYrLNueKNiOSW
# rAFKu75xqRdbZ2De+JKRHh09/SDPc31BmkZ1zcRfNN0Sidb9pSB9fvzZnkXftnIv
# 231fgLrbqn427DZM9ituqBJR6L8FA6PRc6ZNN3SUHDSCD/AQ8rdHGO2n6Jl8P0zb
# r17C89XYcz1DTsEzOUyOArxCaC4Q6oRRRuLRvWoYWmEBc8pnol7XKHYC4jMYcten
# IPDC+hIK12NvDMk2ZItboKaDIV1fMHSRlJTYuVD5C4lh8zYGNRiER9vcG9H9stQc
# xWv2XFJRXRLbJbqvUAV6bMURHXLvjflSxIUXk8A8FdsaN8cIFRg/eKtFtvUeh17a
# j54WcmnGrnu3tz5q4i6tAgMBAAGjggHdMIIB2TASBgkrBgEEAYI3FQEEBQIDAQAB
# MCMGCSsGAQQBgjcVAgQWBBQqp1L+ZMSavoKRPEY1Kc8Q/y8E7jAdBgNVHQ4EFgQU
# n6cVXQBeYl2D9OXSZacbUzUZ6XIwXAYDVR0gBFUwUzBRBgwrBgEEAYI3TIN9AQEw
# QTA/BggrBgEFBQcCARYzaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9E
# b2NzL1JlcG9zaXRvcnkuaHRtMBMGA1UdJQQMMAoGCCsGAQUFBwMIMBkGCSsGAQQB
# gjcUAgQMHgoAUwB1AGIAQwBBMAsGA1UdDwQEAwIBhjAPBgNVHRMBAf8EBTADAQH/
# MB8GA1UdIwQYMBaAFNX2VsuP6KJcYmjRPZSQW9fOmhjEMFYGA1UdHwRPME0wS6BJ
# oEeGRWh0dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01p
# Y1Jvb0NlckF1dF8yMDEwLTA2LTIzLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYB
# BQUHMAKGPmh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljUm9v
# Q2VyQXV0XzIwMTAtMDYtMjMuY3J0MA0GCSqGSIb3DQEBCwUAA4ICAQCdVX38Kq3h
# LB9nATEkW+Geckv8qW/qXBS2Pk5HZHixBpOXPTEztTnXwnE2P9pkbHzQdTltuw8x
# 5MKP+2zRoZQYIu7pZmc6U03dmLq2HnjYNi6cqYJWAAOwBb6J6Gngugnue99qb74p
# y27YP0h1AdkY3m2CDPVtI1TkeFN1JFe53Z/zjj3G82jfZfakVqr3lbYoVSfQJL1A
# oL8ZthISEV09J+BAljis9/kpicO8F7BUhUKz/AyeixmJ5/ALaoHCgRlCGVJ1ijbC
# HcNhcy4sa3tuPywJeBTpkbKpW99Jo3QMvOyRgNI95ko+ZjtPu4b6MhrZlvSP9pEB
# 9s7GdP32THJvEKt1MMU0sHrYUP4KWN1APMdUbZ1jdEgssU5HLcEUBHG/ZPkkvnNt
# yo4JvbMBV0lUZNlz138eW0QBjloZkWsNn6Qo3GcZKCS6OEuabvshVGtqRRFHqfG3
# rsjoiV5PndLQTHa1V1QJsWkBRH58oWFsc/4Ku+xBZj1p/cvBQUl+fpO+y/g75LcV
# v7TOPqUxUYS8vwLBgqJ7Fx0ViY1w/ue10CgaiQuPNtq6TPmb/wrpNPgkNWcr4A24
# 5oyZ1uEi6vAnQj0llOZ0dFtq0Z4+7X6gMTN9vMvpe784cETRkPHIqzqKOghif9lw
# Y1NNje6CbaUFEMFxBmoQtB1VM1izoXBm8qGCAtMwggI8AgEBMIIBAKGB2KSB1TCB
# 0jELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1Jl
# ZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEtMCsGA1UECxMk
# TWljcm9zb2Z0IElyZWxhbmQgT3BlcmF0aW9ucyBMaW1pdGVkMSYwJAYDVQQLEx1U
# aGFsZXMgVFNTIEVTTjo4NkRGLTRCQkMtOTMzNTElMCMGA1UEAxMcTWljcm9zb2Z0
# IFRpbWUtU3RhbXAgU2VydmljZaIjCgEBMAcGBSsOAwIaAxUANiNHGWXbNaDPxnyi
# DbEOciSjFhCggYMwgYCkfjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGlu
# Z3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBv
# cmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDAN
# BgkqhkiG9w0BAQUFAAIFAOmAlH8wIhgPMjAyNDAyMjEyMzI1NTFaGA8yMDI0MDIy
# MjIzMjU1MVowczA5BgorBgEEAYRZCgQBMSswKTAKAgUA6YCUfwIBADAGAgEAAgEB
# MAcCAQACAhFjMAoCBQDpgeX/AgEAMDYGCisGAQQBhFkKBAIxKDAmMAwGCisGAQQB
# hFkKAwKgCjAIAgEAAgMHoSChCjAIAgEAAgMBhqAwDQYJKoZIhvcNAQEFBQADgYEA
# yZ9GxvWrzJKXDRm1ZgwaOk9nmiRrZVSUqoddg607TBt4liPUTIbJV6VvlrjZRvZx
# BWQscxv1Flwcah9i7QqdPFLBmNegg08quwyHxdrU3OKMiDt/RrpILRS1ptGjPEXh
# bsG9LdVvUMq0OrN3nVMJ5WUuTSGeERjSOuNZ95UC+9ExggQNMIIECQIBATCBkzB8
# MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVk
# bW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1N
# aWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMAITMwAAAd1dVx2V1K2qGwABAAAB
# 3TANBglghkgBZQMEAgEFAKCCAUowGgYJKoZIhvcNAQkDMQ0GCyqGSIb3DQEJEAEE
# MC8GCSqGSIb3DQEJBDEiBCAO198zN/nlQDDaKhBnp/FjL6bgu9Wq8aV6ZO9V6qKW
# dDCB+gYLKoZIhvcNAQkQAi8xgeowgecwgeQwgb0EIGH/Di2aZaxPeJmce0fRWTft
# QI3TaVHFj5GI43rAMWNmMIGYMIGApH4wfDELMAkGA1UEBhMCVVMxEzARBgNVBAgT
# Cldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29m
# dCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3RhbXAgUENB
# IDIwMTACEzMAAAHdXVcdldStqhsAAQAAAd0wIgQgw6z7F/zLMY4evDFZO4zpEjw/
# QM8nwyA84GQnsjwN5dAwDQYJKoZIhvcNAQELBQAEggIAdRpXxlQEBz6QeRTymf6R
# mGJglRYA1BQi8AC5OtJrhtiN3joddtMuANCsLbHajet0PBLS368pSKAdS7DJ+HXe
# uBjcTdB0c0q1Wo9AM/k/n2QDhk42d+Nww4boeilFWsNQqutEuj/lx3Q+5zjUvH6F
# toJQpVEJAxKgYUhZU46MFQA7oxoO1TkHluPHs1QoRPx8zzHuXR97NPoYy5mdPT65
# L8aFid+JIeJstFYzc6t+w6tkaLXOcyg/VraDCjmUd44jISS+ySNhWUPYXnNqwOah
# FifO52axBVY4cAniIg0TvxyVoRlzzvQARCxzaRIprFiTGCp7l/Ew95+y7t11DS08
# fcspGHd+HiZfwa0M8GaQvNzaez2Bj31O3ZHbjL/9+cc31LSgYU/YnhM1ftKWA+gf
# Z2rFQE4oyDa95lBqRNWgud7ATYQf+P1YKrbkIs32ov2Ffkz7LpVbzX/GcoXETsWB
# wxJIhnBi8qX11dlcZYc8bvikczRLMxdZstzDNKDRGL2NCyrvTu2a2xjXklS0OqGM
# oTm+yKTYdsriQFf9uAPXZb6LKLNaFSQ/fMThabLj4whgQPiqrlp0LeNoR9GCTj3B
# RK7WR+1m3mK4b5SGqsq41X0yxtVCkuQ1xLOrLFAorrK/ib/rAh5AN4FyRJcWzC22
# YXvcl+szDHCSQfiyY46Y4iM=
# SIG # End signature block
