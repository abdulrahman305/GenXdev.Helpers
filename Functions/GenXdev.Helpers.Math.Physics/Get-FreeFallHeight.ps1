Function Get-FreeFallHeight {
    param (
        [double]$DurationInSeconds,
        [double]$TerminalVelocityInMs = 53  # Default human terminal velocity in m/s
    )

    # Define the acceleration due to gravity in m/s^2
    $gravity = 9.81

    # Calculate height using numerical method with small time steps
    $dt = 0.01  # Time step in seconds
    $time = 0
    $height = 0
    $velocity = 0

    while ($time -lt $DurationInSeconds) {
        # Apply simplified air resistance model
        if ($velocity -ge $TerminalVelocityInMs) {
            $velocity = $TerminalVelocityInMs
        }
        else {
            $velocity += $gravity * $dt
        }

        $height += $velocity * $dt
        $time += $dt

        # Add safety check
        if ($time -gt 1000) {
            Microsoft.PowerShell.Utility\Write-Error 'Calculation timeout'
            return 0
        }
    }

    return [Math]::Round($height, 2)
}

