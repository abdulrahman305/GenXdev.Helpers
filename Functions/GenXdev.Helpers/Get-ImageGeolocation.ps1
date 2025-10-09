<##############################################################################
Part of PowerShell module : GenXdev.Helpers
Original cmdlet filename  : Get-ImageGeolocation.ps1
Original author           : René Vaessen / GenXdev
Version                   : 1.300.2025
################################################################################
Copyright (c)  René Vaessen / GenXdev

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
################################################################################>
###############################################################################
<#
.SYNOPSIS
Extracts geolocation data from an image file.

.DESCRIPTION
This function reads EXIF metadata from an image file to extract its latitude and
longitude coordinates. It supports images that contain GPS metadata in their EXIF
data. The function uses the System.Drawing.Image class to load the image and
parse the GPS coordinates from property items.

.PARAMETER ImagePath
The full path to the image file to analyze. The file must be a valid image format
that supports EXIF metadata (JPEG, TIFF, etc.).

.OUTPUTS
System.Collections.Hashtable
Returns a hashtable containing Latitude and Longitude if GPS data is found,
otherwise returns $null.

.EXAMPLE
Get-ImageGeolocation -ImagePath "C:\Pictures\vacation.jpg"

.EXAMPLE
"C:\Pictures\vacation.jpg" | Get-ImageGeolocation
#>
function Get-ImageGeolocation {

    [CmdletBinding()]
    [OutputType([System.Collections.Hashtable])]

    param (
        ########################################################################
        [Parameter(
            Mandatory = $true,
            Position = 0,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true,
            HelpMessage = 'Path to the image file to analyze'
        )]
        [ValidateNotNullOrEmpty()]
        [string]$ImagePath
        ########################################################################
    )

    begin {

        # check if image path exists
        if (-not (Microsoft.PowerShell.Management\Test-Path -LiteralPath $ImagePath)) {

            Microsoft.PowerShell.Utility\Write-Error (
                "The specified image path '$ImagePath' does not exist."
            )

            return
        }

        Microsoft.PowerShell.Utility\Write-Verbose (
            "Processing image: $ImagePath"
        )
    }

    process {

        try {

            # load the image file
            Microsoft.PowerShell.Utility\Write-Verbose 'Loading image file'

            $image = [System.Drawing.Image]::FromFile($ImagePath)

            # get all property items
            $propertyItems = $image.PropertyItems

            # extract gps metadata properties
            Microsoft.PowerShell.Utility\Write-Verbose 'Extracting GPS metadata'

            $latitudeRef = $propertyItems |
                Microsoft.PowerShell.Core\Where-Object { $PSItem.Id -eq 0x0001 }

            $latitude = $propertyItems |
                Microsoft.PowerShell.Core\Where-Object { $PSItem.Id -eq 0x0002 }

            $longitudeRef = $propertyItems |
                Microsoft.PowerShell.Core\Where-Object { $PSItem.Id -eq 0x0003 }

            $longitude = $propertyItems |
                Microsoft.PowerShell.Core\Where-Object { $PSItem.Id -eq 0x0004 }

            # check if gps data exists
            if ($latitude -and $longitude -and $latitudeRef -and $longitudeRef) {

                # calculate actual latitude and longitude values
                $lat = [BitConverter]::ToUInt32($latitude.Value, 0) /
                [BitConverter]::ToUInt32($latitude.Value, 4)

                $lon = [BitConverter]::ToUInt32($longitude.Value, 0) /
                [BitConverter]::ToUInt32($longitude.Value, 4)

                # adjust for south and west hemispheres
                if ($latitudeRef.Value -eq [byte][char]'S') {

                    $lat = - $lat
                }

                if ($longitudeRef.Value -eq [byte][char]'W') {

                    $lon = - $lon
                }

                Microsoft.PowerShell.Utility\Write-Verbose (
                    "GPS coordinates found: $lat, $lon"
                )

                return @{
                    Latitude  = $lat
                    Longitude = $lon
                }
            }
            else {

                Microsoft.PowerShell.Utility\Write-Verbose (
                    'No GPS metadata found in image'
                )

                return $null
            }
        }
        catch {

            Microsoft.PowerShell.Utility\Write-Error (
                "Failed to process image: $_"
            )
        }
        finally {

            if ($image) {

                $image.Dispose()
            }
        }
    }

    end {
    }
}