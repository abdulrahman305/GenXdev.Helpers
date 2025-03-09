################################################################################
<#
.SYNOPSIS
Extracts geolocation data from an image file.

.DESCRIPTION
This function reads EXIF metadata from an image file to extract its latitude and
longitude coordinates. It supports images that contain GPS metadata in their EXIF
data.

.PARAMETER ImagePath
The full path to the image file to analyze.

.OUTPUTS
System.Collections.Hashtable
Returns a hashtable containing Latitude and Longitude if GPS data is found,
otherwise returns $null.

.EXAMPLE
Get-ImageGeolocation -ImagePath "C:\Photos\vacation.jpg"

.EXAMPLE
"C:\Photos\vacation.jpg" | Get-ImageGeolocation
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
            HelpMessage = "Path to the image file to analyze"
        )]
        [ValidateNotNullOrEmpty()]
        [string]$ImagePath
        ########################################################################
    )

    begin {
        # check if image path exists
        if (-not (Test-Path $ImagePath)) {
            Write-Error "The specified image path '$ImagePath' does not exist."
            return
        }

        Write-Verbose "Processing image: $ImagePath"
    }

    process {
        try {
            # load the image file
            Write-Verbose "Loading image file"
            $image = [System.Drawing.Image]::FromFile($ImagePath)

            # get all property items
            $propertyItems = $image.PropertyItems

            # extract gps metadata properties
            Write-Verbose "Extracting GPS metadata"
            $latitudeRef = $propertyItems |
            Where-Object { $PSItem.Id -eq 0x0001 }
            $latitude = $propertyItems |
            Where-Object { $PSItem.Id -eq 0x0002 }
            $longitudeRef = $propertyItems |
            Where-Object { $PSItem.Id -eq 0x0003 }
            $longitude = $propertyItems |
            Where-Object { $PSItem.Id -eq 0x0004 }

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

                Write-Verbose "GPS coordinates found: $lat, $lon"
                return @{
                    Latitude  = $lat
                    Longitude = $lon
                }
            }
            else {
                Write-Verbose "No GPS metadata found in image"
                return $null
            }
        }
        catch {
            Write-Error "Failed to process image: $_"
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
################################################################################
