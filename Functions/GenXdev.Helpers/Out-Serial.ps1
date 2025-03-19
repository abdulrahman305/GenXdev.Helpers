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