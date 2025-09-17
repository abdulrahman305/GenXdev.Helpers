using System.Management.Automation;
using NAudio.Wave;
using Whisper.net;
using Whisper.net.Ggml;
using System.Management;
using System.Collections.Concurrent;
namespace GenXdev.Helpers
{
    [Cmdlet(VerbsCommunications.Receive, "RealTimeSpeechToText")]
    public class ReceiveRealTimeSpeechToText : PSCmdlet
    {
        #region Cmdlet Parameters
        [Parameter(Mandatory = false, HelpMessage = "Path to the model file")]
        public string ModelFileDirectoryPath { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Whether to use desktop audio capture instead of microphone")]
        public SwitchParameter UseDesktopAudioCapture { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Whether to use both desktop audio capture and recording device simultaneously")]
        public SwitchParameter UseDesktopAndRecordingDevice { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Use both desktop and recording device")]
        public string AudioDevice { get; set; }
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
        [Parameter(Mandatory = false, HelpMessage = "Whether to ignore silence (will mess up timestamps)")]
        public SwitchParameter IgnoreSilence { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Maximum duration of silence before automatically stopping recording")]
        public TimeSpan? MaxDurationOfSilence { get; set; }
        [Parameter(Mandatory = false, HelpMessage = "Silence detect threshold (0..32767 defaults to 30)")]
        [ValidateRange(0, 32767)]
        public int? SilenceThreshold { get; set; }
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
        [Parameter(Mandatory = false, HelpMessage = "Whisper model type to use, defaults to Small")]
        public GgmlType ModelType { get; set; } = GgmlType.Small;
        #endregion
        private readonly ConcurrentQueue<SegmentData> _results = new();
        private readonly ConcurrentQueue<byte[]> _bufferQueue = new();
        private readonly ConcurrentQueue<ErrorRecord> _errorQueue = new();
        private readonly ConcurrentQueue<string> _verboseQueue = new();
        private CancellationTokenSource _cts;
        private WhisperProcessor _processor;
        private WhisperFactory _whisperFactory; // Keep reference for proper disposal
        private bool _isRecordingStarted = true;
        private bool _isDisposed = false;
        private readonly object _disposeLock = new object();
        private Task _processingTask;
        // Fields for dual audio stream support
        private IWaveIn _primaryWaveIn;
        private IWaveIn _secondaryWaveIn;
        private readonly ConcurrentQueue<byte[]> _primaryQueue = new();
        private readonly ConcurrentQueue<byte[]> _secondaryQueue = new();
        private Task _mixingTask;
        private readonly object _audioMixingLock = new object();
        private int _audioCallbackCount = 0;
        private int _bufferQueueCount = 0;
        private bool hadAudio;
        private bool everHadAudio;
        private double totalSilenceSeconds;
        private double seconds;
        private double sumSq;
        private long count;
        private int threshold;
        private MemoryStream wavBufferStream;
        private bool _isFallback = false;
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (string.IsNullOrEmpty(ModelFileDirectoryPath) || !Directory.Exists(ModelFileDirectoryPath))
            {
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
            if (MyInvocation.BoundParameters.ContainsKey("UseDesktopAudioCapture"))
                WriteVerbose($"UseDesktopAudioCapture: {UseDesktopAudioCapture}");
            if (MyInvocation.BoundParameters.ContainsKey("UseDesktopAndRecordingDevice"))
                WriteVerbose($"UseDesktopAndRecordingDevice: {UseDesktopAndRecordingDevice}");
            if (MyInvocation.BoundParameters.ContainsKey("AudioDevice"))
                WriteVerbose($"AudioDevice: {AudioDevice}");
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
            if (MyInvocation.BoundParameters.ContainsKey("IgnoreSilence"))
                WriteVerbose($"IgnoreSilence: {IgnoreSilence}");
            if (MyInvocation.BoundParameters.ContainsKey("MaxDurationOfSilence"))
                WriteVerbose($"MaxDurationOfSilence: {MaxDurationOfSilence}");
            if (MyInvocation.BoundParameters.ContainsKey("SilenceThreshold"))
                WriteVerbose($"SilenceThreshold: {SilenceThreshold}");
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
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            // Initialize Whisper
            var ggmlType = ModelType;
            var modelFileName = Path.GetFullPath(Path.Combine(ModelFileDirectoryPath, GetModelFileName(ModelType)));
            if (!File.Exists(modelFileName))
            {
                DownloadModel(modelFileName, ggmlType).GetAwaiter().GetResult();
            }
            _whisperFactory = WhisperFactory.FromPath(modelFileName);
            var builder = ConfigureWhisperBuilder(_whisperFactory.CreateBuilder());
            _processor = builder.Build();
            // Create audio input(s) based on parameters
            if (UseDesktopAndRecordingDevice.IsPresent)
            {
                CreateDualAudioInputs();
            }
            else
            {
                _primaryWaveIn = CreateAudioInput();
            }
            // Handle single or dual audio inputs
            if (_secondaryWaveIn != null)
            {
                ProcessDualAudioInputs();
            }
            else
            {
                ProcessSingleAudioInput(_primaryWaveIn);
            }
        }
        private void CreateDualAudioInputs()
        {
            WriteVerbose("Setting up dual audio inputs: desktop audio capture and recording device");
            try
            {
                // Create desktop audio capture
                _primaryWaveIn = new WasapiLoopbackCapture();
                WriteVerbose("Desktop audio capture initialized");
                // Create recording device input
                _secondaryWaveIn = CreateRecordingDeviceInput();
                WriteVerbose("Recording device input initialized");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "DualAudioSetupError", ErrorCategory.DeviceError, null));
                // Fallback to single input
                _primaryWaveIn?.Dispose();
                _secondaryWaveIn?.Dispose();
                _primaryWaveIn = CreateAudioInput();
                _secondaryWaveIn = null;
            }
        }
        private IWaveIn CreateRecordingDeviceInput()
        {
            if (!string.IsNullOrWhiteSpace(AudioDevice))
            {
                // Find microphone device by name/GUID with wildcard support
                WriteVerbose($"Looking for microphone device matching: {AudioDevice}");
                for (int i = 0; i < WaveIn.DeviceCount; i++)
                {
                    try
                    {
                        var deviceInfo = WaveIn.GetCapabilities(i);
                        if (IsDeviceMatch(deviceInfo.ProductName, AudioDevice) ||
                            IsDeviceMatch(deviceInfo.ProductGuid.ToString(), AudioDevice))
                        {
                            WriteVerbose($"Selected microphone device: {deviceInfo.ProductName}");
                            var waveIn = new WaveInEvent { DeviceNumber = i };
                            return waveIn;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteVerbose($"Could not check device {i}: {ex.Message}");
                    }
                }
                WriteWarning($"Microphone device '{AudioDevice}' not found, using default");
            }
            return new WaveInEvent();
        }
        private void ProcessDualAudioInputs()
        {
            _primaryWaveIn.WaveFormat = new WaveFormat(16000, 1);
            _secondaryWaveIn.WaveFormat = new WaveFormat(16000, 1);
            var processingTask = Task.Run(() => ProcessAudioBuffer());
            _processingTask = processingTask;
            _mixingTask = Task.Run(() => MixAudioBuffers());
            // Variables for silence detection
            hadAudio = false;
            everHadAudio = false;
            totalSilenceSeconds = 0;
            seconds = 0;
            sumSq = 0;
            count = 0;
            threshold = SilenceThreshold.HasValue ? SilenceThreshold.Value : 30;
            wavBufferStream = new MemoryStream();
            // Set up event handlers for both audio inputs
            _primaryWaveIn.DataAvailable += (sender, args) =>
            {
                if (!_isRecordingStarted || _isDisposed) return;
                lock (_audioMixingLock)
                {
                    if (args.BytesRecorded > 0)
                    {
                        var buffer = new byte[args.BytesRecorded];
                        Array.Copy(args.Buffer, buffer, args.BytesRecorded);
                        _primaryQueue.Enqueue(buffer);
                        _audioCallbackCount++;
                    }
                }
            };
            _secondaryWaveIn.DataAvailable += (sender, args) =>
            {
                if (!_isRecordingStarted || _isDisposed) return;
                lock (_audioMixingLock)
                {
                    if (args.BytesRecorded > 0)
                    {
                        var buffer = new byte[args.BytesRecorded];
                        Array.Copy(args.Buffer, buffer, args.BytesRecorded);
                        _secondaryQueue.Enqueue(buffer);
                        _audioCallbackCount++;
                    }
                }
            };
            try
            {
                _primaryWaveIn.StartRecording();
                _secondaryWaveIn.StartRecording();
                ProcessMainLoop();
            }
            finally
            {
                try
                {
                    _primaryWaveIn?.StopRecording();
                    _secondaryWaveIn?.StopRecording();
                }
                catch (Exception ex)
                {
                    WriteVerbose($"Error stopping dual audio inputs: {ex.Message}");
                }
                WaitForProcessingCompletion();
            }
        }
        private async Task MixAudioBuffers()
        {
            while (!_cts.IsCancellationRequested && !_isDisposed)
            {
                byte[] primaryBuffer = null;
                byte[] secondaryBuffer = null;
                // Try to dequeue from both, with timeout
                var dequeueTasks = new[]
                {
                    Task.Run(() => _primaryQueue.TryDequeue(out primaryBuffer)),
                    Task.Run(() => _secondaryQueue.TryDequeue(out secondaryBuffer))
                };
                await Task.WhenAny(Task.WhenAll(dequeueTasks), Task.Delay(100, _cts.Token));
                if (primaryBuffer == null && secondaryBuffer == null)
                {
                    await Task.Delay(50, _cts.Token);
                    continue;
                }
                // If only one is available, create a zero buffer for the other
                int targetLength = Math.Max(primaryBuffer?.Length ?? 0, secondaryBuffer?.Length ?? 0);
                if (primaryBuffer == null)
                {
                    primaryBuffer = new byte[targetLength];
                }
                else if (secondaryBuffer == null)
                {
                    secondaryBuffer = new byte[targetLength];
                }
                else if (primaryBuffer.Length != secondaryBuffer.Length)
                {
                    // Resize to max and pad with zeros
                    if (primaryBuffer.Length < targetLength)
                    {
                        Array.Resize(ref primaryBuffer, targetLength);
                    }
                    else if (secondaryBuffer.Length < targetLength)
                    {
                        Array.Resize(ref secondaryBuffer, targetLength);
                    }
                }
                // Mix
                var mixedBuffer = MixAudioStreams(primaryBuffer, secondaryBuffer);
                WriteVerbose($"Mixed buffer length: {mixedBuffer.Length}, primary: {primaryBuffer.Length}, secondary: {secondaryBuffer.Length}");
                // Process the mixed buffer
                lock (_audioMixingLock)
                {
                    if (!_isRecordingStarted || _isDisposed) return;
                    if (MaxDurationOfSilence.HasValue || IgnoreSilence.IsPresent)
                    {
                        seconds += mixedBuffer.Length / 32000d;
                        count += mixedBuffer.Length / 2;
                        unsafe
                        {
                            fixed (byte* buffer = mixedBuffer)
                            {
                                var floatBuffer = (Int16*)buffer;
                                var sampleCount = mixedBuffer.Length / 2;
                                for (var i = 0; i < sampleCount; i++)
                                {
                                    sumSq += floatBuffer[i] * floatBuffer[i];
                                }
                            }
                        }
                        wavBufferStream.Write(mixedBuffer, 0, mixedBuffer.Length);
                        wavBufferStream.Flush();
                        var rms = Math.Sqrt(sumSq / count);
                        if (rms > threshold)
                        {
                            hadAudio = true;
                            totalSilenceSeconds = 0;
                            everHadAudio = true;
                        }
                        if (seconds > 0.85)
                        {
                            if (!_isRecordingStarted) return;
                            if (rms < threshold)
                            {
                                totalSilenceSeconds += seconds;
                                if (everHadAudio && MaxDurationOfSilence.HasValue && (totalSilenceSeconds > MaxDurationOfSilence.Value.TotalSeconds))
                                {
                                    _isRecordingStarted = false;
                                    _cts.Cancel();
                                    return;
                                }
                                if (IgnoreSilence.IsPresent && !hadAudio)
                                {
                                    count = 0;
                                    sumSq = 0;
                                    seconds = 0;
                                    hadAudio = false;
                                    wavBufferStream.Position = 0;
                                    wavBufferStream.SetLength(0);
                                    return;
                                }
                                hadAudio = false;
                            }
                            // Add buffer to queue for processing
                            wavBufferStream.Position = 0;
                            var buffer = new byte[wavBufferStream.Length];
                            wavBufferStream.Read(buffer, 0, buffer.Length);
                            _bufferQueue.Enqueue(buffer);
                            _bufferQueueCount++;
                            wavBufferStream.Position = 0;
                            wavBufferStream.SetLength(0);
                            count = 0;
                            sumSq = 0;
                            seconds = 0;
                        }
                    }
                    else
                    {
                        // When not using silence detection, directly add to buffer queue
                        if (mixedBuffer.Length > 0)
                        {
                            _bufferQueue.Enqueue(mixedBuffer);
                            _bufferQueueCount++;
                        }
                    }
                }
            }
        }
        private byte[] MixAudioStreams(byte[] primaryBuffer, byte[] secondaryBuffer)
        {
            int length = Math.Min(primaryBuffer.Length, secondaryBuffer.Length);
            var mixedBuffer = new byte[length];
            unsafe
            {
                fixed (byte* primaryPtr = primaryBuffer)
                fixed (byte* secondaryPtr = secondaryBuffer)
                fixed (byte* mixedPtr = mixedBuffer)
                {
                    var primarySamples = (Int16*)primaryPtr;
                    var secondarySamples = (Int16*)secondaryPtr;
                    var mixedSamples = (Int16*)mixedPtr;
                    int sampleCount = length / 2;
                    for (int i = 0; i < sampleCount; i++)
                    {
                        int mixed = (int)primarySamples[i] + (int)secondarySamples[i];
                        // Clamp to 16-bit range
                        if (mixed > Int16.MaxValue) mixed = Int16.MaxValue;
                        if (mixed < Int16.MinValue) mixed = Int16.MinValue;
                        mixedSamples[i] = (Int16)mixed;
                    }
                }
            }
            // Simple volume normalization: compute RMS and scale if too loud/quiet
            double rmsSum = 0;
            unsafe
            {
                fixed (byte* mixedPtr = mixedBuffer)
                {
                    var samples = (Int16*)mixedPtr;
                    for (int i = 0; i < length / 2; i++)
                    {
                        rmsSum += samples[i] * samples[i];
                    }
                }
            }
            double rms = Math.Sqrt(rmsSum / (length / 2));
            double targetRms = 10000; // Arbitrary target, ~30% of max amplitude
            if (rms > 0)
            {
                double gain = targetRms / rms;
                if (gain < 1.0 || gain > 2.0) // Limit gain to avoid amplifying noise too much
                {
                    gain = Math.Clamp(gain, 0.5, 2.0);
                    unsafe
                    {
                        fixed (byte* mixedPtr = mixedBuffer)
                        {
                            var samples = (Int16*)mixedPtr;
                            for (int i = 0; i < length / 2; i++)
                            {
                                int adjusted = (int)(samples[i] * gain);
                                if (adjusted > Int16.MaxValue) adjusted = Int16.MaxValue;
                                if (adjusted < Int16.MinValue) adjusted = Int16.MinValue;
                                samples[i] = (Int16)adjusted;
                            }
                        }
                    }
                }
            }
            return mixedBuffer;
        }
        private void ProcessSingleAudioInput(IWaveIn waveIn)
        {
            using (waveIn)
            {
                waveIn.WaveFormat = new WaveFormat(16000, 1);
                var processingTask = Task.Run(() => ProcessAudioBuffer());
                _processingTask = processingTask;
                // Variables for silence detection
                hadAudio = false;
                everHadAudio = false;
                totalSilenceSeconds = 0;
                seconds = 0;
                sumSq = 0;
                count = 0;
                threshold = SilenceThreshold.HasValue ? SilenceThreshold.Value : 30;
                wavBufferStream = new MemoryStream();
                waveIn.DataAvailable += (sender, args) =>
                {
                    if (!_isRecordingStarted || _isDisposed) return;
                    _audioCallbackCount++;
                    lock (_audioMixingLock)
                    {
                        if (!_isRecordingStarted || _isDisposed) return;
                        // Validate buffer bounds before processing
                        if (args.Buffer == null || args.BytesRecorded <= 0 || args.BytesRecorded > args.Buffer.Length)
                        {
                            return;
                        }
                        if (MaxDurationOfSilence.HasValue || IgnoreSilence.IsPresent)
                        {
                            seconds += args.BytesRecorded / 32000d;
                            count += args.BytesRecorded / 2;
                            unsafe
                            {
                                fixed (byte* buffer = args.Buffer)
                                {
                                    var floatBuffer = (Int16*)buffer;
                                    var sampleCount = Math.Min(args.BytesRecorded / 2, args.Buffer.Length / 2);
                                    for (var i = 0; i < sampleCount; i++)
                                    {
                                        sumSq += floatBuffer[i] * floatBuffer[i];
                                    }
                                }
                            }
                            wavBufferStream.Write(args.Buffer, 0, args.BytesRecorded);
                            wavBufferStream.Flush();
                            var rms = Math.Sqrt(sumSq / count);
                            if (rms > threshold)
                            {
                                hadAudio = true;
                                totalSilenceSeconds = 0;
                                everHadAudio = true;
                            }
                            if (seconds > 0.85)
                            {
                                if (!_isRecordingStarted) return;
                                if (rms < threshold)
                                {
                                    totalSilenceSeconds += seconds;
                                    if (everHadAudio && MaxDurationOfSilence.HasValue && (totalSilenceSeconds > MaxDurationOfSilence.Value.TotalSeconds))
                                    {
                                        _isRecordingStarted = false;
                                        _cts.Cancel();
                                        return;
                                    }
                                    if (IgnoreSilence.IsPresent && !hadAudio)
                                    {
                                        // Ignoring silence
                                        count = 0;
                                        sumSq = 0;
                                        seconds = 0;
                                        hadAudio = false;
                                        wavBufferStream.Position = 0;
                                        wavBufferStream.SetLength(0);
                                        return;
                                    }
                                    hadAudio = false;
                                }
                                // Add buffer to queue for processing
                                wavBufferStream.Position = 0;
                                var buffer = new byte[wavBufferStream.Length];
                                wavBufferStream.Read(buffer, 0, buffer.Length);
                                _bufferQueue.Enqueue(buffer);
                                _bufferQueueCount++;
                                wavBufferStream.Position = 0;
                                wavBufferStream.SetLength(0);
                                count = 0;
                                sumSq = 0;
                                seconds = 0;
                            }
                        }
                        else
                        {
                            // When not using silence detection, directly add to buffer queue
                            if (args.BytesRecorded > 0 && args.Buffer != null)
                            {
                                var buffer = new byte[args.BytesRecorded];
                                Array.Copy(args.Buffer, buffer, args.BytesRecorded);
                                _bufferQueue.Enqueue(buffer);
                                _bufferQueueCount++;
                            }
                        }
                    }
                };
                try
                {
                    waveIn.StartRecording();
                    ProcessMainLoop();
                }
                catch (Exception ex)
                {
                    if (!_isFallback && !UseDesktopAudioCapture.IsPresent && string.IsNullOrWhiteSpace(AudioDevice))
                    {
                        WriteWarning("No standard input device was found and switching to desktop audio.");
                        waveIn.Dispose();
                        _isFallback = true;
                        var desktopAudio = new WasapiLoopbackCapture();
                        ProcessSingleAudioInput(desktopAudio);
                        return;
                    }
                    WriteError(new ErrorRecord(ex, "AudioInputError", ErrorCategory.DeviceError, null));
                    throw;
                }
                finally
                {
                    try
                    {
                        waveIn.StopRecording();
                    }
                    catch (Exception ex)
                    {
                        WriteVerbose($"Error stopping wave input: {ex.Message}");
                    }
                    WaitForProcessingCompletion();
                }
            }
        }
        private void ProcessMainLoop()
        {
            Console.WriteLine("Recording started. Press Q to stop...");
            var startTime = System.DateTime.UtcNow;
            while (!_cts.IsCancellationRequested && _isRecordingStarted)
            {
                try
                {
                    if (Console.KeyAvailable)
                    {
                        var keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.Q)
                        {
                            _isRecordingStarted = false;
                            _cts.Cancel();
                            break;
                        }
                    }
                    if (MaxDuration.HasValue && (System.DateTime.UtcNow - startTime) > MaxDuration.Value)
                    {
                        Console.WriteLine($"Max recording time of {MaxDuration.Value.TotalSeconds} seconds reached.");
                        _isRecordingStarted = false;
                        _cts.Cancel();
                        break;
                    }
                    // Process all queued messages in the main thread
                    while (_errorQueue.TryDequeue(out var errorRecord))
                    {
                        WriteError(errorRecord);
                    }
                    while (_verboseQueue.TryDequeue(out var verboseMessage))
                    {
                        WriteVerbose(verboseMessage);
                    }
                    while (_results.TryDequeue(out var segment))
                    {
                        WriteObject(Passthru ? segment : segment.Text.Trim());
                    }
                    Thread.Sleep(100);
                }
                catch (InvalidOperationException)
                {
                    // Console input not available, continue
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "MainLoopError", ErrorCategory.OperationStopped, null));
                    break;
                }
            }
        }
        private void WaitForProcessingCompletion()
        {
            // Wait for processing to complete with proper timeout and error handling
            bool taskCompleted = false;
            try
            {
                // First try to wait gracefully
                taskCompleted = _processingTask.Wait(TimeSpan.FromSeconds(5));
                if (!taskCompleted)
                {
                    // If task doesn't complete, cancel and wait a bit more
                    _cts.Cancel();
                    taskCompleted = _processingTask.Wait(TimeSpan.FromSeconds(3));
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
            // Wait for mixing task if exists
            if (_mixingTask != null)
            {
                try
                {
                    _mixingTask.Wait(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    WriteVerbose($"Error waiting for mixing task: {ex.Message}");
                }
            }
            // Process any remaining results regardless of task completion
            int timeout = 0;
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
                    hasResults = true;
                }
                while (_results.TryDequeue(out var segment))
                {
                    WriteObject(Passthru ? segment : segment.Text.Trim());
                    hasResults = true;
                }
                if (!hasResults && taskCompleted)
                {
                    break; // No more results and task is done
                }
                Thread.Sleep(100);
                timeout++;
            }
        }
        private IWaveIn CreateAudioInput()
        {
            if (UseDesktopAudioCapture.IsPresent)
            {
                if (!string.IsNullOrWhiteSpace(AudioDevice))
                {
                    WriteVerbose($"Looking for desktop audio device matching: {AudioDevice}");
                    // For desktop audio capture, we use the default device but can log the attempt
                    WriteWarning($"Desktop audio device selection by name is not supported in this NAudio version. Using default desktop audio capture.");
                }
                return new WasapiLoopbackCapture();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(AudioDevice))
                {
                    // Find microphone device by name/GUID with wildcard support
                    WriteVerbose($"Looking for microphone device matching: {AudioDevice}");
                    for (int i = 0; i < WaveIn.DeviceCount; i++)
                    {
                        try
                        {
                            var deviceInfo = WaveIn.GetCapabilities(i);
                            if (IsDeviceMatch(deviceInfo.ProductName, AudioDevice) ||
                                IsDeviceMatch(deviceInfo.ProductGuid.ToString(), AudioDevice))
                            {
                                WriteVerbose($"Selected microphone device: {deviceInfo.ProductName}");
                                var waveIn = new WaveInEvent { DeviceNumber = i };
                                return waveIn;
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteVerbose($"Could not check device {i}: {ex.Message}");
                        }
                    }
                    WriteWarning($"Microphone device '{AudioDevice}' not found, using default");
                }
                return new WaveInEvent();
            }
        }
        private bool IsDeviceMatch(string deviceName, string pattern)
        {
            if (string.IsNullOrWhiteSpace(deviceName) || string.IsNullOrWhiteSpace(pattern))
                return false;
            // Convert wildcards to regex pattern
            string regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";
            return System.Text.RegularExpressions.Regex.IsMatch(deviceName, regexPattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
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
            if (IgnoreSilence.IsPresent)
            {
                builder.WithNoSpeechThreshold(0.4f); // More sensitive to speech (lower = more sensitive)
            }
            else if (NoSpeechThreshold.HasValue)
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
        private async Task ProcessAudioBuffer()
        {
            using var processingStream = new MemoryStream();
            bool isProcessing = false;
            try
            {
                while ((!_cts.IsCancellationRequested || _bufferQueue.Count > 0) && !_isDisposed)
                {
                    try
                    {
                        if (_bufferQueue.TryDequeue(out var buffer))
                        {
                            // Validate buffer before processing
                            if (buffer == null || buffer.Length == 0)
                            {
                                continue;
                            }
                            processingStream.Write(buffer, 0, buffer.Length);
                            // Increased threshold for better speech recognition
                            // 48000 bytes = ~3 seconds of audio (16kHz * 1 channel * 2 bytes * 3 seconds)
                            if (!isProcessing && processingStream.Length >= 48000)
                            {
                                var audioDurationSeconds = processingStream.Length / 32000.0; // 16kHz * 2 bytes
                                isProcessing = true;
                                // Convert raw PCM data to WAV format that Whisper can understand
                                using var wavStream = ConvertPcmToWav(processingStream.ToArray(), 16000, 1, 16);
                                wavStream.Position = 0;
                                try
                                {
                                    int segmentCount = 0;
                                    await foreach (var segment in _processor.ProcessAsync(wavStream, _cts.Token))
                                    {
                                        if (_cts.IsCancellationRequested || _isDisposed)
                                        {
                                            break;
                                        }
                                        segmentCount++;
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
                                    break;
                                }
                                catch (Exception ex) when (!_isDisposed)
                                {
                                    // Queue error for main thread to process
                                    _errorQueue.Enqueue(new ErrorRecord(ex, "WhisperProcessingError", ErrorCategory.OperationStopped, null));
                                }
                                processingStream.SetLength(0);
                                isProcessing = false;
                            }
                        }
                        else
                        {
                            // If we have data but not enough for a full segment, process it anyway when stopping
                            if (!_isRecordingStarted && processingStream.Length > 0 && !isProcessing && !_isDisposed)
                            {
                                var audioDurationSeconds = processingStream.Length / 32000.0;
                                isProcessing = true;
                                // Convert raw PCM data to WAV format for final processing
                                using var wavStream = ConvertPcmToWav(processingStream.ToArray(), 16000, 1, 16);
                                wavStream.Position = 0;
                                try
                                {
                                    int segmentCount = 0;
                                    await foreach (var segment in _processor.ProcessAsync(wavStream, _cts.Token))
                                    {
                                        if (_cts.IsCancellationRequested || _isDisposed)
                                        {
                                            break;
                                        }
                                        segmentCount++;
                                        if (!string.IsNullOrWhiteSpace(segment.Text))
                                        {
                                            _results.Enqueue(segment);
                                        }
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    break;
                                }
                                catch (Exception ex) when (!_isDisposed)
                                {
                                    // Queue error for main thread to process
                                    _errorQueue.Enqueue(new ErrorRecord(ex, "FinalProcessingError", ErrorCategory.OperationStopped, null));
                                }
                                processingStream.SetLength(0);
                                isProcessing = false;
                            }
                            await Task.Delay(50, _cts.Token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex) when (!(ex is OperationCanceledException) && !_isDisposed)
                    {
                        // Queue error for main thread to process instead of calling WriteError directly
                        _errorQueue.Enqueue(new ErrorRecord(ex, "ProcessingError", ErrorCategory.OperationStopped, null));
                        break;
                    }
                }
            }
            catch when (!_isDisposed)
            {
            }
        }
        private MemoryStream ConvertPcmToWav(byte[] pcmData, int sampleRate, int channels, int bitsPerSample)
        {
            var wavStream = new MemoryStream();
            int bytesPerSample = bitsPerSample / 8;
            int byteRate = sampleRate * channels * bytesPerSample;
            int blockAlign = channels * bytesPerSample;
            // Write WAV header
            // "RIFF" chunk descriptor
            wavStream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);
            wavStream.Write(BitConverter.GetBytes(36 + pcmData.Length), 0, 4); // File size - 8
            wavStream.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, 4);
            // "fmt " sub-chunk
            wavStream.Write(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, 4);
            wavStream.Write(BitConverter.GetBytes(16), 0, 4); // Sub-chunk size
            wavStream.Write(BitConverter.GetBytes((short)1), 0, 2); // Audio format (1 = PCM)
            wavStream.Write(BitConverter.GetBytes((short)channels), 0, 2); // Number of channels
            wavStream.Write(BitConverter.GetBytes(sampleRate), 0, 4); // Sample rate
            wavStream.Write(BitConverter.GetBytes(byteRate), 0, 4); // Byte rate
            wavStream.Write(BitConverter.GetBytes((short)blockAlign), 0, 2); // Block align
            wavStream.Write(BitConverter.GetBytes((short)bitsPerSample), 0, 2); // Bits per sample
            // "data" sub-chunk
            wavStream.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);
            wavStream.Write(BitConverter.GetBytes(pcmData.Length), 0, 4); // Data size
            wavStream.Write(pcmData, 0, pcmData.Length); // The actual audio data
            return wavStream;
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
                // Stop recording first to prevent new audio data
                _isRecordingStarted = false;
                // Cancel any ongoing operations
                if (_cts != null && !_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                }
                // Wait for processing task to complete with timeout
                if (_processingTask != null && !_processingTask.IsCompleted)
                {
                    try
                    {
                        var completed = _processingTask.Wait(TimeSpan.FromSeconds(50));
                        if (!completed)
                        {
                            WriteVerbose("Processing task did not complete within timeout");
                        }
                    }
                    catch (AggregateException ex)
                    {
                        // Expected when task is cancelled
                        WriteVerbose($"Processing task cancelled: {ex.InnerExceptions.FirstOrDefault()?.Message}");
                    }
                    catch (Exception ex)
                    {
                        WriteVerbose($"Error waiting for processing task: {ex.Message}");
                    }
                }
                // Wait for mixing task if exists
                if (_mixingTask != null && !_mixingTask.IsCompleted)
                {
                    try
                    {
                        _mixingTask.Wait(TimeSpan.FromSeconds(50));
                    }
                    catch (AggregateException ex)
                    {
                        WriteVerbose($"Mixing task cancelled: {ex.InnerExceptions.FirstOrDefault()?.Message}");
                    }
                    catch (Exception ex)
                    {
                        WriteVerbose($"Error waiting for mixing task: {ex.Message}");
                    }
                }
                // Dispose audio inputs
                try
                {
                    _primaryWaveIn?.Dispose();
                    _secondaryWaveIn?.Dispose();
                }
                catch (Exception ex)
                {
                    WriteVerbose($"Error disposing audio inputs: {ex.Message}");
                }
                // Dispose wavBufferStream
                try
                {
                    wavBufferStream?.Dispose();
                }
                catch (Exception ex)
                {
                    WriteVerbose($"Error disposing wavBufferStream: {ex.Message}");
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