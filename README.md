<hr/>

<img src="powershell.jpg" alt="GenXdev" width="50%"/>

<hr/>

### NAME

    GenXdev.Helpers

### SYNOPSIS

    A Windows PowerShell module with helpers mostly used by other GenXdev modules
[![GenXdev.Helpers](https://img.shields.io/powershellgallery/v/GenXdev.Helpers.svg?style=flat-square&label=GenXdev.Helpers)](https://www.powershellgallery.com/packages/GenXdev.Helpers/) [![License](https://img.shields.io/github/license/genXdev/GenXdev.Queries?style=flat-square)](./LICENSE)

## APACHE 2.0 License

````text
Copyright (c) 2025 René Vaessen / GenXdev

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

````

### DEPENDENCIES
[![WinOS - Windows-10 or later](https://img.shields.io/badge/WinOS-Windows--10--10.0.19041--SP0-brightgreen)](https://www.microsoft.com/en-us/windows/get-windows-10) [![GenXdev.FileSystem](https://img.shields.io/powershellgallery/v/GenXdev.FileSystem.svg?style=flat-square&label=GenXdev.FileSystem)](https://www.powershellgallery.com/packages/GenXdev.FileSystem/)

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
| [EnsureGenXdev](#ensuregenxdev) | &nbsp; | &nbsp; |
| [EnsureNuGetAssembly](#ensurenugetassembly) | &nbsp; | Downloads and loads .NET assemblies from NuGet packages based on package key or ID. |
| [Get-DefaultWebLanguage](#get-defaultweblanguage) | &nbsp; | Gets the default web language key based on the system's current language settings. |
| [Get-GenXDevCmdlet](#get-genxdevcmdlet) | gcmds | Retrieves and lists all GenXdev cmdlets and their details. |
| [Get-ImageGeolocation](#get-imagegeolocation) | &nbsp; | Extracts geolocation data from an image file. |
| [Get-ImageMetadata](#get-imagemetadata) | &nbsp; | &nbsp; |
| [Get-WebLanguageDictionary](#get-weblanguagedictionary) | &nbsp; | Returns a reversed dictionary for all languages supported by Google Search |
| [GetSpeechToText](#getspeechtotext) | &nbsp; | Converts audio files to text using OpenAI's Whisper speech recognition model. |
| [Import-GenXdevModules](#import-genxdevmodules) | reloadgenxdev | Imports all GenXdev PowerShell modules into the global scope. |
| [Initialize-SearchPaths](#initialize-searchpaths) | &nbsp; | Initializes and configures system search paths for package management. |
| [Invoke-OnEachGenXdevModule](#invoke-oneachgenxdevmodule) | foreach-genxdev-module-do | Executes a script block on each GenXdev module in the workspace. |
| [Out-Serial](#out-serial) | &nbsp; | Sends a string to a serial port |
| [ReceiveRealTimeSpeechToText](#receiverealtimespeechtotext) | &nbsp; | &nbsp; |
| [resetdefaultmonitor](#resetdefaultmonitor) | &nbsp; | &nbsp; |
| [SecondScreen](#secondscreen) | &nbsp; | Sets default second-monitor configuration. |
| [Show-GenXDevCmdlet](#show-genxdevcmdlet) | cmds | Displays GenXdev PowerShell modules with their cmdlets and aliases. |
| [Show-Verb](#show-verb) | showverbs | Shows a short alphabetical list of all PowerShell verbs. |
| [SideBySide](#sidebyside) | &nbsp; | Sets default side-by-side configuration. |

### GenXdev.Helpers.Physics
| Command | Aliases | Description |
| :--- | :--- | :--- |
| [Convert-PhysicsUnit](#convert-physicsunit) | &nbsp; | Converts a value from one physics unit to another within the same category. |
| [Get-ApparentSizeAtArmLength](#get-apparentsizeatarmlength) | &nbsp; | Calculates the apparent size of an object at arm's length. |
| [Get-AtEyeLengthSizeInMM](#get-ateyelengthsizeinmm) | &nbsp; | Calculates the apparent size in mm of an object at arm's length, based on its actual size and distance. |
| [Get-BuoyantForceByDisplacedVolumeAndDensity](#get-buoyantforcebydisplacedvolumeanddensity) | &nbsp; | Calculates buoyant force. |
| [Get-CentripetalAccelerationByVelocityAndRadius](#get-centripetalaccelerationbyvelocityandradius) | &nbsp; | Calculates centripetal acceleration. |
| [Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed](#get-dopplerfrequencyshiftbysourcespeedandobserverspeed) | &nbsp; | Calculates Doppler shifted frequency. |
| [Get-DragForceByVelocityDensityAreaAndCoefficient](#get-dragforcebyvelocitydensityareaandcoefficient) | &nbsp; | Calculates drag force. |
| [Get-EscapeVelocityByMassAndRadius](#get-escapevelocitybymassandradius) | &nbsp; | Calculates escape velocity. |
| [Get-FreeFallDistance](#get-freefalldistance) | &nbsp; | Calculates the distance fallen during free fall for a given time duration. |
| [Get-FreeFallHeight](#get-freefallheight) | &nbsp; | Calculates the height fallen during free fall for a given time duration. |
| [Get-FreeFallTime](#get-freefalltime) | &nbsp; | Calculates the time required for an object to fall a given height during free fall. |
| [Get-ImpactVelocityByHeightAndGravity](#get-impactvelocitybyheightandgravity) | &nbsp; | Calculates impact velocity from height. |
| [Get-KineticEnergyByMassAndVelocity](#get-kineticenergybymassandvelocity) | &nbsp; | Calculates kinetic energy. |
| [Get-LightTravelTimeByDistance](#get-lighttraveltimebydistance) | &nbsp; | Calculates time for light to travel a distance. |
| [Get-MagnificationByObjectDistanceAndImageDistance](#get-magnificationbyobjectdistanceandimagedistance) | &nbsp; | Calculates magnification for a lens. |
| [Get-MomentumByMassAndVelocity](#get-momentumbymassandvelocity) | &nbsp; | Calculates linear momentum. |
| [Get-OrbitalVelocityByRadiusAndMass](#get-orbitalvelocitybyradiusandmass) | &nbsp; | Calculates orbital velocity. |
| [Get-PotentialEnergyByMassHeightAndGravity](#get-potentialenergybymassheightandgravity) | &nbsp; | Calculates gravitational potential energy. |
| [Get-ProjectileRangeByInitialSpeedAndAngle](#get-projectilerangebyinitialspeedandangle) | &nbsp; | Calculates the range of a projectile. |
| [Get-RefractionAngleByIncidentAngleAndIndices](#get-refractionanglebyincidentangleandindices) | &nbsp; | Calculates refraction angle using Snell's law. |
| [Get-ResonantFrequencyByLengthAndSpeed](#get-resonantfrequencybylengthandspeed) | &nbsp; | Calculates resonant frequency for a closed pipe. |
| [Get-SoundTravelDistanceByTime](#get-soundtraveldistancebytime) | &nbsp; | Calculates the distance sound travels in a given time. |
| [Get-TerminalVelocityByMassGravityDensityAndArea](#get-terminalvelocitybymassgravitydensityandarea) | &nbsp; | Calculates terminal velocity. |
| [Get-TimeOfFlightByInitialVelocityAndAngle](#get-timeofflightbyinitialvelocityandangle) | &nbsp; | Calculates the time of flight for a projectile. |
| [Get-WaveSpeedByFrequencyAndWavelength](#get-wavespeedbyfrequencyandwavelength) | &nbsp; | Calculates wave speed. |

<br/><hr/><br/>


# Cmdlets

&nbsp;<hr/>
###	GenXdev.Helpers<hr/> 

##	alignScript 
```PowerShell 

   alignScript  
``` 

### SYNOPSIS 
    Returns a string (with altered indentation) of a provided scriptblock string  

### SYNTAX 
```PowerShell 
alignScript [[-script] <String>] [[-spaces] <Int32>] [<CommonParameters>] 
``` 

### DESCRIPTION 
    Changes the indentation of a scriptblock string while respecting the original code-block identations  

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -script <String>  
        The scriptblock string  
        Required?                    false  
        Position?                    1  
        Default value                  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -spaces <Int32>  
        The minimum number of spaces for each line  
        Required?                    false  
        Position?                    2  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	EnsureGenXdev 
```PowerShell 

   EnsureGenXdev  
``` 

### SYNTAX 
```PowerShell 
EnsureGenXdev [-Force] [-DownloadLMStudioModels] [-DownloadAllNugetPackages] [-ForceConsent] [-ConsentToThirdPartySoftwareInstallation] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ConsentToThirdPartySoftwareInstallation  
        Automatically consent to third-party software installation and set persistent flag for all packages.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DownloadAllNugetPackages  
        Downloads and loads all NuGet packages defined in the packages.json manifest file  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DownloadLMStudioModels  
        Downloads and initializes LMStudio models for various AI query types  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Force  
        Forces the execution of ensure operations even if they appear to be already completed  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ForceConsent  
        Force a consent prompt even if preference is set for third-party software installation.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	EnsureNuGetAssembly 
```PowerShell 

   EnsureNuGetAssembly  
``` 

### SYNTAX 
```PowerShell 
EnsureNuGetAssembly [-PackageKey] <string> [-ManifestPath <string>] [-Version <string>] [-TypeName <string>] [-ForceLatest] [-Destination <string>] [-Description <string>] [-Publisher <string>] [-ForceConsent] [-ConsentToThirdPartySoftwareInstallation] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ConsentToThirdPartySoftwareInstallation  
        Automatically consent to third-party software installation and set persistent flag.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Description <string>  
        Optional description of the software and its purpose for consent.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Destination <string>  
        Custom install destination; defaults to local persistent or global cache.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ForceConsent  
        Force a prompt even if preference is set for consent.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ForceLatest  
        Fallback to latest if exact version fails.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ManifestPath <string>  
        Path to packages.json; defaults to module root if omitted.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -PackageKey <string>  
        Package key from packages.json or direct NuGet PackageId.  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Publisher <string>  
        Optional publisher or vendor of the software for consent.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TypeName <string>  
        TypeName to verify loading.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Version <string>  
        Specific version; if omitted, use highest from JSON or latest.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Get-GenXDevCmdlet 
```PowerShell 

   Get-GenXDevCmdlet                    --> gcmds  
``` 

### SYNTAX 
```PowerShell 
Get-GenXDevCmdlet [[-CmdletName] <string>]
    [[-DefinitionMatches] <string>] [[-ModuleName]
    <string[]>] [-NoLocal] [-OnlyPublished] [-FromScripts]
    [-IncludeScripts] [-OnlyReturnModuleNames] [-ExactMatch]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -CmdletName <string>  
        Search pattern to filter cmdlets  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       true (ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Filter, CmdLet, Cmd, FunctionName, Name  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -DefinitionMatches <string>  
        Regular expression to match cmdlet definitions  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ExactMatch  
        Perform exact matching instead of wildcard matching  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -FromScripts  
        Search in script files instead of modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncludeScripts  
        Includes the scripts directory in addition to regular modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           ModuleName  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModuleName <string[]>  
        GenXdev module names to search  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Module, BaseModuleName, SubModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -NoLocal  
        Skip searching in local module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyPublished  
        Only search in published module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyReturnModuleNames  
        Only return unique module names  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Get-ImageMetadata 
```PowerShell 

   Get-ImageMetadata  
``` 

### SYNTAX 
```PowerShell 
Get-ImageMetadata [-ImagePath] <string> [-ForceConsent]
    [-ConsentToThirdPartySoftwareInstallation]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ConsentToThirdPartySoftwareInstallation  
        Automatically consent to third-party software installation and set persistent flag for ImageSharp packages.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ForceConsent  
        Force a consent prompt even if preference is set for ImageSharp package installation.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ImagePath <string>  
        Path to the image file to analyze  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Import-GenXdevModules 
```PowerShell 

   Import-GenXdevModules                --> reloadgenxdev  
``` 

### SYNOPSIS 
    Imports all GenXdev PowerShell modules into the global scope.  

### SYNTAX 
```PowerShell 
Import-GenXdevModules [-DebugFailedModuleDefinitions]
    [<CommonParameters>] 
``` 

### DESCRIPTION 
    Scans the parent directory for GenXdev modules and imports each one into the  
    global scope. Uses location stack management to preserve the working directory  
    and provides visual feedback for successful and failed imports. Tracks function  
    count changes during the import process.  

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -DebugFailedModuleDefinitions [<SwitchParameter>]  
        When enabled, provides detailed debug output for modules that fail to import.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Invoke-OnEachGenXdevModule 
```PowerShell 

   Invoke-OnEachGenXdevModule           --> foreach-genxdev-module-do  
``` 

### SYNTAX 
```PowerShell 
Invoke-OnEachGenXdevModule [-Script] <scriptblock>
    [[-ModuleName] <string[]>] [-NoLocal] [-OnlyPublished]
    [-FromScripts] [-IncludeScripts]
    [-IncludeGenXdevMainModule] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -FromScripts  
        Process scripts directory instead of module directories  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncludeGenXdevMainModule  
        Includes the main GenXdev module in addition to sub-modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncludeScripts  
        Includes the scripts directory in addition to regular modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModuleName <string[]>  
        Filter to apply to module names  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      Module, BaseModuleName, SubModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -NoLocal  
        Excludes local development modules from processing  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyPublished  
        Includes only published modules that have LICENSE and README.md files  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Script <scriptblock>  
        The script block to execute for each GenXdev module  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      ScriptBlock  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Out-Serial 
```PowerShell 

   Out-Serial  
``` 

### SYNOPSIS 
    Sends a string to a serial port  

### SYNTAX 
```PowerShell 
Out-Serial [[-Portname] <String>] [[-BaudRate] <Int32>]
    [[-MaxBytesToRead] <UInt32>] [[-ReadTimeout] <UInt32>]
    [[-WriteTimeout] <UInt32>] [[-Parity] <String>]
    [[-DataBits] <Int32>] [[-StopBits] <String>] [-Text]
    <Object> [-AddCRLinefeeds] [<CommonParameters>] 
``` 

### DESCRIPTION 
    Allows you to send a string to a serial communication port  

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -Portname <String>  
        The port to use (for example, COM1).  
        Required?                    false  
        Position?                    1  
        Default value                COM5  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -BaudRate <Int32>  
        The baud rate.  
        Required?                    false  
        Position?                    2  
        Default value                9600  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxBytesToRead <UInt32>  
        Limits the nr of bytes to read.  
        Required?                    false  
        Position?                    3  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -ReadTimeout <UInt32>  
        Enables reading with a specified timeout in milliseconds.  
        Required?                    false  
        Position?                    4  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -WriteTimeout <UInt32>  
        Enables writing with a specified timeout in milliseconds.  
        Required?                    false  
        Position?                    5  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -Parity <String>  
        One of the System.IO.Ports.SerialPort.Parity values.  
        Required?                    false  
        Position?                    6  
        Default value                None  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -DataBits <Int32>  
        The data bits value.  
        Required?                    false  
        Position?                    7  
        Default value                8  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -StopBits <String>  
        One of the System.IO.Ports.SerialPort.StopBits values.  
        Required?                    false  
        Position?                    8  
        Default value                One  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -Text <Object>  
        Text to sent to serial port.  
        Required?                    true  
        Position?                    9  
        Default value                  
        Accept pipeline input?       true (ByValue)  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -AddCRLinefeeds [<SwitchParameter>]  
        Add linefeeds to input text parts.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	resetdefaultmonitor 
```PowerShell 

   resetdefaultmonitor  
``` 

### SYNTAX 
```PowerShell 
resetdefaultmonitor [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	SecondScreen 
```PowerShell 

   secondscreen  
``` 

### SYNTAX 
```PowerShell 
secondscreen [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Show-GenXDevCmdlet 
```PowerShell 

   Show-GenXdevCmdlet                   --> cmds  
``` 

### SYNTAX 
```PowerShell 
Show-GenXdevCmdlet [[-CmdletName] <string>]
    [[-DefinitionMatches] <string>] [[-ModuleName]
    <string[]>] [-NoLocal] [-OnlyPublished] [-FromScripts]
    [-IncludeScripts] [-OnlyReturnModuleNames] [-ExactMatch]
    [-Online] [-OnlyAliases] [-ShowTable] [-PassThru]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -CmdletName <string>  
        Search pattern to filter cmdlets  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       true (ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Filter, CmdLet, Cmd, FunctionName, Name  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -DefinitionMatches <string>  
        Regular expression to match cmdlet definitions  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ExactMatch  
        Perform exact matching instead of wildcard matching  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -FromScripts  
        Search in script files instead of modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncludeScripts  
        Includes the scripts directory in addition to regular modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           ModuleName  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModuleName <string[]>  
        GenXdev module names to search  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Module, BaseModuleName, SubModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -NoLocal  
        Skip searching in local module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Online  
        Open GitHub documentation instead of console output  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyAliases  
        When specified displays only aliases of cmdlets  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      aliases, nonboring, notlame, handyonces  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyPublished  
        Only search in published module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyReturnModuleNames  
        Only return unique module names  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -PassThru  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ShowTable  
        Display results in table format  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      table, grid  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Show-Verb 
```PowerShell 

   Show-Verb                            --> showverbs  
``` 

### SYNTAX 
```PowerShell 
Show-Verb [[-Verb] <string[]>] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -Verb <string[]>  
        One or more verb patterns to filter (supports wildcards)  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	SideBySide 
```PowerShell 

   sidebyside  
``` 

### SYNTAX 
```PowerShell 
sidebyside [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers.Physics<hr/> 

##	Convert-PhysicsUnit 
```PowerShell 

   Convert-PhysicsUnit  
``` 

### SYNTAX 
```PowerShell 
Convert-PhysicsUnit [-Value] <double> [-FromUnit] <string>
    [-ToUnit] <string> [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -FromUnit <string>  
        The unit of the input value  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ToUnit <string>  
        The desired output unit  
        Required?                    true  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Value <double>  
        The numerical value to convert  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers<hr/> 

##	ConvertTo-HashTable 
```PowerShell 

   ConvertTo-HashTable  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-DefaultWebLanguage 
```PowerShell 

   Get-DefaultWebLanguage  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-ImageGeolocation 
```PowerShell 

   Get-ImageGeolocation  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-WebLanguageDictionary 
```PowerShell 

   Get-WebLanguageDictionary  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

<br/><hr/><br/>
 

##	Initialize-SearchPaths 
```PowerShell 

   Initialize-SearchPaths  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

<br/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers.Physics<hr/> 

##	Get-ApparentSizeAtArmLength 
```PowerShell 

   Get-ApparentSizeAtArmLength  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-AtEyeLengthSizeInMM 
```PowerShell 

   Get-AtEyeLengthSizeInMM  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-BuoyantForceByDisplacedVolumeAndDensity 
```PowerShell 

   Get-BuoyantForceByDisplacedVolumeAndDensity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-CentripetalAccelerationByVelocityAndRadius 
```PowerShell 

   Get-CentripetalAccelerationByVelocityAndRadius  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed 
```PowerShell 

   Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-DragForceByVelocityDensityAreaAndCoefficient 
```PowerShell 

   Get-DragForceByVelocityDensityAreaAndCoefficient  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-EscapeVelocityByMassAndRadius 
```PowerShell 

   Get-EscapeVelocityByMassAndRadius  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-FreeFallDistance 
```PowerShell 

   Get-FreeFallDistance  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-FreeFallHeight 
```PowerShell 

   Get-FreeFallHeight  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-FreeFallTime 
```PowerShell 

   Get-FreeFallTime  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-ImpactVelocityByHeightAndGravity 
```PowerShell 

   Get-ImpactVelocityByHeightAndGravity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-KineticEnergyByMassAndVelocity 
```PowerShell 

   Get-KineticEnergyByMassAndVelocity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-LightTravelTimeByDistance 
```PowerShell 

   Get-LightTravelTimeByDistance  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-MagnificationByObjectDistanceAndImageDistance 
```PowerShell 

   Get-MagnificationByObjectDistanceAndImageDistance  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-MomentumByMassAndVelocity 
```PowerShell 

   Get-MomentumByMassAndVelocity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-OrbitalVelocityByRadiusAndMass 
```PowerShell 

   Get-OrbitalVelocityByRadiusAndMass  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-PotentialEnergyByMassHeightAndGravity 
```PowerShell 

   Get-PotentialEnergyByMassHeightAndGravity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-ProjectileRangeByInitialSpeedAndAngle 
```PowerShell 

   Get-ProjectileRangeByInitialSpeedAndAngle  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-RefractionAngleByIncidentAngleAndIndices 
```PowerShell 

   Get-RefractionAngleByIncidentAngleAndIndices  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-ResonantFrequencyByLengthAndSpeed 
```PowerShell 

   Get-ResonantFrequencyByLengthAndSpeed  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-SoundTravelDistanceByTime 
```PowerShell 

   Get-SoundTravelDistanceByTime  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-TerminalVelocityByMassGravityDensityAndArea 
```PowerShell 

   Get-TerminalVelocityByMassGravityDensityAndArea  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-TimeOfFlightByInitialVelocityAndAngle 
```PowerShell 

   Get-TimeOfFlightByInitialVelocityAndAngle  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-WaveSpeedByFrequencyAndWavelength 
```PowerShell 

   Get-WaveSpeedByFrequencyAndWavelength  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers<hr/> 

##	alignScript 
```PowerShell 

   alignScript  
``` 

### SYNOPSIS 
    Returns a string (with altered indentation) of a provided scriptblock string  

### SYNTAX 
```PowerShell 
alignScript [[-script] <String>] [[-spaces] <Int32>] [<CommonParameters>] 
``` 

### DESCRIPTION 
    Changes the indentation of a scriptblock string while respecting the original code-block identations  

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -script <String>  
        The scriptblock string  
        Required?                    false  
        Position?                    1  
        Default value                  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -spaces <Int32>  
        The minimum number of spaces for each line  
        Required?                    false  
        Position?                    2  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	EnsureGenXdev 
```PowerShell 

   EnsureGenXdev  
``` 

### SYNTAX 
```PowerShell 
EnsureGenXdev [-Force] [-DownloadLMStudioModels] [-DownloadAllNugetPackages] [-ForceConsent] [-ConsentToThirdPartySoftwareInstallation] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ConsentToThirdPartySoftwareInstallation  
        Automatically consent to third-party software installation and set persistent flag for all packages.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DownloadAllNugetPackages  
        Downloads and loads all NuGet packages defined in the packages.json manifest file  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DownloadLMStudioModels  
        Downloads and initializes LMStudio models for various AI query types  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Force  
        Forces the execution of ensure operations even if they appear to be already completed  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ForceConsent  
        Force a consent prompt even if preference is set for third-party software installation.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	EnsureNuGetAssembly 
```PowerShell 

   EnsureNuGetAssembly  
``` 

### SYNTAX 
```PowerShell 
EnsureNuGetAssembly [-PackageKey] <string> [-ManifestPath <string>] [-Version <string>] [-TypeName <string>] [-ForceLatest] [-Destination <string>] [-Description <string>] [-Publisher <string>] [-ForceConsent] [-ConsentToThirdPartySoftwareInstallation] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ConsentToThirdPartySoftwareInstallation  
        Automatically consent to third-party software installation and set persistent flag.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Description <string>  
        Optional description of the software and its purpose for consent.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Destination <string>  
        Custom install destination; defaults to local persistent or global cache.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ForceConsent  
        Force a prompt even if preference is set for consent.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ForceLatest  
        Fallback to latest if exact version fails.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ManifestPath <string>  
        Path to packages.json; defaults to module root if omitted.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -PackageKey <string>  
        Package key from packages.json or direct NuGet PackageId.  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Publisher <string>  
        Optional publisher or vendor of the software for consent.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TypeName <string>  
        TypeName to verify loading.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Version <string>  
        Specific version; if omitted, use highest from JSON or latest.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Get-GenXDevCmdlet 
```PowerShell 

   Get-GenXDevCmdlet                    --> gcmds  
``` 

### SYNTAX 
```PowerShell 
Get-GenXDevCmdlet [[-CmdletName] <string>]
    [[-DefinitionMatches] <string>] [[-ModuleName]
    <string[]>] [-NoLocal] [-OnlyPublished] [-FromScripts]
    [-IncludeScripts] [-OnlyReturnModuleNames] [-ExactMatch]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -CmdletName <string>  
        Search pattern to filter cmdlets  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       true (ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Filter, CmdLet, Cmd, FunctionName, Name  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -DefinitionMatches <string>  
        Regular expression to match cmdlet definitions  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ExactMatch  
        Perform exact matching instead of wildcard matching  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -FromScripts  
        Search in script files instead of modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncludeScripts  
        Includes the scripts directory in addition to regular modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           ModuleName  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModuleName <string[]>  
        GenXdev module names to search  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Module, BaseModuleName, SubModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -NoLocal  
        Skip searching in local module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyPublished  
        Only search in published module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyReturnModuleNames  
        Only return unique module names  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Get-ImageMetadata 
```PowerShell 

   Get-ImageMetadata  
``` 

### SYNTAX 
```PowerShell 
Get-ImageMetadata [-ImagePath] <string> [-ForceConsent]
    [-ConsentToThirdPartySoftwareInstallation]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ConsentToThirdPartySoftwareInstallation  
        Automatically consent to third-party software installation and set persistent flag for ImageSharp packages.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ForceConsent  
        Force a consent prompt even if preference is set for ImageSharp package installation.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ImagePath <string>  
        Path to the image file to analyze  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Import-GenXdevModules 
```PowerShell 

   Import-GenXdevModules                --> reloadgenxdev  
``` 

### SYNOPSIS 
    Imports all GenXdev PowerShell modules into the global scope.  

### SYNTAX 
```PowerShell 
Import-GenXdevModules [-DebugFailedModuleDefinitions]
    [<CommonParameters>] 
``` 

### DESCRIPTION 
    Scans the parent directory for GenXdev modules and imports each one into the  
    global scope. Uses location stack management to preserve the working directory  
    and provides visual feedback for successful and failed imports. Tracks function  
    count changes during the import process.  

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -DebugFailedModuleDefinitions [<SwitchParameter>]  
        When enabled, provides detailed debug output for modules that fail to import.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Invoke-OnEachGenXdevModule 
```PowerShell 

   Invoke-OnEachGenXdevModule           --> foreach-genxdev-module-do  
``` 

### SYNTAX 
```PowerShell 
Invoke-OnEachGenXdevModule [-Script] <scriptblock>
    [[-ModuleName] <string[]>] [-NoLocal] [-OnlyPublished]
    [-FromScripts] [-IncludeScripts]
    [-IncludeGenXdevMainModule] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -FromScripts  
        Process scripts directory instead of module directories  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncludeGenXdevMainModule  
        Includes the main GenXdev module in addition to sub-modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncludeScripts  
        Includes the scripts directory in addition to regular modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModuleName <string[]>  
        Filter to apply to module names  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      Module, BaseModuleName, SubModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -NoLocal  
        Excludes local development modules from processing  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyPublished  
        Includes only published modules that have LICENSE and README.md files  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Script <scriptblock>  
        The script block to execute for each GenXdev module  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      ScriptBlock  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Out-Serial 
```PowerShell 

   Out-Serial  
``` 

### SYNOPSIS 
    Sends a string to a serial port  

### SYNTAX 
```PowerShell 
Out-Serial [[-Portname] <String>] [[-BaudRate] <Int32>]
    [[-MaxBytesToRead] <UInt32>] [[-ReadTimeout] <UInt32>]
    [[-WriteTimeout] <UInt32>] [[-Parity] <String>]
    [[-DataBits] <Int32>] [[-StopBits] <String>] [-Text]
    <Object> [-AddCRLinefeeds] [<CommonParameters>] 
``` 

### DESCRIPTION 
    Allows you to send a string to a serial communication port  

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -Portname <String>  
        The port to use (for example, COM1).  
        Required?                    false  
        Position?                    1  
        Default value                COM5  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -BaudRate <Int32>  
        The baud rate.  
        Required?                    false  
        Position?                    2  
        Default value                9600  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxBytesToRead <UInt32>  
        Limits the nr of bytes to read.  
        Required?                    false  
        Position?                    3  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -ReadTimeout <UInt32>  
        Enables reading with a specified timeout in milliseconds.  
        Required?                    false  
        Position?                    4  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -WriteTimeout <UInt32>  
        Enables writing with a specified timeout in milliseconds.  
        Required?                    false  
        Position?                    5  
        Default value                0  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -Parity <String>  
        One of the System.IO.Ports.SerialPort.Parity values.  
        Required?                    false  
        Position?                    6  
        Default value                None  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -DataBits <Int32>  
        The data bits value.  
        Required?                    false  
        Position?                    7  
        Default value                8  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -StopBits <String>  
        One of the System.IO.Ports.SerialPort.StopBits values.  
        Required?                    false  
        Position?                    8  
        Default value                One  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -Text <Object>  
        Text to sent to serial port.  
        Required?                    true  
        Position?                    9  
        Default value                  
        Accept pipeline input?       true (ByValue)  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    -AddCRLinefeeds [<SwitchParameter>]  
        Add linefeeds to input text parts.  
        Required?                    false  
        Position?                    named  
        Default value                False  
        Accept pipeline input?       false  
        Aliases                        
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	resetdefaultmonitor 
```PowerShell 

   resetdefaultmonitor  
``` 

### SYNTAX 
```PowerShell 
resetdefaultmonitor [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	SecondScreen 
```PowerShell 

   secondscreen  
``` 

### SYNTAX 
```PowerShell 
secondscreen [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Show-GenXDevCmdlet 
```PowerShell 

   Show-GenXdevCmdlet                   --> cmds  
``` 

### SYNTAX 
```PowerShell 
Show-GenXdevCmdlet [[-CmdletName] <string>]
    [[-DefinitionMatches] <string>] [[-ModuleName]
    <string[]>] [-NoLocal] [-OnlyPublished] [-FromScripts]
    [-IncludeScripts] [-OnlyReturnModuleNames] [-ExactMatch]
    [-Online] [-OnlyAliases] [-ShowTable] [-PassThru]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -CmdletName <string>  
        Search pattern to filter cmdlets  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       true (ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Filter, CmdLet, Cmd, FunctionName, Name  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -DefinitionMatches <string>  
        Regular expression to match cmdlet definitions  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ExactMatch  
        Perform exact matching instead of wildcard matching  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -FromScripts  
        Search in script files instead of modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncludeScripts  
        Includes the scripts directory in addition to regular modules  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           ModuleName  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModuleName <string[]>  
        GenXdev module names to search  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      Module, BaseModuleName, SubModuleName  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    -NoLocal  
        Skip searching in local module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Online  
        Open GitHub documentation instead of console output  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyAliases  
        When specified displays only aliases of cmdlets  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      aliases, nonboring, notlame, handyonces  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyPublished  
        Only search in published module paths  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OnlyReturnModuleNames  
        Only return unique module names  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -PassThru  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ShowTable  
        Display results in table format  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      table, grid  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Show-Verb 
```PowerShell 

   Show-Verb                            --> showverbs  
``` 

### SYNTAX 
```PowerShell 
Show-Verb [[-Verb] <string[]>] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -Verb <string[]>  
        One or more verb patterns to filter (supports wildcards)  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       true (ByValue, ByPropertyName)  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  true  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	SideBySide 
```PowerShell 

   sidebyside  
``` 

### SYNTAX 
```PowerShell 
sidebyside [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	ConvertTo-HashTable 
```PowerShell 

   ConvertTo-HashTable  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-DefaultWebLanguage 
```PowerShell 

   Get-DefaultWebLanguage  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-ImageGeolocation 
```PowerShell 

   Get-ImageGeolocation  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-WebLanguageDictionary 
```PowerShell 

   Get-WebLanguageDictionary  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

<br/><hr/><br/>
 

##	Initialize-SearchPaths 
```PowerShell 

   Initialize-SearchPaths  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

<br/><hr/><br/>
 

&nbsp;<hr/>
###	GenXdev.Helpers.Physics<hr/> 

##	Convert-PhysicsUnit 
```PowerShell 

   Convert-PhysicsUnit  
``` 

### SYNTAX 
```PowerShell 
Convert-PhysicsUnit [-Value] <double> [-FromUnit] <string>
    [-ToUnit] <string> [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -FromUnit <string>  
        The unit of the input value  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ToUnit <string>  
        The desired output unit  
        Required?                    true  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Value <double>  
        The numerical value to convert  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    <CommonParameters>  
        This cmdlet supports the common parameters: Verbose, Debug,  
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,  
        OutBuffer, PipelineVariable, and OutVariable. For more information, see  
        about_CommonParameters     (https://go.microsoft.com/fwlink/?LinkID=113216).   
``` 

<br/><hr/><br/>
 

##	Get-ApparentSizeAtArmLength 
```PowerShell 

   Get-ApparentSizeAtArmLength  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-AtEyeLengthSizeInMM 
```PowerShell 

   Get-AtEyeLengthSizeInMM  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-BuoyantForceByDisplacedVolumeAndDensity 
```PowerShell 

   Get-BuoyantForceByDisplacedVolumeAndDensity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-CentripetalAccelerationByVelocityAndRadius 
```PowerShell 

   Get-CentripetalAccelerationByVelocityAndRadius  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed 
```PowerShell 

   Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-DragForceByVelocityDensityAreaAndCoefficient 
```PowerShell 

   Get-DragForceByVelocityDensityAreaAndCoefficient  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-EscapeVelocityByMassAndRadius 
```PowerShell 

   Get-EscapeVelocityByMassAndRadius  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-FreeFallDistance 
```PowerShell 

   Get-FreeFallDistance  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-FreeFallHeight 
```PowerShell 

   Get-FreeFallHeight  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-FreeFallTime 
```PowerShell 

   Get-FreeFallTime  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-ImpactVelocityByHeightAndGravity 
```PowerShell 

   Get-ImpactVelocityByHeightAndGravity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-KineticEnergyByMassAndVelocity 
```PowerShell 

   Get-KineticEnergyByMassAndVelocity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-LightTravelTimeByDistance 
```PowerShell 

   Get-LightTravelTimeByDistance  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-MagnificationByObjectDistanceAndImageDistance 
```PowerShell 

   Get-MagnificationByObjectDistanceAndImageDistance  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-MomentumByMassAndVelocity 
```PowerShell 

   Get-MomentumByMassAndVelocity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-OrbitalVelocityByRadiusAndMass 
```PowerShell 

   Get-OrbitalVelocityByRadiusAndMass  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-PotentialEnergyByMassHeightAndGravity 
```PowerShell 

   Get-PotentialEnergyByMassHeightAndGravity  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-ProjectileRangeByInitialSpeedAndAngle 
```PowerShell 

   Get-ProjectileRangeByInitialSpeedAndAngle  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-RefractionAngleByIncidentAngleAndIndices 
```PowerShell 

   Get-RefractionAngleByIncidentAngleAndIndices  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-ResonantFrequencyByLengthAndSpeed 
```PowerShell 

   Get-ResonantFrequencyByLengthAndSpeed  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-SoundTravelDistanceByTime 
```PowerShell 

   Get-SoundTravelDistanceByTime  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-TerminalVelocityByMassGravityDensityAndArea 
```PowerShell 

   Get-TerminalVelocityByMassGravityDensityAndArea  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-TimeOfFlightByInitialVelocityAndAngle 
```PowerShell 

   Get-TimeOfFlightByInitialVelocityAndAngle  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
 

##	Get-WaveSpeedByFrequencyAndWavelength 
```PowerShell 

   Get-WaveSpeedByFrequencyAndWavelength  
``` 

### SYNOPSIS 

### SYNTAX 
```PowerShell 
 
``` 

### DESCRIPTION 

### PARAMETERS 
```yaml 
 
``` 

<br/><hr/><br/>
