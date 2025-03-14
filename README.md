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
| [alignScript](#alignScript) |  | Returns a string (with altered indentation) of a provided scriptblock string |
| [ConvertTo-HashTable](#ConvertTo-HashTable) |  | Converts a PSCustomObject to a HashTable recursively. |
| [ConvertTo-JsonEx](#ConvertTo-JsonEx) | tojsonex | Converts an object to a JSON string with extended options. |
| [Copy-IdenticalParamValues](#Copy-IdenticalParamValues) |  |  |
| [Get-DefaultWebLanguage](#Get-DefaultWebLanguage) |  | Gets the default web language key based on the system's current language settings. |
| [Get-GenXDevCmdlets](#Get-GenXDevCmdlets) | gcmds | Retrieves and lists all GenXdev cmdlets and their details. |
| [Get-ImageGeolocation](#Get-ImageGeolocation) |  | Extracts geolocation data from an image file. |
| [Get-WebLanguageDictionary](#Get-WebLanguageDictionary) |  | Returns a reversed dictionary for all languages supported by Google Search |
| [Import-GenXdevModules](#Import-GenXdevModules) | reloadgenxdev | Imports all GenXdev PowerShell modules into the global scope. |
| [Initialize-SearchPaths](#Initialize-SearchPaths) |  | Initializes and configures system search paths for package management. |
| [Invoke-OnEachGenXdevModule](#Invoke-OnEachGenXdevModule) | foreach-genxdev-module-do |  |
| [Out-Serial](#Out-Serial) |  | Sends a string to a serial port |
| [Remove-JSONComments](#Remove-JSONComments) |  | Removes comments from JSON content. |
| [Show-GenXDevCmdlets](#Show-GenXDevCmdlets) | cmds | Displays GenXdev PowerShell modules with their cmdlets and aliases. |
| [Show-Verb](#Show-Verb) | showverbs | Shows a short alphabetical list of all PowerShell verbs. |

<hr/>
&nbsp;

### GenXdev.Helpers.Math.Physics</hr>
| Command&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | aliases&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- | --- |
| [Get-FreeFallTime](#Get-FreeFallTime) |  |  |

<br/><hr/><hr/><br/>


# Cmdlets

&nbsp;<hr/>
###	GenXdev.Helpers<hr/> 

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
        Aliases                        
        Accept wildcard characters?  false  
    -spaces <Int32>  
        The minimum number of spaces for each line  
        Required?                    false  
        Position?                    2  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	ConvertTo-HashTable 
````PowerShell 

   ConvertTo-HashTable  
```` 

### SYNOPSIS 
    Converts a PSCustomObject to a HashTable recursively.  

### SYNTAX 
````PowerShell 

   ConvertTo-HashTable [-InputObject] <Object[]> [<CommonParameters>]  
```` 

### DESCRIPTION 
    This function converts a PSCustomObject and all its nested PSCustomObject  
    properties into HashTables. It handles arrays and other collection types by  
    processing each element recursively.  

### PARAMETERS 
    -InputObject <Object[]>  
        The PSCustomObject to convert into a HashTable. Accepts pipeline input.  
        Required?                    true  
        Position?                    1  
        Default value                  
        Accept pipeline input?       true (ByValue)  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	ConvertTo-JsonEx 
````PowerShell 

   ConvertTo-JsonEx                     --> tojsonex  
```` 

### SYNOPSIS 
    Converts an object to a JSON string with extended options.  

### SYNTAX 
````PowerShell 

   ConvertTo-JsonEx [-Object] <Object> [-Compress] [<CommonParameters>]  
```` 

### DESCRIPTION 
    Converts a PowerShell object to a JSON string using GenXdev.Helpers.Serialization  
    with optional compression.  

### PARAMETERS 
    -Object <Object>  
        The PowerShell object to convert to JSON.  
        Required?                    true  
        Position?                    1  
        Default value                  
        Accept pipeline input?       true (ByValue)  
        Aliases                        
        Accept wildcard characters?  false  
    -Compress [<SwitchParameter>]  
        If specified, removes whitespace from the output JSON string.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Copy-IdenticalParamValues 
````PowerShell 

   Copy-IdenticalParamValues  
```` 

### SYNOPSIS 
    Copies parameter values from bound parameters to a new hashtable based on  
    another function's possible parameters.  

### SYNTAX 
````PowerShell 

   Copy-IdenticalParamValues [-BoundParameters] <Object[]> [-FunctionName] <String> [[-DefaultValues] <PSVariable[]>] [<CommonParameters>]  
```` 

### DESCRIPTION 
    This function creates a new hashtable containing only the parameter values that  
    match the parameters defined in the specified target function.  
    This can then be used to invoke the function using splatting.  

### PARAMETERS 
    -BoundParameters <Object[]>  
        The bound parameters from which to copy values, typically $PSBoundParameters.  
        Required?                    true  
        Position?                    1  
        Default value                  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -FunctionName <String>  
        The name of the function whose parameter set will be used as a filter.  
        Required?                    true  
        Position?                    2  
        Default value                  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -DefaultValues <PSVariable[]>  
        Required?                    false  
        Position?                    3  
        Default value                @()  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Get-DefaultWebLanguage 
````PowerShell 

   Get-DefaultWebLanguage  
```` 

### SYNOPSIS 
    Gets the default web language key based on the system's current language settings.  

### SYNTAX 
````PowerShell 

   Get-DefaultWebLanguage [<CommonParameters>]  
```` 

### DESCRIPTION 
    Retrieves the current system language and culture settings and maps them to the  
    corresponding web language dictionary key used by translation services.  

### PARAMETERS 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Get-GenXDevCmdlets 
````PowerShell 

   Get-GenXDevCmdlets                   --> gcmds  
```` 

### SYNOPSIS 
    Retrieves and lists all GenXdev cmdlets and their details.  

### SYNTAX 
````PowerShell 

   Get-GenXDevCmdlets [[-CmdletName] <String>] [[-BaseModuleName] <String[]>] [-NoLocal] [-OnlyPublished] [-FromScripts] [<CommonParameters>]  
```` 

### DESCRIPTION 
    Searches through installed GenXdev modules and script files to find cmdlets,  
    their aliases, and descriptions. Can filter by name pattern and module name.  

### PARAMETERS 
    -CmdletName <String>  
        Required?                    false  
        Position?                    1  
        Default value                *  
        Accept pipeline input?       true (ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  true  
    -BaseModuleName <String[]>  
        One or more GenXdev module names to search. Can omit GenXdev prefix.  
        Required?                    false  
        Position?                    2  
        Default value                @("GenXdev*")  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  true  
    -NoLocal [<SwitchParameter>]  
        Skip searching in local module paths.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -OnlyPublished [<SwitchParameter>]  
        Limit search to published module paths only.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -FromScripts [<SwitchParameter>]  
        Search in script files instead of module files.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
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

### SYNOPSIS 
    Extracts geolocation data from an image file.  

### SYNTAX 
````PowerShell 

   Get-ImageGeolocation [-ImagePath] <String> [<CommonParameters>]  
```` 

### DESCRIPTION 
    This function reads EXIF metadata from an image file to extract its latitude and  
    longitude coordinates. It supports images that contain GPS metadata in their EXIF  
    data.  

### PARAMETERS 
    -ImagePath <String>  
        The full path to the image file to analyze.  
        Required?                    true  
        Position?                    1  
        Default value                  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Get-WebLanguageDictionary 
````PowerShell 

   Get-WebLanguageDictionary  
```` 

### SYNOPSIS 
    Returns a reversed dictionary for all languages supported by Google Search  

### SYNTAX 
````PowerShell 

   Get-WebLanguageDictionary [<CommonParameters>]  
```` 

### DESCRIPTION 
    Returns a reversed dictionary for all languages supported by Google Search  

### PARAMETERS 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Import-GenXdevModules 
````PowerShell 

   Import-GenXdevModules                --> reloadgenxdev  
```` 

### SYNOPSIS 
    Imports all GenXdev PowerShell modules into the global scope.  

### SYNTAX 
````PowerShell 

   Import-GenXdevModules [-DebugFailedModuleDefinitions] [<CommonParameters>]  
```` 

### DESCRIPTION 
    Scans the parent directory for GenXdev modules and imports each one into the  
    global scope. Uses location stack management to preserve the working directory  
    and provides visual feedback for successful and failed imports. Tracks function  
    count changes during the import process.  

### PARAMETERS 
    -DebugFailedModuleDefinitions [<SwitchParameter>]  
        When enabled, provides detailed debug output for modules that fail to import.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Initialize-SearchPaths 
````PowerShell 

   Initialize-SearchPaths  
```` 

### SYNOPSIS 
    Initializes and configures system search paths for package management.  

### SYNTAX 
````PowerShell 

   Initialize-SearchPaths [[-WorkspaceFolder] <String>] [<CommonParameters>]  
```` 

### DESCRIPTION 
    This function builds a comprehensive list of search paths by combining default  
    system locations, chocolatey paths, development tool paths, and custom package  
    paths. It then updates the system's PATH environment variable with these  
    consolidated paths.  

### PARAMETERS 
    -WorkspaceFolder <String>  
        The workspace folder path to use for node modules and PowerShell paths.  
        Required?                    false  
        Position?                    1  
        Default value                "$PSScriptRoot\..\..\..\..\..\"  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Invoke-OnEachGenXdevModule 
````PowerShell 

   Invoke-OnEachGenXdevModule           --> foreach-genxdev-module-do  
```` 

### SYNTAX 
````PowerShell 

   Invoke-OnEachGenXdevModule [-Script] <scriptblock> [[-BaseModuleName] <string[]>] [-NoLocal] [-OnlyPublished] [-FromScripts]   
   [<CommonParameters>]  
```` 

### PARAMETERS 
    -BaseModuleName <string[]>  
        Filter to apply to module names  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Module, ModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
    -FromScripts  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -NoLocal  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -OnlyPublished  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -Script <scriptblock>  
        The script block to execute for each GenXdev module  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      ScriptBlock  
        Dynamic?                     false  
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

   Out-Serial [[-Portname] <String>] [[-BaudRate] <Int32>] [[-MaxBytesToRead] <UInt32>] [[-ReadTimeout] <UInt32>] [[-WriteTimeout] <UInt32>]   
   [[-Parity] <String>] [[-DataBits] <Int32>] [[-StopBits] <String>] [-Text] <Object> [-AddCRLinefeeds] [<CommonParameters>]  
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
        Aliases                        
        Accept wildcard characters?  false  
    -BaudRate <Int32>  
        The baud rate.  
        Required?                    false  
        Position?                    2  
        Default value                9600  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -MaxBytesToRead <UInt32>  
        Limits the nr of bytes to read.  
        Required?                    false  
        Position?                    3  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -ReadTimeout <UInt32>  
        Enables reading with a specified timeout in milliseconds.  
        Required?                    false  
        Position?                    4  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -WriteTimeout <UInt32>  
        Enables writing with a specified timeout in milliseconds.  
        Required?                    false  
        Position?                    5  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -Parity <String>  
        One of the System.IO.Ports.SerialPort.Parity values.  
        Required?                    false  
        Position?                    6  
        Default value                None  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -DataBits <Int32>  
        The data bits value.  
        Required?                    false  
        Position?                    7  
        Default value                8  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -StopBits <String>  
        One of the System.IO.Ports.SerialPort.StopBits values.  
        Required?                    false  
        Position?                    8  
        Default value                One  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -Text <Object>  
        Text to sent to serial port.  
        Required?                    true  
        Position?                    9  
        Default value                  
        Accept pipeline input?       true (ByValue)  
        Aliases                        
        Accept wildcard characters?  false  
    -AddCRLinefeeds [<SwitchParameter>]  
        Add linefeeds to input text parts.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
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
    Removes comments from JSON content.  

### SYNTAX 
````PowerShell 

   Remove-JSONComments [-Json] <String[]> [<CommonParameters>]  
```` 

### DESCRIPTION 
    Processes JSON content and removes both single-line and multi-line comments while  
    preserving the JSON structure. This is useful for cleaning up JSON files that  
    contain documentation comments before parsing.  

### PARAMETERS 
    -Json <String[]>  
        The JSON content to process as a string array. Each element represents a line of  
        JSON content.  
        Required?                    true  
        Position?                    1  
        Default value                  
        Accept pipeline input?       true (ByValue)  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Show-GenXDevCmdlets 
````PowerShell 

   Show-GenXDevCmdlets                  --> cmds  
```` 

### SYNOPSIS 
    Displays GenXdev PowerShell modules with their cmdlets and aliases.  

### SYNTAX 
````PowerShell 

   Show-GenXDevCmdlets [[-CmdletName] <String>] [[-BaseModuleName] <String[]>] [-NoLocal] [-OnlyPublished] [-FromScripts] [-Online]   
   [-OnlyAliases] [-ShowTable] [<CommonParameters>]  
```` 

### DESCRIPTION 
    Lists all installed GenXdev PowerShell modules and their associated cmdlets and  
    aliases. Uses Get-GenXDevCmdlets to retrieve cmdlet information and optionally  
    their script positions. Provides filtering and various display options.  

### PARAMETERS 
    -CmdletName <String>  
        Required?                    false  
        Position?                    1  
        Default value                *  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  true  
    -BaseModuleName <String[]>  
        Required?                    false  
        Position?                    2  
        Default value                @("GenXdev*")  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  true  
    -NoLocal [<SwitchParameter>]  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -OnlyPublished [<SwitchParameter>]  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -FromScripts [<SwitchParameter>]  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -Online [<SwitchParameter>]  
        When specified, opens the GitHub documentation page instead of console output.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -OnlyAliases [<SwitchParameter>]  
        When specified displays only aliases of cmdlets who have them.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -ShowTable [<SwitchParameter>]  
        When specified, displays results in a table format with Name and Description.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Show-Verb 
````PowerShell 

   Show-Verb                            --> showverbs  
```` 

### SYNOPSIS 
    Shows a short alphabetical list of all PowerShell verbs.  

### SYNTAX 
````PowerShell 

   Show-Verb [[-Verb] <String[]>] [<CommonParameters>]  
```` 

### DESCRIPTION 
    Displays PowerShell approved verbs in a comma-separated list. If specific verbs  
    are provided as input, only matching verbs will be shown. Supports wildcards.  

### PARAMETERS 
    -Verb <String[]>  
        One or more verb patterns to filter the output. Supports wildcards.  
        If omitted, all approved verbs are shown.  
        Required?                    false  
        Position?                    1  
        Default value                @()  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  true  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers.Math.Physics<hr/> 

##	Get-FreeFallTime 
````PowerShell 

   Get-FreeFallTime  
```` 

### SYNTAX 
````PowerShell 

   Get-FreeFallTime [[-HeightInMeters] <double>] [[-TerminalVelocityInMs] <double>]   
```` 

### PARAMETERS 
    -HeightInMeters <double>  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -TerminalVelocityInMs <double>  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  

<br/><hr/><hr/><br/>
