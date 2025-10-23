// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Get-AudioDeviceNames.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 2.1.2025
// ################################################################################
// Copyright (c)  René Vaessen / GenXdev
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ################################################################################



using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Management.Automation;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Retrieves the names of available audio devices for microphone or desktop audio capture.
    /// </para>
    ///
    /// <para type="description">
    /// This cmdlet enumerates audio devices that can be used with other cmdlets that accept an AudioDevice parameter.
    /// It supports both microphone devices and desktop audio capture devices.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -UseDesktopAudioCapture &lt;SwitchParameter&gt;<br/>
    /// Specifies whether to list desktop audio capture devices instead of microphone devices.<br/>
    /// - <b>Position</b>: Named<br/>
    /// - <b>Default</b>: False<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -Passthru &lt;SwitchParameter&gt;<br/>
    /// Returns detailed device objects instead of just device names.<br/>
    /// - <b>Position</b>: Named<br/>
    /// - <b>Default</b>: False<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get microphone device names</para>
    /// <para>Retrieves a list of available microphone device names.</para>
    /// <code>
    /// Get-AudioDeviceNames
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get desktop audio capture device names</para>
    /// <para>Retrieves a list of available desktop audio capture device names.</para>
    /// <code>
    /// Get-AudioDeviceNames -UseDesktopAudioCapture
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get detailed microphone device information</para>
    /// <para>Retrieves detailed objects containing information about microphone devices.</para>
    /// <code>
    /// Get-AudioDeviceNames -Passthru
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AudioDeviceNames")]
    public class GetAudioDeviceNames : PSGenXdevCmdlet
    {
        #region Cmdlet Parameters
        /// <summary>
        /// Specifies whether to list desktop audio capture devices instead of microphone devices
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Whether to list desktop audio capture devices instead of microphone devices")]
        public SwitchParameter UseDesktopAudioCapture { get; set; }

        /// <summary>
        /// Returns detailed device objects instead of just names
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Returns detailed device objects instead of just names")]
        public SwitchParameter Passthru { get; set; }
        #endregion

        /// <summary>
        /// Processes each input object and retrieves audio device information
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                // Determine which type of devices to enumerate based on the switch parameter
                if (UseDesktopAudioCapture.ToBool())
                {
                    ListDesktopAudioDevices();
                }
                else
                {
                    ListMicrophoneDevices();
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors during device enumeration
                WriteError(new ErrorRecord(ex, "AudioDeviceEnumerationError", ErrorCategory.OperationStopped, null));
            }
        }

        /// <summary>
        /// Enumerates and lists available microphone devices
        /// </summary>
        private void ListMicrophoneDevices()
        {
            // Inform the user that microphone device enumeration is starting
            WriteVerbose("Enumerating microphone devices...");

            // Loop through all available microphone devices
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                try
                {
                    // Retrieve capabilities for the current device index
                    var deviceInfo = WaveIn.GetCapabilities(i);

                    // Check if detailed objects should be returned
                    if (Passthru.ToBool())
                    {
                        // Create a PowerShell object with detailed device information
                        var deviceObject = new PSObject();
                        deviceObject.Properties.Add(new PSNoteProperty("Index", i));
                        deviceObject.Properties.Add(new PSNoteProperty("Name", deviceInfo.ProductName));
                        deviceObject.Properties.Add(new PSNoteProperty("Guid", deviceInfo.ProductGuid));
                        deviceObject.Properties.Add(new PSNoteProperty("Channels", deviceInfo.Channels));
                        deviceObject.Properties.Add(new PSNoteProperty("Type", "Microphone"));
                        deviceObject.Properties.Add(new PSNoteProperty("WildcardPattern", $"*{deviceInfo.ProductName}*"));

                        // Output the detailed device object
                        WriteObject(deviceObject);
                    }
                    else
                    {
                        // Output just the device name
                        WriteObject(deviceInfo.ProductName);
                    }
                }
                catch (Exception ex)
                {
                    // Log verbose message for devices that cannot be enumerated
                    WriteVerbose($"Could not enumerate device {i}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Enumerates and lists available desktop audio capture devices
        /// </summary>
        private void ListDesktopAudioDevices()
        {
            // Inform the user that desktop audio device enumeration is starting
            WriteVerbose("Enumerating desktop audio capture devices...");

            try
            {
                // Create device enumerator for audio endpoints
                using var deviceEnumerator = new MMDeviceEnumerator();

                // Get active render devices (speakers/headphones for capture)
                var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

                // Initialize index counter for device numbering
                int index = 0;

                // Loop through each active device
                foreach (var device in devices)
                {
                    try
                    {
                        // Check if detailed objects should be returned
                        if (Passthru.ToBool())
                        {
                            // Create a PowerShell object with detailed device information
                            var deviceObject = new PSObject();
                            deviceObject.Properties.Add(new PSNoteProperty("Index", index));
                            deviceObject.Properties.Add(new PSNoteProperty("Name", device.FriendlyName));
                            deviceObject.Properties.Add(new PSNoteProperty("Id", device.ID));
                            deviceObject.Properties.Add(new PSNoteProperty("State", device.State.ToString()));
                            deviceObject.Properties.Add(new PSNoteProperty("Type", "DesktopAudio"));
                            deviceObject.Properties.Add(new PSNoteProperty("WildcardPattern", $"*{device.FriendlyName}*"));

                            // Output the detailed device object
                            WriteObject(deviceObject);
                        }
                        else
                        {
                            // Output just the device name
                            WriteObject(device.FriendlyName);
                        }

                        // Increment the index for the next device
                        index++;
                    }
                    catch (Exception ex)
                    {
                        // Log verbose message for devices that cannot be processed
                        WriteVerbose($"Could not get details for device {device?.ID}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle enumeration failure and provide fallback
                WriteWarning($"Desktop audio device enumeration failed: {ex.Message}");
                WriteWarning("Desktop audio device selection by name is not supported in this NAudio version.");

                // Check if detailed objects should be returned
                if (Passthru.ToBool())
                {
                    // Create a default device object as fallback
                    var defaultDevice = new PSObject();
                    defaultDevice.Properties.Add(new PSNoteProperty("Index", 0));
                    defaultDevice.Properties.Add(new PSNoteProperty("Name", "Default Desktop Audio"));
                    defaultDevice.Properties.Add(new PSNoteProperty("Id", "default"));
                    defaultDevice.Properties.Add(new PSNoteProperty("State", "Available"));
                    defaultDevice.Properties.Add(new PSNoteProperty("Type", "DesktopAudio"));
                    defaultDevice.Properties.Add(new PSNoteProperty("WildcardPattern", "*default*"));

                    // Output the default device object
                    WriteObject(defaultDevice);
                }
                else
                {
                    // Output the default device name
                    WriteObject("Default Desktop Audio");
                }
            }
        }
    }
}