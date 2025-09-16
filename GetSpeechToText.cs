using System.Management.Automation;
using Whisper.net;
using Whisper.net.Ggml;
using System.Management;
using System.Collections.Concurrent;
namespace GenXdev.Helpers
{
    [Cmdlet(VerbsCommon.Get, "SpeechToText")]
    public class GetSpeechToText : PSCmdlet
    {
        #region Cmdlet Parameters
        [Parameter(Mandatory = false, HelpMessage = "Path to the model file")]
        public string ModelFileDirectoryPath { get; set; }
        [Alias("WaveFile")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Audio file path, FileInfo object, or any audio format supported by Whisper.")]
        public object Input { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Returns objects instead of strings")]
        public SwitchParameter Passthru { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Whether to include token timestamps")]
        public SwitchParameter WithTokenTimestamps { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Sum threshold for token timestamps, defaults to 0.5")]
        public float TokenTimestampsSumThreshold { get; set; } = 0.5f;
        [Parameter(Mandatory = false, HelpMessage = "Whether to split on word boundaries")]
        public SwitchParameter SplitOnWord { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Maximum number of tokens per segment")]
        public int? MaxTokensPerSegment { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Sets the input language to detect, defaults to 'en'")]
        public string LanguageIn { get; set; } = "en";
        [Parameter(Mandatory = false, HelpMessage = "Sets the output language")]
        public int CpuThreads { get; set; } = 0;
        [Parameter(Mandatory = false, HelpMessage = "Temperature for speech detection")]
        [ValidateRange(0, 1)]
        public float? Temperature { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Temperature increment")]
        [ValidateRange(0, 1)]
        public float? TemperatureInc { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Whether to translate the output")]
        public SwitchParameter WithTranslate { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Prompt to use for the model")]
        public string Prompt { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Regex to suppress tokens from the output")]
        public string SuppressRegex { get; set; } = null;
        [Parameter(Mandatory = false, HelpMessage = "Whether to show progress")]
        public SwitchParameter WithProgress { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Size of the audio context")]
        public int? AudioContextSize { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Whether to NOT suppress blank lines")]
        public SwitchParameter DontSuppressBlank { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Maximum duration of the audio")]
        public TimeSpan? MaxDuration { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Offset for the audio")]
        public TimeSpan? Offset { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Maximum number of last text tokens")]
        public int? MaxLastTextTokens { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Whether to use single segment only")]
        public SwitchParameter SingleSegmentOnly { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Whether to print special tokens")]
        public SwitchParameter PrintSpecialTokens { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Maximum segment length")]
        public int? MaxSegmentLength { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Start timestamps at this moment")]
        public TimeSpan? MaxInitialTimestamp { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Length penalty")]
        [ValidateRange(0, 1)]
        public float? LengthPenalty { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Entropy threshold")]
        [ValidateRange(0, 1)]
        public float? EntropyThreshold { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Log probability threshold")]
        [ValidateRange(0, 1)]
        public float? LogProbThreshold { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "No speech threshold")]
        [ValidateRange(0, 1)]
        public float? NoSpeechThreshold { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Don't use context")]
        public SwitchParameter NoContext { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Use beam search sampling strategy")]
        public SwitchParameter WithBeamSearchSamplingStrategy { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Whisper model type to use, defaults to LargeV3Turbo")]
        public GgmlType ModelType { get; set; } = GgmlType.LargeV3Turbo;
        #endregion
        private readonly ConcurrentQueue<SegmentData> _results = new();
        private readonly ConcurrentQueue<ErrorRecord> _errorQueue = new();
        private readonly ConcurrentQueue<string> _verboseQueue = new();
        private CancellationTokenSource _cts;
        private WhisperProcessor _processor;
        private WhisperFactory _whisperFactory; // Keep reference for proper disposal
        private bool _isDisposed = false;
        private readonly object _disposeLock = new object();
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (string.IsNullOrEmpty(ModelFileDirectoryPath) || !Directory.Exists(ModelFileDirectoryPath))
            {
                // With this:
                var localAppData = System.Environment.GetEnvironmentVariable("LOCALAPPDATA");
                if (!string.IsNullOrEmpty(localAppData))
                {
                    ModelFileDirectoryPath = Path.Combine(localAppData, "GenXdev.PowerShell");
                }
                if (!Directory.Exists(ModelFileDirectoryPath))
                {
                    try
                    {
                        Directory.CreateDirectory(ModelFileDirectoryPath);
                    }
                    catch (Exception ex)
                    {
                        ThrowTerminatingError(new ErrorRecord(ex, "ModelPathCreationFailed", ErrorCategory.ResourceUnavailable, ModelFileDirectoryPath));
                    }
                }
            }
            if (!MyInvocation.BoundParameters.ContainsKey("LanguageIn"))
            {
                LanguageIn = "auto";
            }
            // Only log parameters that were actually set by the user
            WriteVerbose($"ModelFileDirectoryPath: {ModelFileDirectoryPath}");

            if (MyInvocation.BoundParameters.ContainsKey("Input"))
                WriteVerbose($"Input: {Input}");
            if (MyInvocation.BoundParameters.ContainsKey("Passthru"))
                WriteVerbose($"Passthru: {Passthru}");
            if (MyInvocation.BoundParameters.ContainsKey("WithTokenTimestamps"))
                WriteVerbose($"WithTokenTimestamps: {WithTokenTimestamps}");
            if (MyInvocation.BoundParameters.ContainsKey("TokenTimestampsSumThreshold"))
                WriteVerbose($"TokenTimestampsSumThreshold: {TokenTimestampsSumThreshold}");
            if (MyInvocation.BoundParameters.ContainsKey("SplitOnWord"))
                WriteVerbose($"SplitOnWord: {SplitOnWord}");
            if (MyInvocation.BoundParameters.ContainsKey("MaxTokensPerSegment"))
                WriteVerbose($"MaxTokensPerSegment: {MaxTokensPerSegment}");
            if (MyInvocation.BoundParameters.ContainsKey("LanguageIn"))
                WriteVerbose($"LanguageIn: {LanguageIn}");
            if (MyInvocation.BoundParameters.ContainsKey("CpuThreads"))
                WriteVerbose($"CpuThreads: {CpuThreads}");
            if (MyInvocation.BoundParameters.ContainsKey("Temperature"))
                WriteVerbose($"Temperature: {Temperature}");
            if (MyInvocation.BoundParameters.ContainsKey("TemperatureInc"))
                WriteVerbose($"TemperatureInc: {TemperatureInc}");
            if (MyInvocation.BoundParameters.ContainsKey("WithTranslate"))
                WriteVerbose($"WithTranslate: {WithTranslate}");
            if (MyInvocation.BoundParameters.ContainsKey("Prompt"))
                WriteVerbose($"Prompt: {Prompt}");
            if (MyInvocation.BoundParameters.ContainsKey("SuppressRegex"))
                WriteVerbose($"SuppressRegex: {SuppressRegex}");
            if (MyInvocation.BoundParameters.ContainsKey("WithProgress"))
                WriteVerbose($"WithProgress: {WithProgress}");
            if (MyInvocation.BoundParameters.ContainsKey("AudioContextSize"))
                WriteVerbose($"AudioContextSize: {AudioContextSize}");
            if (MyInvocation.BoundParameters.ContainsKey("DontSuppressBlank"))
                WriteVerbose($"DontSuppressBlank: {DontSuppressBlank}");
            if (MyInvocation.BoundParameters.ContainsKey("MaxDuration"))
                WriteVerbose($"MaxDuration: {MaxDuration}");
            if (MyInvocation.BoundParameters.ContainsKey("Offset"))
                WriteVerbose($"Offset: {Offset}");
            if (MyInvocation.BoundParameters.ContainsKey("MaxLastTextTokens"))
                WriteVerbose($"MaxLastTextTokens: {MaxLastTextTokens}");
            if (MyInvocation.BoundParameters.ContainsKey("SingleSegmentOnly"))
                WriteVerbose($"SingleSegmentOnly: {SingleSegmentOnly}");
            if (MyInvocation.BoundParameters.ContainsKey("PrintSpecialTokens"))
                WriteVerbose($"PrintSpecialTokens: {PrintSpecialTokens}");
            if (MyInvocation.BoundParameters.ContainsKey("MaxSegmentLength"))
                WriteVerbose($"MaxSegmentLength: {MaxSegmentLength}");
            if (MyInvocation.BoundParameters.ContainsKey("MaxInitialTimestamp"))
                WriteVerbose($"MaxInitialTimestamp: {MaxInitialTimestamp}");
            if (MyInvocation.BoundParameters.ContainsKey("LengthPenalty"))
                WriteVerbose($"LengthPenalty: {LengthPenalty}");
            if (MyInvocation.BoundParameters.ContainsKey("EntropyThreshold"))
                WriteVerbose($"EntropyThreshold: {EntropyThreshold}");
            if (MyInvocation.BoundParameters.ContainsKey("LogProbThreshold"))
                WriteVerbose($"LogProbThreshold: {LogProbThreshold}");
            if (MyInvocation.BoundParameters.ContainsKey("NoSpeechThreshold"))
                WriteVerbose($"NoSpeechThreshold: {NoSpeechThreshold}");
            if (MyInvocation.BoundParameters.ContainsKey("NoContext"))
                WriteVerbose($"NoContext: {NoContext}");
            if (MyInvocation.BoundParameters.ContainsKey("WithBeamSearchSamplingStrategy"))
                WriteVerbose($"WithBeamSearchSamplingStrategy: {WithBeamSearchSamplingStrategy}");
            if (MyInvocation.BoundParameters.ContainsKey("ModelType"))
                WriteVerbose($"ModelType: {ModelType}");
            _cts = new CancellationTokenSource();
            // Initialize Whisper processor once
            var ggmlType = ModelType;
            var modelFileName = Path.GetFullPath(Path.Combine(ModelFileDirectoryPath, GetModelFileName(ModelType)));
            if (!File.Exists(modelFileName))
            {
                DownloadModel(modelFileName, ggmlType).GetAwaiter().GetResult();
            }
            _whisperFactory = WhisperFactory.FromPath(modelFileName);
            var builder = ConfigureWhisperBuilder(_whisperFactory.CreateBuilder());
            _processor = builder.Build();
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            // Get file path from input
            string filePath = GetFilePathFromInput(Input);
            if (string.IsNullOrEmpty(filePath))
            {
                WriteError(new ErrorRecord(
                    new ArgumentException("Input parameter is required and must be a valid file path or FileInfo object."),
                    "MissingInput",
                    ErrorCategory.InvalidArgument,
                    Input));
                return;
            }
            // Validate input file exists
            if (!File.Exists(filePath))
            {
                WriteError(new ErrorRecord(
                    new FileNotFoundException($"Audio file not found: {filePath}"),
                    "FileNotFound",
                    ErrorCategory.ObjectNotFound,
                    filePath));
                return;
            }
            WriteVerbose($"Processing audio file: {filePath}");
            // Process the audio file using the already-initialized processor
            ProcessAudioFile(filePath);
        }
        private string GetFilePathFromInput(object input)
        {
            if (input == null) return null;
            // Handle FileInfo objects
            if (input is FileInfo fileInfo)
            {
                return fileInfo.FullName;
            }
            // Handle string paths
            if (input is string stringPath)
            {
                return stringPath;
            }
            // Handle PSObject wrapper around FileInfo
            if (input is PSObject psObject)
            {
                var baseObject = psObject.BaseObject;
                if (baseObject is FileInfo fi)
                {
                    return fi.FullName;
                }
                if (baseObject is string str)
                {
                    return str;
                }
            }
            // Try to convert to string as fallback
            try
            {
                return input.ToString();
            }
            catch
            {
                return null;
            }
        }
        private void ProcessAudioFile(string filePath)
        {
            using var audioStream = File.OpenRead(filePath);

            var processingTask = Task.Run(async () =>
            {
                try
                {
                    await foreach (var segment in _processor.ProcessAsync(audioStream, _cts.Token))
                    {
                        if (_cts.IsCancellationRequested || _isDisposed)
                            break;

                        if (!string.IsNullOrWhiteSpace(segment.Text))
                        {
                            if (!(segment.Text.Trim("\r\n\t ".ToCharArray()) == "[BLANK_AUDIO]"))
                            {
                                _results.Enqueue(segment);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                }
                catch (Exception ex)
                {
                    _errorQueue.Enqueue(new ErrorRecord(ex, "ProcessingError", ErrorCategory.OperationStopped, null));
                }
            });
            Console.WriteLine("Processing audio file. Press Q to abort...");
            // Main processing loop with improved error handling
            while (!processingTask.IsCompleted)
            {
                try
                {
                    // Don't dequeue results during processing - let them accumulate
                    // This prevents double output when used in SRT generation

                    // Check for Q key to abort
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                    {
                        _cts.Cancel();
                        _errorQueue.Enqueue(new ErrorRecord(new Exception("Processing aborted"), "ProcessingAborted", ErrorCategory.OperationStopped, null));
                        break;
                    }
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "ProcessingLoopError", ErrorCategory.OperationStopped, null));
                    break;
                }
            }
            // Wait for processing to complete with proper timeout and error handling
            bool taskCompleted = false;
            try
            {
                // First try to wait gracefully
                taskCompleted = processingTask.Wait(TimeSpan.FromSeconds(10));
                if (!taskCompleted)
                {
                    // If task doesn't complete, cancel and wait a bit more
                    _cts.Cancel();
                    taskCompleted = processingTask.Wait(TimeSpan.FromSeconds(5));
                }
            }
            catch (AggregateException ex)
            {
                // Handle task exceptions
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (!(innerEx is OperationCanceledException))
                    {
                        WriteVerbose($"Processing task error: {innerEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteVerbose($"Error waiting for processing task: {ex.Message}");
            }
            // Process any remaining results regardless of task completion
            int timeout = 0;
            List<SegmentData> allSegments = new List<SegmentData>();

            while (timeout < 50) // Max 5 seconds
            {
                bool hasResults = false;
                // Process all queued messages in the main thread during cleanup
                while (_errorQueue.TryDequeue(out var errorRecord))
                {
                    WriteError(errorRecord);
                    hasResults = true;
                }
                while (_verboseQueue.TryDequeue(out var verboseMessage))
                {
                    WriteVerbose(verboseMessage);
                    hasResults = true;
                }
                while (_results.TryDequeue(out var segment))
                {
                    allSegments.Add(segment);
                    hasResults = true;
                }
                if (!hasResults && taskCompleted)
                {
                    break; // No more results and task is done
                }
                Thread.Sleep(100);
                timeout++;
            }

            // Output all segments at once at the end
            foreach (var segment in allSegments)
            {
                WriteObject(Passthru ? segment : segment.Text.Trim());
            }
        }
        private WhisperProcessorBuilder ConfigureWhisperBuilder(WhisperProcessorBuilder builder)
        {
            int physicalCoreCount = 0;
            var searcher = new ManagementObjectSearcher("select NumberOfCores from Win32_Processor");
            foreach (var item in searcher.Get())
            {
                physicalCoreCount += Convert.ToInt32(item["NumberOfCores"]);
            }
            builder.WithLanguage(LanguageIn)
                   .WithThreads(CpuThreads > 0 ? CpuThreads : physicalCoreCount);

            // Check for LanguageIn to enable WithTranslate
            if (MyInvocation.BoundParameters.ContainsKey("LanguageIn"))
            {
                builder.WithTranslate();
            }
            // Improved speech detection settings
            if (Temperature.HasValue)
            {
                builder.WithTemperature(Temperature.Value);
            }
            else
            {
                builder.WithTemperature(0.0f); // Lower temperature for more consistent results
            }
            if (TemperatureInc.HasValue) builder.WithTemperatureInc(TemperatureInc.Value);
            if (WithTokenTimestamps.IsPresent) builder.WithTokenTimestamps().WithTokenTimestampsSumThreshold(TokenTimestampsSumThreshold);
            if (WithTranslate.IsPresent) builder.WithTranslate();
            if (!string.IsNullOrWhiteSpace(Prompt)) builder.WithPrompt(Prompt);
            if (!string.IsNullOrWhiteSpace(SuppressRegex)) builder.WithSuppressRegex(SuppressRegex);
            if (WithProgress.IsPresent)
            {
                builder.WithProgressHandler(progress => WriteProgress(new ProgressRecord(1, "Processing", $"Progress: {progress}%") { PercentComplete = progress }));
            }
            if (SplitOnWord.IsPresent) builder.SplitOnWord();
            if (MaxTokensPerSegment.HasValue) builder.WithMaxTokensPerSegment(MaxTokensPerSegment.Value);
            // Improved silence/speech detection
            if (NoSpeechThreshold.HasValue)
            {
                builder.WithNoSpeechThreshold(NoSpeechThreshold.Value);
            }
            else
            {
                builder.WithNoSpeechThreshold(0.6f); // Default Whisper threshold
            }
            if (AudioContextSize.HasValue) builder.WithAudioContextSize(AudioContextSize.Value);
            if (DontSuppressBlank.IsPresent) builder.WithoutSuppressBlank();
            if (MaxDuration.HasValue) builder.WithDuration(MaxDuration.Value);
            if (Offset.HasValue) builder.WithOffset(Offset.Value);
            if (MaxLastTextTokens.HasValue) builder.WithMaxLastTextTokens(MaxLastTextTokens.Value);
            if (SingleSegmentOnly.IsPresent) builder.WithSingleSegment();
            if (PrintSpecialTokens.IsPresent) builder.WithPrintSpecialTokens();
            if (MaxSegmentLength.HasValue) builder.WithMaxSegmentLength(MaxSegmentLength.Value);
            if (MaxInitialTimestamp.HasValue) builder.WithMaxInitialTs((int)MaxInitialTimestamp.Value.TotalSeconds);
            if (LengthPenalty.HasValue) builder.WithLengthPenalty(LengthPenalty.Value);
            if (EntropyThreshold.HasValue) builder.WithEntropyThreshold(EntropyThreshold.Value);
            if (LogProbThreshold.HasValue) builder.WithLogProbThreshold(LogProbThreshold.Value);
            if (NoContext.IsPresent) builder.WithNoContext();
            if (WithBeamSearchSamplingStrategy.IsPresent) builder.WithBeamSearchSamplingStrategy();
            return builder;
        }
        protected override void EndProcessing()
        {
            lock (_disposeLock)
            {
                if (_isDisposed) return;
                _isDisposed = true;
            }
            try
            {
                // Cancel any ongoing operations first
                if (_cts != null && !_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                }
                // Dispose Whisper processor first (it uses the factory)
                if (_processor != null)
                {
                    try
                    {
                        // Properly dispose async resources
                        if (_processor is IAsyncDisposable asyncDisposable)
                        {
                            // Wait for async disposal to complete
                            asyncDisposable.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(50));
                        }
                        else if (_processor is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteVerbose($"Error disposing Whisper processor: {ex.Message}");
                    }
                    finally
                    {
                        _processor = null;
                    }
                }
                // Then dispose Whisper factory
                if (_whisperFactory != null)
                {
                    try
                    {
                        _whisperFactory.Dispose();
                    }
                    catch (Exception ex)
                    {
                        WriteVerbose($"Error disposing Whisper factory: {ex.Message}");
                    }
                    finally
                    {
                        _whisperFactory = null;
                    }
                }
                // Finally dispose cancellation token source
                if (_cts != null)
                {
                    try
                    {
                        _cts.Dispose();
                    }
                    catch (Exception ex)
                    {
                        WriteVerbose($"Error disposing cancellation token source: {ex.Message}");
                    }
                    finally
                    {
                        _cts = null;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteVerbose($"Error in EndProcessing: {ex.Message}");
            }
            base.EndProcessing();
        }
        private static async Task DownloadModel(string fileName, GgmlType ggmlType)
        {
            Console.WriteLine($"Downloading Model {fileName}");
            using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(ggmlType);
            using var fileWriter = File.OpenWrite(fileName);
            await modelStream.CopyToAsync(fileWriter);
        }
        private static string GetModelFileName(GgmlType modelType)
        {
            return $"ggml-{modelType}.bin";
        }
    }
}