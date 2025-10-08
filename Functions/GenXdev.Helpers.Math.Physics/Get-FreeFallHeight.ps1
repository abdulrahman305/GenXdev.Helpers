<##############################################################################
Part of PowerShell module : GenXdev.Helpers.Math.Physics
Original cmdlet filename  : Get-FreeFallHeight.ps1
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
################################################################################
<#
.SYNOPSIS
Calculates the height fallen during free fall for a given time duration.

.DESCRIPTION
This function calculates the distance fallen during free fall using a
numerical method that accounts for air resistance and terminal velocity. The
calculation uses small time steps to accurately model the physics of falling
objects with realistic terminal velocity constraints.

.PARAMETER DurationInSeconds
The time duration of the fall in seconds for which to calculate the height.

.PARAMETER TerminalVelocityInMs
The terminal velocity in meters per second. Defaults to 53 m/s which is the
typical terminal velocity for a human in free fall.

.EXAMPLE
Get-FreeFallHeight -DurationInSeconds 10 -TerminalVelocityInMs 53

Calculates the height fallen in 10 seconds with default human terminal velocity.

.EXAMPLE
Get-FreeFallHeight 5

Calculates the height fallen in 5 seconds using positional parameter and
default terminal velocity.
#>
function Get-FreeFallHeight {

    [CmdletBinding()]
    [OutputType([double])]

    param(
        ################################################################################
        [parameter(
            Mandatory = $true,
            Position = 0,
            HelpMessage = "The time duration of the fall in seconds"
        )]
        [double] $DurationInSeconds,
        ################################################################################
        [parameter(
            Mandatory = $false,
            Position = 1,
            HelpMessage = ("The terminal velocity in meters per second " +
                          "(default: 53 m/s for human)")
        )]
        [double] $TerminalVelocityInMs = 53
        ################################################################################
    )

    begin {

        # define the acceleration due to gravity in meters per second squared
        $gravity = 9.81

        # set up numerical integration parameters for accurate calculation
        $dt = 0.01

        # initialize time tracking variable
        $time = 0

        # initialize height accumulator
        $height = 0

        # initialize velocity tracker
        $velocity = 0

        Microsoft.PowerShell.Utility\Write-Verbose (
            "Starting free fall calculation for ${DurationInSeconds} seconds " +
            "with terminal velocity ${TerminalVelocityInMs} m/s"
        )
    }

    process {

        # perform numerical integration using small time steps
        while ($time -lt $DurationInSeconds) {

            # apply air resistance model by capping velocity at terminal velocity
            if ($velocity -ge $TerminalVelocityInMs) {

                # maintain constant terminal velocity when reached
                $velocity = $TerminalVelocityInMs
            }
            else {

                # accelerate under gravity when below terminal velocity
                $velocity += $gravity * $dt
            }

            # calculate distance traveled in this time step
            $height += $velocity * $dt

            # advance time by one step
            $time += $dt

            # prevent infinite loops with safety timeout
            if ($time -gt 1000) {

                Microsoft.PowerShell.Utility\Write-Error (
                    'Calculation timeout exceeded 1000 seconds'
                )

                return [double]0
            }
        }

        Microsoft.PowerShell.Utility\Write-Verbose (
            "Calculated fall height: ${height} meters"
        )
    }

    end {

        # return the calculated height rounded to 2 decimal places as double
        return [double][Math]::Round($height, 2)
    }
}
################################################################################