## Todoos

<hr/>

<img src="powershell.jpg" alt="GenXdev" width="50%"/>

<hr/>

### NAME

    GenXdev.Helpers

### SYNOPSIS

    A Windows PowerShell module with helpers mostly used by other GenXdev modules
[![GenXdev.Helpers](https://img.shields.io/powershellgallery/v/GenXdev.Helpers.svg?style=flat-square&label=GenXdev.Helpers)](https://www.powershellgallery.com/packages/GenXdev.Helpers/)

### DEPENDENCIES
[![WinOS - Windows-10 or later](https://img.shields.io/badge/WinOS-Windows--10--10.0.19041--SP0-brightgreen)](https://www.microsoft.com/en-us/windows/get-windows-10)

### INSTALLATION
````PowerShell
Install-Module "GenXdev.Helpers"
Import-Module "GenXdev.Helpers"
````
### UPDATE
````PowerShell
Update-Module
````

<br/><hr/><hr/><br/>

# Cmdlet Index
### GenXdev.Helpers<hr/>
| Command&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | aliases&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- | --- |
| [ConvertTo-JsonEx](#ConvertTo-JsonEx) |  | The `ConvertTo-JsonEx` cmdlet converts any .NET object to a string in JavaScript Object Notation (JSON) format - at full depth |
| [Remove-JSONComments](#Remove-JSONComments) |  | Removes any comment lines from a json file and return the result |
| [Copy-CommandParameters](#Copy-CommandParameters) |  | The dynamic parameter block of a proxy function. This block can be used to copy a proxy function target's parameters . |
| [alignScript](#alignScript) |  | Changes the indentation of a scriptblock string while respecting the original code-block identations |
| [Out-Serial](#Out-Serial) |  | Allows you to send a string to a serial communication port |
| [Get-ImageGeolocation](#Get-ImageGeolocation) |  |  |
| [AssurePester](#AssurePester) |  |  |

<br/><hr/><hr/><br/>


# Cmdlets

&nbsp;<hr/>
###	GenXdev.Helpers<hr/>

##	ConvertTo-JsonEx
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

##	Remove-JSONComments
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

##	Copy-CommandParameters
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
    The dynamic parameter block of a proxy function. This block can be used to copy a proxy function target's parameters .

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

##	alignScript
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

##	Out-Serial
````PowerShell
Out-Serial
````

### SYNOPSIS
    Sends a string to a serial port

### SYNTAX
````PowerShell
Out-Serial [[-Portname] <String>] [[-BaudRate] <Int32>] [[-MaxBytesToRead] <UInt32>] [[-ReadTimeout] <UInt32>] [[-WriteTimeout] <UInt32>] [[-Parity] <String>] [[-DataBits] <Int32>] [[-StopBits] <String>] [-Text] <Object> 
[-AddCRLinefeeds] [<CommonParameters>]
````

### DESCRIPTION
    Allows you to send a string to a serial communication port

### PARAMETERS
    -Portname <String>
        The port to use (for example, COM1).
        Required?                    false
        Position?                    1
        Default value                COM5
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -BaudRate <Int32>
        The baud rate.
        Required?                    false
        Position?                    2
        Default value                9600
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -MaxBytesToRead <UInt32>
        Limits the nr of bytes to read.
        Required?                    false
        Position?                    3
        Default value                0
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -ReadTimeout <UInt32>
        Enables reading with a specified timeout in milliseconds.
        Required?                    false
        Position?                    4
        Default value                0
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -WriteTimeout <UInt32>
        Enables writing with a specified timeout in milliseconds.
        Required?                    false
        Position?                    5
        Default value                0
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -Parity <String>
        One of the System.IO.Ports.SerialPort.Parity values.
        Required?                    false
        Position?                    6
        Default value                None
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -DataBits <Int32>
        The data bits value.
        Required?                    false
        Position?                    7
        Default value                8
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -StopBits <String>
        One of the System.IO.Ports.SerialPort.StopBits values.
        Required?                    false
        Position?                    8
        Default value                One
        Accept pipeline input?       false
        Accept wildcard characters?  false
    -Text <Object>
        Text to sent to serial port.
        Required?                    true
        Position?                    9
        Default value                
        Accept pipeline input?       true (ByValue)
        Accept wildcard characters?  false
    -AddCRLinefeeds [<SwitchParameter>]
        Add linefeeds to input text parts.
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

##	Get-ImageGeolocation
````PowerShell
Get-ImageGeolocation
````

### SYNTAX
````PowerShell
Get-ImageGeolocation [-ImagePath] <string> [<CommonParameters>]
````

### PARAMETERS
    -ImagePath <string>
        Required?                    true
        Position?                    0
        Accept pipeline input?       false
        Parameter set name           (All)
        Aliases                      None
        Dynamic?                     false
        Accept wildcard characters?  false
    <CommonParameters>
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216). 

<br/><hr/><hr/><br/>

##	AssurePester
````PowerShell
AssurePester
````

### SYNTAX
````PowerShell
AssurePester 
````

### PARAMETERS
    None

<br/><hr/><hr/><br/>
