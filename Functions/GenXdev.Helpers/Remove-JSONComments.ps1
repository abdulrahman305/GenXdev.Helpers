function Remove-JSONComments {

    [CmdletBinding()]

    param(

        [parameter(ValueFromPipeline, Position = 0, Mandatory)]
        [string[]] $Json
    )

    process {

        [GenXdev.Helpers.Serialization]::RemoveJSONComments($json)
    }
}
