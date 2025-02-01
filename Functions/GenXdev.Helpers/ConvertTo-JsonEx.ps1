function ConvertTo-JsonEx {

    [CmdletBinding()]

    param(

        [parameter(Position = 0, Mandatory)]
        [object] $object,

        [parameter(Mandatory = $false)]
        [switch] $Compress
    )

    process {

        [GenXdev.Helpers.Serialization]::ToJson($object, ($Compress -eq $true));
    }
}
