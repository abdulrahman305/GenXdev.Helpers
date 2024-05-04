#
# Module manifest for module 'GenXdev.Helpers'

@{

    # Script module or binary module file associated with this manifest.
    RootModule             = 'GenXdev.Helpers.psm1'

    # Version number of this module.
    ModuleVersion          = '1.36.2024'
    # Supported PSEditions
    # CompatiblePSEditions = @()

    # ID used to uniquely identify this module
    GUID                   = '2f62080f-0483-4421-8497-b3d433b65173'

    # Author of this module
    Author                 = 'René Vaessen'

    # Company or vendor of this module
    CompanyName            = 'GenXdev'

    # Copyright statement for this module
    Copyright              = 'Copyright (c) 2021 René Vaessen'

    # Description of the functionality provided by this module
    Description            = 'A Windows PowerShell module with helpers mostly used by other GenXdev modules'

    # Minimum version of the PowerShell engine required by this module
    PowerShellVersion      = '5.1.19041.906'

    # Name of the PowerShell host required by this module
    # PowerShellHostName = ''

    # Minimum version of the PowerShell host required by this module
    # PowerShellHostVersion = ''

    # Minimum version of Microsoft .NET Framework required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
    DotNetFrameworkVersion = '4.6.1'

    # Minimum version of the common language runtime (CLR) required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
    ClrVersion             = '4.0.0'

    # Processor architecture (None, X86, Amd64) required by this module
    # ProcessorArchitecture = ''

    # Modules that must be imported into the global environment prior to importing this module
    RequiredModules        = @(

    )

    # Assemblies that must be loaded prior to importing this module
    RequiredAssemblies     = @(

        "GenXdev.Helpers.dll"
    )

    # Script files (.ps1) that are run in the caller's environment prior to importing this module.
    # ScriptsToProcess       = @("GenXdev.Helpers.Load.ps1")

    # Type files (.ps1xml) to be loaded when importing this module
    # TypesToProcess = @()

    # Format files (.ps1xml) to be loaded when importing this module
    # FormatsToProcess = @()

    # Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
    # NestedModules = @()

    # Functions to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no functions to export.
    FunctionsToExport      = '*'

    # Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no Cmdlets to export.
    CmdletsToExport        = '*'

    # Variables to export from this module
    VariablesToExport      = '*'

    # Aliases to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no aliases to export.
    AliasesToExport        = '*'

    # DSC resources to export from this module
    # DscResourcesToExport = @()

    # List of all modules packaged with this module
    ModuleList             = @("GenXdev.Helpers")

    # List of all files packaged with this module
    FileList               = @(
        "BouncyCastle.Cryptography.dll",
  "EmbedIO.dll",
  "GenXdev.Helpers.deps.json",
  "GenXdev.Helpers.dll",
  "GenXdev.Helpers.pdb",
  "GenXdev.Helpers.psd1",
  "GenXdev.Helpers.psm1",
  "LICENSE",
  "license.txt",
  "Microsoft.Bcl.AsyncInterfaces.dll",
  "Microsoft.Extensions.Configuration.Abstractions.dll",
  "Microsoft.Extensions.Configuration.Binder.dll",
  "Microsoft.Extensions.Configuration.dll",
  "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
  "Microsoft.Extensions.DependencyInjection.dll",
  "Microsoft.Extensions.Diagnostics.Abstractions.dll",
  "Microsoft.Extensions.Diagnostics.dll",
  "Microsoft.Extensions.Http.dll",
  "Microsoft.Extensions.Logging.Abstractions.dll",
  "Microsoft.Extensions.Logging.dll",
  "Microsoft.Extensions.Options.ConfigurationExtensions.dll",
  "Microsoft.Extensions.Options.dll",
  "Microsoft.Extensions.Primitives.dll",
  "Microsoft.Windows.SDK.NET.dll",
  "Newtonsoft.Json.dll",
  "OpenAI_API.dll",
  "powershell.jpg",
  "README.md",
  "SpotifyAPI.Web.Auth.dll",
  "SpotifyAPI.Web.dll",
  "SuperSocket.ClientEngine.dll",
  "Swan.Lite.dll",
  "System.Diagnostics.DiagnosticSource.dll",
  "WebSocket4Net.dll",
  "WinRT.Runtime.dll",
  "ref\\Microsoft.CSharp.dll",
  "ref\\Microsoft.VisualBasic.Core.dll",
  "ref\\Microsoft.VisualBasic.dll",
  "ref\\Microsoft.Win32.Primitives.dll",
  "ref\\Microsoft.Win32.Registry.dll",
  "ref\\mscorlib.dll",
  "ref\\netstandard.dll",
  "ref\\System.AppContext.dll",
  "ref\\System.Buffers.dll",
  "ref\\System.Collections.Concurrent.dll",
  "ref\\System.Collections.dll",
  "ref\\System.Collections.Immutable.dll",
  "ref\\System.Collections.NonGeneric.dll",
  "ref\\System.Collections.Specialized.dll",
  "ref\\System.ComponentModel.Annotations.dll",
  "ref\\System.ComponentModel.DataAnnotations.dll",
  "ref\\System.ComponentModel.dll",
  "ref\\System.ComponentModel.EventBasedAsync.dll",
  "ref\\System.ComponentModel.Primitives.dll",
  "ref\\System.ComponentModel.TypeConverter.dll",
  "ref\\System.Configuration.dll",
  "ref\\System.Console.dll",
  "ref\\System.Core.dll",
  "ref\\System.Data.Common.dll",
  "ref\\System.Data.DataSetExtensions.dll",
  "ref\\System.Data.dll",
  "ref\\System.Diagnostics.Contracts.dll",
  "ref\\System.Diagnostics.Debug.dll",
  "ref\\System.Diagnostics.DiagnosticSource.dll",
  "ref\\System.Diagnostics.FileVersionInfo.dll",
  "ref\\System.Diagnostics.Process.dll",
  "ref\\System.Diagnostics.StackTrace.dll",
  "ref\\System.Diagnostics.TextWriterTraceListener.dll",
  "ref\\System.Diagnostics.Tools.dll",
  "ref\\System.Diagnostics.TraceSource.dll",
  "ref\\System.Diagnostics.Tracing.dll",
  "ref\\System.dll",
  "ref\\System.Drawing.dll",
  "ref\\System.Drawing.Primitives.dll",
  "ref\\System.Dynamic.Runtime.dll",
  "ref\\System.Formats.Asn1.dll",
  "ref\\System.Formats.Tar.dll",
  "ref\\System.Globalization.Calendars.dll",
  "ref\\System.Globalization.dll",
  "ref\\System.Globalization.Extensions.dll",
  "ref\\System.IO.Compression.Brotli.dll",
  "ref\\System.IO.Compression.dll",
  "ref\\System.IO.Compression.FileSystem.dll",
  "ref\\System.IO.Compression.ZipFile.dll",
  "ref\\System.IO.dll",
  "ref\\System.IO.FileSystem.AccessControl.dll",
  "ref\\System.IO.FileSystem.dll",
  "ref\\System.IO.FileSystem.DriveInfo.dll",
  "ref\\System.IO.FileSystem.Primitives.dll",
  "ref\\System.IO.FileSystem.Watcher.dll",
  "ref\\System.IO.IsolatedStorage.dll",
  "ref\\System.IO.MemoryMappedFiles.dll",
  "ref\\System.IO.Pipes.AccessControl.dll",
  "ref\\System.IO.Pipes.dll",
  "ref\\System.IO.UnmanagedMemoryStream.dll",
  "ref\\System.Linq.dll",
  "ref\\System.Linq.Expressions.dll",
  "ref\\System.Linq.Parallel.dll",
  "ref\\System.Linq.Queryable.dll",
  "ref\\System.Memory.dll",
  "ref\\System.Net.dll",
  "ref\\System.Net.Http.dll",
  "ref\\System.Net.Http.Json.dll",
  "ref\\System.Net.HttpListener.dll",
  "ref\\System.Net.Mail.dll",
  "ref\\System.Net.NameResolution.dll",
  "ref\\System.Net.NetworkInformation.dll",
  "ref\\System.Net.Ping.dll",
  "ref\\System.Net.Primitives.dll",
  "ref\\System.Net.Quic.dll",
  "ref\\System.Net.Requests.dll",
  "ref\\System.Net.Security.dll",
  "ref\\System.Net.ServicePoint.dll",
  "ref\\System.Net.Sockets.dll",
  "ref\\System.Net.WebClient.dll",
  "ref\\System.Net.WebHeaderCollection.dll",
  "ref\\System.Net.WebProxy.dll",
  "ref\\System.Net.WebSockets.Client.dll",
  "ref\\System.Net.WebSockets.dll",
  "ref\\System.Numerics.dll",
  "ref\\System.Numerics.Vectors.dll",
  "ref\\System.ObjectModel.dll",
  "ref\\System.Reflection.DispatchProxy.dll",
  "ref\\System.Reflection.dll",
  "ref\\System.Reflection.Emit.dll",
  "ref\\System.Reflection.Emit.ILGeneration.dll",
  "ref\\System.Reflection.Emit.Lightweight.dll",
  "ref\\System.Reflection.Extensions.dll",
  "ref\\System.Reflection.Metadata.dll",
  "ref\\System.Reflection.Primitives.dll",
  "ref\\System.Reflection.TypeExtensions.dll",
  "ref\\System.Resources.Reader.dll",
  "ref\\System.Resources.ResourceManager.dll",
  "ref\\System.Resources.Writer.dll",
  "ref\\System.Runtime.CompilerServices.Unsafe.dll",
  "ref\\System.Runtime.CompilerServices.VisualC.dll",
  "ref\\System.Runtime.dll",
  "ref\\System.Runtime.Extensions.dll",
  "ref\\System.Runtime.Handles.dll",
  "ref\\System.Runtime.InteropServices.dll",
  "ref\\System.Runtime.InteropServices.JavaScript.dll",
  "ref\\System.Runtime.InteropServices.RuntimeInformation.dll",
  "ref\\System.Runtime.Intrinsics.dll",
  "ref\\System.Runtime.Loader.dll",
  "ref\\System.Runtime.Numerics.dll",
  "ref\\System.Runtime.Serialization.dll",
  "ref\\System.Runtime.Serialization.Formatters.dll",
  "ref\\System.Runtime.Serialization.Json.dll",
  "ref\\System.Runtime.Serialization.Primitives.dll",
  "ref\\System.Runtime.Serialization.Xml.dll",
  "ref\\System.Security.AccessControl.dll",
  "ref\\System.Security.Claims.dll",
  "ref\\System.Security.Cryptography.Algorithms.dll",
  "ref\\System.Security.Cryptography.Cng.dll",
  "ref\\System.Security.Cryptography.Csp.dll",
  "ref\\System.Security.Cryptography.dll",
  "ref\\System.Security.Cryptography.Encoding.dll",
  "ref\\System.Security.Cryptography.OpenSsl.dll",
  "ref\\System.Security.Cryptography.Primitives.dll",
  "ref\\System.Security.Cryptography.X509Certificates.dll",
  "ref\\System.Security.dll",
  "ref\\System.Security.Principal.dll",
  "ref\\System.Security.Principal.Windows.dll",
  "ref\\System.Security.SecureString.dll",
  "ref\\System.ServiceModel.Web.dll",
  "ref\\System.ServiceProcess.dll",
  "ref\\System.Text.Encoding.CodePages.dll",
  "ref\\System.Text.Encoding.dll",
  "ref\\System.Text.Encoding.Extensions.dll",
  "ref\\System.Text.Encodings.Web.dll",
  "ref\\System.Text.Json.dll",
  "ref\\System.Text.RegularExpressions.dll",
  "ref\\System.Threading.Channels.dll",
  "ref\\System.Threading.dll",
  "ref\\System.Threading.Overlapped.dll",
  "ref\\System.Threading.Tasks.Dataflow.dll",
  "ref\\System.Threading.Tasks.dll",
  "ref\\System.Threading.Tasks.Extensions.dll",
  "ref\\System.Threading.Tasks.Parallel.dll",
  "ref\\System.Threading.Thread.dll",
  "ref\\System.Threading.ThreadPool.dll",
  "ref\\System.Threading.Timer.dll",
  "ref\\System.Transactions.dll",
  "ref\\System.Transactions.Local.dll",
  "ref\\System.ValueTuple.dll",
  "ref\\System.Web.dll",
  "ref\\System.Web.HttpUtility.dll",
  "ref\\System.Windows.dll",
  "ref\\System.Xml.dll",
  "ref\\System.Xml.Linq.dll",
  "ref\\System.Xml.ReaderWriter.dll",
  "ref\\System.Xml.Serialization.dll",
  "ref\\System.Xml.XDocument.dll",
  "ref\\System.Xml.XmlDocument.dll",
  "ref\\System.Xml.XmlSerializer.dll",
  "ref\\System.Xml.XPath.dll",
  "ref\\System.Xml.XPath.XDocument.dll",
  "ref\\WindowsBase.dll",
  "runtimes\\unix\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Host\\Microsoft.PowerShell.Host.psd1",
  "runtimes\\unix\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Management\\Microsoft.PowerShell.Management.psd1",
  "runtimes\\unix\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Security\\Microsoft.PowerShell.Security.psd1",
  "runtimes\\unix\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Utility\\Microsoft.PowerShell.Utility.psd1",
  "runtimes\\win\\lib\\net9.0\\Modules\\CimCmdlets\\CimCmdlets.psd1",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Diagnostics\\Diagnostics.format.ps1xml",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Diagnostics\\Event.format.ps1xml",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Diagnostics\\GetEvent.types.ps1xml",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Diagnostics\\Microsoft.PowerShell.Diagnostics.psd1",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Host\\Microsoft.PowerShell.Host.psd1",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Management\\Microsoft.PowerShell.Management.psd1",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Security\\Microsoft.PowerShell.Security.psd1",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Security\\Security.types.ps1xml",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.PowerShell.Utility\\Microsoft.PowerShell.Utility.psd1",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.WSMan.Management\\Microsoft.WSMan.Management.psd1",
  "runtimes\\win\\lib\\net9.0\\Modules\\Microsoft.WSMan.Management\\WSMan.format.ps1xml",
  "runtimes\\win\\lib\\net9.0\\Modules\\PSDiagnostics\\PSDiagnostics.psd1",
  "runtimes\\win\\lib\\net9.0\\Modules\\PSDiagnostics\\PSDiagnostics.psm1"
    )

    # Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
    PrivateData            = @{

        PSData = @{

            # Tags applied to this module. These help with module discovery in online galleries.
            Tags                     = 'GenXdev'

            # A URL to the license for this module.
            LicenseUri               = 'https://raw.githubusercontent.com/genXdev/GenXdev.Helpers/main/LICENSE'

            # A URL to the main website for this project.
            ProjectUri               = 'https://github.com/genXdev/GenXdev.Helpers'

            # A URL to an icon representing this module.
            IconUri                  = 'https://genxdev.net/favicon.ico'

            # ReleaseNotes of this module
            # ReleaseNotes = ''

            # Prerelease string of this module
            # Prerelease = ''

            # Flag to indicate whether the module requires explicit user acceptance for install/update/save
            RequireLicenseAcceptance = $true

            # External dependent modules of this module
            # ExternalModuleDependencies = @()

        } # End of PSData hashtable

    } # End of PrivateData hashtable

    # HelpInfo URI of this module
    HelpInfoURI            = 'https://github.com/genXdev/GenXdev.Helpers/blob/main/README.md#cmdlet-index'

    # Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
    # DefaultCommandPrefix = ''
}
