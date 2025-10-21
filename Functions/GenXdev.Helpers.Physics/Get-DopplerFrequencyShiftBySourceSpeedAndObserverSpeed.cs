// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.304.2025
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



using System;
using System.Management.Automation;

namespace GenXdev.Helpers.Physics
{
    /// <summary>
    /// <para type="synopsis">
    /// Calculates Doppler shifted frequency.
    /// </para>
    ///
    /// <para type="description">
    /// Uses f' = f * (v + vo) / (v - vs), speeds positive towards each other.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -OriginalFrequencyInHertz &lt;double&gt;<br/>
    /// Original frequency in Hz.<br/>
    /// - <b>Position</b>: 0<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -SourceSpeedInMetersPerSecond &lt;double&gt;<br/>
    /// Source speed in m/s (positive towards observer).<br/>
    /// - <b>Position</b>: 1<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -ObserverSpeedInMetersPerSecond &lt;double&gt;<br/>
    /// Observer speed in m/s (positive towards source).<br/>
    /// - <b>Position</b>: 2<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -SpeedOfSoundInMetersPerSecond &lt;double&gt;<br/>
    /// Speed of sound in m/s (default: 343).<br/>
    /// - <b>Position</b>: 3<br/>
    /// - <b>Default</b>: 343<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -Medium &lt;string&gt;<br/>
    /// The medium (mutually exclusive with SpeedOfSoundInMetersPerSecond).<br/>
    /// - <b>Position</b>: 3<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for frequency. Default 'hertz'.<br/>
    /// - <b>Position</b>: 4<br/>
    /// - <b>Default</b>: "hertz"<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed -OriginalFrequencyInHertz 440 -SourceSpeedInMetersPerSecond 10 -ObserverSpeedInMetersPerSecond 5 -Medium "water" -As "kilohertz"</para>
    /// <para>Calculates the Doppler shifted frequency for a 440 Hz tone with source moving at 10 m/s towards observer, observer moving at 5 m/s towards source, in water medium, output in kilohertz.</para>
    /// <code>
    /// Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed -OriginalFrequencyInHertz 440 -SourceSpeedInMetersPerSecond 10 -ObserverSpeedInMetersPerSecond 5 -Medium "water" -As "kilohertz"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed 440 10 0 -SpeedOfSoundInMetersPerSecond 1480</para>
    /// <para>Calculates the Doppler shifted frequency for a 440 Hz tone with source moving at 10 m/s, observer stationary, speed of sound 1480 m/s.</para>
    /// <code>
    /// Get-DopplerFrequencyShiftBySourceSpeedAndObserverSpeed 440 10 0 -SpeedOfSoundInMetersPerSecond 1480
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DopplerFrequencyShiftBySourceSpeedAndObserverSpeed")]
    [OutputType(typeof(double))]
    public class GetDopplerFrequencyShiftBySourceSpeedAndObserverSpeedCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Original frequency in Hz
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Original frequency in Hz"
        )]
        public double OriginalFrequencyInHertz { get; set; }

        /// <summary>
        /// Source speed in m/s (positive towards observer)
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Source speed in m/s (positive towards observer)"
        )]
        public double SourceSpeedInMetersPerSecond { get; set; }

        /// <summary>
        /// Observer speed in m/s (positive towards source)
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 2,
            HelpMessage = "Observer speed in m/s (positive towards source)"
        )]
        public double ObserverSpeedInMetersPerSecond { get; set; }

        /// <summary>
        /// Speed of sound in m/s (default: 343)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 3,
            ParameterSetName = "BySpeed",
            HelpMessage = "Speed of sound in m/s (default: 343)"
        )]
        public double SpeedOfSoundInMetersPerSecond { get; set; } = 343;

        /// <summary>
        /// The medium
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 3,
            ParameterSetName = "ByMedium",
            HelpMessage = "The medium"
        )]
        [ValidateSet("air", "water", "seawater", "steel", "glass", "lead", "gold", "copper", "rubber", "vacuum", "helium", "co2", "methane")]
        public string Medium { get; set; }

        /// <summary>
        /// Output unit for frequency
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 4,
            HelpMessage = "Output unit for frequency"
        )]
        [ValidateSet("hertz", "kilohertz", "megahertz")]
        public string As { get; set; } = "hertz";

        /// <summary>
        /// Begin processing - set speed of sound based on medium if specified
        /// </summary>
        protected override void BeginProcessing()
        {
            if (ParameterSetName == "ByMedium")
            {
                switch (Medium)
                {
                    case "air":
                        SpeedOfSoundInMetersPerSecond = 343;
                        break;
                    case "water":
                        SpeedOfSoundInMetersPerSecond = 1480;
                        break;
                    case "seawater":
                        SpeedOfSoundInMetersPerSecond = 1530;
                        break;
                    case "steel":
                        SpeedOfSoundInMetersPerSecond = 5960;
                        break;
                    case "glass":
                        SpeedOfSoundInMetersPerSecond = 4540;
                        break;
                    case "lead":
                        SpeedOfSoundInMetersPerSecond = 1210;
                        break;
                    case "gold":
                        SpeedOfSoundInMetersPerSecond = 3240;
                        break;
                    case "copper":
                        SpeedOfSoundInMetersPerSecond = 4600;
                        break;
                    case "rubber":
                        SpeedOfSoundInMetersPerSecond = 60;
                        break;
                    case "vacuum":
                        SpeedOfSoundInMetersPerSecond = 0;
                        break;
                    case "helium":
                        SpeedOfSoundInMetersPerSecond = 965;
                        break;
                    case "co2":
                        SpeedOfSoundInMetersPerSecond = 259;
                        break;
                    case "methane":
                        SpeedOfSoundInMetersPerSecond = 430;
                        break;
                }

                WriteVerbose($"Using speed of sound in {Medium}: {SpeedOfSoundInMetersPerSecond} m/s");

                if (SpeedOfSoundInMetersPerSecond == 0)
                {
                    ThrowTerminatingError(new ErrorRecord(
                        new Exception("No sound propagation in vacuum"),
                        "VacuumError",
                        ErrorCategory.InvalidArgument,
                        null
                    ));
                }
            }
        }

        /// <summary>
        /// Process record - calculate Doppler shift and convert unit
        /// </summary>
        protected override void ProcessRecord()
        {
            // Calculate the shifted frequency
            double shifted = OriginalFrequencyInHertz * (SpeedOfSoundInMetersPerSecond + ObserverSpeedInMetersPerSecond) / (SpeedOfSoundInMetersPerSecond - SourceSpeedInMetersPerSecond);

            // Convert to desired unit using PowerShell function
            var convertScript = ScriptBlock.Create("param($value, $fromUnit, $toUnit) GenXdev.Helpers\\Convert-PhysicsUnit -Value $value -FromUnit $fromUnit -ToUnit $toUnit");
            var result = convertScript.Invoke(shifted, "hertz", As);

            WriteObject(result[0]);
        }

        /// <summary>
        /// End processing - no cleanup needed
        /// </summary>
        protected override void EndProcessing()
        {
        }
    }
}