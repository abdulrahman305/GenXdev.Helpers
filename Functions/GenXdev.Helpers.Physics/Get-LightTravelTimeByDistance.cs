// ################################################################################
// Part of PowerShell module : GenXdev.Helpers.Physics
// Original cmdlet filename  : Get-LightTravelTimeByDistance.cs
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



using System.Management.Automation;

namespace GenXdev.Helpers.Physics
{
    /// <summary>
    /// <para type="synopsis">
    /// Calculates time for light to travel a distance.
    /// </para>
    ///
    /// <para type="description">
    /// Uses t = d / c, with c depending on medium.
    /// </para>
    ///
    /// <para type="description">
    /// PARAMETERS
    /// </para>
    ///
    /// <para type="description">
    /// -DistanceInMeters &lt;double&gt;<br/>
    /// Distance in meters.<br/>
    /// - <b>Position</b>: 0<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -SpeedOfLightInMetersPerSecond &lt;double&gt;<br/>
    /// Speed of light in m/s. Default 299792458 (vacuum).<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Default</b>: 299792458<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -Medium &lt;string&gt;<br/>
    /// The medium through which light travels (mutually exclusive with SpeedOfLightInMetersPerSecond).<br/>
    /// - <b>Position</b>: 1<br/>
    /// - <b>Accepted values</b>: vacuum, air, water, glass, diamond<br/>
    /// </para>
    ///
    /// <para type="description">
    /// -As &lt;string&gt;<br/>
    /// Output unit for time. Default 'seconds'.<br/>
    /// - <b>Position</b>: 2<br/>
    /// - <b>Default</b>: seconds<br/>
    /// - <b>Accepted values</b>: seconds, minutes, hours, milliseconds, days<br/>
    /// </para>
    ///
    /// <example>
    /// <para>Get-LightTravelTimeByDistance -DistanceInMeters 149597870700 -Medium "water" -As "minutes"</para>
    /// <para>Calculates the time for light to travel the distance to the sun through water, in minutes.</para>
    /// <code>
    /// Get-LightTravelTimeByDistance -DistanceInMeters 149597870700 -Medium "water" -As "minutes"
    /// </code>
    /// </example>
    ///
    /// <example>
    /// <para>Get-LightTravelTimeByDistance 300000000 -SpeedOfLightInMetersPerSecond 225000000</para>
    /// <para>Calculates time for light to travel 300000000 meters at 225000000 m/s.</para>
    /// <code>
    /// Get-LightTravelTimeByDistance 300000000 -SpeedOfLightInMetersPerSecond 225000000
    /// </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "LightTravelTimeByDistance")]
    [OutputType(typeof(double))]
    public class GetLightTravelTimeByDistanceCommand : PSGenXdevCmdlet
    {
        /// <summary>
        /// Distance in meters
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Distance in meters"
        )]
        public double DistanceInMeters { get; set; }

        /// <summary>
        /// Speed of light in m/s (default: 299792458)
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 1,
            ParameterSetName = "BySpeed",
            HelpMessage = "Speed of light in m/s (default: 299792458)"
        )]
        public double SpeedOfLightInMetersPerSecond { get; set; } = 299792458;

        /// <summary>
        /// The medium through which light travels
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            ParameterSetName = "ByMedium",
            HelpMessage = "The medium through which light travels"
        )]
        [ValidateSet("vacuum", "air", "water", "glass", "diamond")]
        public string Medium { get; set; }

        /// <summary>
        /// Output unit for time
        /// </summary>
        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "Output unit for time"
        )]
        [ValidateSet("seconds", "minutes", "hours", "milliseconds", "days")]
        public string As { get; set; } = "seconds";

        private double speedOfLight;

        /// <summary>
        /// Begin processing - initialization logic
        /// </summary>
        protected override void BeginProcessing()
        {
            if (ParameterSetName == "ByMedium")
            {
                switch (Medium)
                {
                    case "vacuum":
                        speedOfLight = 299792458;
                        break;
                    case "air":
                        speedOfLight = 299702547;
                        break;
                    case "water":
                        speedOfLight = 225000000;
                        break;
                    case "glass":
                        speedOfLight = 200000000;
                        break;
                    case "diamond":
                        speedOfLight = 124000000;
                        break;
                }

                WriteVerbose($"Using speed of light in {Medium}: {speedOfLight} m/s");
            }
            else
            {
                speedOfLight = SpeedOfLightInMetersPerSecond;
            }
        }

        /// <summary>
        /// Process record - main cmdlet logic
        /// </summary>
        protected override void ProcessRecord()
        {
            double time = DistanceInMeters / speedOfLight;

            var scriptBlock = ScriptBlock.Create("param($value, $fromUnit, $toUnit) GenXdev.Helpers\\Convert-PhysicsUnit -Value $value -FromUnit $fromUnit -ToUnit $toUnit");

            var result = scriptBlock.Invoke(time, "seconds", As);

            WriteObject(result[0]);
        }

        /// <summary>
        /// End processing - cleanup logic
        /// </summary>
        protected override void EndProcessing()
        {
            // No cleanup needed
        }
    }
}