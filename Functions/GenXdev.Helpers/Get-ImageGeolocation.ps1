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
