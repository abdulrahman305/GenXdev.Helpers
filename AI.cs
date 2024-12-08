using System.Management.Automation;
using NAudio.Wave;
using Whisper.net;
using Whisper.net.Ggml;
using System.Management;
using System.Text.RegularExpressions;

namespace GenXdev.Helpers
{
    [Cmdlet(VerbsCommon.Get, "SpeechToText")]
    public class GetSpeechToText : Cmdlet
    {
        #region Cmdlet Parameters

        [Parameter(Mandatory = true, HelpMessage = "Path to the model file")]
        public string ModelFilePath { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Path to the 16Khz mono, .WAV file to process")]
        public string WaveFile { get; set; } = null;

        [Parameter(Mandatory = false, HelpMessage = "Whether to use desktop audio capture instead of microphone input")]
        public SwitchParameter UseDesktopAudioCapture { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Returns objects instead of strings")]
        public SwitchParameter Passthru { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Whether to include token timestamps in the output")]
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

        [Parameter(Mandatory = false, HelpMessage = "Sets the language to detect, defaults to 'auto'")]
        public string Language { get; set; } = "auto";

        [Parameter(Mandatory = false, HelpMessage = "Number of CPU threads to use, defaults to 0 (auto)")]
        public int CpuThreads { get; set; } = 0;

        [Parameter(Mandatory = false, HelpMessage = "Temperature for speech generation")]
        [ValidateRange(0, 100)]
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

        #endregion

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            bool isRecordingStarted = true;
            bool isProcessingAborted = false;
            object syncLock = new object();
            var objects = new List<SegmentData>();
            var progressRecords = new List<ProgressRecord>();

            int physicalCoreCount = 0;
            var searcher = new ManagementObjectSearcher("select NumberOfCores from Win32_Processor");
            foreach (var item in searcher.Get())
            {
                physicalCoreCount += Convert.ToInt32(item["NumberOfCores"]);
            }

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            using var outputStream = new MemoryStream();
            using var waveFileWriter = new WaveFileWriter(outputStream, new WaveFormat(16000, 1));

            var task = Task.Run(async () =>
            {
                var ggmlType = GgmlType.LargeV3Turbo;
                var modelFileName = Path.GetFullPath(Path.Combine(ModelFilePath, "ggml-largeV3Turbo.bin"));

                if (!File.Exists(modelFileName))
                {
                    await DownloadModel(modelFileName, ggmlType);
                }

                using var whisperFactory = WhisperFactory.FromPath(modelFileName);

                var builder = whisperFactory.CreateBuilder()
                    .WithLanguage(Language);

                if (Temperature.HasValue)
                {
                    builder.WithTemperature(Temperature.Value);
                }

                if (CpuThreads > 0)
                {
                    builder.WithThreads(CpuThreads);
                }
                else if (CpuThreads < 0)
                {
                    builder.WithThreads(physicalCoreCount);
                }

                if (!string.IsNullOrWhiteSpace(SuppressRegex))
                {
                    builder.WithSuppressRegex(SuppressRegex);
                }

                if (WithTokenTimestamps.ToBool())
                {
                    builder.WithTokenTimestamps().WithTokenTimestampsSumThreshold(TokenTimestampsSumThreshold);
                }

                if (WithTranslate.ToBool())
                {
                    builder.WithTranslate();
                }

                if (!string.IsNullOrWhiteSpace(Prompt))
                {
                    builder.WithPrompt(Prompt);
                }

                if (WithProgress.ToBool())
                {
                    builder.WithProgressHandler((int progress) =>
                    {
                        if (isProcessingAborted) return;
                        lock (syncLock)
                        {
                            if (!isRecordingStarted) return;
                            var p = new ProgressRecord(1, "transcribing", "transcribing audio");
                            p.PercentComplete = progress;
                            progressRecords.Add(p);
                        }
                    });
                }

                if (SplitOnWord.ToBool())
                {
                    builder.SplitOnWord();
                }

                if (MaxTokensPerSegment.HasValue)
                {
                    builder.WithMaxTokensPerSegment(MaxTokensPerSegment.Value);
                }

                if (AudioContextSize.HasValue)
                {
                    builder.WithAudioContextSize(AudioContextSize.Value);
                }

                if (WithBeamSearchSamplingStrategy.ToBool())
                {
                    builder.WithBeamSearchSamplingStrategy();
                }

                if (MaxDuration.HasValue)
                {
                    builder.WithDuration(MaxDuration.Value);
                }

                if (Offset.HasValue)
                {
                    builder.WithOffset(Offset.Value);
                }

                if (MaxLastTextTokens.HasValue)
                {
                    builder.WithMaxLastTextTokens(MaxLastTextTokens.Value);
                }

                if (SingleSegmentOnly.ToBool())
                {
                    builder.WithSingleSegment();
                }

                if (PrintSpecialTokens.ToBool())
                {
                    builder.WithPrintSpecialTokens();
                }

                if (MaxSegmentLength.HasValue)
                {
                    builder.WithMaxSegmentLength(MaxSegmentLength.Value);
                }

                if (MaxTokensPerSegment.HasValue)
                {
                    builder.WithMaxTokensPerSegment(MaxTokensPerSegment.Value);
                }

                if (AudioContextSize.HasValue)
                {
                    builder.WithAudioContextSize(AudioContextSize.Value);
                }

                if (DontSuppressBlank.ToBool())
                {
                    builder.WithoutSuppressBlank();
                }

                if (MaxInitialTimestamp.HasValue)
                {
                    builder.WithMaxInitialTs(Convert.ToInt32(MaxInitialTimestamp.Value.TotalSeconds));
                }

                if (LengthPenalty.HasValue)
                {
                    builder.WithLengthPenalty(LengthPenalty.Value);
                }

                if (TemperatureInc.HasValue)
                {
                    builder.WithTemperatureInc(TemperatureInc.Value);
                }

                if (EntropyThreshold.HasValue)
                {
                    builder.WithEntropyThreshold(EntropyThreshold.Value);
                }

                if (LogProbThreshold.HasValue)
                {
                    builder.WithLogProbThreshold(LogProbThreshold.Value);
                }

                if (NoSpeechThreshold.HasValue)
                {
                    builder.WithNoSpeechThreshold(NoSpeechThreshold.Value);
                }

                if (NoContext.ToBool())
                {
                    builder.WithNoContext();
                }

                using var processor = builder.Build();

                if (WaveFile == null)
                {
                    using IWaveIn waveIn = UseDesktopAudioCapture.ToBool() ? new WasapiLoopbackCapture() : new WaveInEvent();
                    waveIn.WaveFormat = new WaveFormat(16000, 1);
                    bool hadAudio = false;
                    bool everHadAudio = false;
                    using MemoryStream wavBufferStream = new MemoryStream();

                    double seconds = 0;
                    double sum = 0;
                    long count = 0;

                    int threshold = SilenceThreshold.HasValue ? SilenceThreshold.Value : 30;
                    //Console.WriteLine("threshold: " + threshold.ToString());

                    double totalSilenceSeconds = 0;

                    waveIn.DataAvailable += (sender, args) =>
                    {
                        if (!isRecordingStarted) return;

                        lock (syncLock)
                        {
                            if (!isRecordingStarted) return;

                            if (MaxDurationOfSilence.HasValue || IgnoreSilence.ToBool())
                            {
                                seconds += args.BytesRecorded / 32000d;
                                count += args.BytesRecorded / 2;

                                unsafe
                                {
                                    fixed (byte* buffer = args.Buffer)
                                    {
                                        var floatBuffer = (Int16*)buffer;
                                        for (var i = 0; i < args.BytesRecorded / 2; i++)
                                        {
                                            sum += Math.Abs(floatBuffer[i]);
                                        }
                                    }
                                }

                                wavBufferStream.Write(args.Buffer, 0, args.BytesRecorded);
                                wavBufferStream.Flush();

                                var current = (sum / count);

                                if (current > threshold)
                                {
                                    // Console.WriteLine("Audio detected");
                                    hadAudio = true;
                                    totalSilenceSeconds = 0;
                                    everHadAudio = true;
                                }

                                if (seconds > 0.85)
                                {
                                    if (!isRecordingStarted) return;

                                    // Console.WriteLine("threshold: " + threshold.ToString() + " -> current: " + current.ToString());

                                    if (current < threshold)
                                    {
                                        totalSilenceSeconds += seconds;

                                        if (everHadAudio && MaxDurationOfSilence.HasValue && (totalSilenceSeconds > MaxDurationOfSilence.Value.TotalSeconds))
                                        {
                                            // Console.WriteLine("Max duration of silence reached");
                                            isRecordingStarted = false;
                                        }

                                        if (IgnoreSilence.ToBool() && !hadAudio)
                                        {
                                            // Console.WriteLine("Ignoring " + seconds.ToString("0.0") + " seconds of silence skipped");
                                            count = 0;
                                            sum = 0;
                                            seconds = 0;
                                            hadAudio = false;

                                            wavBufferStream.Position = 0;
                                            wavBufferStream.SetLength(0);

                                            return;
                                        }

                                        hadAudio = false;
                                    }

                                    wavBufferStream.Position = 0;
                                    wavBufferStream.CopyTo(waveFileWriter);
                                    wavBufferStream.Position = 0;
                                    wavBufferStream.SetLength(0);

                                    count = 0;
                                    sum = 0;
                                    seconds = 0;
                                }
                            }
                            else
                            {
                                waveFileWriter.Write(args.Buffer, 0, args.BytesRecorded);
                                waveFileWriter.Flush();
                            }
                        }
                    };

                    waveIn.StartRecording();
                    try
                    {
                        Console.WriteLine("Press any key to stop recording... Q to skip processing");

                        var startTime = System.DateTime.UtcNow;

                        while (!Console.KeyAvailable && isRecordingStarted)
                        {
                            System.Threading.Thread.Sleep(500);

                            if (MaxDuration.HasValue && MaxDuration.Value.TotalSeconds > 0 && (System.DateTime.UtcNow - startTime).TotalSeconds > MaxDuration.Value.TotalSeconds)
                            {
                                Console.Write("\u001b[1A"); // Move cursor up one line
                                Console.Write("\u001b[2K"); // Erase the entire line
                                Console.WriteLine("Max recording time of " + MaxDuration.Value.TotalSeconds.ToString() + " seconds, reached.");

                                break;
                            }
                        }
                    }
                    finally
                    {
                        try
                        {
                            isRecordingStarted = false;
                            waveIn.StopRecording();
                        }
                        catch
                        {

                        }
                    }

                    while (Console.KeyAvailable)
                    {
                        if (Console.ReadKey().Key == ConsoleKey.Q)
                        {
                            isProcessingAborted = true;
                            throw new Exception("Processing aborted");
                        }
                    }

                    // Move cursor up one line
                    Console.Write("\u001b[1A"); // Move cursor up one line
                    Console.Write("\u001b[2K"); // Erase the entire line

                    Console.WriteLine("Recording stopped, processing, press Q to abort...");

                    lock (syncLock)
                    {
                        wavBufferStream.Position = 0;
                        wavBufferStream.CopyTo(waveFileWriter);
                        wavBufferStream.Position = 0;
                        wavBufferStream.SetLength(0);
                        waveFileWriter.Flush();
                        outputStream.Position = 0;
                    }

                    try
                    {
                        await foreach (var segment in processor.ProcessAsync(outputStream, cancellationToken))
                        {
                            if (isProcessingAborted) return;
                            cancellationToken.ThrowIfCancellationRequested();

                            lock (syncLock)
                            {
                                objects.Add(segment);
                            }
                        }
                    }
                    finally
                    {
                        await processor.DisposeAsync();
                    }

                }
                else
                {
                    using var stream = File.OpenRead(WaveFile);
                    try
                    {
                        await foreach (var segment in processor.ProcessAsync(stream, cancellationToken))
                        {
                            if (isProcessingAborted) return;
                            cancellationToken.ThrowIfCancellationRequested();

                            lock (syncLock)
                            {
                                objects.Add(segment);
                            }
                        }
                    }
                    finally
                    {
                        await processor.DisposeAsync();
                    }
                }
            }, cancellationToken);
            try
            {
                while (!task.IsCompleted)
                {
                    lock (syncLock)
                    {
                        foreach (var o in objects)
                        {
                            if (Passthru)
                            {
                                WriteObject(o);
                            }
                            else
                            {
                                WriteObject(o.Text);
                            }
                        }
                        objects.Clear();

                        foreach (var o in progressRecords)
                        {
                            WriteProgress(o);
                        }
                        progressRecords.Clear();
                    }

                    while (Console.KeyAvailable && !cancellationToken.IsCancellationRequested)
                    {
                        if (Console.ReadKey().Key == ConsoleKey.Q)
                        {
                            isProcessingAborted = true;
                            WriteError(new ErrorRecord(new Exception("Processing aborted"), "ProcessingAborted", ErrorCategory.OperationStopped, null));

                            return;
                        }
                    }

                    System.Threading.Thread.Sleep(500);
                }

                Console.Write("\u001b[1A"); // Move cursor up one line
                Console.Write("\u001b[2K"); // Erase the entire line
            }
            finally
            {
                try
                {
                    isProcessingAborted = true;
                    cancellationTokenSource.Cancel();
                }
                finally
                {
                    if (!task.IsCompleted)
                    {
                        task.Wait();
                    }
                }
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }

        private static async Task DownloadModel(string fileName, GgmlType ggmlType)
        {
            Console.WriteLine($"Downloading Model {fileName}");
            using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(ggmlType);
            using var fileWriter = File.OpenWrite(fileName);
            await modelStream.CopyToAsync(fileWriter);
            Console.Write("\u001b[1A"); // Move cursor up one line
            Console.Write("\u001b[2K"); // Erase the entire line
        }
    }
}
