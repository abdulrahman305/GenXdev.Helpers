<hr/>

<img src="powershell.jpg" alt="GenXdev" width="50%"/>

<hr/>

### NAME

    GenXdev.Helpers

### SYNOPSIS

    A Windows PowerShell module with helpers mostly used by other GenXdev modules
[![GenXdev.Helpers](https://img.shields.io/powershellgallery/v/GenXdev.Helpers.svg?style=flat-square&label=GenXdev.Helpers)](https://www.powershellgallery.com/packages/GenXdev.Helpers/)

## MIT License

```text
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
### GenXdev.Helpers
| Command | Aliases | Description |
| --- | --- | --- |
| [alignScript](#alignscript) | &nbsp; | Returns a string (with altered indentation) of a provided scriptblock string |
| [ConvertTo-HashTable](#convertto-hashtable) | &nbsp; | Converts a PSCustomObject to a HashTable recursively. |
| [ConvertTo-JsonEx](#convertto-jsonex) | tojsonex | Converts an object to a JSON string with extended options. |
| [Copy-IdenticalParamValues](#copy-identicalparamvalues) | &nbsp; |  |
| [EnsureGenXdev](#ensuregenxdev) | &nbsp; |  |
| [Get-DefaultWebLanguage](#get-defaultweblanguage) | &nbsp; | Gets the default web language key based on the system's current language settings. |
| [Get-GenXDevCmdlets](#get-genxdevcmdlets) | gcmds | Retrieves and lists all GenXdev cmdlets and their details. |
| [Get-ImageGeolocation](#get-imagegeolocation) | &nbsp; | Extracts geolocation data from an image file. |
| [Get-ImageMetadata](#get-imagemetadata) | &nbsp; | Extracts comprehensive metadata from an image file. |
| [Get-WebLanguageDictionary](#get-weblanguagedictionary) | &nbsp; | Returns a reversed dictionary for all languages supported by Google Search |
| [Import-GenXdevModules](#import-genxdevmodules) | reloadgenxdev | Imports all GenXdev PowerShell modules into the global scope. |
| [Initialize-SearchPaths](#initialize-searchpaths) | &nbsp; | Initializes and configures system search paths for package management. |
| [Invoke-OnEachGenXdevModule](#invoke-oneachgenxdevmodule) | foreach-genxdev-module-do | Executes a script block on each GenXdev module in the workspace. |
| [Out-Serial](#out-serial) | &nbsp; | Sends a string to a serial port |
| [Remove-JSONComments](#remove-jsoncomments) | &nbsp; | Removes comments from JSON content. |
| [Show-GenXDevCmdlets](#show-genxdevcmdlets) | cmds | Displays GenXdev PowerShell modules with their cmdlets and aliases. |
| [Show-Verb](#show-verb) | showverbs | Shows a short alphabetical list of all PowerShell verbs. |

### GenXdev.Helpers.Math.Physics
| Command | Aliases | Description |
| --- | --- | --- |
| [Get-FreeFallHeight](#get-freefallheight) | &nbsp; | Calculates the height fallen during free fall for a given time duration. |
| [Get-FreeFallTime](#get-freefalltime) | &nbsp; | Calculates the time it takes for an object to fall a specified distance. |

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
    Converts a PowerShell object to a JSON string using the GenXdev.Helpers.  
    Serialization library. This function provides enhanced JSON serialization  
    capabilities with optional compression to remove whitespace for reduced  
    output size. The function is designed to handle complex PowerShell objects  
    and provide more control over the JSON conversion process compared to the  
    built-in ConvertTo-Json cmdlet.  

### PARAMETERS 
    -Object <Object>  
        The PowerShell object to convert to JSON format. This can be any type of  
        PowerShell object including hashtables, arrays, custom objects, or primitive  
        types.  
        Required?                    true  
        Position?                    1  
        Default value                  
        Accept pipeline input?       true (ByValue)  
        Aliases                        
        Accept wildcard characters?  false  
    -Compress [<SwitchParameter>]  
        When specified, removes all unnecessary whitespace from the output JSON string  
        to minimize the size. This is useful when transmitting JSON data over  
        networks or storing in space-constrained environments.  
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
    Switch parameters are only included in the result if they were explicitly provided  
    and set to $true in the bound parameters. Non-present switch parameters are  
    excluded from the result to maintain proper parameter semantics.  

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
        Default values for non-switch parameters that are not present in BoundParameters.  
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

### NOTES 
````PowerShell 

       - Switch parameters are only included if explicitly set to $true  
       - Default values are only applied to non-switch parameters  
       - Common PowerShell parameters are automatically filtered out  
   -------------------------- EXAMPLE 1 --------------------------  
   PS C:\> function Test-Function {  
       [CmdletBinding()]  
       param(  
           [Parameter(Mandatory = $true)]  
           [string] $Path,  
           [Parameter(Mandatory = $false)]  
           [switch] $Recurse  
       )  
   $params = GenXdev.Helpers\Copy-IdenticalParamValues -BoundParameters $PSBoundParameters `  
           -FunctionName 'Get-ChildItem'  
       Get-ChildItem @params  
   }  
```` 

<br/><hr/><hr/><br/>
 

##	EnsureGenXdev 
````PowerShell 

   EnsureGenXdev  
```` 

### SYNOPSIS 
    Ensures all GenXdev modules are properly loaded by invoking all Ensure*  
    cmdlets.  

### SYNTAX 
````PowerShell 
EnsureGenXdev [-Force] [-DownloadLMStudioModels] [<CommonParameters>] 
```` 

### DESCRIPTION 
    This function retrieves all GenXdev cmdlets that start with "Ensure" and  
    executes each one to guarantee that all required GenXdev modules and  
    dependencies are properly loaded and available for use. Any failures during  
    the execution are caught and displayed as informational messages.  

### PARAMETERS 
    -Force [<SwitchParameter>]  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -DownloadLMStudioModels [<SwitchParameter>]  
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
Get-GenXDevCmdlets [[-CmdletName] <String>] [[-DefinitionMatches] <String>] [[-ModuleName] <String[]>] [-NoLocal] [-OnlyPublished] [-FromScripts] [-IncludeScripts] [-OnlyReturnModuleNames] [-ExactMatch] [<CommonParameters>] 
```` 

### DESCRIPTION 
    Searches through installed GenXdev modules and script files to find cmdlets,  
    their aliases, and descriptions. Can filter by name pattern and module name.  
    Supports filtering by cmdlet definitions and provides flexible search options  
    across both local and published module paths.  

### PARAMETERS 
    -CmdletName <String>  
        Search pattern to filter cmdlets. Supports wildcards (*) and exact matching.  
        When ExactMatch is false, automatically wraps simple strings with wildcards.  
        Required?                    false  
        Position?                    1  
        Default value                  
        Accept pipeline input?       true (ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  true  
    -DefinitionMatches <String>  
        Regular expression to match cmdlet definitions. Used to filter cmdlets based  
        on their function content or implementation details.  
        Required?                    false  
        Position?                    2  
        Default value                  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  false  
    -ModuleName <String[]>  
        One or more GenXdev module names to search. Can omit GenXdev prefix. Supports  
        wildcards and validates module name patterns for GenXdev modules.  
        Required?                    false  
        Position?                    3  
        Default value                  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  true  
    -NoLocal [<SwitchParameter>]  
        Skip searching in local module paths. When specified, only searches in  
        published or system module locations.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -OnlyPublished [<SwitchParameter>]  
        Limit search to published module paths only. Excludes local development  
        modules and focuses on released versions.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -FromScripts [<SwitchParameter>]  
        Search in script files instead of module files. Changes the search target  
        from PowerShell modules to standalone script files.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -IncludeScripts [<SwitchParameter>]  
        Includes the scripts directory in addition to regular modules. Expands the  
        search scope to cover both modules and scripts simultaneously.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -OnlyReturnModuleNames [<SwitchParameter>]  
        Only return unique module names instead of full cmdlet details. Provides a  
        summary view of available modules rather than detailed cmdlet information.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -ExactMatch [<SwitchParameter>]  
        Perform exact matching instead of wildcard matching. When specified, disables  
        automatic wildcard wrapping for simple search patterns.  
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
    data. The function uses the System.Drawing.Image class to load the image and  
    parse the GPS coordinates from property items.  

### PARAMETERS 
    -ImagePath <String>  
        The full path to the image file to analyze. The file must be a valid image format  
        that supports EXIF metadata (JPEG, TIFF, etc.).  
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
 

##	Get-ImageMetadata 
````PowerShell 

   Get-ImageMetadata  
```` 

### SYNOPSIS 
    Extracts comprehensive metadata from an image file.  

### SYNTAX 
````PowerShell 
Get-ImageMetadata [-ImagePath] <String> [<CommonParameters>] 
```` 

### DESCRIPTION 
    This function reads EXIF, IPTC and other metadata from an image file. It extracts  
    a wide range of information including camera details, exposure settings, GPS coordinates,  
    dates, copyright information, and more. It supports images that contain metadata  
    in their EXIF data (JPEG, TIFF) as well as PNG metadata.  

### PARAMETERS 
    -ImagePath <String>  
        The full path to the image file to analyze. The file must be a valid image format  
        that supports metadata (JPEG, TIFF, PNG, etc.).  
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

### SYNOPSIS 
    Executes a script block on each GenXdev module in the workspace.  

### SYNTAX 
````PowerShell 
Invoke-OnEachGenXdevModule [-Script] <ScriptBlock> [[-ModuleName] <String[]>] [-NoLocal] [-OnlyPublished] [-FromScripts] [-IncludeScripts] [-IncludeGenXdevMainModule] [<CommonParameters>] 
```` 

### DESCRIPTION 
    This function iterates through GenXdev modules in the workspace and executes  
    a provided script block against each module. It can filter modules by name  
    pattern, exclude local modules, include only published modules, or process  
    scripts instead of modules. The function automatically navigates to the  
    correct module directory before executing the script block.  

### PARAMETERS 
    -Script <ScriptBlock>  
        The script block to execute for each GenXdev module. The module object is  
        passed as an argument to the script block.  
        Required?                    true  
        Position?                    1  
        Default value                  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -ModuleName <String[]>  
        Filter to apply to module names. Supports wildcards and multiple patterns.  
        Defaults to 'GenXdev*' to include all GenXdev modules.  
        Required?                    false  
        Position?                    2  
        Default value                @('GenXdev*')  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  true  
    -NoLocal [<SwitchParameter>]  
        Excludes local development modules from processing.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -OnlyPublished [<SwitchParameter>]  
        Includes only published modules that have LICENSE and README.md files.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -FromScripts [<SwitchParameter>]  
        Process scripts directory instead of module directories.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -IncludeScripts [<SwitchParameter>]  
        Includes the scripts directory in addition to regular modules.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -IncludeGenXdevMainModule [<SwitchParameter>]  
        Includes the main GenXdev module in addition to sub-modules.  
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
 

##	Out-Serial 
````PowerShell 

   Out-Serial  
```` 

### SYNOPSIS 
    Sends a string to a serial port  

### SYNTAX 
````PowerShell 
Out-Serial [[-Portname] <String>] [[-BaudRate] <Int32>] [[-MaxBytesToRead] <UInt32>] [[-ReadTimeout] <UInt32>] [[-WriteTimeout] <UInt32>] [[-Parity] <String>] [[-DataBits] <Int32>] [[-StopBits] <String>] [-Text] <Object> [-AddCRLinefeeds] [<CommonParameters>] 
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
Show-GenXDevCmdlets [[-CmdletName] <String>] [[-DefinitionMatches] <String>] [[-ModuleName] <String[]>] [-NoLocal] [-OnlyPublished] [-FromScripts] [-IncludeScripts] [-OnlyReturnModuleNames] [-ExactMatch] [-Online] [-OnlyAliases] [-ShowTable] [-PassThru] [<CommonParameters>] 
```` 

### DESCRIPTION 
    Lists all installed GenXdev PowerShell modules and their associated cmdlets and  
    aliases. Uses Get-GenXDevCmdlets to retrieve cmdlet information and optionally  
    their script positions. Provides filtering and various display options.  

### PARAMETERS 
    -CmdletName <String>  
        Search pattern to filter cmdlets. Supports wildcards (*) and exact matching.  
        When ExactMatch is false, automatically wraps simple strings with wildcards.  
        Required?                    false  
        Position?                    1  
        Default value                  
        Accept pipeline input?       true (ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  true  
    -DefinitionMatches <String>  
        Regular expression to match cmdlet definitions. Used to filter cmdlets based  
        on their function content or implementation details.  
        Required?                    false  
        Position?                    2  
        Default value                  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  false  
    -ModuleName <String[]>  
        One or more GenXdev module names to search. Can omit GenXdev prefix. Supports  
        wildcards and validates module name patterns for GenXdev modules.  
        Required?                    false  
        Position?                    3  
        Default value                  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Aliases                        
        Accept wildcard characters?  true  
    -NoLocal [<SwitchParameter>]  
        Skip searching in local module paths. When specified, only searches in  
        published or system module locations.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -OnlyPublished [<SwitchParameter>]  
        Limit search to published module paths only. Excludes local development  
        modules and focuses on released versions.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -FromScripts [<SwitchParameter>]  
        Search in script files instead of module files. Changes the search target  
        from PowerShell modules to standalone script files.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -IncludeScripts [<SwitchParameter>]  
        Includes the scripts directory in addition to regular modules. Expands the  
        search scope to cover both modules and scripts simultaneously.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -OnlyReturnModuleNames [<SwitchParameter>]  
        Only return unique module names instead of full cmdlet details. Provides a  
        summary view of available modules rather than detailed cmdlet information.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -ExactMatch [<SwitchParameter>]  
        Perform exact matching instead of wildcard matching. When specified, disables  
        automatic wildcard wrapping for simple search patterns.  
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
    -PassThru [<SwitchParameter>]  
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

##	Get-FreeFallHeight 
````PowerShell 

   Get-FreeFallHeight  
```` 

### SYNOPSIS 
    Calculates the height fallen during free fall for a given time duration.  

### SYNTAX 
````PowerShell 
Get-FreeFallHeight [-DurationInSeconds] <Double> [[-TerminalVelocityInMs] <Double>] [<CommonParameters>] 
```` 

### DESCRIPTION 
    This function calculates the distance fallen during free fall using a  
    numerical method that accounts for air resistance and terminal velocity. The  
    calculation uses small time steps to accurately model the physics of falling  
    objects with realistic terminal velocity constraints.  

### PARAMETERS 
    -DurationInSeconds <Double>  
        The time duration of the fall in seconds for which to calculate the height.  
        Required?                    true  
        Position?                    1  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -TerminalVelocityInMs <Double>  
        The terminal velocity in meters per second. Defaults to 53 m/s which is the  
        typical terminal velocity for a human in free fall.  
        Required?                    false  
        Position?                    2  
        Default value                53  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

##	Get-FreeFallTime 
````PowerShell 

   Get-FreeFallTime  
```` 

### SYNOPSIS 
    Calculates the time it takes for an object to fall a specified distance.  

### SYNTAX 
````PowerShell 
Get-FreeFallTime [-HeightInMeters] <Double> [[-TerminalVelocityInMs] <Double>] [<CommonParameters>] 
```` 

### DESCRIPTION 
    This function calculates the time it takes for an object to fall from a given  
    height, taking into account terminal velocity due to air resistance. It uses a  
    numerical method with small time steps for accurate calculation.  

### PARAMETERS 
    -HeightInMeters <Double>  
        The initial height of the falling object in meters.  
        Required?                    true  
        Position?                    1  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    -TerminalVelocityInMs <Double>  
        The terminal velocity of the falling object in meters per second. Default value  
        is 53 m/s, which is the approximate terminal velocity of a human in free fall.  
        Required?                    false  
        Position?                    2  
        Default value                53  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   

<br/><hr/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers<hr/>