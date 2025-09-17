using System.Management.Automation;
using NAudio.Wave;
using NAudio.CoreAudioApi;

/// <summary>
/// Gets available audio device names that can be used with AudioDevice parameter
/// </summary>
[Cmdlet(VerbsCommon.Get, "AudioDeviceNames")]
public class GetAudioDeviceNames : PSCmdlet
{
    #region Cmdlet Parameters
    [Parameter(Mandatory = false, HelpMessage = "Whether to list desktop audio capture devices instead of microphone devices")]
    public SwitchParameter UseDesktopAudioCapture { get; set; }

    [Parameter(Mandatory = false, HelpMessage = "Returns detailed device objects instead of just names")]
    public SwitchParameter Passthru { get; set; }
    #endregion

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        try
        {
            if (UseDesktopAudioCapture.IsPresent)
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
            WriteError(new ErrorRecord(ex, "AudioDeviceEnumerationError", ErrorCategory.OperationStopped, null));
        }
    }

    private void ListMicrophoneDevices()
    {
        WriteVerbose("Enumerating microphone devices...");

        for (int i = 0; i < WaveIn.DeviceCount; i++)
        {
            try
            {
                var deviceInfo = WaveIn.GetCapabilities(i);

                if (Passthru.IsPresent)
                {
                    var deviceObject = new PSObject();
                    deviceObject.Properties.Add(new PSNoteProperty("Index", i));
                    deviceObject.Properties.Add(new PSNoteProperty("Name", deviceInfo.ProductName));
                    deviceObject.Properties.Add(new PSNoteProperty("Guid", deviceInfo.ProductGuid));
                    deviceObject.Properties.Add(new PSNoteProperty("Channels", deviceInfo.Channels));
                    deviceObject.Properties.Add(new PSNoteProperty("Type", "Microphone"));
                    deviceObject.Properties.Add(new PSNoteProperty("WildcardPattern", $"*{deviceInfo.ProductName}*"));

                    WriteObject(deviceObject);
                }
                else
                {
                    WriteObject(deviceInfo.ProductName);
                }
            }
            catch (Exception ex)
            {
                WriteVerbose($"Could not enumerate device {i}: {ex.Message}");
            }
        }
    }

    private void ListDesktopAudioDevices()
    {
        WriteVerbose("Enumerating desktop audio capture devices...");

        try
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            int index = 0;
            foreach (var device in devices)
            {
                try
                {
                    if (Passthru.IsPresent)
                    {
                        var deviceObject = new PSObject();
                        deviceObject.Properties.Add(new PSNoteProperty("Index", index));
                        deviceObject.Properties.Add(new PSNoteProperty("Name", device.FriendlyName));
                        deviceObject.Properties.Add(new PSNoteProperty("Id", device.ID));
                        deviceObject.Properties.Add(new PSNoteProperty("State", device.State.ToString()));
                        deviceObject.Properties.Add(new PSNoteProperty("Type", "DesktopAudio"));
                        deviceObject.Properties.Add(new PSNoteProperty("WildcardPattern", $"*{device.FriendlyName}*"));

                        WriteObject(deviceObject);
                    }
                    else
                    {
                        WriteObject(device.FriendlyName);
                    }

                    index++;
                }
                catch (Exception ex)
                {
                    WriteVerbose($"Could not get details for device {device?.ID}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            WriteWarning($"Desktop audio device enumeration failed: {ex.Message}");
            WriteWarning("Desktop audio device selection by name is not supported in this NAudio version.");

            if (Passthru.IsPresent)
            {
                var defaultDevice = new PSObject();
                defaultDevice.Properties.Add(new PSNoteProperty("Index", 0));
                defaultDevice.Properties.Add(new PSNoteProperty("Name", "Default Desktop Audio"));
                defaultDevice.Properties.Add(new PSNoteProperty("Id", "default"));
                defaultDevice.Properties.Add(new PSNoteProperty("State", "Available"));
                defaultDevice.Properties.Add(new PSNoteProperty("Type", "DesktopAudio"));
                defaultDevice.Properties.Add(new PSNoteProperty("WildcardPattern", "*default*"));

                WriteObject(defaultDevice);
            }
            else
            {
                WriteObject("Default Desktop Audio");
            }
        }
    }
}
