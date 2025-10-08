<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Out-Serial.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.298.2025
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
            HelpMessage = 'The port to use (for example, COM1).')]
        [string]$Portname = 'COM5',

        [Parameter(Mandatory = $false,
            HelpMessage = 'The baud rate.')]
        [int]$BaudRate = 9600,

        [Parameter(Mandatory = $false,
            HelpMessage = 'Limits the nr of bytes to read.')]
        [uint]$MaxBytesToRead = 0,

        [Parameter(Mandatory = $false,
            HelpMessage = 'Enables reading with a specified timeout in milliseconds.')]
        [uint]$ReadTimeout,

        [Parameter(Mandatory = $false,
            HelpMessage = 'Enables writing with a specified timeout in milliseconds.')]
        [uint]$WriteTimeout,

        [Parameter(Mandatory = $false,
            HelpMessage = 'One of the System.IO.Ports.SerialPort.Parity values.')]
        [string]$Parity = 'None',

        [Parameter(Mandatory = $false,
            HelpMessage = 'The data bits value.')]
        [int]$DataBits = 8,

        [Parameter(Mandatory = $false,
            HelpMessage = 'One of the System.IO.Ports.SerialPort.StopBits values.')]
        [string]$StopBits = 'One',

        [Parameter(Mandatory, ValueFromPipeline,
            HelpMessage = 'Text to sent to serial port.')]
        [object]$Text,

        [Parameter(Mandatory = $false,
            HelpMessage = 'Add linefeeds to input text parts.')]
        [switch]$AddCRLinefeeds = $false
    )
    begin {
        $serialPort = Microsoft.PowerShell.Utility\New-Object System.IO.Ports.SerialPort -ArgumentList $Portname, $BaudRate, $Parity, $DataBits, $StopBits
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
                Microsoft.PowerShell.Utility\Start-Sleep -Milliseconds $serialPort.ReadTimeout
                $bytes = Microsoft.PowerShell.Utility\New-Object byte[] ($MaxBytesToRead -gt 0 ? $MaxBytesToRead : 1024)
                $readTotal = 0
                while ($readTotal -lt $bytes.Length) {
                    try {
                        $read = $serialPort.Read($bytes, $readTotal, $bytes.Length - $readTotal)
                        if ($read -eq 0) { break }
                        $readTotal += $read
                    }
                    catch {
                        if ($readTotal -gt 0) { Microsoft.PowerShell.Utility\Write-Output ([System.Text.Encoding]::ASCII.GetString($bytes, 0, $readTotal)) }
                        break
                    }
                }
                if ($readTotal -gt 0) { Microsoft.PowerShell.Utility\Write-Output ([System.Text.Encoding]::ASCII.GetString($bytes, 0, $readTotal)) }
            }
        }
        catch {
            Microsoft.PowerShell.Utility\Write-Verbose "Error occured: $PSItem"
        }
    }
    end {
        $serialPort.Close()
    }
}