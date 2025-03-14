Function Get-FreeFallTime {
    param (
        [double]$HeightInMeters,
        [double]$TerminalVelocityInMs = 53  # Default human terminal velocity in m/s
    )

    # Define the acceleration due to gravity in m/s^2
    $gravity = 9.81

    # Calculate time using numerical method with small time steps
    $dt = 0.01  # Time step in seconds
    $height = $HeightInMeters
    $velocity = 0
    $time = 0

    while ($height -gt 0) {
        # Apply simplified air resistance model
        if ($velocity -ge $TerminalVelocityInMs) {
            $velocity = $TerminalVelocityInMs
        }
        else {
            $velocity += $gravity * $dt
        }

        $height -= $velocity * $dt
        $time += $dt

        # Add safety check
        if ($time -gt 1000) {
            Write-Error "Calculation timeout"
            return 0
        }
    }

    return [Math]::Round($time, 2)
}