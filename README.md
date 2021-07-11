<hr/>

<img src="powershell.jpg" alt="GenXdev" width="50%"/>

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

<br/><hr/><hr/><br/>

# Cmdlet Index
| Command&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | aliases&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- | --- |
| [alignScript](#alignScript) |  | Changes the indentation of a scriptblock string while respecting the original code-block identations |
| [ConvertTo-JsonEx](#ConvertTo-JsonEx) |  | The `ConvertTo-JsonEx` cmdlet converts any .NET object to a string in JavaScript Object Notation (JSON) format - at full depth |
| [Copy-CommandParameters](#Copy-CommandParameters) |  | The dynamic parameter block of a proxy function. This block can be used to copy a proxy function target's parameters, regardless of changes from version to version. |
| [Remove-JSONComments](#Remove-JSONComments) |  | Removes any comment lines from a json file and return the result |

<br/><hr/><hr/><br/>
# Cmdlets
## alignScript
````PowerShell
alignScript
````
### SYNOPSIS
    Returns a string (with altered indentation) of a provided scriptblock string
### SYNTAX
````PowerShell
alignScript [[-script] <String>] [[-spaces] <Int32>] [<CommonParameters>]
````
### DESCRIPTION
    Changes the indentation of a scriptblock string while respecting the original code-block identations
### PARAMETERS
    -script <String>
        The scriptblock string
        Required?                    false
        Position?                    1
        Default value                
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -spaces <Int32>
        The minimum number of spaces for each line
        Required?                    false
        Position?                    2
        Default value                0
        Accept pipeline input?       false
        Accept wildcard characters?  false
    <CommonParameters>
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216). 
<br/><hr/><hr/><br/>
## ConvertTo-JsonEx
````PowerShell
ConvertTo-JsonEx
````
### SYNOPSIS
    Converts an object to a JSON-formatted string - at full depth
### SYNTAX
````PowerShell
ConvertTo-JsonEx [-object] <Object> [-Compress] [<CommonParameters>]
````
### DESCRIPTION
    The `ConvertTo-JsonEx` cmdlet converts any .NET object to a string in JavaScript Object Notation (JSON) format - at full depth
### PARAMETERS
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
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216). 
<br/><hr/><hr/><br/>
## Copy-CommandParameters
````PowerShell
Copy-CommandParameters
````
### SYNOPSIS
    Proxy function dynamic parameter block for the Set-WindowPosition cmdlet
### SYNTAX
````PowerShell
Copy-CommandParameters [-CommandName] <String> [[-ParametersToSkip] <String[]>] [<CommonParameters>]
````
### DESCRIPTION
    The dynamic parameter block of a proxy function. This block can be used to copy a proxy function target's parameters, regardless of changes from version to version.
### PARAMETERS
    -CommandName <String>
        Required?                    true
        Position?                    1
        Default value                
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -ParametersToSkip <String[]>
        Required?                    false
        Position?                    2
        Default value                @()
        Accept pipeline input?       false
        Accept wildcard characters?  false
    <CommonParameters>
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216). 
<br/><hr/><hr/><br/>
## Remove-JSONComments
````PowerShell
Remove-JSONComments
````
### SYNOPSIS
    Removes any comment lines from a json file and return the result
### SYNTAX
````PowerShell
Remove-JSONComments [-Json] <String[]> [<CommonParameters>]
````
### DESCRIPTION
    Removes any comment lines from a json file and return the result
### PARAMETERS
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
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216). 
<br/><hr/><hr/><br/>