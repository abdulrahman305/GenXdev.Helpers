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
| [Get-AudioDeviceNames](#get-audiodevicenames) | &nbsp; | Retrieves the names of available audio devices for microphone or desktop audio capture. |
| [Get-DefaultWebLanguage](#get-defaultweblanguage) | &nbsp; | Gets the default web language key based on the system's current language settings. |
| [Get-GenXDevCmdlet](#get-genxdevcmdlet) | gcmds | Retrieves and lists all GenXdev cmdlets and their details. |
| [Get-ImageGeolocation](#get-imagegeolocation) | &nbsp; | Extracts geolocation data from an image file. |
| [Get-ImageMetadata](#get-imagemetadata) | &nbsp; | &nbsp; |
| [Get-SpeechToText](#get-speechtotext) | &nbsp; | Converts audio files to text using OpenAI's Whisper speech recognition model. |
| [Get-WebLanguageDictionary](#get-weblanguagedictionary) | &nbsp; | Returns a reversed dictionary for all languages supported by Google Search |
| [Import-GenXdevModules](#import-genxdevmodules) | reloadgenxdev | Imports all GenXdev PowerShell modules into the global scope. |
| [Initialize-SearchPaths](#initialize-searchpaths) | &nbsp; | Initializes and configures system search paths for package management. |
| [Invoke-OnEachGenXdevModule](#invoke-oneachgenxdevmodule) | foreach-genxdev-module-do | Executes a script block on each GenXdev module in the workspace. |
| [Out-Serial](#out-serial) | &nbsp; | Sends a string to a serial port |
| [Receive-RealTimeSpeechToText](#receive-realtimespeechtotext) | &nbsp; | Converts real-time audio input to text using Whisper AI model. |
| [Remove-JSONComments](#remove-jsoncomments) | &nbsp; | Removes comments from JSON content. |
| [resetdefaultmonitor](#resetdefaultmonitor) | &nbsp; | &nbsp; |
| [SecondScreen](#secondscreen) | &nbsp; | Sets default second-monitor configuration. |
| [Show-GenXDevCmdlet](#show-genxdevcmdlet) | cmds | Displays GenXdev PowerShell modules with their cmdlets and aliases. |
| [Show-Verb](#show-verb) | showverbs | Shows a short alphabetical list of all PowerShell verbs. |
| [SideBySide](#sidebyside) | &nbsp; | Sets default side-by-side configuration. |
| [Test-UnattendedMode](#test-unattendedmode) | &nbsp; | Detects if PowerShell is running in unattended/automated mode |

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

### SYNTAX 
```PowerShell 
ConvertTo-HashTable [-InputObject] <Object[]>
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -InputObject <Object[]>  
        The PSCustomObject to convert into a HashTable  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue)  
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
 

##	Get-AudioDeviceNames 
```PowerShell 

   Get-AudioDeviceNames  
``` 

### SYNTAX 
```PowerShell 
Get-AudioDeviceNames [-UseDesktopAudioCapture] [-Passthru]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -Passthru  
        Returns detailed device objects instead of just names  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -UseDesktopAudioCapture  
        Whether to list desktop audio capture devices instead of microphone devices  
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
 

##	Get-DefaultWebLanguage 
```PowerShell 

   Get-DefaultWebLanguage  
``` 

### SYNTAX 
```PowerShell 
Get-DefaultWebLanguage [<CommonParameters>] 
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
 

##	Get-ImageGeolocation 
```PowerShell 

   Get-ImageGeolocation  
``` 

### SYNTAX 
```PowerShell 
Get-ImageGeolocation [-ImagePath] <string>
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
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
 

##	Get-SpeechToText 
```PowerShell 

   Get-SpeechToText  
``` 

### SYNTAX 
```PowerShell 
Get-SpeechToText [-Input] <Object> [-ModelFileDirectoryPath
    <string>] [-LanguageIn <string>] [-CpuThreads <int>]
    [-Temperature <float>] [-TemperatureInc <float>]
    [-NoSpeechThreshold <float>] [-Prompt <string>]
    [-SuppressRegex <string>] [-TokenTimestampsSumThreshold
    <float>] [-MaxTokensPerSegment <int>] [-AudioContextSize
    <int>] [-MaxDuration <timespan>] [-Offset <timespan>]
    [-MaxLastTextTokens <int>] [-MaxSegmentLength <int>]
    [-MaxInitialTimestamp <timespan>] [-LengthPenalty
    <float>] [-EntropyThreshold <float>] [-LogProbThreshold
    <float>] [-ModelType {Tiny | TinyEn | Base | BaseEn |
    Small | SmallEn | Medium | MediumEn | LargeV1 | LargeV2
    | LargeV3 | LargeV3Turbo}] [-Passthru]
    [-WithTokenTimestamps] [-SplitOnWord] [-WithTranslate]
    [-WithProgress] [-DontSuppressBlank]
    [-SingleSegmentOnly] [-PrintSpecialTokens] [-NoContext]
    [-WithBeamSearchSamplingStrategy] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AudioContextSize <int>  
        Size of the audio context  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -CpuThreads <int>  
        Sets the output language  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DontSuppressBlank  
        Whether to NOT suppress blank lines  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -EntropyThreshold <float>  
        Entropy threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Input <Object>  
        Audio file path, FileInfo object, or any audio format supported by Whisper.  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue)  
        Parameter set name           (All)  
        Aliases                      WaveFile  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LanguageIn <string>  
        Sets the input language to detect, defaults to 'en'  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LengthPenalty <float>  
        Length penalty  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LogProbThreshold <float>  
        Log probability threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxDuration <timespan>  
        Maximum duration of the audio  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxInitialTimestamp <timespan>  
        Start timestamps at this moment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxLastTextTokens <int>  
        Maximum number of last text tokens  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxSegmentLength <int>  
        Maximum segment length  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxTokensPerSegment <int>  
        Maximum number of tokens per segment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModelFileDirectoryPath <string>  
        Path to the model file directory  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModelType <GgmlType>  
        Whisper model type to use, defaults to LargeV3Turbo  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -NoContext  
        Do not use context  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -NoSpeechThreshold <float>  
        No speech threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Offset <timespan>  
        Offset for the audio  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Passthru  
        Returns objects instead of strings  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -PrintSpecialTokens  
        Whether to print special tokens  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Prompt <string>  
        Prompt to use for the model  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SingleSegmentOnly  
        Whether to use single segment only  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SplitOnWord  
        Whether to split on word boundaries  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SuppressRegex <string>  
        Regex to suppress tokens from the output  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Temperature <float>  
        Temperature for speech detection  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TemperatureInc <float>  
        Temperature increment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TokenTimestampsSumThreshold <float>  
        Sum threshold for token timestamps, defaults to 0.5  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithBeamSearchSamplingStrategy  
        Use beam search sampling strategy  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithProgress  
        Whether to show progress  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithTokenTimestamps  
        Whether to include token timestamps  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithTranslate  
        Whether to translate the output  
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
 

##	Get-WebLanguageDictionary 
```PowerShell 

   Get-WebLanguageDictionary  
``` 

### SYNTAX 
```PowerShell 
Get-WebLanguageDictionary [<CommonParameters>] 
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
 

##	Initialize-SearchPaths 
```PowerShell 

   Initialize-SearchPaths  
``` 

### SYNTAX 
```PowerShell 
Initialize-SearchPaths [[-WorkspaceFolder] <string>]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -WorkspaceFolder <string>  
        The workspace folder path to use for node modules and PowerShell paths.  
        Required?                    false  
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
 

##	Receive-RealTimeSpeechToText 
```PowerShell 

   Receive-RealTimeSpeechToText  
``` 

### SYNTAX 
```PowerShell 
Receive-RealTimeSpeechToText [-ModelFileDirectoryPath
    <string>] [-UseDesktopAudioCapture]
    [-UseDesktopAndRecordingDevice] [-AudioDevice <string>]
    [-Passthru] [-WithTokenTimestamps]
    [-TokenTimestampsSumThreshold <float>] [-SplitOnWord]
    [-MaxTokensPerSegment <int>] [-IgnoreSilence]
    [-MaxDurationOfSilence <timespan>] [-SilenceThreshold
    <int>] [-LanguageIn <string>] [-CpuThreads <int>]
    [-Temperature <float>] [-TemperatureInc <float>]
    [-WithTranslate] [-Prompt <string>] [-SuppressRegex
    <string>] [-WithProgress] [-AudioContextSize <int>]
    [-DontSuppressBlank] [-MaxDuration <timespan>] [-Offset
    <timespan>] [-MaxLastTextTokens <int>]
    [-SingleSegmentOnly] [-PrintSpecialTokens]
    [-MaxSegmentLength <int>] [-MaxInitialTimestamp
    <timespan>] [-LengthPenalty <float>] [-EntropyThreshold
    <float>] [-LogProbThreshold <float>] [-NoSpeechThreshold
    <float>] [-NoContext] [-WithBeamSearchSamplingStrategy]
    [-ModelType {Tiny | TinyEn | Base | BaseEn | Small |
    SmallEn | Medium | MediumEn | LargeV1 | LargeV2 |
    LargeV3 | LargeV3Turbo}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AudioContextSize <int>  
        Size of the audio context  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -AudioDevice <string>  
        Use both desktop and recording device  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -CpuThreads <int>  
        Sets the output language  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DontSuppressBlank  
        Whether to NOT suppress blank lines  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -EntropyThreshold <float>  
        Entropy threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IgnoreSilence  
        Whether to ignore silence (will mess up timestamps)  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LanguageIn <string>  
        Sets the input language to detect, defaults to 'en'  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LengthPenalty <float>  
        Length penalty  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LogProbThreshold <float>  
        Log probability threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxDuration <timespan>  
        Maximum duration of the audio  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxDurationOfSilence <timespan>  
        Maximum duration of silence before automatically stopping recording  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxInitialTimestamp <timespan>  
        Start timestamps at this moment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxLastTextTokens <int>  
        Maximum number of last text tokens  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxSegmentLength <int>  
        Maximum segment length  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxTokensPerSegment <int>  
        Maximum number of tokens per segment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModelFileDirectoryPath <string>  
        Path to the model file  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModelType <GgmlType>  
        Whisper model type to use, defaults to Small  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -NoContext  
        Do not use context  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -NoSpeechThreshold <float>  
        No speech threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Offset <timespan>  
        Offset for the audio  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Passthru  
        Returns objects instead of strings  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -PrintSpecialTokens  
        Whether to print special tokens  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Prompt <string>  
        Prompt to use for the model  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SilenceThreshold <int>  
        Silence detect threshold (0..32767 defaults to 30)  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SingleSegmentOnly  
        Whether to use single segment only  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SplitOnWord  
        Whether to split on word boundaries  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SuppressRegex <string>  
        Regex to suppress tokens from the output  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Temperature <float>  
        Temperature for speech detection  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TemperatureInc <float>  
        Temperature increment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TokenTimestampsSumThreshold <float>  
        Sum threshold for token timestamps, defaults to 0.5  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -UseDesktopAndRecordingDevice  
        Whether to use both desktop audio capture and recording device simultaneously  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -UseDesktopAudioCapture  
        Whether to use desktop audio capture instead of microphone  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithBeamSearchSamplingStrategy  
        Use beam search sampling strategy  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithProgress  
        Whether to show progress  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithTokenTimestamps  
        Whether to include token timestamps  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithTranslate  
        Whether to translate the output  
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
 

##	Remove-JSONComments 
```PowerShell 

   Remove-JSONComments  
``` 

### SYNTAX 
```PowerShell 
Remove-JSONComments [-Json] <string[]> [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -Json <string[]>  
        JSON content to process as string array  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue)  
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
 

##	Test-UnattendedMode 
```PowerShell 

   Test-UnattendedMode  
``` 

### SYNTAX 
```PowerShell 
Test-UnattendedMode [[-CallersInvocation] <InvocationInfo>]
    [-Detailed] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -CallersInvocation <InvocationInfo>  
        Caller's invocation info for pipeline and automation detection  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Detailed  
        Return detailed analysis object instead of simple boolean  
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
 

&nbsp;<hr/>
###	GenXdev.Helpers.Physics<hr/> 

##	Get-ApparentSizeAtArmLength 
```PowerShell 

   Get-ApparentSizeAtArmLength  
``` 

### SYNTAX 
```PowerShell 
Get-ApparentSizeAtArmLength [-DistanceInMeters] <double>
    [-SizeInMeters] <double> [[-ArmLengthInMeters] <double>]
    [[-As] {millimeters | centimeters | meters | inches |
    feet}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ArmLengthInMeters <double>  
        The arm length in meters (default: 0.7)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        The unit for the output size  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DistanceInMeters <double>  
        The distance to the object in meters  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SizeInMeters <double>  
        The actual size of the object in meters  
        Required?                    true  
        Position?                    1  
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
 

##	Get-AtEyeLengthSizeInMM 
```PowerShell 

   Get-AtEyeLengthSizeInMM  
``` 

### SYNTAX 
```PowerShell 
Get-AtEyeLengthSizeInMM [-DistanceInMeters] <double>
    [-SizeInMeters] <double> [-EyeToArmLengthInMeters
    <double>] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -DistanceInMeters <double>  
        The distance to the object in meters.  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -EyeToArmLengthInMeters <double>  
        The arm's length distance in meters. Default value is 0.7.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SizeInMeters <double>  
        The actual size of the object in meters.  
        Required?                    true  
        Position?                    1  
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
 

##	Get-BuoyantForceByDisplacedVolumeAndDensity 
```PowerShell 

   Get-BuoyantForceByDisplacedVolumeAndDensity  
``` 

### SYNTAX 
```PowerShell 
Get-BuoyantForceByDisplacedVolumeAndDensity
    [-DisplacedVolumeInCubicMeters] <double>
    [-FluidDensityInKilogramsPerCubicMeter] <double>
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {newtons | poundforce}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for force  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DisplacedVolumeInCubicMeters <double>  
        Displaced volume in m³  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -FluidDensityInKilogramsPerCubicMeter <double>  
        Fluid density in kg/m³  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
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
 

##	Get-CentripetalAccelerationByVelocityAndRadius 
```PowerShell 

   Get-CentripetalAccelerationByVelocityAndRadius  
``` 

### SYNTAX 
```PowerShell 
Get-CentripetalAccelerationByVelocityAndRadius
    [-VelocityInMetersPerSecond] <double> [-RadiusInMeters]
    <double> [[-As] {m/s² | g}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for acceleration  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -RadiusInMeters <double>  
        Radius in meters  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -VelocityInMetersPerSecond <double>  
        Velocity in m/s  
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
 

##	Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed 
```PowerShell 

   Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed  
``` 

### SYNTAX 
```PowerShell 
Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed
    [-OriginalFrequencyInHertz] <double>
    [-SourceSpeedInMetersPerSecond] <double>
    [-ObserverSpeedInMetersPerSecond] <double>
    [[-SpeedOfSoundInMetersPerSecond] <double>] [[-As]
    {hertz | kilohertz | megahertz}] [<CommonParameters>]
Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed
    [-OriginalFrequencyInHertz] <double>
    [-SourceSpeedInMetersPerSecond] <double>
    [-ObserverSpeedInMetersPerSecond] <double> [-Medium]
    {air | water | seawater | steel | glass | lead | gold |
    copper | rubber | vacuum | helium | co2 | methane}
    [[-As] {hertz | kilohertz | megahertz}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for frequency  
        Required?                    false  
        Position?                    4  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Medium <string>  
        The medium  
        Required?                    true  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           ByMedium  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ObserverSpeedInMetersPerSecond <double>  
        Observer speed in m/s (positive towards source)  
        Required?                    true  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OriginalFrequencyInHertz <double>  
        Original frequency in Hz  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SourceSpeedInMetersPerSecond <double>  
        Source speed in m/s (positive towards observer)  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SpeedOfSoundInMetersPerSecond <double>  
        Speed of sound in m/s (default: 343)  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           BySpeed  
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
 

##	Get-DragForceByVelocityDensityAreaAndCoefficient 
```PowerShell 

   Get-DragForceByVelocityDensityAreaAndCoefficient  
``` 

### SYNTAX 
```PowerShell 
Get-DragForceByVelocityDensityAreaAndCoefficient
    [-VelocityInMetersPerSecond] <double>
    [-DensityInKilogramsPerCubicMeter] <double>
    [-AreaInSquareMeters] <double> [-Coefficient] <double>
    [[-As] {newtons | poundforce}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AreaInSquareMeters <double>  
        Cross-sectional area in m²  
        Required?                    true  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        Output unit for force  
        Required?                    false  
        Position?                    4  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Coefficient <double>  
        Drag coefficient  
        Required?                    true  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DensityInKilogramsPerCubicMeter <double>  
        Fluid density in kg/m³  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -VelocityInMetersPerSecond <double>  
        Velocity in m/s  
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
 

##	Get-EscapeVelocityByMassAndRadius 
```PowerShell 

   Get-EscapeVelocityByMassAndRadius  
``` 

### SYNTAX 
```PowerShell 
Get-EscapeVelocityByMassAndRadius [-MassInKilograms]
    <double> [-RadiusInMeters] <double> [[-As] {m/s | km/h |
    mph | ft/s}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for velocity  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass of body in kg  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -RadiusInMeters <double>  
        Radius in meters  
        Required?                    true  
        Position?                    1  
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
 

##	Get-FreeFallDistance 
```PowerShell 

   Get-FreeFallDistance  
``` 

### SYNTAX 
```PowerShell 
Get-FreeFallDistance [-DurationInSeconds] <double>
    [[-TerminalVelocityInMetersPerSecond] <double>]
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {meters | kilometers | centimeters | millimeters | feet
    | inches | miles | yards}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        The unit for the output distance  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DurationInSeconds <double>  
        The time duration of the fall in seconds  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        The acceleration due to gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TerminalVelocityInMetersPerSecond <double>  
        The terminal velocity in meters per second (default: 53)  
        Required?                    false  
        Position?                    1  
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
 

##	Get-FreeFallHeight 
```PowerShell 

   Get-FreeFallHeight  
``` 

### SYNTAX 
```PowerShell 
Get-FreeFallHeight [-DurationInSeconds] <double>
    [[-TerminalVelocityInMs] <double>] [[-As] {meters |
    kilometers | centimeters | millimeters | feet | inches |
    miles | yards}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        The unit for the output height  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DurationInSeconds <double>  
        The time duration of the fall in seconds  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TerminalVelocityInMs <double>  
        The terminal velocity in meters per second (default: 53 m/s for human)  
        Required?                    false  
        Position?                    1  
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
 

##	Get-FreeFallTime 
```PowerShell 

   Get-FreeFallTime  
``` 

### SYNTAX 
```PowerShell 
Get-FreeFallTime [-HeightInMeters] <double>
    [[-TerminalVelocityInMs] <double>] [[-As] {seconds |
    minutes | hours | milliseconds | days}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        The unit for the output time  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -HeightInMeters <double>  
        The height to fall in meters  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TerminalVelocityInMs <double>  
        The terminal velocity in meters per second (default: 53 m/s for human)  
        Required?                    false  
        Position?                    1  
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
 

##	Get-ImpactVelocityByHeightAndGravity 
```PowerShell 

   Get-ImpactVelocityByHeightAndGravity  
``` 

### SYNTAX 
```PowerShell 
Get-ImpactVelocityByHeightAndGravity [-HeightInMeters]
    <double> [[-GravityInMetersPerSecondSquared] <double>]
    [[-As] {m/s | km/h | mph | ft/s}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for velocity. Default 'm/s'.  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s². Default 9.81.  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -HeightInMeters <double>  
        Height in meters  
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
 

##	Get-KineticEnergyByMassAndVelocity 
```PowerShell 

   Get-KineticEnergyByMassAndVelocity  
``` 

### SYNTAX 
```PowerShell 
Get-KineticEnergyByMassAndVelocity [-MassInKilograms]
    <double> [-VelocityInMetersPerSecond] <double> [[-As]
    {joules | calories | kilowatthours}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for energy  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass in kg  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -VelocityInMetersPerSecond <double>  
        Velocity in m/s  
        Required?                    true  
        Position?                    1  
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
 

##	Get-LightTravelTimeByDistance 
```PowerShell 

   Get-LightTravelTimeByDistance  
``` 

### SYNTAX 
```PowerShell 
Get-LightTravelTimeByDistance [-DistanceInMeters] <double>
    [[-SpeedOfLightInMetersPerSecond] <double>] [[-As]
    {seconds | minutes | hours | milliseconds | days}]
    [<CommonParameters>]
Get-LightTravelTimeByDistance [-DistanceInMeters] <double>
    [-Medium] {vacuum | air | water | glass | diamond}
    [[-As] {seconds | minutes | hours | milliseconds |
    days}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for time  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DistanceInMeters <double>  
        Distance in meters  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Medium <string>  
        The medium through which light travels  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           ByMedium  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SpeedOfLightInMetersPerSecond <double>  
        Speed of light in m/s (default: 299792458)  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           BySpeed  
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
 

##	Get-MagnificationByObjectDistanceAndImageDistance 
```PowerShell 

   Get-MagnificationByObjectDistanceAndImageDistance  
``` 

### SYNTAX 
```PowerShell 
Get-MagnificationByObjectDistanceAndImageDistance
    [-ObjectDistanceInMeters] <double>
    [-ImageDistanceInMeters] <double> [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ImageDistanceInMeters <double>  
        Image distance in meters  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ObjectDistanceInMeters <double>  
        Object distance in meters  
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
 

##	Get-MomentumByMassAndVelocity 
```PowerShell 

   Get-MomentumByMassAndVelocity  
``` 

### SYNTAX 
```PowerShell 
Get-MomentumByMassAndVelocity [-MassInKilograms] <double>
    [-VelocityInMetersPerSecond] <double> [[-As] {kgm/s}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for momentum  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass in kg  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -VelocityInMetersPerSecond <double>  
        Velocity in m/s  
        Required?                    true  
        Position?                    1  
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
 

##	Get-OrbitalVelocityByRadiusAndMass 
```PowerShell 

   Get-OrbitalVelocityByRadiusAndMass  
``` 

### SYNTAX 
```PowerShell 
Get-OrbitalVelocityByRadiusAndMass [-RadiusInMeters]
    <double> [-CentralMassInKilograms] <double> [[-As] {m/s
    | km/h | mph | ft/s}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for velocity  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -CentralMassInKilograms <double>  
        Central mass in kg  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -RadiusInMeters <double>  
        Orbital radius in meters  
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
 

##	Get-PotentialEnergyByMassHeightAndGravity 
```PowerShell 

   Get-PotentialEnergyByMassHeightAndGravity  
``` 

### SYNTAX 
```PowerShell 
Get-PotentialEnergyByMassHeightAndGravity [-MassInKilograms]
    <double> [-HeightInMeters] <double>
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {joules | calories | kilowatthours}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for energy  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -HeightInMeters <double>  
        Height in meters  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass in kg  
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
 

##	Get-ProjectileRangeByInitialSpeedAndAngle 
```PowerShell 

   Get-ProjectileRangeByInitialSpeedAndAngle  
``` 

### SYNTAX 
```PowerShell 
Get-ProjectileRangeByInitialSpeedAndAngle
    [-InitialSpeedInMetersPerSecond] <double>
    [-AngleInDegrees] <double>
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {meters | kilometers | centimeters | millimeters | feet
    | inches | miles | yards}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AngleInDegrees <double>  
        Launch angle in degrees  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        Output unit for range  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -InitialSpeedInMetersPerSecond <double>  
        Initial speed in m/s  
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
 

##	Get-RefractionAngleByIncidentAngleAndIndices 
```PowerShell 

   Get-RefractionAngleByIncidentAngleAndIndices  
``` 

### SYNTAX 
```PowerShell 
Get-RefractionAngleByIncidentAngleAndIndices
    [-IncidentAngleInDegrees] <double> [-IndexOfRefraction1]
    <double> [-IndexOfRefraction2] <double> [[-As] {degrees
    | radians}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for angle  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncidentAngleInDegrees <double>  
        Incident angle in degrees  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IndexOfRefraction1 <double>  
        Refractive index of first medium  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IndexOfRefraction2 <double>  
        Refractive index of second medium  
        Required?                    true  
        Position?                    2  
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
 

##	Get-ResonantFrequencyByLengthAndSpeed 
```PowerShell 

   Get-ResonantFrequencyByLengthAndSpeed  
``` 

### SYNTAX 
```PowerShell 
Get-ResonantFrequencyByLengthAndSpeed [-LengthInMeters]
    <double> [-SpeedInMetersPerSecond] <double> [[-As]
    {hertz | kilohertz | megahertz}] [<CommonParameters>]
Get-ResonantFrequencyByLengthAndSpeed [-LengthInMeters]
    <double> [-Medium] {air | water | seawater | steel |
    glass | lead | gold | copper | rubber | vacuum | helium
    | co2 | methane} [[-As] {hertz | kilohertz | megahertz}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for frequency  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LengthInMeters <double>  
        Length in meters  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Medium <string>  
        The medium  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           ByMedium  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SpeedInMetersPerSecond <double>  
        Wave speed in m/s  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           BySpeed  
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
 

##	Get-SoundTravelDistanceByTime 
```PowerShell 

   Get-SoundTravelDistanceByTime  
``` 

### SYNTAX 
```PowerShell 
Get-SoundTravelDistanceByTime [-TimeInSeconds] <double>
    [[-SpeedOfSoundInMetersPerSecond] <double>] [[-As]
    {meters | kilometers | centimeters | millimeters | feet
    | inches | miles | yards}] [<CommonParameters>]
Get-SoundTravelDistanceByTime [-TimeInSeconds] <double>
    [-Medium] {air | water | seawater | steel | glass | lead
    | gold | copper | rubber | vacuum | helium | co2 |
    methane} [[-As] {meters | kilometers | centimeters |
    millimeters | feet | inches | miles | yards}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        The unit for the output distance  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Medium <string>  
        The medium through which sound travels  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           ByMedium  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SpeedOfSoundInMetersPerSecond <double>  
        Speed of sound in m/s (default: 343)  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           BySpeed  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TimeInSeconds <double>  
        The time in seconds  
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
 

##	Get-TerminalVelocityByMassGravityDensityAndArea 
```PowerShell 

   Get-TerminalVelocityByMassGravityDensityAndArea  
``` 

### SYNTAX 
```PowerShell 
Get-TerminalVelocityByMassGravityDensityAndArea
    [-MassInKilograms] <double>
    [[-GravityInMetersPerSecondSquared] <double>]
    [-DensityInKilogramsPerCubicMeter] <double>
    [-AreaInSquareMeters] <double> [[-DragCoefficient]
    <double>] [[-As] {m/s | km/h | mph | ft/s}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AreaInSquareMeters <double>  
        Cross-sectional area in m²  
        Required?                    true  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        Output unit for velocity  
        Required?                    false  
        Position?                    5  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DensityInKilogramsPerCubicMeter <double>  
        Fluid density in kg/m³  
        Required?                    true  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DragCoefficient <double>  
        Drag coefficient (default: 0.5)  
        Required?                    false  
        Position?                    4  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass in kg  
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
 

##	Get-TimeOfFlightByInitialVelocityAndAngle 
```PowerShell 

   Get-TimeOfFlightByInitialVelocityAndAngle  
``` 

### SYNTAX 
```PowerShell 
Get-TimeOfFlightByInitialVelocityAndAngle
    [-InitialVelocityInMetersPerSecond] <double>
    [-AngleInDegrees] <double>
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {seconds | minutes | hours | milliseconds | days}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AngleInDegrees <double>  
        Launch angle in degrees  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        Output unit for time  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -InitialVelocityInMetersPerSecond <double>  
        Initial velocity in m/s  
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
 

##	Get-WaveSpeedByFrequencyAndWavelength 
```PowerShell 

   Get-WaveSpeedByFrequencyAndWavelength  
``` 

### SYNTAX 
```PowerShell 
Get-WaveSpeedByFrequencyAndWavelength [-FrequencyInHertz]
    <double> [-WavelengthInMeters] <double> [[-As] {m/s |
    km/h | mph | ft/s}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for speed  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -FrequencyInHertz <double>  
        Frequency in Hz  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WavelengthInMeters <double>  
        Wavelength in meters  
        Required?                    true  
        Position?                    1  
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

### SYNTAX 
```PowerShell 
ConvertTo-HashTable [-InputObject] <Object[]>
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -InputObject <Object[]>  
        The PSCustomObject to convert into a HashTable  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue)  
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
 

##	Get-AudioDeviceNames 
```PowerShell 

   Get-AudioDeviceNames  
``` 

### SYNTAX 
```PowerShell 
Get-AudioDeviceNames [-UseDesktopAudioCapture] [-Passthru]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -Passthru  
        Returns detailed device objects instead of just names  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -UseDesktopAudioCapture  
        Whether to list desktop audio capture devices instead of microphone devices  
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
 

##	Get-DefaultWebLanguage 
```PowerShell 

   Get-DefaultWebLanguage  
``` 

### SYNTAX 
```PowerShell 
Get-DefaultWebLanguage [<CommonParameters>] 
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
 

##	Get-ImageGeolocation 
```PowerShell 

   Get-ImageGeolocation  
``` 

### SYNTAX 
```PowerShell 
Get-ImageGeolocation [-ImagePath] <string>
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
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
 

##	Get-SpeechToText 
```PowerShell 

   Get-SpeechToText  
``` 

### SYNTAX 
```PowerShell 
Get-SpeechToText [-Input] <Object> [-ModelFileDirectoryPath
    <string>] [-LanguageIn <string>] [-CpuThreads <int>]
    [-Temperature <float>] [-TemperatureInc <float>]
    [-NoSpeechThreshold <float>] [-Prompt <string>]
    [-SuppressRegex <string>] [-TokenTimestampsSumThreshold
    <float>] [-MaxTokensPerSegment <int>] [-AudioContextSize
    <int>] [-MaxDuration <timespan>] [-Offset <timespan>]
    [-MaxLastTextTokens <int>] [-MaxSegmentLength <int>]
    [-MaxInitialTimestamp <timespan>] [-LengthPenalty
    <float>] [-EntropyThreshold <float>] [-LogProbThreshold
    <float>] [-ModelType {Tiny | TinyEn | Base | BaseEn |
    Small | SmallEn | Medium | MediumEn | LargeV1 | LargeV2
    | LargeV3 | LargeV3Turbo}] [-Passthru]
    [-WithTokenTimestamps] [-SplitOnWord] [-WithTranslate]
    [-WithProgress] [-DontSuppressBlank]
    [-SingleSegmentOnly] [-PrintSpecialTokens] [-NoContext]
    [-WithBeamSearchSamplingStrategy] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AudioContextSize <int>  
        Size of the audio context  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -CpuThreads <int>  
        Sets the output language  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DontSuppressBlank  
        Whether to NOT suppress blank lines  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -EntropyThreshold <float>  
        Entropy threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Input <Object>  
        Audio file path, FileInfo object, or any audio format supported by Whisper.  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue)  
        Parameter set name           (All)  
        Aliases                      WaveFile  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LanguageIn <string>  
        Sets the input language to detect, defaults to 'en'  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LengthPenalty <float>  
        Length penalty  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LogProbThreshold <float>  
        Log probability threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxDuration <timespan>  
        Maximum duration of the audio  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxInitialTimestamp <timespan>  
        Start timestamps at this moment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxLastTextTokens <int>  
        Maximum number of last text tokens  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxSegmentLength <int>  
        Maximum segment length  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxTokensPerSegment <int>  
        Maximum number of tokens per segment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModelFileDirectoryPath <string>  
        Path to the model file directory  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModelType <GgmlType>  
        Whisper model type to use, defaults to LargeV3Turbo  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -NoContext  
        Do not use context  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -NoSpeechThreshold <float>  
        No speech threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Offset <timespan>  
        Offset for the audio  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Passthru  
        Returns objects instead of strings  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -PrintSpecialTokens  
        Whether to print special tokens  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Prompt <string>  
        Prompt to use for the model  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SingleSegmentOnly  
        Whether to use single segment only  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SplitOnWord  
        Whether to split on word boundaries  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SuppressRegex <string>  
        Regex to suppress tokens from the output  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Temperature <float>  
        Temperature for speech detection  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TemperatureInc <float>  
        Temperature increment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TokenTimestampsSumThreshold <float>  
        Sum threshold for token timestamps, defaults to 0.5  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithBeamSearchSamplingStrategy  
        Use beam search sampling strategy  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithProgress  
        Whether to show progress  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithTokenTimestamps  
        Whether to include token timestamps  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithTranslate  
        Whether to translate the output  
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
 

##	Get-WebLanguageDictionary 
```PowerShell 

   Get-WebLanguageDictionary  
``` 

### SYNTAX 
```PowerShell 
Get-WebLanguageDictionary [<CommonParameters>] 
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
 

##	Initialize-SearchPaths 
```PowerShell 

   Initialize-SearchPaths  
``` 

### SYNTAX 
```PowerShell 
Initialize-SearchPaths [[-WorkspaceFolder] <string>]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -WorkspaceFolder <string>  
        The workspace folder path to use for node modules and PowerShell paths.  
        Required?                    false  
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
 

##	Receive-RealTimeSpeechToText 
```PowerShell 

   Receive-RealTimeSpeechToText  
``` 

### SYNTAX 
```PowerShell 
Receive-RealTimeSpeechToText [-ModelFileDirectoryPath
    <string>] [-UseDesktopAudioCapture]
    [-UseDesktopAndRecordingDevice] [-AudioDevice <string>]
    [-Passthru] [-WithTokenTimestamps]
    [-TokenTimestampsSumThreshold <float>] [-SplitOnWord]
    [-MaxTokensPerSegment <int>] [-IgnoreSilence]
    [-MaxDurationOfSilence <timespan>] [-SilenceThreshold
    <int>] [-LanguageIn <string>] [-CpuThreads <int>]
    [-Temperature <float>] [-TemperatureInc <float>]
    [-WithTranslate] [-Prompt <string>] [-SuppressRegex
    <string>] [-WithProgress] [-AudioContextSize <int>]
    [-DontSuppressBlank] [-MaxDuration <timespan>] [-Offset
    <timespan>] [-MaxLastTextTokens <int>]
    [-SingleSegmentOnly] [-PrintSpecialTokens]
    [-MaxSegmentLength <int>] [-MaxInitialTimestamp
    <timespan>] [-LengthPenalty <float>] [-EntropyThreshold
    <float>] [-LogProbThreshold <float>] [-NoSpeechThreshold
    <float>] [-NoContext] [-WithBeamSearchSamplingStrategy]
    [-ModelType {Tiny | TinyEn | Base | BaseEn | Small |
    SmallEn | Medium | MediumEn | LargeV1 | LargeV2 |
    LargeV3 | LargeV3Turbo}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AudioContextSize <int>  
        Size of the audio context  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -AudioDevice <string>  
        Use both desktop and recording device  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -CpuThreads <int>  
        Sets the output language  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DontSuppressBlank  
        Whether to NOT suppress blank lines  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -EntropyThreshold <float>  
        Entropy threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IgnoreSilence  
        Whether to ignore silence (will mess up timestamps)  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LanguageIn <string>  
        Sets the input language to detect, defaults to 'en'  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LengthPenalty <float>  
        Length penalty  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LogProbThreshold <float>  
        Log probability threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxDuration <timespan>  
        Maximum duration of the audio  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxDurationOfSilence <timespan>  
        Maximum duration of silence before automatically stopping recording  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxInitialTimestamp <timespan>  
        Start timestamps at this moment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxLastTextTokens <int>  
        Maximum number of last text tokens  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxSegmentLength <int>  
        Maximum segment length  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MaxTokensPerSegment <int>  
        Maximum number of tokens per segment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModelFileDirectoryPath <string>  
        Path to the model file  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ModelType <GgmlType>  
        Whisper model type to use, defaults to Small  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -NoContext  
        Do not use context  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -NoSpeechThreshold <float>  
        No speech threshold  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Offset <timespan>  
        Offset for the audio  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Passthru  
        Returns objects instead of strings  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -PrintSpecialTokens  
        Whether to print special tokens  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Prompt <string>  
        Prompt to use for the model  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SilenceThreshold <int>  
        Silence detect threshold (0..32767 defaults to 30)  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SingleSegmentOnly  
        Whether to use single segment only  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SplitOnWord  
        Whether to split on word boundaries  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SuppressRegex <string>  
        Regex to suppress tokens from the output  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Temperature <float>  
        Temperature for speech detection  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TemperatureInc <float>  
        Temperature increment  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TokenTimestampsSumThreshold <float>  
        Sum threshold for token timestamps, defaults to 0.5  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -UseDesktopAndRecordingDevice  
        Whether to use both desktop audio capture and recording device simultaneously  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -UseDesktopAudioCapture  
        Whether to use desktop audio capture instead of microphone  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithBeamSearchSamplingStrategy  
        Use beam search sampling strategy  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithProgress  
        Whether to show progress  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithTokenTimestamps  
        Whether to include token timestamps  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WithTranslate  
        Whether to translate the output  
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
 

##	Remove-JSONComments 
```PowerShell 

   Remove-JSONComments  
``` 

### SYNTAX 
```PowerShell 
Remove-JSONComments [-Json] <string[]> [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -Json <string[]>  
        JSON content to process as string array  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       true (ByValue)  
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
 

##	Test-UnattendedMode 
```PowerShell 

   Test-UnattendedMode  
``` 

### SYNTAX 
```PowerShell 
Test-UnattendedMode [[-CallersInvocation] <InvocationInfo>]
    [-Detailed] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -CallersInvocation <InvocationInfo>  
        Caller's invocation info for pipeline and automation detection  
        Required?                    false  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Detailed  
        Return detailed analysis object instead of simple boolean  
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

### SYNTAX 
```PowerShell 
Get-ApparentSizeAtArmLength [-DistanceInMeters] <double>
    [-SizeInMeters] <double> [[-ArmLengthInMeters] <double>]
    [[-As] {millimeters | centimeters | meters | inches |
    feet}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ArmLengthInMeters <double>  
        The arm length in meters (default: 0.7)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        The unit for the output size  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DistanceInMeters <double>  
        The distance to the object in meters  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SizeInMeters <double>  
        The actual size of the object in meters  
        Required?                    true  
        Position?                    1  
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
 

##	Get-AtEyeLengthSizeInMM 
```PowerShell 

   Get-AtEyeLengthSizeInMM  
``` 

### SYNTAX 
```PowerShell 
Get-AtEyeLengthSizeInMM [-DistanceInMeters] <double>
    [-SizeInMeters] <double> [-EyeToArmLengthInMeters
    <double>] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -DistanceInMeters <double>  
        The distance to the object in meters.  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -EyeToArmLengthInMeters <double>  
        The arm's length distance in meters. Default value is 0.7.  
        Required?                    false  
        Position?                    Named  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SizeInMeters <double>  
        The actual size of the object in meters.  
        Required?                    true  
        Position?                    1  
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
 

##	Get-BuoyantForceByDisplacedVolumeAndDensity 
```PowerShell 

   Get-BuoyantForceByDisplacedVolumeAndDensity  
``` 

### SYNTAX 
```PowerShell 
Get-BuoyantForceByDisplacedVolumeAndDensity
    [-DisplacedVolumeInCubicMeters] <double>
    [-FluidDensityInKilogramsPerCubicMeter] <double>
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {newtons | poundforce}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for force  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DisplacedVolumeInCubicMeters <double>  
        Displaced volume in m³  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -FluidDensityInKilogramsPerCubicMeter <double>  
        Fluid density in kg/m³  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
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
 

##	Get-CentripetalAccelerationByVelocityAndRadius 
```PowerShell 

   Get-CentripetalAccelerationByVelocityAndRadius  
``` 

### SYNTAX 
```PowerShell 
Get-CentripetalAccelerationByVelocityAndRadius
    [-VelocityInMetersPerSecond] <double> [-RadiusInMeters]
    <double> [[-As] {m/s² | g}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for acceleration  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -RadiusInMeters <double>  
        Radius in meters  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -VelocityInMetersPerSecond <double>  
        Velocity in m/s  
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
 

##	Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed 
```PowerShell 

   Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed  
``` 

### SYNTAX 
```PowerShell 
Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed
    [-OriginalFrequencyInHertz] <double>
    [-SourceSpeedInMetersPerSecond] <double>
    [-ObserverSpeedInMetersPerSecond] <double>
    [[-SpeedOfSoundInMetersPerSecond] <double>] [[-As]
    {hertz | kilohertz | megahertz}] [<CommonParameters>]
Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed
    [-OriginalFrequencyInHertz] <double>
    [-SourceSpeedInMetersPerSecond] <double>
    [-ObserverSpeedInMetersPerSecond] <double> [-Medium]
    {air | water | seawater | steel | glass | lead | gold |
    copper | rubber | vacuum | helium | co2 | methane}
    [[-As] {hertz | kilohertz | megahertz}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for frequency  
        Required?                    false  
        Position?                    4  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Medium <string>  
        The medium  
        Required?                    true  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           ByMedium  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ObserverSpeedInMetersPerSecond <double>  
        Observer speed in m/s (positive towards source)  
        Required?                    true  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -OriginalFrequencyInHertz <double>  
        Original frequency in Hz  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SourceSpeedInMetersPerSecond <double>  
        Source speed in m/s (positive towards observer)  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SpeedOfSoundInMetersPerSecond <double>  
        Speed of sound in m/s (default: 343)  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           BySpeed  
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
 

##	Get-DragForceByVelocityDensityAreaAndCoefficient 
```PowerShell 

   Get-DragForceByVelocityDensityAreaAndCoefficient  
``` 

### SYNTAX 
```PowerShell 
Get-DragForceByVelocityDensityAreaAndCoefficient
    [-VelocityInMetersPerSecond] <double>
    [-DensityInKilogramsPerCubicMeter] <double>
    [-AreaInSquareMeters] <double> [-Coefficient] <double>
    [[-As] {newtons | poundforce}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AreaInSquareMeters <double>  
        Cross-sectional area in m²  
        Required?                    true  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        Output unit for force  
        Required?                    false  
        Position?                    4  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Coefficient <double>  
        Drag coefficient  
        Required?                    true  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DensityInKilogramsPerCubicMeter <double>  
        Fluid density in kg/m³  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -VelocityInMetersPerSecond <double>  
        Velocity in m/s  
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
 

##	Get-EscapeVelocityByMassAndRadius 
```PowerShell 

   Get-EscapeVelocityByMassAndRadius  
``` 

### SYNTAX 
```PowerShell 
Get-EscapeVelocityByMassAndRadius [-MassInKilograms]
    <double> [-RadiusInMeters] <double> [[-As] {m/s | km/h |
    mph | ft/s}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for velocity  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass of body in kg  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -RadiusInMeters <double>  
        Radius in meters  
        Required?                    true  
        Position?                    1  
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
 

##	Get-FreeFallDistance 
```PowerShell 

   Get-FreeFallDistance  
``` 

### SYNTAX 
```PowerShell 
Get-FreeFallDistance [-DurationInSeconds] <double>
    [[-TerminalVelocityInMetersPerSecond] <double>]
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {meters | kilometers | centimeters | millimeters | feet
    | inches | miles | yards}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        The unit for the output distance  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DurationInSeconds <double>  
        The time duration of the fall in seconds  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        The acceleration due to gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TerminalVelocityInMetersPerSecond <double>  
        The terminal velocity in meters per second (default: 53)  
        Required?                    false  
        Position?                    1  
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
 

##	Get-FreeFallHeight 
```PowerShell 

   Get-FreeFallHeight  
``` 

### SYNTAX 
```PowerShell 
Get-FreeFallHeight [-DurationInSeconds] <double>
    [[-TerminalVelocityInMs] <double>] [[-As] {meters |
    kilometers | centimeters | millimeters | feet | inches |
    miles | yards}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        The unit for the output height  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DurationInSeconds <double>  
        The time duration of the fall in seconds  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TerminalVelocityInMs <double>  
        The terminal velocity in meters per second (default: 53 m/s for human)  
        Required?                    false  
        Position?                    1  
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
 

##	Get-FreeFallTime 
```PowerShell 

   Get-FreeFallTime  
``` 

### SYNTAX 
```PowerShell 
Get-FreeFallTime [-HeightInMeters] <double>
    [[-TerminalVelocityInMs] <double>] [[-As] {seconds |
    minutes | hours | milliseconds | days}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        The unit for the output time  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -HeightInMeters <double>  
        The height to fall in meters  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TerminalVelocityInMs <double>  
        The terminal velocity in meters per second (default: 53 m/s for human)  
        Required?                    false  
        Position?                    1  
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
 

##	Get-ImpactVelocityByHeightAndGravity 
```PowerShell 

   Get-ImpactVelocityByHeightAndGravity  
``` 

### SYNTAX 
```PowerShell 
Get-ImpactVelocityByHeightAndGravity [-HeightInMeters]
    <double> [[-GravityInMetersPerSecondSquared] <double>]
    [[-As] {m/s | km/h | mph | ft/s}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for velocity. Default 'm/s'.  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s². Default 9.81.  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -HeightInMeters <double>  
        Height in meters  
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
 

##	Get-KineticEnergyByMassAndVelocity 
```PowerShell 

   Get-KineticEnergyByMassAndVelocity  
``` 

### SYNTAX 
```PowerShell 
Get-KineticEnergyByMassAndVelocity [-MassInKilograms]
    <double> [-VelocityInMetersPerSecond] <double> [[-As]
    {joules | calories | kilowatthours}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for energy  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass in kg  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -VelocityInMetersPerSecond <double>  
        Velocity in m/s  
        Required?                    true  
        Position?                    1  
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
 

##	Get-LightTravelTimeByDistance 
```PowerShell 

   Get-LightTravelTimeByDistance  
``` 

### SYNTAX 
```PowerShell 
Get-LightTravelTimeByDistance [-DistanceInMeters] <double>
    [[-SpeedOfLightInMetersPerSecond] <double>] [[-As]
    {seconds | minutes | hours | milliseconds | days}]
    [<CommonParameters>]
Get-LightTravelTimeByDistance [-DistanceInMeters] <double>
    [-Medium] {vacuum | air | water | glass | diamond}
    [[-As] {seconds | minutes | hours | milliseconds |
    days}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for time  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DistanceInMeters <double>  
        Distance in meters  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Medium <string>  
        The medium through which light travels  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           ByMedium  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SpeedOfLightInMetersPerSecond <double>  
        Speed of light in m/s (default: 299792458)  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           BySpeed  
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
 

##	Get-MagnificationByObjectDistanceAndImageDistance 
```PowerShell 

   Get-MagnificationByObjectDistanceAndImageDistance  
``` 

### SYNTAX 
```PowerShell 
Get-MagnificationByObjectDistanceAndImageDistance
    [-ObjectDistanceInMeters] <double>
    [-ImageDistanceInMeters] <double> [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -ImageDistanceInMeters <double>  
        Image distance in meters  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -ObjectDistanceInMeters <double>  
        Object distance in meters  
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
 

##	Get-MomentumByMassAndVelocity 
```PowerShell 

   Get-MomentumByMassAndVelocity  
``` 

### SYNTAX 
```PowerShell 
Get-MomentumByMassAndVelocity [-MassInKilograms] <double>
    [-VelocityInMetersPerSecond] <double> [[-As] {kgm/s}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for momentum  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass in kg  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -VelocityInMetersPerSecond <double>  
        Velocity in m/s  
        Required?                    true  
        Position?                    1  
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
 

##	Get-OrbitalVelocityByRadiusAndMass 
```PowerShell 

   Get-OrbitalVelocityByRadiusAndMass  
``` 

### SYNTAX 
```PowerShell 
Get-OrbitalVelocityByRadiusAndMass [-RadiusInMeters]
    <double> [-CentralMassInKilograms] <double> [[-As] {m/s
    | km/h | mph | ft/s}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for velocity  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -CentralMassInKilograms <double>  
        Central mass in kg  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -RadiusInMeters <double>  
        Orbital radius in meters  
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
 

##	Get-PotentialEnergyByMassHeightAndGravity 
```PowerShell 

   Get-PotentialEnergyByMassHeightAndGravity  
``` 

### SYNTAX 
```PowerShell 
Get-PotentialEnergyByMassHeightAndGravity [-MassInKilograms]
    <double> [-HeightInMeters] <double>
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {joules | calories | kilowatthours}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for energy  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -HeightInMeters <double>  
        Height in meters  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass in kg  
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
 

##	Get-ProjectileRangeByInitialSpeedAndAngle 
```PowerShell 

   Get-ProjectileRangeByInitialSpeedAndAngle  
``` 

### SYNTAX 
```PowerShell 
Get-ProjectileRangeByInitialSpeedAndAngle
    [-InitialSpeedInMetersPerSecond] <double>
    [-AngleInDegrees] <double>
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {meters | kilometers | centimeters | millimeters | feet
    | inches | miles | yards}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AngleInDegrees <double>  
        Launch angle in degrees  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        Output unit for range  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -InitialSpeedInMetersPerSecond <double>  
        Initial speed in m/s  
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
 

##	Get-RefractionAngleByIncidentAngleAndIndices 
```PowerShell 

   Get-RefractionAngleByIncidentAngleAndIndices  
``` 

### SYNTAX 
```PowerShell 
Get-RefractionAngleByIncidentAngleAndIndices
    [-IncidentAngleInDegrees] <double> [-IndexOfRefraction1]
    <double> [-IndexOfRefraction2] <double> [[-As] {degrees
    | radians}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for angle  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IncidentAngleInDegrees <double>  
        Incident angle in degrees  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IndexOfRefraction1 <double>  
        Refractive index of first medium  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -IndexOfRefraction2 <double>  
        Refractive index of second medium  
        Required?                    true  
        Position?                    2  
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
 

##	Get-ResonantFrequencyByLengthAndSpeed 
```PowerShell 

   Get-ResonantFrequencyByLengthAndSpeed  
``` 

### SYNTAX 
```PowerShell 
Get-ResonantFrequencyByLengthAndSpeed [-LengthInMeters]
    <double> [-SpeedInMetersPerSecond] <double> [[-As]
    {hertz | kilohertz | megahertz}] [<CommonParameters>]
Get-ResonantFrequencyByLengthAndSpeed [-LengthInMeters]
    <double> [-Medium] {air | water | seawater | steel |
    glass | lead | gold | copper | rubber | vacuum | helium
    | co2 | methane} [[-As] {hertz | kilohertz | megahertz}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for frequency  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -LengthInMeters <double>  
        Length in meters  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Medium <string>  
        The medium  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           ByMedium  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SpeedInMetersPerSecond <double>  
        Wave speed in m/s  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           BySpeed  
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
 

##	Get-SoundTravelDistanceByTime 
```PowerShell 

   Get-SoundTravelDistanceByTime  
``` 

### SYNTAX 
```PowerShell 
Get-SoundTravelDistanceByTime [-TimeInSeconds] <double>
    [[-SpeedOfSoundInMetersPerSecond] <double>] [[-As]
    {meters | kilometers | centimeters | millimeters | feet
    | inches | miles | yards}] [<CommonParameters>]
Get-SoundTravelDistanceByTime [-TimeInSeconds] <double>
    [-Medium] {air | water | seawater | steel | glass | lead
    | gold | copper | rubber | vacuum | helium | co2 |
    methane} [[-As] {meters | kilometers | centimeters |
    millimeters | feet | inches | miles | yards}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        The unit for the output distance  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -Medium <string>  
        The medium through which sound travels  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           ByMedium  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -SpeedOfSoundInMetersPerSecond <double>  
        Speed of sound in m/s (default: 343)  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           BySpeed  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -TimeInSeconds <double>  
        The time in seconds  
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
 

##	Get-TerminalVelocityByMassGravityDensityAndArea 
```PowerShell 

   Get-TerminalVelocityByMassGravityDensityAndArea  
``` 

### SYNTAX 
```PowerShell 
Get-TerminalVelocityByMassGravityDensityAndArea
    [-MassInKilograms] <double>
    [[-GravityInMetersPerSecondSquared] <double>]
    [-DensityInKilogramsPerCubicMeter] <double>
    [-AreaInSquareMeters] <double> [[-DragCoefficient]
    <double>] [[-As] {m/s | km/h | mph | ft/s}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AreaInSquareMeters <double>  
        Cross-sectional area in m²  
        Required?                    true  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        Output unit for velocity  
        Required?                    false  
        Position?                    5  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DensityInKilogramsPerCubicMeter <double>  
        Fluid density in kg/m³  
        Required?                    true  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -DragCoefficient <double>  
        Drag coefficient (default: 0.5)  
        Required?                    false  
        Position?                    4  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -MassInKilograms <double>  
        Mass in kg  
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
 

##	Get-TimeOfFlightByInitialVelocityAndAngle 
```PowerShell 

   Get-TimeOfFlightByInitialVelocityAndAngle  
``` 

### SYNTAX 
```PowerShell 
Get-TimeOfFlightByInitialVelocityAndAngle
    [-InitialVelocityInMetersPerSecond] <double>
    [-AngleInDegrees] <double>
    [[-GravityInMetersPerSecondSquared] <double>] [[-As]
    {seconds | minutes | hours | milliseconds | days}]
    [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -AngleInDegrees <double>  
        Launch angle in degrees  
        Required?                    true  
        Position?                    1  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -As <string>  
        Output unit for time  
        Required?                    false  
        Position?                    3  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -GravityInMetersPerSecondSquared <double>  
        Gravity in m/s² (default: 9.81)  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -InitialVelocityInMetersPerSecond <double>  
        Initial velocity in m/s  
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
 

##	Get-WaveSpeedByFrequencyAndWavelength 
```PowerShell 

   Get-WaveSpeedByFrequencyAndWavelength  
``` 

### SYNTAX 
```PowerShell 
Get-WaveSpeedByFrequencyAndWavelength [-FrequencyInHertz]
    <double> [-WavelengthInMeters] <double> [[-As] {m/s |
    km/h | mph | ft/s}] [<CommonParameters>] 
``` 

### PARAMETERS 
```yaml 
 
``` 
```yaml 
    -As <string>  
        Output unit for speed  
        Required?                    false  
        Position?                    2  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -FrequencyInHertz <double>  
        Frequency in Hz  
        Required?                    true  
        Position?                    0  
        Accept pipeline input?       false  
        Parameter set name           (All)  
        Aliases                      None  
        Dynamic?                     false  
        Accept wildcard characters?  false  
``` 
```yaml 
    -WavelengthInMeters <double>  
        Wavelength in meters  
        Required?                    true  
        Position?                    1  
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
