<hr/>

<img src="powershell.jpg" alt="drawing" width="50%"/>

<hr/>

### NAME

    GenXdev.Helpers

### SYNOPSIS

    A Windows PowerShell module with helpers mostly used by other GenXdev modules
[![GenXdev.Helpers](https://img.shields.io/powershellgallery/v/GenXdev.Helpers.svg?style=flat-square&label=GenXdev.Helpers)](https://www.powershellgallery.com/packages/GenXdev.Helpers/)

### DEPENDENCIES
[![WinOS - Windows-10](https://img.shields.io/badge/WinOS-Windows--10--10.0.19041--SP0-brightgreen)](https://www.microsoft.com/en-us/windows/get-windows-10)

### INSTALLATION
````PowerShell
Install-Module "GenXdev.Helpers" -Force
Import-Module "GenXdev.Helpers"
````
### UPDATE
````PowerShell
Update-Module
````

<br/><hr/><hr/><hr/><hr/><br/>

# Cmdlets

### NAME
    ConvertTo-JsonEx

### SYNOPSIS
    Converts an object to a JSON-formatted string - at full depth

### SYNTAX
````PowerShell
    ConvertTo-JsonEx [-object] <Object> [-Compress] [<CommonParameters>]
````
### DESCRIPTION
    The `ConvertTo-JsonEx` cmdlet converts any .NET object to a string in
    JavaScript Object Notation (JSON) format - at full depth
### PARAMETERS
````
    -object <Object>
        Object to serialize

        Required?                    true
        Position?                    1
        Default value
        Accept pipeline input?       false
        Accept wildcard characters?  false

    -Compress [<SwitchParameter>]
        Omits white space and indented formatting in the output string.

        Required?                    false
        Position?                    named
        Default value                False
        Accept pipeline input?       false
        Accept wildcard characters?  false

    <CommonParameters>
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see
        about_CommonParameters

        (https://go.microsoft.com/fwlink/?LinkID=113216).
````
### NOTES
````PowerShell
    -------------------------- EXAMPLE 1 --------------------------

    The `ConvertTo-JsonEx` cmdlet is implemented using Newtonsoft Json.NET
    (https://www.newtonsoft.com/json).

    -------------------------- Example 1 --------------------------

       (Get-UICulture).Calendar | ConvertTo-JsonEx

       {
         "MinSupportedDateTime": "0001-01-01T00:00:00",
         "MaxSupportedDateTime": "9999-12-31T23:59:59.9999999",
         "AlgorithmType": 1,
         "CalendarType": 1,
         "Eras": [
           1
         ],
         "TwoDigitYearMax": 2029,
         "IsReadOnly": true
       }
````

<br/><hr/><hr/><hr/><hr/><br/>

### NAME
    Remove-JSONComments

### SYNOPSIS
    Removes any comment lines from a json file and return the result

### SYNTAX
````PowerShell
    Remove-JSONComments [-Json] <String[]> [<CommonParameters>]
````
### DESCRIPTION
    Removes any comment lines from a json file and return the result

### PARAMETERS
````l
    -Json <String[]>
        The json to filter for comments

        Required?                    true
        Position?                    1
        Default value
        Accept pipeline input?       true (ByValue)
        Accept wildcard characters?  false

    <CommonParameters>
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see
        about_CommonParameters

        (https://go.microsoft.com/fwlink/?LinkID=113216).
````

<br/><hr/><hr/><hr/><hr/><br/>
