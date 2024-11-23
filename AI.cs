using System.Management.Automation;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whisper.net;
using Whisper.net.Ggml;
using Whisper.net.Logger;
using Org.BouncyCastle.Crypto.IO;
using System.CodeDom;
using System.Runtime.Intrinsics.X86;
using System.Drawing.Drawing2D;
using System.IO;
using SpotifyAPI.Web;
using Org.BouncyCastle.Utilities.Zlib;
using System.Management;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace GenXdev.Helpers
{
    [Cmdlet(VerbsCommon.Get, "SpeechToText")]
    public class GetSpeechToText : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string ModelFilePath { get; set; } = null;

        [Parameter(Position = 1, Mandatory = false)]
        public string WaveFile { get; set; } = null;

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Sets the language to detect, defaults to 'auto'")]
        public string Language { get; set; } = "auto";

        [Parameter(Position = 3, Mandatory = false, HelpMessage = "Returns objects instead of strings")]
        public SwitchParameter Passthru { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        // Rest of the code...
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var results = new StringBuilder();
            var objects = new List<object>();

            int physicalCoreCount = 0;
            var searcher = new ManagementObjectSearcher("select NumberOfCores from Win32_Processor");
            foreach (var item in searcher.Get())
            {
                physicalCoreCount += Convert.ToInt32(item["NumberOfCores"]);
            }

            Task.Run(async () =>
            {
                // We declare three variables which we will use later, ggmlType, modelFileName and inputFileName
                var ggmlType = GgmlType.LargeV3Turbo;
                var modelFileName = Path.GetFullPath(Path.Combine(ModelFilePath, "ggml-largeV3Turbo.bin"));

                // This section detects whether the "ggml-base.bin" file exists in our project disk. If it doesn't, it downloads it from the internet
                if (!File.Exists(modelFileName))
                {
                    await DownloadModel(modelFileName, ggmlType);
                }

                // This section creates the whisperFactory object which is used to create the processor object.
                using var whisperFactory = WhisperFactory.FromPath(modelFileName);

                // This section creates the processor object which is used to process the audio data sampled from the default microphone, it uses language `auto` to detect the language of the audio.
                using var processor = whisperFactory.CreateBuilder()
                    .WithLanguage(Language)
                    .WithSegmentEventHandler((segment) =>
                     {
                         // Do whetever you want with your segment here.
                         lock (results)
                         {
                             results.Append($"{segment.Text} ");
                             objects.Add(segment);
                         }
                     })
                    .Build();

                // Optional logging from the native library
                //LogProvider.Instance.OnLog += (level, message) =>
                //            {
                //                Console.WriteLine($"{level}: {message}");
                //            };
                // This section initializes the default microphone input

                // This examples shows how to use Whisper.net to create a transcription from audio data sampled from the default microphone with 16Khz sample rate.
                // This section initializes the default microphone input
                if (WaveFile == null)
                {
                    using var waveIn = new WaveInEvent();
                    waveIn.WaveFormat = new WaveFormat(16000, 1); // 16Khz sample rate, mono channel
                    bool started = true;
                    using var wavStream = new MemoryStream();
                    // Add logging to console to display the selected input audio device
                    // Console.WriteLine($"Selected input audio device: {waveIn.DeviceNumber} - {WaveIn.GetCapabilities(waveIn.DeviceNumber).ProductName}");

                    waveIn.DataAvailable += (sender, args) =>
                    {
                        if (!started) return;

                        // This section processes the audio data and writes it to the MemoryStream
                        lock (wavStream)
                        {
                            wavStream.Write(args.Buffer, 0, args.BytesRecorded);
                            wavStream.Flush();
                        }
                    };

                    // This section starts recording from the default microphone
                    waveIn.StartRecording();

                    // This section waits for the user to press any key to stop recording
                    Console.WriteLine("Press any key to stop recording...");
                    while (Console.KeyAvailable) { Console.ReadKey(); }

                    while (!Console.KeyAvailable)
                    {
                        System.Threading.Thread.Sleep(100);

                        if (Passthru)
                        {
                            lock (results)
                            {
                                foreach (var segment in objects)
                                {
                                    WriteObject(segment);
                                }
                                objects.Clear();
                                results.Clear();
                            }
                        }
                    }

                    while (Console.KeyAvailable) { Console.ReadKey(); }

                    try
                    {
                        started = false;
                        waveIn.StopRecording();
                    }
                    catch
                    {

                    }

                    Console.WriteLine("recording stopped, processing...");

                    lock (wavStream)
                    {
                        using var outputStream = new MemoryStream();
                        using var waveFileWriter = new WaveFileWriter(outputStream, waveIn.WaveFormat);

                        wavStream.Position = 0;
                        wavStream.CopyTo(waveFileWriter);
                        wavStream.Flush();
                        wavStream.Position = 0;
                        wavStream.SetLength(0);

                        waveFileWriter.Flush();

                        outputStream.Position = 0;
                        processor.Process(outputStream);
                    }
                }
                else
                {
                    using var stream = File.OpenRead(WaveFile);
                    processor.Process(stream);
                }
            }).Wait();

            if (Passthru)
            {
                foreach (var o in objects)
                {
                    WriteObject(o);
                }
                return;
            }

            WriteObject(results.ToString());
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
        }
    }
}
