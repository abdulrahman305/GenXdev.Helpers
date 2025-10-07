<hr/>

<img src="powershell.jpg" alt="GenXdev" width="50%"/>

<hr/>

### NAME

    GenXdev.Helpers

### SYNOPSIS

    A Windows PowerShell module with helpers mostly used by other GenXdev modules
[![GenXdev.Helpers](https://img.shields.io/powershellgallery/v/GenXdev.Helpers.svg?style=flat-square&label=GenXdev.Helpers)](https://www.powershellgallery.com/packages/GenXdev.Helpers/)

## MIT License

````text
MIT License

Copyright (c) 2025 GenXdev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
````

### DEPENDENCIES
[![WinOS - Windows-10 or later](https://img.shields.io/badge/WinOS-Windows--10--10.0.19041--SP0-brightgreen)](https://www.microsoft.com/en-us/windows/get-windows-10)

### INSTALLATION
```PowerShell
Install-Module "GenXdev.Helpers"
Import-Module "GenXdev.Helpers"
```
### UPDATE
```PowerShell
Update-Module
```

<br/><hr/><br/>

# Cmdlet Index
### GenXdev.Helpers
| Command | Aliases | Description |
| :--- | :--- | :--- |
| [alignScript](#alignscript) | &nbsp; | Returns a string (with altered indentation) of a provided scriptblock string |
| [ConvertTo-HashTable](#convertto-hashtable) | &nbsp; | Converts a PSCustomObject to a HashTable recursively. |
| [ConvertTo-JsonEx](#convertto-jsonex) | tojsonex | Converts an object to a JSON string with extended options. |
| [EnsureGenXdev](#ensuregenxdev) | &nbsp; | &nbsp; |
| [EnsureNuGetAssembly](#ensurenugetassembly) | &nbsp; | Downloads and loads .NET assemblies from NuGet packages based on package key or ID. |
| [Get-DefaultWebLanguage](#get-defaultweblanguage) | &nbsp; | Gets the default web language key based on the system's current language settings. |
| [Get-GenXDevCmdlet](#get-genxdevcmdlet) | gcmds | Retrieves and lists all GenXdev cmdlets and their details. |
| [Get-ImageGeolocation](#get-imagegeolocation) | &nbsp; | Extracts geolocation data from an image file. |
| [Get-ImageMetadata](#get-imagemetadata) | &nbsp; | Extracts comprehensive metadata from an image file. |
| [Get-WebLanguageDictionary](#get-weblanguagedictionary) | &nbsp; | Returns a reversed dictionary for all languages supported by Google Search |
| [Import-GenXdevModules](#import-genxdevmodules) | reloadgenxdev | Imports all GenXdev PowerShell modules into the global scope. |
| [Initialize-SearchPaths](#initialize-searchpaths) | &nbsp; | Initializes and configures system search paths for package management. |
| [Invoke-OnEachGenXdevModule](#invoke-oneachgenxdevmodule) | foreach-genxdev-module-do | Executes a script block on each GenXdev module in the workspace. |
| [Out-Serial](#out-serial) | &nbsp; | Sends a string to a serial port |
| [Remove-JSONComments](#remove-jsoncomments) | &nbsp; | Removes comments from JSON content. |
| [resetdefaultmonitor](#resetdefaultmonitor) | &nbsp; | Restores default secondary monitor configuration. |
| [SecondScreen](#secondscreen) | &nbsp; | Sets default second-monitor configuration. |
| [Show-GenXDevCmdlet](#show-genxdevcmdlet) | cmds | Displays GenXdev PowerShell modules with their cmdlets and aliases. |
| [Show-Verb](#show-verb) | showverbs | Shows a short alphabetical list of all PowerShell verbs. |
| [SideBySide](#sidebyside) | &nbsp; | Sets default side-by-side configuration. |
| [Test-UnattendedMode](#test-unattendedmode) | &nbsp; | Detects if PowerShell is running in unattended/automated mode |

### GenXdev.Helpers.Math.Physics
| Command | Aliases | Description |
| :--- | :--- | :--- |
| [Get-FreeFallHeight](#get-freefallheight) | &nbsp; | Calculates the height fallen during free fall for a given time duration. |
| [Get-FreeFallTime](#get-freefalltime) | &nbsp; | Calculates the time it takes for an object to fall a specified distance. |

<br/><hr/><br/>


# Cmdlets

&nbsp;<hr/>
###	GenXdev.Helpers<hr/> 

##	alignScript 
```PowerShell 

   alignScript  
```` 

### SYNOPSIS 
    Returns a string (with altered indentation) of a provided scriptblock string  

### SYNTAX 
```PowerShell 
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

<br/><hr/><br/>
 

##	ConvertTo-HashTable 
```PowerShell 

   ConvertTo-HashTable  
```` 

### SYNTAX 
```PowerShell 
ConvertTo-HashTable [-InputObject] <Object[]>
    [<CommonParameters>] 
```` 

### PARAMETERS 
    -InputObject <Object[]>  
        The PSCustomObject to convert into a HashTable  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue)  
        Parameter set name           Default  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	ConvertTo-JsonEx 
```PowerShell 

   ConvertTo-JsonEx                     --> tojsonex  
```` 

### SYNTAX 
```PowerShell 
ConvertTo-JsonEx [-Object] <Object> [-Compress]
    [<CommonParameters>] 
```` 

### PARAMETERS 
    -Compress  
        Compress the JSON output by removing whitespace  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -Object <Object>  
        The object to convert to JSON  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	EnsureGenXdev 
```PowerShell 

   EnsureGenXdev  
```` 

### SYNTAX 
```PowerShell 
EnsureGenXdev [-Force] [-DownloadLMStudioModels] [-DownloadAllNugetPackages] [-ForceConsent] [-ConsentToThirdPartySoftwareInstallation] [<CommonParameters>] 
```` 

### PARAMETERS 
    -ConsentToThirdPartySoftwareInstallation  
        Automatically consent to third-party software installation and set persistent flag for all packages.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -DownloadAllNugetPackages  
        Downloads and loads all NuGet packages defined in the packages.json manifest file  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -DownloadLMStudioModels  
        Downloads and initializes LMStudio models for various AI query types  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -Force  
        Forces the execution of ensure operations even if they appear to be already completed  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ForceConsent  
        Force a consent prompt even if preference is set for third-party software installation.  
        Required?                    false  
        Position?                    Named  
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

<br/><hr/><br/>
 

##	EnsureNuGetAssembly 
```PowerShell 

   EnsureNuGetAssembly  
```` 

### SYNTAX 
```PowerShell 
EnsureNuGetAssembly [-PackageKey] <string> [-ManifestPath <string>] [-Version <string>] [-TypeName <string>] [-ForceLatest] [-Destination <string>] [-Description <string>] [-Publisher <string>] [-ForceConsent] [-ConsentToThirdPartySoftwareInstallation] [<CommonParameters>] 
```` 

### PARAMETERS 
    -ConsentToThirdPartySoftwareInstallation  
        Automatically consent to third-party software installation and set persistent flag.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -Description <string>  
        Optional description of the software and its purpose for consent.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -Destination <string>  
        Custom install destination; defaults to local persistent or global cache.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ForceConsent  
        Force a prompt even if preference is set for consent.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ForceLatest  
        Fallback to latest if exact version fails.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ManifestPath <string>  
        Path to packages.json; defaults to module root if omitted.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -PackageKey <string>  
        Package key from packages.json or direct NuGet PackageId.  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -Publisher <string>  
        Optional publisher or vendor of the software for consent.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -TypeName <string>  
        TypeName to verify loading.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -Version <string>  
        Specific version; if omitted, use highest from JSON or latest.  
        Required?                    false  
        Position?                    Named  
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

<br/><hr/><br/>
 

##	Get-DefaultWebLanguage 
```PowerShell 

   Get-DefaultWebLanguage  
```` 

### SYNTAX 
```PowerShell 
Get-DefaultWebLanguage [<CommonParameters>] 
```` 

### PARAMETERS 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	Get-GenXDevCmdlet 
```PowerShell 

   Get-GenXDevCmdlet                    --> gcmds  
```` 

### SYNTAX 
```PowerShell 
Get-GenXDevCmdlet [[-CmdletName] <string>]
    [[-DefinitionMatches] <string>] [[-ModuleName]
    <string[]>] [-NoLocal] [-OnlyPublished] [-FromScripts]
    [-IncludeScripts] [-OnlyReturnModuleNames] [-ExactMatch]
    [<CommonParameters>] 
```` 

### PARAMETERS 
    -CmdletName <string>  
        Search pattern to filter cmdlets  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       true (ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Filter, CmdLet, Cmd, FunctionName, Name  
        Dynamic?                     false  
        Accept wildcard characters?  true  
    -DefinitionMatches <string>  
        Regular expression to match cmdlet definitions  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ExactMatch  
        Perform exact matching instead of wildcard matching  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -FromScripts  
        Search in script files instead of modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -IncludeScripts  
        Includes the scripts directory in addition to regular modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           ModuleName  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ModuleName <string[]>  
        GenXdev module names to search  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Module, BaseModuleName, SubModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
    -NoLocal  
        Skip searching in local module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -OnlyPublished  
        Only search in published module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -OnlyReturnModuleNames  
        Only return unique module names  
        Required?                    false  
        Position?                    Named  
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

<br/><hr/><br/>
 

##	Get-ImageGeolocation 
```PowerShell 

   Get-ImageGeolocation  
```` 

### SYNTAX 
```PowerShell 
Get-ImageGeolocation [-ImagePath] <string>
    [<CommonParameters>] 
```` 

### PARAMETERS 
    -ImagePath <string>  
        Path to the image file to analyze  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	Get-ImageMetadata 
```PowerShell 

   Get-ImageMetadata  
```` 

### SYNTAX 
```PowerShell 
Get-ImageMetadata [-ImagePath] <string> [-ForceConsent]
    [-ConsentToThirdPartySoftwareInstallation]
    [<CommonParameters>] 
```` 

### PARAMETERS 
    -ConsentToThirdPartySoftwareInstallation  
        Automatically consent to third-party software installation and set persistent flag for ImageSharp packages.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ForceConsent  
        Force a consent prompt even if preference is set for ImageSharp package installation.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ImagePath <string>  
        Path to the image file to analyze  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	Get-WebLanguageDictionary 
```PowerShell 

   Get-WebLanguageDictionary  
```` 

### SYNOPSIS 
    Returns a reversed dictionary for all languages supported by Google Search  

### SYNTAX 
```PowerShell 
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

<br/><hr/><br/>
 

##	Import-GenXdevModules 
```PowerShell 

   Import-GenXdevModules                --> reloadgenxdev  
```` 

### SYNTAX 
```PowerShell 
Import-GenXdevModules [-DebugFailedModuleDefinitions]
    [<CommonParameters>] 
```` 

### PARAMETERS 
    -DebugFailedModuleDefinitions  
        Enable debug output for failed module definitions  
        Required?                    false  
        Position?                    Named  
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

<br/><hr/><br/>
 

##	Initialize-SearchPaths 
```PowerShell 

   Initialize-SearchPaths  
```` 

### SYNTAX 
```PowerShell 
Initialize-SearchPaths [[-WorkspaceFolder] <string>]
    [<CommonParameters>] 
```` 

### PARAMETERS 
    -WorkspaceFolder <string>  
        Required?                    false  
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

<br/><hr/><br/>
 

##	Invoke-OnEachGenXdevModule 
```PowerShell 

   Invoke-OnEachGenXdevModule           --> foreach-genxdev-module-do  
```` 

### SYNTAX 
```PowerShell 
Invoke-OnEachGenXdevModule [-Script] <scriptblock>
    [[-ModuleName] <string[]>] [-NoLocal] [-OnlyPublished]
    [-FromScripts] [-IncludeScripts]
    [-IncludeGenXdevMainModule] [<CommonParameters>] 
```` 

### PARAMETERS 
    -FromScripts  
        Process scripts directory instead of module directories  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -IncludeGenXdevMainModule  
        Includes the main GenXdev module in addition to sub-modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -IncludeScripts  
        Includes the scripts directory in addition to regular modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ModuleName <string[]>  
        Filter to apply to module names  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      Module, BaseModuleName, SubModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
    -NoLocal  
        Excludes local development modules from processing  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -OnlyPublished  
        Includes only published modules that have LICENSE and README.md files  
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

<br/><hr/><br/>
 

##	Out-Serial 
```PowerShell 

   Out-Serial  
```` 

### SYNOPSIS 
    Sends a string to a serial port  

### SYNTAX 
```PowerShell 
Out-Serial [[-Portname] <String>] [[-BaudRate] <Int32>]
    [[-MaxBytesToRead] <UInt32>] [[-ReadTimeout] <UInt32>]
    [[-WriteTimeout] <UInt32>] [[-Parity] <String>]
    [[-DataBits] <Int32>] [[-StopBits] <String>] [-Text]
    <Object> [-AddCRLinefeeds] [<CommonParameters>] 
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

<br/><hr/><br/>
 

##	Remove-JSONComments 
```PowerShell 

   Remove-JSONComments  
```` 

### SYNTAX 
```PowerShell 
Remove-JSONComments [-Json] <string[]> [<CommonParameters>] 
```` 

### PARAMETERS 
    -Json <string[]>  
        JSON content to process as string array  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue)  
        Parameter set name           Default  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	resetdefaultmonitor 
```PowerShell 

   resetdefaultmonitor  
```` 

### SYNOPSIS 
    Restores default secondary monitor configuration.  

### SYNTAX 
```PowerShell 
resetdefaultmonitor [<CommonParameters>] 
```` 

### DESCRIPTION 
    This script restores the default secondary monitor configuration for the system,  
    setting the secondary monitor to the original default value.  
    This is useful for users who want to revert to their previous multi-monitor setup after using side-by-side configurations.  
    See also: 'sidebyside' function to switch to side-by-side mode for new windows.  

### PARAMETERS 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	SecondScreen 
```PowerShell 

   secondscreen  
```` 

### SYNOPSIS 
    Sets default second-monitor configuration.  

### SYNTAX 
```PowerShell 
secondscreen [<CommonParameters>] 
```` 

### DESCRIPTION 
    Sets the default behavior for GenXdev window openings to be on the secondary monitor.  
    This is useful for users with a single monitor or those who prefer side-by-side window layouts.  
    See also cmdlet 'sidebyside' and 'restoredefaultmonitor'  

### PARAMETERS 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	Show-GenXDevCmdlet 
```PowerShell 

   Show-GenXdevCmdlet                   --> cmds  
```` 

### SYNTAX 
```PowerShell 
Show-GenXdevCmdlet [[-CmdletName] <string>]
    [[-DefinitionMatches] <string>] [[-ModuleName]
    <string[]>] [-NoLocal] [-OnlyPublished] [-FromScripts]
    [-IncludeScripts] [-OnlyReturnModuleNames] [-ExactMatch]
    [-Online] [-OnlyAliases] [-ShowTable] [-PassThru]
    [<CommonParameters>] 
```` 

### PARAMETERS 
    -CmdletName <string>  
        Search pattern to filter cmdlets  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       true (ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Filter, CmdLet, Cmd, FunctionName, Name  
        Dynamic?                     false  
        Accept wildcard characters?  true  
    -DefinitionMatches <string>  
        Regular expression to match cmdlet definitions  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ExactMatch  
        Perform exact matching instead of wildcard matching  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -FromScripts  
        Search in script files instead of modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -IncludeScripts  
        Includes the scripts directory in addition to regular modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           ModuleName  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ModuleName <string[]>  
        GenXdev module names to search  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Module, BaseModuleName, SubModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
    -NoLocal  
        Skip searching in local module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -Online  
        Open GitHub documentation instead of console output  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -OnlyAliases  
        When specified displays only aliases of cmdlets  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      aliases, nonboring, notlame, handyonces  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -OnlyPublished  
        Only search in published module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -OnlyReturnModuleNames  
        Only return unique module names  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -PassThru  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -ShowTable  
        Display results in table format  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      table, grid  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	Show-Verb 
```PowerShell 

   Show-Verb                            --> showverbs  
```` 

### SYNTAX 
```PowerShell 
Show-Verb [[-Verb] <string[]>] [<CommonParameters>] 
```` 

### PARAMETERS 
    -Verb <string[]>  
        One or more verb patterns to filter (supports wildcards)  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  true  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	SideBySide 
```PowerShell 

   sidebyside  
```` 

### SYNOPSIS 
    Sets default side-by-side configuration.  

### SYNTAX 
```PowerShell 
sidebyside [<CommonParameters>] 
```` 

### DESCRIPTION 
    Sets the default behavior for GenXdev window openings to be side-by-side with PowerShell.  
    This is useful for users with a single monitor or those who prefer side-by-side window layouts.  
    See also cmdlet 'secondscreen' and 'restoredefaultmonitor'  

### PARAMETERS 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><br/>
 

##	Test-UnattendedMode 
```PowerShell 

   Test-UnattendedMode  
```` 

### SYNOPSIS 
    Detects if PowerShell is running in unattended/automated mode  

### SYNTAX 
```PowerShell 
Test-UnattendedMode [[-CallersInvocation] <InvocationInfo>]
    [-Detailed] [<CommonParameters>] 
```` 

### DESCRIPTION 
    Analyzes various indicators to determine if PowerShell is running in an  
    unattended or automated context, including pipeline analysis, environment  
    variables, console redirection, and invocation context.  
    When CallersInvocation is provided, it analyzes the pipeline position and  
    count to determine if the function is being called as part of an automated  
    pipeline or script execution.  

### PARAMETERS 
    -CallersInvocation <InvocationInfo>  
        The caller's invocation information for pipeline and automation detection.  
        Pass $MyInvocation from the calling function to analyze pipeline context.  
        Required?                    false  
        Position?                    1  
        Default value                  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -Detailed [<SwitchParameter>]  
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

### NOTES 
```PowerShell 

       The function combines multiple detection methods:  
       - Environment variables (CI/CD systems)  
       - Console redirection  
       - Interactive session detection  
       - PowerShell host type  
       - Pipeline analysis (when CallersInvocation provided)  
       - Console availability  
   -------------------------- EXAMPLE 1 --------------------------  
   PS C:\> Test-UnattendedMode  
   Returns a boolean indicating if running in unattended mode using standard detection.  
   -------------------------- EXAMPLE 2 --------------------------  
   PS C:\> Test-UnattendedMode -CallersInvocation $MyInvocation  
   Analyzes the caller's invocation context and returns a boolean result.  
   -------------------------- EXAMPLE 3 --------------------------  
   PS C:\> Test-UnattendedMode -CallersInvocation $MyInvocation -Detailed  
   Returns detailed analysis object with all indicators and pipeline information.  
   -------------------------- EXAMPLE 4 --------------------------  
   PS C:\> function My-Function {  
       $isUnattended = Test-UnattendedMode -CallersInvocation $MyInvocation  
       if ($isUnattended) {  
           Write-Verbose "Running in unattended mode, skipping interactive prompts"  
       }  
   }  
```` 

<br/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers.Math.Physics<hr/> 

##	Get-FreeFallHeight 
```PowerShell 

   Get-FreeFallHeight  
```` 

### SYNTAX 
```PowerShell 
Get-FreeFallHeight [-DurationInSeconds] <double>
    [[-TerminalVelocityInMs] <double>] [<CommonParameters>] 
```` 

### PARAMETERS 
    -DurationInSeconds <double>  
        The time duration of the fall in seconds  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -TerminalVelocityInMs <double>  
        The terminal velocity in meters per second (default: 53 m/s for human)  
        Required?                    false  
        Position?                    1  
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

<br/><hr/><br/>
 

##	Get-FreeFallTime 
```PowerShell 

   Get-FreeFallTime  
```` 

### SYNTAX 
```PowerShell 
Get-FreeFallTime [-HeightInMeters] <double>
    [[-TerminalVelocityInMs] <double>] [<CommonParameters>] 
```` 

### PARAMETERS 
    -HeightInMeters <double>  
        The initial height of the falling object in meters  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
    -TerminalVelocityInMs <double>  
        The terminal velocity of the falling object in m/s  
        Required?                    false  
        Position?                    1  
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

<br/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers<hr/>