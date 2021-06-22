<hr/>

![](https://genxdev.net/Powershell.jpg)

<hr/>

## NAME

    GenXdev.Helpers

## SYNOPSIS

    A collection of .Net libraries and cmdlets, mostly used by other GenXdev modules

## TYPE
    PowerShell Module

## INSTALLATION
````Powershell

    Install-Module "GenXdev.Helpers" -Force
    Import-Module "GenXdev.Helpers"

````

<br/><hr/><hr/><hr/><hr/><br/>

# Cmdlets

### NAME
    ConvertTo-JsonEx

### SYNOPSIS
    Converts an object to a JSON-formatted string - at full depth

### SYNTAX
````Powershell
    ConvertTo-JsonEx [-object] <Object> [-Compress] [<CommonParameters>]
````
### DESCRIPTION
    The `ConvertTo-JsonEx` cmdlet converts any .NET object to a string in
    JavaScript Object Notation (JSON) format - at full depth
### PARAMETERS\r
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
````Powershell
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
````Powershell
    Remove-JSONComments [-Json] <String[]> [<CommonParameters>]
````
### DESCRIPTION
    Removes any comment lines from a json file and return the result

### PARAMETERS\r
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
