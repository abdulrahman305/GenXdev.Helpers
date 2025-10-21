// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-SoundTravelDistanceByTime.cs
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



using System.Management.Automation;

namespace GenXdev.Helpers.Physics
{
    /// <summary>
    /// <para type="synopsis">
    /// Calculates the distance sound travels in a given time.
    /// </para>
    ///
    /// <para type="description">
    /// Uses the formula distance = speed * time, with default speed of sound in air.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -TimeInSeconds &lt;double&gt;<br/>
    /// The time in seconds.<br/>
    /// - <b>Position</b>: 0<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -SpeedOfSoundInMetersPerSecond &lt;double&gt;<br/>
    /// Speed of sound in m/s. Defaults to 343 m/s (air).<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Default</b>: 343<br/>
    /// - <b>ParameterSet</b>: BySpeed<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -Medium &lt;string&gt;<br/>
    /// The medium through which sound travels (mutually exclusive with SpeedOfSoundInMetersPerSecond).<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>ParameterSet</b>: ByMedium<br/>
    /// - <b>Allowed Values</b>: air, water, seawater, steel, glass, lead, gold, copper, rubber, vacuum, helium, co2, methane<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// The unit for the output distance. Defaults to 'meters'.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: meters<br/>
    /// - <b>Allowed Values</b>: meters, kilometers, centimeters, millimeters, feet, inches, miles, yards<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get the distance sound travels in 5 seconds through water, in kilometers</para>
    /// <para>This example calculates how far sound travels in water over 5 seconds and converts the result to kilometers.</para>
    /// <code>
    /// Get-SoundTravelDistanceByTime -TimeInSeconds 5 -Medium "water" -As "kilometers"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get the distance sound travels in 10 seconds at 1480 m/s</para>
    /// <para>This example calculates the distance using a specific speed of sound.</para>
    /// <code>
    /// Get-SoundTravelDistanceByTime 10 -SpeedOfSoundInMetersPerSecond 1480
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SoundTravelDistanceByTime")]
    [OutputType(typeof(double))]
    public class GetSoundTravelDistanceByTimeCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// The time in seconds
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "The time in seconds")]
        public double TimeInSeconds { get; set; }

        /// <summary>
        /// Speed of sound in m/s (default: 343)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 1,
            ParameterSetName = "BySpeed",
            HelpMessage = "Speed of sound in m/s (default: 343)")]
        public double SpeedOfSoundInMetersPerSecond { get; set; } = 343;

        /// <summary>
        /// The medium through which sound travels
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            ParameterSetName = "ByMedium",
            HelpMessage = "The medium through which sound travels")]
        [ValidateSet("air", "water", "seawater", "steel", "glass", "lead", "gold", "copper", "rubber", "vacuum", "helium", "co2", "methane")]
        public string Medium { get; set; }

        /// <summary>
        /// The unit for the output distance
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "The unit for the output distance")]
        [ValidateSet("meters", "kilometers", "centimeters", "millimeters", "feet", "inches", "miles", "yards")]
        public string As { get; set; } = "meters";

        /// <summary>
        /// Begin processing - set speed based on medium if specified
        /// </summary>
        protected override void BeginProcessing()
        {
            // Check if using ByMedium parameter set
            if (this.ParameterSetName == "ByMedium")
            {
                // Set speed based on medium
                switch (this.Medium)
                {
                    case "air":
                        this.SpeedOfSoundInMetersPerSecond = 343;
                        break;
                    case "water":
                        this.SpeedOfSoundInMetersPerSecond = 1480;
                        break;
                    case "seawater":
                        this.SpeedOfSoundInMetersPerSecond = 1530;
                        break;
                    case "steel":
                        this.SpeedOfSoundInMetersPerSecond = 5960;
                        break;
                    case "glass":
                        this.SpeedOfSoundInMetersPerSecond = 4540;
                        break;
                    case "lead":
                        this.SpeedOfSoundInMetersPerSecond = 1210;
                        break;
                    case "gold":
                        this.SpeedOfSoundInMetersPerSecond = 3240;
                        break;
                    case "copper":
                        this.SpeedOfSoundInMetersPerSecond = 4600;
                        break;
                    case "rubber":
                        this.SpeedOfSoundInMetersPerSecond = 60;
                        break;
                    case "vacuum":
                        this.SpeedOfSoundInMetersPerSecond = 0;
                        break;
                    case "helium":
                        this.SpeedOfSoundInMetersPerSecond = 965;
                        break;
                    case "co2":
                        this.SpeedOfSoundInMetersPerSecond = 259;
                        break;
                    case "methane":
                        this.SpeedOfSoundInMetersPerSecond = 430;
                        break;
                }

                // Write verbose message about speed being used
                this.WriteVerbose($"Using speed of sound in {this.Medium}: {this.SpeedOfSoundInMetersPerSecond} m/s");
            }
        }

        /// <summary>
        /// Process record - calculate distance and convert units
        /// </summary>
        protected override void ProcessRecord()
        {
            // Calculate distance using formula: distance = speed * time
            double distance = this.SpeedOfSoundInMetersPerSecond * this.TimeInSeconds;

            // Use InvokeCommand to call the unit conversion function for exact compatibility
            var scriptBlock = ScriptBlock.Create("param($Value, $FromUnit, $ToUnit) GenXdev.Helpers\\Convert-PhysicsUnit -Value $Value -FromUnit $FromUnit -ToUnit $ToUnit");
            var results = scriptBlock.Invoke(distance, "meters", this.As);

            // Output the result
            this.WriteObject(results[0]);
        }

        /// <summary>
        /// End processing - no cleanup needed
        /// </summary>
        protected override void EndProcessing()
        {
            // No cleanup required
        }
    }
}
// ###############################################################################