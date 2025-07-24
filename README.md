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

Copyright (c) [year] [fullname]

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
| Command&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | aliases&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- | --- |
| [alignScript](#alignScript) |  | Returns a string (with altered indentation) of a provided scriptblock string |
| [ConvertTo-HashTable](#ConvertTo-HashTable) |  | Converts a PSCustomObject to a HashTable recursively. |
| [ConvertTo-JsonEx](#ConvertTo-JsonEx) | tojsonex | Converts an object to a JSON string with extended options. |
| [Copy-IdenticalParamValues](#Copy-IdenticalParamValues) |  |  |
| [EnsureGenXdev](#EnsureGenXdev) |  |  |
| [Get-DefaultWebLanguage](#Get-DefaultWebLanguage) |  | Gets the default web language key based on the system's current language settings. |
| [Get-GenXDevCmdlets](#Get-GenXDevCmdlets) | gcmds | Retrieves and lists all GenXdev cmdlets and their details. |
| [Get-ImageGeolocation](#Get-ImageGeolocation) |  | Extracts geolocation data from an image file. |
| [Get-ImageMetadata](#Get-ImageMetadata) |  | Extracts comprehensive metadata from an image file. |
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
| [Get-FreeFallHeight](#Get-FreeFallHeight) |  |  |
| [Get-FreeFallTime](#Get-FreeFallTime) |  | Calculates the time it takes for an object to fall a specified distance. |

<br/><hr/><hr/><br/>


# Cmdlets

&nbsp;<hr/>
###	GenXdev.Helpers<hr/> 
NAME
    alignScript
    
SYNOPSIS
    Returns a string (with altered indentation) of a provided scriptblock string
    
    
SYNTAX
    alignScript [[-script] <String>] [[-spaces] <Int32>] [<CommonParameters>]
    
    
DESCRIPTION
    Changes the indentation of a scriptblock string while respecting the original code-block identations
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    ConvertTo-HashTable
    
SYNOPSIS
    Converts a PSCustomObject to a HashTable recursively.
    
    
SYNTAX
    ConvertTo-HashTable [-InputObject] <Object[]> [<CommonParameters>]
    
    
DESCRIPTION
    This function converts a PSCustomObject and all its nested PSCustomObject
    properties into HashTables. It handles arrays and other collection types by
    processing each element recursively.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    System.Collections.Hashtable
    
    System.Collections.IEnumerable
    
    System.ValueType
    
    System.String
    
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > $object = [PSCustomObject]@{
        Name = "John"
        Age = 30
        Details = [PSCustomObject]@{
            City = "New York"
        }
    }
    $hashTable = GenXdev.Helpers\ConvertTo-HashTable -InputObject $object
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    ConvertTo-JsonEx
    
SYNOPSIS
    Converts an object to a JSON string with extended options.
    
    
SYNTAX
    ConvertTo-JsonEx [-Object] <Object> [-Compress] [<CommonParameters>]
    
    
DESCRIPTION
    Converts a PowerShell object to a JSON string using the GenXdev.Helpers.
    Serialization library. This function provides enhanced JSON serialization
    capabilities with optional compression to remove whitespace for reduced
    output size. The function is designed to handle complex PowerShell objects
    and provide more control over the JSON conversion process compared to the
    built-in ConvertTo-Json cmdlet.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    System.String
    
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > $data = @{ name = "test"; value = 123 }
    ConvertTo-JsonEx -Object $data
    
    Converts a hashtable to JSON format with standard formatting.
    
    
    
    
    -------------------------- EXAMPLE 2 --------------------------
    
    PS > $data | ConvertTo-JsonEx -Compress
    
    Converts pipeline input to compressed JSON format without whitespace.
    
    
    
    
    -------------------------- EXAMPLE 3 --------------------------
    
    PS > tojsonex $data
    
    Uses the alias to convert an object to JSON format.
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Copy-IdenticalParamValues
    
SYNOPSIS
    Copies parameter values from bound parameters to a new hashtable based on
    another function's possible parameters.
    
    
SYNTAX
    Copy-IdenticalParamValues [-BoundParameters] <Object[]> [-FunctionName] <String> [[-DefaultValues] <PSVariable[]>] [<CommonParameters>]
    
    
DESCRIPTION
    This function creates a new hashtable containing only the parameter values that
    match the parameters defined in the specified target function.
    This can then be used to invoke the function using splatting.
    
    Switch parameters are only included in the result if they were explicitly provided
    and set to $true in the bound parameters. Non-present switch parameters are
    excluded from the result to maintain proper parameter semantics.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    System.Collections.Hashtable
    
    
NOTES
    
    
        - Switch parameters are only included if explicitly set to $true
        - Default values are only applied to non-switch parameters
        - Common PowerShell parameters are automatically filtered out
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > function Test-Function {
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
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    EnsureGenXdev
    
SYNOPSIS
    Ensures all GenXdev modules are properly loaded by invoking all Ensure*
    cmdlets.
    
    
SYNTAX
    EnsureGenXdev [<CommonParameters>]
    
    
DESCRIPTION
    This function retrieves all GenXdev cmdlets that start with "Ensure" and
    executes each one to guarantee that all required GenXdev modules and
    dependencies are properly loaded and available for use. Any failures during
    the execution are caught and displayed as informational messages.
    

PARAMETERS
    <CommonParameters>
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > EnsureGenXdev
    
    This command runs all available Ensure* cmdlets to initialize the GenXdev
    environment.
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Get-DefaultWebLanguage
    
SYNOPSIS
    Gets the default web language key based on the system's current language settings.
    
    
SYNTAX
    Get-DefaultWebLanguage [<CommonParameters>]
    
    
DESCRIPTION
    Retrieves the current system language and culture settings and maps them to the
    corresponding web language dictionary key used by translation services.
    

PARAMETERS
    <CommonParameters>
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    System.String
    
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > Get-DefaultWebLanguage
    Returns "English" for an English system, "Dutch" for a Dutch system, etc.
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Get-GenXDevCmdlets
    
SYNOPSIS
    Retrieves and lists all GenXdev cmdlets and their details.
    
    
SYNTAX
    Get-GenXDevCmdlets [[-CmdletName] <String>] [[-BaseModuleName] <String[]>] [-NoLocal] [-OnlyPublished] [-FromScripts] [-OnlyReturnModuleNames] [-ExactMatch] [<CommonParameters>]
    
    
DESCRIPTION
    Searches through installed GenXdev modules and script files to find cmdlets,
    their aliases, and descriptions. Can filter by name pattern and module name.
    

PARAMETERS
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
        Default value                @('GenXdev*')
        Accept pipeline input?       true (ByValue, ByPropertyName)
        Aliases                      
        Accept wildcard characters?  false
        
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
        
    -OnlyReturnModuleNames [<SwitchParameter>]
        Only return unique module names instead of full cmdlet details.
        
        Required?                    false
        Position?                    named
        Default value                False
        Accept pipeline input?       false
        Aliases                      
        Accept wildcard characters?  false
        
    -ExactMatch [<SwitchParameter>]
        
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > Get-GenXDevCmdlets -CmdletName "Get-*" -BaseModuleName "Console" -NoLocal
    
    
    
    
    
    
    -------------------------- EXAMPLE 2 --------------------------
    
    PS > gcmds Get-*
    
    
    
    
    
    
    -------------------------- EXAMPLE 3 --------------------------
    
    PS > Get-GenXDevCmdlets -OnlyReturnModuleNames
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Get-ImageGeolocation
    
SYNOPSIS
    Extracts geolocation data from an image file.
    
    
SYNTAX
    Get-ImageGeolocation [-ImagePath] <String> [<CommonParameters>]
    
    
DESCRIPTION
    This function reads EXIF metadata from an image file to extract its latitude and
    longitude coordinates. It supports images that contain GPS metadata in their EXIF
    data. The function uses the System.Drawing.Image class to load the image and
    parse the GPS coordinates from property items.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    System.Collections.Hashtable
    Returns a hashtable containing Latitude and Longitude if GPS data is found,
    otherwise returns $null.
    
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > Get-ImageGeolocation -ImagePath "C:\Photos\vacation.jpg"
    
    
    
    
    
    
    -------------------------- EXAMPLE 2 --------------------------
    
    PS > "C:\Photos\vacation.jpg" | Get-ImageGeolocation
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Get-ImageMetadata
    
SYNOPSIS
    Extracts comprehensive metadata from an image file.
    
    
SYNTAX
    Get-ImageMetadata [-ImagePath] <String> [<CommonParameters>]
    
    
DESCRIPTION
    This function reads EXIF, IPTC and other metadata from an image file. It extracts
    a wide range of information including camera details, exposure settings, GPS coordinates,
    dates, copyright information, and more. It supports images that contain metadata
    in their EXIF data (JPEG, TIFF) as well as PNG metadata.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    System.Collections.Hashtable
    Returns a hashtable containing all available metadata categories including:
    - Basic (dimensions, format, etc.)
    - Camera (make, model, etc.)
    - Exposure (aperture, shutter speed, ISO, etc.)
    - GPS (latitude, longitude, etc.)
    - DateTime (when taken, modified, etc.)
    - Author (artist, copyright, etc.)
    - Additional (software, comments, etc.)
    
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > Get-ImageMetadata -ImagePath "C:\Photos\vacation.jpg"
    
    
    
    
    
    
    -------------------------- EXAMPLE 2 --------------------------
    
    PS > "C:\Photos\vacation.jpg" | Get-ImageMetadata
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Get-WebLanguageDictionary
    
SYNOPSIS
    Returns a reversed dictionary for all languages supported by Google Search
    
    
SYNTAX
    Get-WebLanguageDictionary [<CommonParameters>]
    
    
DESCRIPTION
    Returns a reversed dictionary for all languages supported by Google Search
    

PARAMETERS
    <CommonParameters>
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Import-GenXdevModules
    
SYNOPSIS
    Imports all GenXdev PowerShell modules into the global scope.
    
    
SYNTAX
    Import-GenXdevModules [-DebugFailedModuleDefinitions] [<CommonParameters>]
    
    
DESCRIPTION
    Scans the parent directory for GenXdev modules and imports each one into the
    global scope. Uses location stack management to preserve the working directory
    and provides visual feedback for successful and failed imports. Tracks function
    count changes during the import process.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > Import-GenXdevModules -DebugFailedModuleDefinitions
    Imports modules with debug output for failures
    
    
    
    
    
    
    -------------------------- EXAMPLE 2 --------------------------
    
    PS > reloadgenxdev
    Imports all modules using the alias
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Initialize-SearchPaths
    
SYNOPSIS
    Initializes and configures system search paths for package management.
    
    
SYNTAX
    Initialize-SearchPaths [[-WorkspaceFolder] <String>] [<CommonParameters>]
    
    
DESCRIPTION
    This function builds a comprehensive list of search paths by combining default
    system locations, chocolatey paths, development tool paths, and custom package
    paths. It then updates the system's PATH environment variable with these
    consolidated paths.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > Initialize-SearchPaths -WorkspaceFolder "C:\workspace"
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Invoke-OnEachGenXdevModule
    
SYNTAX
    Invoke-OnEachGenXdevModule [-Script] <scriptblock> [[-BaseModuleName] <string[]>] [-NoLocal] [-OnlyPublished] [-FromScripts] [<CommonParameters>]
    
    
PARAMETERS
    -BaseModuleName <string[]>
        Filter to apply to module names
        
        Required?                    false
        Position?                    1
        Accept pipeline input?       true (ByValue, ByPropertyName)
        Parameter set name           (All)
        Aliases                      Module, ModuleName
        Dynamic?                     false
        Accept wildcard characters?  false
        
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
    
INPUTS
    System.String[]
    
    
OUTPUTS
    System.Object
    
ALIASES
    foreach-genxdev-module-do
    

REMARKS
    None 

<br/><hr/><hr/><br/>
 
NAME
    Out-Serial
    
SYNOPSIS
    Sends a string to a serial port
    
    
SYNTAX
    Out-Serial [[-Portname] <String>] [[-BaudRate] <Int32>] [[-MaxBytesToRead] <UInt32>] [[-ReadTimeout] <UInt32>] [[-WriteTimeout] <UInt32>] [[-Parity] <String>] [[-DataBits] <Int32>] [[-StopBits] <String>] [-Text] <Object> [-AddCRLinefeeds] [<CommonParameters>]
    
    
DESCRIPTION
    Allows you to send a string to a serial communication port
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Remove-JSONComments
    
SYNOPSIS
    Removes comments from JSON content.
    
    
SYNTAX
    Remove-JSONComments [-Json] <String[]> [<CommonParameters>]
    
    
DESCRIPTION
    Processes JSON content and removes both single-line and multi-line comments while
    preserving the JSON structure. This is useful for cleaning up JSON files that
    contain documentation comments before parsing.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    System.String
    
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > $jsonContent = @'
    {
        // This is a comment
        "name": "test", /* inline comment */
        /* multi-line
           comment */
        "value": 123
    }
    '@ -split "`n"
    Remove-JSONComments -Json $jsonContent
    
    
    
    
    
    
    -------------------------- EXAMPLE 2 --------------------------
    
    PS > $jsonContent | Remove-JSONComments
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Show-GenXDevCmdlets
    
SYNOPSIS
    Displays GenXdev PowerShell modules with their cmdlets and aliases.
    
    
SYNTAX
    Show-GenXDevCmdlets [[-CmdletName] <String>] [[-BaseModuleName] <String[]>] [-NoLocal] [-OnlyPublished] [-FromScripts] [-Online] [-OnlyAliases] [-ShowTable] [-PassThru] [-OnlyReturnModuleNames] [<CommonParameters>]
    
    
DESCRIPTION
    Lists all installed GenXdev PowerShell modules and their associated cmdlets and
    aliases. Uses Get-GenXDevCmdlets to retrieve cmdlet information and optionally
    their script positions. Provides filtering and various display options.
    

PARAMETERS
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
        Default value                @('GenXdev*')
        Accept pipeline input?       true (ByValue, ByPropertyName)
        Aliases                      
        Accept wildcard characters?  false
        
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
        
    -PassThru [<SwitchParameter>]
        
        Required?                    false
        Position?                    named
        Default value                False
        Accept pipeline input?       false
        Aliases                      
        Accept wildcard characters?  false
        
    -OnlyReturnModuleNames [<SwitchParameter>]
        Only return unique module names instead of displaying cmdlet details.
        
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    System.Collections.ArrayList
    
    System.Void
    
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > Show-GenXDevCmdlets -CmdletName "Get" -ModuleName "Console" -ShowTable
    Lists all cmdlets starting with "Get" in the Console module as a table
    
    
    
    
    
    
    -------------------------- EXAMPLE 2 --------------------------
    
    PS > cmds get -m console
    Lists all cmdlets starting with "Get" in the Console module
    
    
    
    
    
    
    -------------------------- EXAMPLE 3 --------------------------
    
    PS > Show-GenXDevCmdlets -OnlyReturnModuleNames
    Returns only unique module names
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 
NAME
    Show-Verb
    
SYNOPSIS
    Shows a short alphabetical list of all PowerShell verbs.
    
    
SYNTAX
    Show-Verb [[-Verb] <String[]>] [<CommonParameters>]
    
    
DESCRIPTION
    Displays PowerShell approved verbs in a comma-separated list. If specific verbs
    are provided as input, only matching verbs will be shown. Supports wildcards.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > Show-Verb
    Shows all approved PowerShell verbs
    
    
    
    
    
    
    -------------------------- EXAMPLE 2 --------------------------
    
    PS > Show-Verb -Verb "Get*"
    Shows all approved verbs starting with "Get"
    
    
    
    
    
    
    -------------------------- EXAMPLE 3 --------------------------
    
    PS > showverbs "Set*", "Get*"
    Shows all approved verbs starting with "Set" or "Get" using the alias
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers.Math.Physics<hr/> 
NAME
    Get-FreeFallHeight
    
SYNTAX
    Get-FreeFallHeight [[-DurationInSeconds] <double>] [[-TerminalVelocityInMs] <double>] 
    
    
PARAMETERS
    -DurationInSeconds <double>
        
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
        
    
INPUTS
    None
    
    
OUTPUTS
    System.Object
    
ALIASES
    None
    

REMARKS
    None 

<br/><hr/><hr/><br/>
 
NAME
    Get-FreeFallTime
    
SYNOPSIS
    Calculates the time it takes for an object to fall a specified distance.
    
    
SYNTAX
    Get-FreeFallTime [-HeightInMeters] <Double> [[-TerminalVelocityInMs] <Double>] [<CommonParameters>]
    
    
DESCRIPTION
    This function calculates the time it takes for an object to fall from a given
    height, taking into account terminal velocity due to air resistance. It uses a
    numerical method with small time steps for accurate calculation.
    

PARAMETERS
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
        about_CommonParameters (https://go.microsoft.com/fwlink/?LinkID=113216). 
    
INPUTS
    
OUTPUTS
    System.Double
    
    System.Int32
    
    
    -------------------------- EXAMPLE 1 --------------------------
    
    PS > Get-FreeFallTime -HeightInMeters 100
    Returns the time in seconds for an object to fall 100 meters with default
    terminal velocity
    
    
    
    
    
    
    -------------------------- EXAMPLE 2 --------------------------
    
    PS > Get-FreeFallTime 500 45
    Returns the time in seconds for an object to fall 500 meters with a terminal
    velocity of 45 m/s
    
    
    
    
    
    
    
RELATED LINKS 

<br/><hr/><hr/><br/>
